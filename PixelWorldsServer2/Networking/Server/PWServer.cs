﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.World;

namespace PixelWorldsServer2.Networking.Server
{
    class PWServer
    {
        public int Version = 91;
        public int Port; // for quick-accessibility
        private FeatherServer fServer = null;
        private MessageHandler msgHandler = null;
        private SQLiteManager sqlManager = null;
        private WorldManager worldManager = null;
        private AccountHelper accountHelper = null;
        public Dictionary<uint, Player> players = new Dictionary<uint, Player>();
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

        public void Host()
        {
            var evs = fServer.Service(16);
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

                    case FeatherEvent.Types.PING_NOW:
                        ev.client.Send(new BSONObject("p"));
                        break;

                    case FeatherEvent.Types.RECEIVE:
                        {
                            BSONObject bObj = null;

                            try
                            {
                                bObj = SimpleBSON.Load(ev.packetData.Skip(4).ToArray());
                            }
                            catch (Exception ex) 
                            {
#if DEBUG
                                Util.Log(ex.Message);
#endif
                                break; 
                            }

                            if (bObj != null)
                                onReceive(ev.client, bObj, ev.flags);
                        }
                        break;

                    default:
                        break;
                }
            }

            Tick();
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
        }

        private void onConnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            
        }
    }
}
