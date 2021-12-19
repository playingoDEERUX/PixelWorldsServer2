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

            Player p = client.data == null ? null : ((Player.PlayerData)client.data).player; // re-retrieve the player here.

#if RELEASE
            try
            {
#endif
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
                        HandlePlayerLogon(client, mObj);
                        break;

                    case MsgLabels.Ident.TryToJoinWorld:
                        HandleTryToJoinWorld(p, mObj);
                        break;

                    case MsgLabels.Ident.GetWorld:
                        HandleGetWorld(p, mObj);
                        break;

                    case "rOP": // request other players
                    case "rAI": // request AI (bots, etc.)??
                    case "rAIp": // ??
                        client.Send(mObj); // all todo
                        break;

                    case MsgLabels.Ident.MovePlayer:
                        break;

                    default:
                        client.Send(new BSONObject(MsgLabels.Ident.Ping));
                        break;

                    }
                }
#if RELEASE
            }
        
            catch (Exception ex)
            {
                Util.Log("Bad BSON packet, force disconnect! (ex: " + ex.Message + ")");
                client.DisconnectLater();
            }
#endif
        }


        public void HandlePlayerLogon(FeatherClient client, BSONObject bObj)
        {
#if DEBUG
            Util.Log("Handling player logon...");
#endif

            string cogID = bObj[MsgLabels.CognitoId];
            string cogToken = bObj[MsgLabels.CognitoToken];

            var accHelper = pServer.GetAccountHelper();

            Player p = accHelper.LoginPlayer(cogID, cogToken, client.GetIPString());
            uint userID = p.Data.UserID;

            if (!pServer.players.ContainsKey(userID))
            {
                pServer.players[userID] = p; // just a test with userID = 0
            }
            else
            {
                p = pServer.players[userID];
            }

            p.SetClient(client); // override client...
            client.data = p.Data;
            p.isInGame = true;

            var bsObj = SimpleBSON.Load(File.ReadAllBytes("player.dat").Skip(4).ToArray())["m0"] as BSONObject;
            client.Send(bsObj);
        }

        public void HandleTryToJoinWorld(Player p, BSONObject bObj)
        {

            if (p == null)
            {
                Console.WriteLine("Player is null at TryToJoinWorld?!");
                return;
            }

            BSONObject resp = new BSONObject(MsgLabels.Ident.TryToJoinWorld);
            resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.UNAVAILABLE;

            var wmgr = pServer.GetWorldManager();
            string worldName = bObj["W"];

            WorldSession world = wmgr.GetByName(worldName, true);

            if (SQLiteManager.HasIllegalChar(worldName))
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.INVALID_NAME;
            }
            else if (world != null)
            {
#if DEBUG
                Console.WriteLine("World not null, JoinResult SUCCESS, joining world...");
#endif
                resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.SUCCESS;
            }
            else
            {
                resp[MsgLabels.JoinResult] = (int)MsgLabels.JR.UNAVAILABLE;
            }

            p.Send(ref resp);
        }

        public void HandleGetWorld(Player p, BSONObject bObj)
        {
            Console.WriteLine("Handling GetWorld... (todo)");

            string worldName = bObj["W"];
            var wmgr = pServer.GetWorldManager();

            WorldSession world = wmgr.GetByName(worldName, false);

            BSONObject resp = new BSONObject();
            BSONObject wObj = world.Serialize();

            resp[MsgLabels.MessageID] = MsgLabels.Ident.GetWorldCompressed;
            resp["W"] = Util.LZMAHelper.CompressLZMA(SimpleBSON.Dump(wObj));

            p.Send(ref resp);
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
