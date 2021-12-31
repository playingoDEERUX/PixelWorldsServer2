using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Kernys.Bson;
using System.Linq;
using System.Threading;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.Networking.Server;
using System.Threading.Tasks;

namespace FeatherNet
{
    public class FeatherUtil
    {
        public static long GetTimeSec()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds();
        }

        public static long GetTimeMs()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }

    public struct FeatherDefaults
    {
        public const int PING_CLOCK_MS = 15; // essentially acts like a tickrate.
        public const int PING_MULTIPLIER = 1;
        public const int BUFFER_SIZE = 8192;
        public const int MAX_PACKET_SIZE = 160000;
        public const int MIN_TRANSACTION_SIZE = 4;
        public const int TIMEOUT_DURATION = 15000;
    }

    public class FeatherState
    {
        public byte[] buffer = new byte[FeatherDefaults.BUFFER_SIZE];
        public byte[] data;
        public int bytesReadToData = 0;
    }

    public class FeatherClient
    {
        private bool disconnectLater = false;
        private bool timedOut = false;
        public bool isDisconnecting() => disconnectLater;
        public bool isTimedOut() => timedOut;
        public bool isConnected() => GetClient().Connected;
        public EventWaitHandle syncHandle = new AutoResetEvent(false);

        public bool needsPing()
        {
            return FeatherUtil.GetTimeMs() > lastResponse + (FeatherDefaults.PING_CLOCK_MS * pingMul);
        }

        private TcpClient client = null;
        private long lastResponse = 0;
        public int pingMul = 1;
        private object host = null;
        public object data = null; // Freely applicable data that developer may or may not use.
        private List<BSONObject> outgoingPackets = new List<BSONObject>();
        public List<FeatherEvent> incomingEvents = new List<FeatherEvent>();
        public object link = null; // can link to any wanted object in here as well. Should suffice for anything extra.

        private void OnEndWrite(IAsyncResult i)
        {
            try
            {
                var ns = client.GetStream();
                ns.EndWrite(i);
            }
            catch (ArgumentNullException) { }
            catch (IOException) { }
            catch (InvalidOperationException) { }

            StartReading(link as PWServer);
        }

        private void OnEndRead(IAsyncResult i)
        {
            bool continueNetwork = false;

            try
            {
                var pServer = (PWServer)link;
                var ns = client.GetStream();
                int recv = ns.EndRead(i);
                FeatherState fState = i.AsyncState as FeatherState;

                lock (pServer.locker)
                {
                    continueNetwork = client.Connected && !isTimedOut() && !isDisconnecting();
                    if (recv <= 0 || recv >= FeatherDefaults.BUFFER_SIZE)
                    {
                        DisconnectLater();
                        return;
                    }

                    if (recv > 0)
                    {
                        if (fState.data == null)
                        {
                            int num2 = BitConverter.ToInt32(fState.buffer, 0);

                            if (num2 <= 4 || num2 > FeatherDefaults.MAX_PACKET_SIZE)
                            {
                                Timeout();
                                return;
                            }

                            fState.data = new byte[num2 - 4];
                            Array.Copy(fState.buffer, 4, fState.data, 0, recv - 4);
                            fState.bytesReadToData = recv - 4;
                        }
                        else
                        {
                            Array.Copy(fState.buffer, 0, fState.data, fState.bytesReadToData, recv);
                            fState.bytesReadToData += recv;
                        }
                        if (fState.bytesReadToData == fState.data.Length)
                        {
                            // packet is ready!
                            incomingEvents.Add(Receive(fState.data));
                        }
                        else
                        {
                            ns.BeginRead(fState.buffer, 0, FeatherDefaults.BUFFER_SIZE, OnEndRead, fState);
                        }
                    }
                }
            }
            catch (InvalidOperationException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (IOException) { }
        }
        public void StartReading(PWServer server)
        {
            this.link = server;
            try
            {
                FeatherState fState = new FeatherState();

                var ns = client.GetStream();
                ns.BeginRead(fState.buffer, 0, FeatherDefaults.BUFFER_SIZE, OnEndRead, fState);
            }
            catch (ObjectDisposedException) { }
            catch (IOException) { }
            catch (InvalidOperationException) { }
            catch (SocketException) { }
        }
        public string GetIPString()
        {
            // Return the IPv4-Address in string format:

            if (client == null)
                return "0.0.0.0";

            if (!client.Connected)
                return "0.0.0.0";

            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }
        public object GetHost() => host;
        public void UpdateLastResponse() 
        { 
            lastResponse = FeatherUtil.GetTimeMs();
            pingMul = FeatherDefaults.PING_MULTIPLIER;
        }

        public bool areWeSending = false;

        public bool CanFlush() => outgoingPackets.Count > 0 && areWeSending;

        public void Flush()
        {
            if (client == null)
                return;

            if (!client.Connected)
                return;

            try
            {
                var ns = client.GetStream();

                if (ns.CanWrite && outgoingPackets.Count > 0)
                {
                    // Serialize all bson objects into a single one:

                    BSONObject packet = new BSONObject();

                    for (int i = 0; i < outgoingPackets.Count; i++)
                    {
                        packet[$"m{i}"] = outgoingPackets[i];
                    }
                    packet["mc"] = outgoingPackets.Count;

                    outgoingPackets.Clear();

                    byte[] bData = SimpleBSON.Dump(packet);

                    int len = bData.Length + 4;
                    byte[] data = new byte[len];

                    Array.Copy(BitConverter.GetBytes(len), data, sizeof(int));

                    if (bData.Length > 0)
                        Buffer.BlockCopy(bData, 0, data, 4, bData.Length);
                    else
                        return; // huh? Treat it to be legal just incase anyway...

                    ns.BeginWrite(data, 0, data.Length, OnEndWrite, null);
                    areWeSending = false;
                }
            }
            catch
            {

            }
        }

        public void Send(BSONObject bObj)
        {
            if (this.isConnected())
                outgoingPackets.Add(bObj);
        }

        public void SendIfDoesNotContain(BSONObject bObj)
        {
            string id = bObj["ID"];
            for (int i = 0; i < outgoingPackets.Count; i++)
            {
                if (outgoingPackets[i]["ID"] == id)
                    return;
            }

            Send(bObj);
        }

        public void Free()
        {
            this.outgoingPackets.Clear();
            this.client.Close();
        }

        private FeatherEvent Ping()
        {
            FeatherEvent ev = new FeatherEvent();
            ev.client = this;
            ev.type = FeatherEvent.Types.PING_NOW;

            return ev;
        }

        private FeatherEvent Disconnect(bool timeout = false)
        {
            disconnectLater = false;

            FeatherEvent ev = new FeatherEvent();
            ev.client = this;
            ev.type = FeatherEvent.Types.DISCONNECT;

            int flag = 0;

            if (timeout)
                flag |= (int)FeatherEvent.Flags.TIMEOUT;
           
            ev.flags = flag;
            return ev;
        }

        public void Timeout() => this.timedOut = true;
        public void DisconnectLater() => this.disconnectLater = true;
        private FeatherEvent Receive(byte[] buffer)
        {
            FeatherEvent ev = new FeatherEvent();
            ev.client = this;
            ev.type = FeatherEvent.Types.RECEIVE;
            ev.packetData = buffer;

            return ev;
        }

        public FeatherEvent CheckTimeout()
        {
            FeatherEvent fEv = new FeatherEvent();
            fEv.type = FeatherEvent.Types.NONE;

            if (lastResponse + FeatherDefaults.TIMEOUT_DURATION < FeatherUtil.GetTimeMs())
                fEv = this.Disconnect(true);
            else if (disconnectLater)
                fEv = this.Disconnect();
            else if (needsPing())
                fEv = this.Ping();

            return fEv;
        }

        public TcpClient GetClient() { return client; }
        public FeatherClient(TcpClient client, object host) 
        { 
            this.client = client; 
            this.host = host;
            this.UpdateLastResponse();
        }
    }

    public struct FeatherPacket
    {
        public byte[] data;
        byte flags;

        public FeatherPacket(byte[] data = null, byte flags = 0)
        {
            this.data = data;
            this.flags = flags;
        }
    }

    public struct FeatherEvent
    {
        public FeatherClient client;
        public byte[] packetData;
        public Types type;
        public int flags;

        public enum Flags
        {
            NONE = (1 << 0),
            TIMEOUT = (1 << 1)
        }

        public enum Types
        {
            NONE,
            CONNECT,
            RECEIVE,
            DISCONNECT,
            PING_NOW,
            ERROR
        }
    }

    public class FeatherServer
    {
        private TcpListener listener = null;
        private List<FeatherClient> clients = new List<FeatherClient>();

        [Obsolete]
        public FeatherServer(int port)
        {
            if (listener == null)
            {
                listener = new TcpListener(port);
            }
        }

        public TcpListener GetListener() { return listener; }
        public FeatherClient[] GetClients() { return clients.ToArray(); }

        public bool Start()
        {
            if (listener == null)
                return false;

            try
            {
                listener.Server.NoDelay = true;
                listener.Start();
            }
            catch (SocketException) { return false; }
            return true;
        }

        public void Stop()
        {
            if (listener == null)
                return;

            try
            {
                listener.Stop();
            }
            catch (SocketException) { }
        }

        private List<FeatherEvent> CheckIncomingConnections()
        {
            List<FeatherEvent> events = new List<FeatherEvent>();

            while (listener.Pending())
            {
                FeatherEvent ev = new FeatherEvent();
                ev.client = new FeatherClient(listener.AcceptTcpClient(), this);
                ev.type = FeatherEvent.Types.CONNECT;
                
                events.Add(ev);
            }

            return events;
        }

        // CheckClients checks for incoming packets, disconnections and perhaps misc. things:

        private List<FeatherEvent> CheckClients()
        {
            List<FeatherEvent> events = new List<FeatherEvent>();

            byte[] buffer = new byte[FeatherDefaults.BUFFER_SIZE];

            foreach (FeatherClient fClient in clients)
            {
                var ev = fClient.CheckTimeout();

                if (ev.type == FeatherEvent.Types.PING_NOW)
                    fClient.pingMul++;
                else if (ev.type == FeatherEvent.Types.DISCONNECT)
                {
                    events.Add(ev);
                    continue; // dont handle this any further!
                }
            

                if (fClient.isTimedOut() || !fClient.isConnected())
                    continue; // user supposed to time-out later, receiving any packets is not permitted.

                // see what events have been collected up:

                if (fClient.incomingEvents.Count > 0 && !fClient.areWeSending)
                {
                    events.Add(fClient.incomingEvents[0]);
                    fClient.incomingEvents.RemoveAt(0);
                }

                if (ev.type != FeatherEvent.Types.NONE)
                    events.Add(ev);
            }

            return events;
        }

        private FeatherEvent[] DispatchEvents()
        {
            List<FeatherEvent> events = new List<FeatherEvent>();
            events.AddRange(CheckIncomingConnections());
            events.AddRange(CheckClients());

            return events.ToArray();
        }

        public FeatherEvent[] Service(int timeout = 1)
        {
            FeatherEvent[] events = DispatchEvents();
            foreach (FeatherEvent ev in events)
            {
                switch (ev.type)
                {
                    case FeatherEvent.Types.CONNECT:
                        clients.Add(ev.client);
                        break;

                    case FeatherEvent.Types.DISCONNECT:
                        if (ev.client == null)
                            break;

                        clients.Remove(ev.client);
                        ev.client.Free();

                        break;

                    case FeatherEvent.Types.RECEIVE:
                        ev.client.UpdateLastResponse();
                        break;

                    default:
                        break;
                }
            }

            return events;
        }
    }
}
