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
    public class MessageHandler
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

                    /*if (metaID != mID)
                    {
                        client.metaObj[MsgLabels.MessageID] = mID;
                        client.metaObj["c"] = 0;
                    }
                    else
                    {
                        client.metaObj["c"]++;
                    }*/
                    
                    //Util.Log("Got message: " + mID);

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

                    case "WCM":
                        HandleWorldChatMessage(p, mObj);
                        break;

                    case "MWli":
                        HandleMoreWorldInfo(p, mObj);
                        break;

                    case "PSicU":
                        HandlePlayerStatusChange(p, mObj);
                        break;

                    case "rOP": // request other players
                        HandleRequestOtherPlayers(p, mObj);
                        break;

                    case "RtP":
                        HandleSpawnPlayer(p, mObj);
                        break;

                    case MsgLabels.Ident.LeaveWorld:
                        HandleLeaveWorld(p, mObj);
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

                    case "HA":
                        if (p != null)
                        {
                            bObj["U"] = p.Data.UserID;

                            if (p.world != null)
                                p.world.Broadcast(ref bObj, p);
                        }
                        break;

                    case MsgLabels.Ident.SetBlock:
                        HandleSetBlock(p, mObj);
                        break;

                    case MsgLabels.Ident.HitBlock:
                        HandleHitBlock(p, mObj);
                        break;

                    case MsgLabels.Ident.HitBackgroundBlock:
                        HandleHitBackground(p, mObj);
                        break;

                    case "A":
                        client.Send(mObj);
                        break;

                    case "GSb":
                        break;

                    case MsgLabels.Ident.SyncTime:
                        HandleSyncTime(client);
                        break;

                    default:
                        break;

                    }
                }

                client.Send(new BSONObject(MsgLabels.Ident.Ping));
#if RELEASE
            }
        
            catch (Exception ex)
            {
                Util.Log("Bad BSON packet, force disconnect! (ex: " + ex.Message + ")");
                client.DisconnectLater();
            }
