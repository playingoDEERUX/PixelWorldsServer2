using System;
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
        public int Port = 10001; // for quick-accessibility
        private FeatherServer fServer = null;
        private MessageHandler msgHandler = null;
        private SQLiteManager sqlManager = null;
        private WorldManager worldManager = null;
        public FeatherServer GetServer() => fServer;
        public MessageHandler GetMessageHandler() => msgHandler;
        public WorldManager GetWorldManager() => worldManager;

        [Obsolete]
        public PWServer(int port = 10001)
        {
            Port = port;
            fServer = new FeatherServer(Port);
            msgHandler = new MessageHandler(this);
            sqlManager = new SQLiteManager();
            worldManager = new WorldManager();
        }

        public SQLiteManager GetSQL() { return sqlManager; }

        public bool Start()
        {
            return fServer == null ? false : fServer.Start();
        }

        public void Host()
        {
            foreach (var ev in fServer.Service())
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
        }

        private void onDisconnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;
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
