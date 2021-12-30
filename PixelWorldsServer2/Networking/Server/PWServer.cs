using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.World;

namespace PixelWorldsServer2.Networking.Server
{
    public class PWServer
    {
        public int Version = 91;
        public int Port; // for quick-accessibility
        private FeatherServer fServer = null;
        private MessageHandler msgHandler = null;
        private SQLiteManager sqlManager = null;
        private WorldManager worldManager = null;
        private AccountHelper accountHelper = null;
        public Dictionary<uint, Player> players = new Dictionary<uint, Player>();
        public object locker = new object();
        private long lastDiscordUpdateTime;
        public FeatherServer GetServer() => fServer;
        public MessageHandler GetMessageHandler() => msgHandler;
        public WorldManager GetWorldManager() => worldManager;
        public AccountHelper GetAccountHelper() => accountHelper;

        public void Shutdown()
        {
            lock (locker)
            {
                Util.Log("Server is shutting down...");

                // will call destructors:
                long ms = Util.GetMs();

                //fServer.Stop();
                worldManager.Clear();
               
                foreach (var p in players.Values)
                    p.Save();

                players.Clear();

                GC.Collect();

                Util.Log($"Shutdown finished in {Util.GetMs() - ms} ms.");
                Environment.Exit(0);
            }
        }

        public Player[] GetPlayersIngame()
        {
            List<Player> ingame = new List<Player>();

            foreach (Player player in players.Values)
            {
                if (player.isInGame)
                    ingame.Add(player);
            }

            return ingame.ToArray();
        }

        [Obsolete]
        public PWServer(int port = 10001)
        {
            Port = port;
            fServer = new FeatherServer(Port);
            sqlManager = new SQLiteManager();
            msgHandler = new MessageHandler(this);
            worldManager = new WorldManager(this);
            accountHelper = new AccountHelper(this);
        }
        public SQLiteManager GetSQL() { return sqlManager; }

        public bool Start()
        {
            return fServer == null ? false : fServer.Start();
        }

        public void Tick()
        {
            int playersOn = 0;
            foreach (var p in players.Values)
            {
                if (p.isInGame)
                {
                    playersOn++;
                    p.Tick();
                }
            }

            var clients = fServer.GetClients();
            foreach (var client in clients)
            {
                if (client.areWeSending)
                    client.Flush();
            }

            if (Util.GetSec() > lastDiscordUpdateTime + 14)
            {
                _ = DiscordBot.UpdateStatus($"Join {playersOn} other players!");
                lastDiscordUpdateTime = Util.GetSec();
            }
        }

        public bool Poll(int duration = 1)
        {
            return fServer.GetListener().Server.Poll(duration * 1000, SelectMode.SelectRead);
        }

        public void Host()
        {
            lock (locker)
            {
                var evs = fServer.Service(1);
                foreach (var ev in evs)
                {
                    switch (ev.type)
                    {
                        case FeatherEvent.Types.CONNECT:
                            onConnect(ev.client, ev.flags);
                            break;

                        case FeatherEvent.Types.DISCONNECT:
                            onDisconnect(ev.client, ev.flags);
                            break;

                        case FeatherEvent.Types.RECEIVE:
                            onReceive(ev.client, SimpleBSON.Load(ev.packetData), ev.flags);
                            break;

                        case FeatherEvent.Types.PING_NOW:
                            onPing(ev.client, ev.flags);
                            break;

                        default:
                            break;
                    }
                }

                Tick();
            }
        }

        // onPing is used for other stuff too so it's public here...
        public void onPing(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            Player p = client.data == null ? null : ((Player.PlayerData)client.data).player;

            if (p == null)
            {
                client.SendIfDoesNotContain(new BSONObject("p"));
            }
            else
            {
                p.SendPing();
            }
        }   

        private void onDisconnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            if (client.data == null)
                return;

            var pData = (Player.PlayerData)client.data;

            if (players.ContainsKey(pData.UserID))
            {


                // depends on whether we were the last instance to disconnect with that userID:
                // have to this as the player might try to relogon onto the same session.
                ushort instances = 0;
                foreach (FeatherClient fClient in fServer.GetClients())
                {
                    if (fClient.data == null)
                        continue;

                    if (((Player.PlayerData)fClient.data).UserID == pData.UserID)
                        instances++;
                }

                Player p = players[pData.UserID];
                if (p.world != null)
                {
                    GetMessageHandler().HandleLeaveWorld(p, null);
                }

                p.isInGame = instances > 0;

                if (!p.isInGame)
                {
                    Console.WriteLine("Player nowhere ingame anymore, unregistering session...");

                    p.SetClient(null);
                }
            }
        }

        private void onReceive(FeatherClient client, BSONObject packet, int flags)
        {
            if (client == null)
                return;

            msgHandler.ProcessBSONPacket(client, packet);
            client.areWeSending = true;
        }

        private void onConnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            client.StartReading(this);
        }
    }
}
