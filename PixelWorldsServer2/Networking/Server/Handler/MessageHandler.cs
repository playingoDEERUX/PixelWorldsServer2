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
                    string metaID = "";

                    if (client.metaObj.ContainsKey(MsgLabels.MessageID))
                    {
                        metaID = client.metaObj[MsgLabels.MessageID];
                    }

                    if (metaID != mID)
                    {
                        client.metaObj[MsgLabels.MessageID] = mID;
                        client.metaObj["c"] = 0;
                    }
                    else
                    {
                        client.metaObj["c"]++;
                    }
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
                        HandleRequestOtherPlayers(p, mObj);
                        break;

                    case "rAI": // request AI (bots, etc.)??
                        HandleRequestAI(p, mObj);
                        break;

                    case "rAIp": // ??
                        HandleRequestAIp(p, mObj);
                        break;

                    case MsgLabels.Ident.MovePlayer:
                        HandleMovePlayer(p, mObj);
                        break;

                    case MsgLabels.Ident.SetBlock:
                        HandleSetBlock(p, mObj);
                        break;

                    case MsgLabels.Ident.HitBlock:
                        HandleHitBlock(p, mObj);
                        break;

                    case "MWli":
                        
                        break;

                    case "A":
                        client.Send(mObj);
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

            var bsObj = SimpleBSON.Load(File.ReadAllBytes("player.dat").Skip(4).ToArray())["m0"] as BSONObject;
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

                if (p.isInGame)
                {
                    Console.WriteLine("Account is online already, notifying error...");
                    client.DisconnectLater();
                }
            }

            p.SetClient(client); // override client...
            client.data = p.Data;
            p.isInGame = true;

            client.Send(bsObj);
        }

        public void HandleTryToJoinWorld(Player p, BSONObject bObj)
        {

            if (p == null)
            {
                Console.WriteLine("Player is null at TryToJoinWorld?!");
                return;
            }

            int req = p.Client.metaObj["c"].int32Value;

            Console.WriteLine("Req: " + req.ToString());

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
            if (p.world != null)
                p.world.RemovePlayer(p);

            string worldName = bObj["W"];
            var wmgr = pServer.GetWorldManager();

            WorldSession world = wmgr.GetByName(worldName, false);
            world.AddPlayer(p);

            BSONObject resp = new BSONObject();
            BSONObject wObj = world.Serialize();

            resp[MsgLabels.MessageID] = MsgLabels.Ident.GetWorldCompressed;
            resp["W"] = Util.LZMAHelper.CompressLZMA(SimpleBSON.Dump(wObj));

            p.Send(ref resp);
        }

        public void HandleRequestOtherPlayers(Player p, BSONObject bObj)
        {
            if (p.world == null)
                return;

            BSONObject pObj = new BSONObject("AnP");
            foreach (var player in p.world.Players)
            {
                pObj["x"] = player.Data.Coords.X;
                pObj["y"] = player.Data.Coords.Y;
                pObj["t"] = Util.GetKukouriTime();
                pObj["a"] = player.Data.Coords.Z;
                pObj["d"] = player.Data.Coords.W;
                List<int> spotsList = new List<int>();
                //spotsList.AddRange(Enumerable.Repeat(0, 35));

                pObj["spots"] = spotsList;
                pObj["familiar"] = 0;
                pObj["familiarName"] = "";
                pObj["familiarLvl"] = 0;
                pObj["familiarAge"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                pObj["isFamiliarMaxLevel"] = false;
                pObj["UN"] = player.Data.Name;
                pObj["U"] = player.Data.UserID.ToString("X8");
                pObj["Age"] = 69;
                pObj["LvL"] = 99;
                pObj["xpLvL"] = 99;
                pObj["pAS"] = 0;
                pObj["PlayerAdminEditMode"] = false;
                pObj["Ctry"] = 999;
                pObj["GAmt"] = player.Data.Gems;
                pObj["ACo"] = 0;
                pObj["QCo"] = 0;
                pObj["Gnd"] = 0;
                pObj["skin"] = 7;
                pObj["faceAnim"] = 0;
                pObj["inPortal"] = false;
                pObj["SIc"] = 0;
                pObj["VIPEndTimeAge"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                pObj["IsVIP"] = false;

                p.Send(ref pObj);
            }

            p.Send(ref bObj);
        }

        public void HandleSpawnPlayer(Player p, BSONObject bObj)
        {
            if (p.world == null)
                return;

            BSONObject pObj = new BSONObject();
            pObj[MsgLabels.MessageID] = "AnP";
            pObj["x"] = p.Data.Coords.X;
            pObj["y"] = p.Data.Coords.Y;
            pObj["t"] = Util.GetKukouriTime();
            pObj["a"] = p.Data.Coords.Z;
            pObj["d"] = p.Data.Coords.W;
            List<int> spotsList = new List<int>();
            //spotsList.AddRange(Enumerable.Repeat(0, 35));

            pObj["spots"] = spotsList;
            pObj["familiar"] = 0;
            pObj["familiarName"] = "";
            pObj["familiarLvl"] = 0;
            pObj["familiarAge"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            pObj["isFamiliarMaxLevel"] = false;
            pObj["UN"] = p.Data.Name;
            pObj["U"] = p.Data.UserID.ToString("X8");
            pObj["Age"] = 69;
            pObj["LvL"] = 99;
            pObj["xpLvL"] = 99;
            pObj["pAS"] = 0;
            pObj["PlayerAdminEditMode"] = false;
            pObj["Ctry"] = 999;
            pObj["GAmt"] = p.Data.Gems;
            pObj["ACo"] = 0;
            pObj["QCo"] = 0;
            pObj["Gnd"] = 0;
            pObj["skin"] = 7;
            pObj["faceAnim"] = 0;
            pObj["inPortal"] = false;
            pObj["SIc"] = 0;
            pObj["VIPEndTimeAge"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            pObj["IsVIP"] = false;

            p.world.Broadcast(ref pObj, p);
        }

        public void HandleRequestAI(Player p, BSONObject bObj)
        {
            p.Send(ref bObj);
        }

        public void HandleRequestAIp(Player p, BSONObject bObj)
        {
            p.Send(ref bObj);
        }

        public void HandleHitBlock(Player p, BSONObject bObj)
        {
            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            var tile = w.GetTile(x, y);

            BSONObject resp = new BSONObject("DB");
           
            if (tile != null)
            {
                if (tile.fg.id <= 0)
                    return;

                if (++tile.fg.damage > 3)
                {
                    resp[MsgLabels.DestroyBlockBlockType] = (int)tile.fg.id;
                    resp[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
                    resp["x"] = x;
                    resp["y"] = y;
                    w.Broadcast(ref resp);

                    tile.fg.id = 0;
                }
            }
        }

        public void HandleSetBlock(Player p, BSONObject bObj)
        {
            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            short blockType = (short)bObj["BlockType"];
            ItemDB.Item it = ItemDB.GetByID(blockType);
           
            bObj["U"] = p.Data.UserID;

            switch (it.type)
            {
                case 3:
                    {
                        bObj[MsgLabels.MessageID] = MsgLabels.Ident.SetBlockWater;

                        var t = w.GetTile(x, y);
                        t.water.id = blockType;
                        t.water.damage = 0;
                        t.water.lastHit = 0;

                        w.Broadcast(ref bObj);
                        break;
                    }
                default:
                    {

                        var t = w.GetTile(x, y);
                        t.fg.id = blockType;
                        t.fg.damage = 0;
                        t.fg.lastHit = 0;

                        w.Broadcast(ref bObj);
                        break;
                    }
            }
        }
        public void HandleMovePlayer(Player p, BSONObject bObj)
        {
            if (p.world == null)
                return;

            p.world.Broadcast(ref bObj, p);
            p.Client.Send(new BSONObject(MsgLabels.Ident.Ping)); // ok well this is a hack to make the game dispatch packets instantly when in a world.
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