#endif
        }

        private byte[] playerDataTemp = File.ReadAllBytes("player.dat").Skip(4).ToArray(); // template for playerdata, too painful to reverse rn so I am just gonna modify whats needed.
        public void HandlePlayerLogon(FeatherClient client, BSONObject bObj)
        {
#if DEBUG
            Util.Log("Handling player logon...");
#endif

            string cogID = bObj[MsgLabels.CognitoId];
            string cogToken = bObj[MsgLabels.CognitoToken];

            var resp = SimpleBSON.Load(playerDataTemp)["m0"] as BSONObject;
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
                    return;
                }
            }

            BSONObject pd = new BSONObject("pD");
            pd[MsgLabels.PlayerData.ByteCoinAmount] = p.Data.Coins;
            pd[MsgLabels.PlayerData.GemsAmount] = p.Data.Gems;
            pd[MsgLabels.PlayerData.Username] = p.Data.Name.ToUpper();
            pd[MsgLabels.PlayerData.PlayerOPStatus] = 2;
            pd[MsgLabels.PlayerData.InventorySlots] = 400;

            if (p.Data.Inventory.Items.Count == 0)
            {
                p.Data.Inventory.Items.Add(new InventoryItem(605, 0, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(869, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(870, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(871, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(890, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(1018, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(1019, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(4266, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(4267, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(4268, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(4269, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(4093, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(4266, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(2152, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(1412, (short)ItemFlags.IS_WEARABLE, 9999));
                p.Data.Inventory.Items.Add(new InventoryItem(3175, (short)ItemFlags.IS_WEARABLE, 9999));
            }

            using (var stream = new MemoryStream())
            {
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var item in p.Data.Inventory.Items)
                    {
                        bw.Write(item.itemID);
                        bw.Write(item.flags);
                        bw.Write(item.amount);
                    }
                }
                pd["inv"] = stream.ToArray();
            }

            pd["tutorialState"] = 3;
            //pd["tutorialQuestCompleteState"] = 1;
            resp["rUN"] = p.Data.Name;
            resp["pD"] = SimpleBSON.Dump(pd);
            resp["U"] = p.Data.UserID.ToString("X8");
            resp["Wo"] = "PIXELSTATION";

            p.SetClient(client); // override client...
            client.data = p.Data;
            p.isInGame = true;

            client.Send(resp);
        }

        public void HandleWorldChatMessage(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            bObj[MsgLabels.MessageID] = "WCM";
            bObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage(p.Data.Name, p.Data.UserID.ToString("X8"), "#" + p.world.WorldName, 0, bObj["msg"]);
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleMoreWorldInfo(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            var w = pServer.GetWorldManager().GetByName(bObj["WN"]);

            bObj[MsgLabels.Count] = w == null ? 0 : w.Players.Count;
            p.Send(ref bObj);
        }

        public void HandlePlayerStatusChange(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;


            bObj["U"] = p.Data.UserID;
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleTryToJoinWorld(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            Console.WriteLine($"Player with userID: { p.Data.UserID.ToString() } is trying to join a world [{pServer.GetPlayersIngame().Length} players online!]...");

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
            if (p == null)
                return;

            if (p.world != null)
            {
                HandleLeaveWorld(p, null);
            }

            string worldName = bObj["W"];
            var wmgr = pServer.GetWorldManager();

            WorldSession world = wmgr.GetByName(worldName, true);
            world.AddPlayer(p);

            BSONObject resp = new BSONObject();
            BSONObject wObj = world.Serialize();

            resp[MsgLabels.MessageID] = MsgLabels.Ident.GetWorldCompressed;
            resp["W"] = Util.LZMAHelper.CompressLZMA(SimpleBSON.Dump(wObj));

            p.Send(ref resp);
        }

        public void HandleLeaveWorld(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            BSONObject resp = new BSONObject("PL");
            resp[MsgLabels.UserID] = p.Data.UserID.ToString("X8");

            p.world.Broadcast(ref resp, p);
            
            if (bObj != null)
                p.Send(ref bObj);

            p.world.RemovePlayer(p);

            Console.WriteLine($"Player with UserID {p.Data.UserID} left the world!");
        }

        public void HandleRequestOtherPlayers(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            BSONObject pObj = new BSONObject("AnP");
            foreach (var player in p.world.Players)
            {
                if (player == p)
                    continue;

                pObj["x"] = player.Data.PosX;
                pObj["y"] = player.Data.PosY;
                pObj["t"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                pObj["a"] = player.Data.Anim;
                pObj["d"] = player.Data.Dir;
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
            if (p == null)
                return;

            if (p.world == null)
                return;

            BSONObject pObj = new BSONObject();
            pObj[MsgLabels.MessageID] = "AnP";
            pObj["x"] = p.Data.PosX;
            pObj["y"] = p.Data.PosY;
            pObj["t"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            pObj["a"] = p.Data.Anim;
            pObj["d"] = p.Data.Dir;
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
            if (p == null)
                return;

            p.Send(ref bObj);
        }

        public void HandleRequestAIp(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            p.Send(ref bObj);
        }
        public void HandleHitBackground(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            var tile = w.GetTile(x, y);

            BSONObject resp = new BSONObject("DB");
   
            if (tile != null)
            {
                if (tile.bg.id <= 0)
                    return;

                if (++tile.bg.damage > 2)
                {
                    resp[MsgLabels.DestroyBlockBlockType] = (int)tile.bg.id;
                    resp[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
                    resp["x"] = x;
                    resp["y"] = y;
                    w.Broadcast(ref resp);

                    tile.bg.id = 0;
                }
            }
        }

        public void HandleHitBlock(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

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

                if (++tile.fg.damage > 2)
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
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            short blockType = (short)bObj["BlockType"];
            Item it = ItemDB.GetByID(blockType);
           
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
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (bObj.ContainsKey("x") &&
                bObj.ContainsKey("y") &&
                bObj.ContainsKey("a") &&
                bObj.ContainsKey("d") &&
                bObj.ContainsKey("t"))

            {
                p.Data.PosX = bObj["x"].doubleValue;
                p.Data.PosY = bObj["y"].doubleValue;

                p.Data.Anim = bObj["a"];
                p.Data.Dir = bObj["d"];
                bObj["U"] = p.Data.UserID.ToString("X8");

                if (bObj.ContainsKey("tp"))
                    bObj.Remove("tp");

                p.world.Broadcast(ref bObj, p);
            }
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
