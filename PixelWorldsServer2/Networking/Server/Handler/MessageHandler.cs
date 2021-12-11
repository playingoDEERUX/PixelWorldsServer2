using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;

namespace PixelWorldsServer2.Networking.Server
{
    class MessageHandler
    {
        private PWServer pServer = null;

        public MessageHandler(PWServer pwServer)
        {
            pServer = pwServer;
        }

        public void ProcessBSONPacket(FeatherClient client, BSONObject bObj)
        {
            if (pServer == null)
            {
                Util.Log("ERROR cannot process BSON packet when pServer is null!");
                return;
            }

            if (!bObj.ContainsKey("mc"))
            {
                Console.WriteLine("Invalid bson packet (no mc!)");
                client.DisconnectLater();
                return; // Invalid Pixel Worlds BSON packet!
            }

            try
            {
                int messageCount = bObj["mc"];

                for (int i = 0; i < messageCount; i++)
                {
                    if (!bObj.ContainsKey($"m{i}"))
                        throw new Exception($"Non existing message object failed to be accessed by index '{i}'!");

                    BSONObject mObj = bObj[$"m{i}"] as BSONObject;

                    string mID = mObj[MsgLabels.MessageID];

#if DEBUG
                    Util.Log("Got message: " + mID);
#endif

                    switch (mID)
                    {
                        case MsgLabels.Ident.VersionCheck:
#if DEBUG
                            Util.Log("Client requests version check, responding now...");
#endif

                            BSONObject resp = new BSONObject();
                            resp[MsgLabels.MessageID] = MsgLabels.Ident.VersionCheck;
                            resp[MsgLabels.VersionNumberKey] = pServer.Version;
                            client.Send(resp);
                            break;

                        case MsgLabels.Ident.GetPlayerData:
                            HandlePlayerLogon(client, bObj);
                            break;

                        case MsgLabels.Ident.TryToJoinWorld:
                            HandleTryToJoinWorld(client, bObj);
                            break;

                        default:
                            client.Send(new BSONObject(MsgLabels.Ident.Ping));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Util.Log("Bad BSON packet, force disconnect! (ex: " + ex.Message + ")");
#endif
                client.DisconnectLater();
            }
        }

        public void HandlePlayerLogon(FeatherClient client, BSONObject bObj)
        {
#if DEBUG
            Util.Log("Handling player logon...");
#endif

            var bsObj = SimpleBSON.Load(File.ReadAllBytes("player.dat").Skip(4).ToArray())["m0"] as BSONObject;
            client.Send(bsObj);
        }

        public void HandleTryToJoinWorld(FeatherClient client, BSONObject bObj)
        {
            BSONObject resp = new BSONObject(MsgLabels.Ident.TryToJoinWorld);
            resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.UNAVAILABLE;

            var wmgr = pServer.GetWorldManager();
            string worldName = bObj["W"];

            WorldSession world = wmgr.GetByName(bObj[worldName], true);

            if (SQLiteManager.HasIllegalChar(worldName))
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.INVALID_NAME;
            }
            else if (world != null)
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.SUCCESS;
            }
            else
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.UNAVAILABLE;
            }

            client.Send(resp);
        }

        public void HandleSyncTime(FeatherClient client)
        {
            BSONObject resp = new BSONObject(MsgLabels.Ident.SyncTime);
            resp[MsgLabels.MessageID] = MsgLabels.Ident.SyncTime;
            resp[MsgLabels.TimeStamp] = Util.GetKukouriTime();
            resp[MsgLabels.SequencingInterval] = 60;

            client.Send(resp);
        }
    }
}
