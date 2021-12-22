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
        public const int PING_CLOCK_MS = 500; // essentially acts like a tickrate.
        public const int PING_MULTIPLIER = 1;
        public const int BUFFER_SIZE = 8192;
        public const int MIN_TRANSACTION_SIZE = 4;
        public const int TIMEOUT_DURATION = 15000;
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
            return !alreadyPinged() && FeatherUtil.GetTimeMs() > lastResponse + (FeatherDefaults.PING_CLOCK_MS * pingMul);
        }

        private bool alreadyPinged()
        {
            foreach (var bson in outgoingPackets)
            {
                if (!bson.Contains(MsgLabels.MessageID))
                    continue;

                if (bson[MsgLabels.MessageID] == MsgLabels.Ident.Ping)
                    return true;
            }

            return false;
        }

        public BSONObject metaObj = new BSONObject(); // reserved.
        private TcpClient client = null;
        private long lastResponse = 0;
        public int pingMul = 1;
        private object host = null;
        public object data = null; // Freely applicable data that developer may or may not use.
        private List<BSONObject> outgoingPackets = new List<BSONObject>();
        public List<FeatherEvent> incomingEvents = new List<FeatherEvent>();
        private byte[] recvBuf = new byte[FeatherDefaults.BUFFER_SIZE];

        private void OnEndRead(IAsyncResult i)
        {
            try
            {
                var pServer = (PWServer)i.AsyncState;
                var ns = client.GetStream();
                int recv = ns.EndRead(i);

                lock (pServer.locker)
                {
                    if (recv < 0 || recv >= FeatherDefaults.BUFFER_SIZE)
                    {
                        incomingEvents.Add(Disconnect());
                        return;
                    }

                    if (recv > 0)
                    {
                        var ev = Receive(recvBuf, recv);

                        BSONObject bObj = null;

                        try
                        {
                            bObj = SimpleBSON.Load(ev.packetData.Skip(4).ToArray());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("EX: " + ex.Message);
                        }

                        if (bObj != null)
                        {
                            pServer.GetMessageHandler().ProcessBSONPacket(this, bObj);
                            UpdateLastResponse();
                        }
                    }

                    if (client.Connected)
                    {
                        Flush();

                        if (!isTimedOut() && !isDisconnecting())
                            ns.BeginRead(recvBuf, 0, FeatherDefaults.BUFFER_SIZE, OnEndRead, pServer);
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (IOException) { }
            catch (InvalidOperationException) { }
            catch (SocketException) { }
        }
        public void StartReading(PWServer server)
        {
            try
            {
                var ns = client.GetStream();
                ns.BeginRead(recvBuf, 0, FeatherDefaults.BUFFER_SIZE, OnEndRead, server);
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

        private void OnEndWrite(IAsyncResult i)
        {
            try
            {
                var ns = client.GetStream();
                ns.EndWrite(i);
            }
            catch (IOException) { }
            catch (ObjectDisposedException) { }
        }

        public bool Flush()
        {
            if (client == null)
                return false;

            if (!client.Connected)
                return false;

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
                    return true; // huh? Treat it to be legal just incase anyway...

                try
                {
                    ns.BeginWrite(data, 0, data.Length, OnEndWrite, null);
                }
                catch (IOException)
                {
                    return false;
                }
            }
            return true;
        }

        public void Send(BSONObject bObj)
        {

            if (this.isConnected())
                outgoingPackets.Add(bObj);
        }

        public void Free()
        {
            this.outgoingPackets.Clear();
            this.client.Close();
        }

        public FeatherEvent Ping()
        {
            FeatherEvent ev = new FeatherEvent();
            ev.client = this;
            ev.type = FeatherEvent.Types.PING_NOW;

            return ev;
        }

        public FeatherEvent Disconnect(bool timeout = false)
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
        public FeatherEvent Receive(byte[] buffer, int len)
        {
            // No message framing is done in server side, since requests are supposed to be small.
            // If you require additional message framing, please implement so on an external layer.

            FeatherEvent ev = new FeatherEvent();
            ev.client = this;
            ev.type = FeatherEvent.Types.RECEIVE;
            ev.packetData = buffer.Take(len).ToArray();

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

                if (ev.type != FeatherEvent.Types.NONE)
                {
                    if (ev.type == FeatherEvent.Types.PING_NOW)
                        fClient.pingMul++;
                    else if (ev.type == FeatherEvent.Types.DISCONNECT)
                    {
                        events.Add(ev);
                        continue; // dont handle this any further!
                    }
                }

                if (fClient.isTimedOut() || !fClient.isConnected())
                    continue; // user supposed to time-out later, receiving any packets is not permitted.

                // see what events have been collected up:

                while (fClient.incomingEvents.Count > 0)
                {
                    events.Add(fClient.incomingEvents[0]);
                    fClient.incomingEvents.RemoveAt(0);
                }

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

                        Console.WriteLine("Client disconnects!");

                        clients.Remove(ev.client);
                        ev.client.Free();

                        break;
                    default:
                        break;
                }
            }

            return events;
        }
    }
}
