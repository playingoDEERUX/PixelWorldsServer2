using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
        public FeatherServer GetServer() => fServer;
        public MessageHandler GetMessageHandler() => msgHandler;
        public WorldManager GetWorldManager() => worldManager;
        public AccountHelper GetAccountHelper() => accountHelper;

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
            msgHandler = new MessageHandler(this);
            sqlManager = new SQLiteManager();
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
            foreach (var p in players.Values)
            {
                if (p.isInGame)
                    p.Tick();
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

                        default:
                            break;
                    }
                }

                Tick();
            }
        }

        private void onPing(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            if (client.data != null)
            {
                var pData = (Player.PlayerData)client.data;
                pData.player.Ping();
            }
            else
            {
                client.Ping();
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

        private void onConnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            client.StartReading(this);
        }
    }
}
