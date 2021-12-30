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

                    //if (mID.ToLower() != "mp")
                        //Console.WriteLine("Got message: " + mID);

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

                    case "TTJWR":
                        HandleTryToJoinWorldRandom(p);
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
                        HandleSpawnPlayer(p, mObj);
                        HandleRequestOtherPlayers(p, mObj);
                        break;

                    case "RtP":
                        if (p != null)
                            p.Send(ref mObj);

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

                    case "Rez":
                        if (p == null)
                            break;

                        if (p.world == null)
                            break;

                        mObj["U"] = p.Data.UserID.ToString("X8");
                        p.world.Broadcast(ref mObj, p);
                        break;

                    case MsgLabels.Ident.WearableUsed:
                        HandleWearableUsed(p, mObj);
                        break;
                    case MsgLabels.Ident.WearableRemoved:
                        HandleWearableRemoved(p, mObj);
                        break;

                    case "C":
                        HandleCollect(p, mObj["CollectableID"]);
                        break;

                    case "RsP":
                        HandleRespawn(p, mObj);
                        break;

                    case "GAW":
                        HandleGetActiveWorlds(p);
                        break;

                    case "TDmg":
                        {
                            if (p != null)
                            {
                                if (p.world != null)
                                {

                                    BSONObject rsp = new BSONObject();
                                    rsp["ID"] = "UD";
                                    rsp["U"] = p.Data.UserID.ToString("X8");
                                    rsp["x"] = p.world.SpawnPointX;
                                    rsp["y"] = p.world.SpawnPointY;
                                    rsp["DBl"] = 0;
                                    p.world.Broadcast(ref rsp);
                                    p.Send(ref mObj);
                                }
                            }
                            break;
                        }
                    case "PDC":
                        {
                            if (p != null)
                            {
                                if (p.world != null)
                                {
                                    BSONObject rsp = new BSONObject();
                                    rsp["ID"] = "UD";
                                    rsp["U"] = p.Data.UserID.ToString("X8");
                                    rsp["x"] = p.world.SpawnPointX;
                                    rsp["y"] = p.world.SpawnPointY;
                                    rsp["DBl"] = 0;
                                    p.world.Broadcast(ref rsp);
                                    p.Send(ref mObj);
                                }
                            }
                            break;
                        }

                    case MsgLabels.Ident.MovePlayer:
                        HandleMovePlayer(p, mObj);
                        break;

                    case MsgLabels.Ident.SetBlock:
                        HandleSetBlock(p, mObj);
                        break;

                    case MsgLabels.Ident.SetBackgroundBlock:
                        HandleSetBackgroundBlock(p, mObj);
                        break;

                    case MsgLabels.Ident.HitBlock:
                        HandleHitBlock(p, mObj);
                        break;

                    case MsgLabels.Ident.HitBackgroundBlock:
                        HandleHitBackground(p, mObj);
                        break;

                    default:
                        pServer.onPing(client, 0);
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
            if (p == null)
            {
                Util.Log("Player was null upon logon!!");
                client.DisconnectLater();
                return;
            }

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
                    Console.WriteLine("Account is online already, disconnecting current client...");
                    if (p.Client != null)
                    {
                        if (p.Client.isConnected())
                            p.Client.DisconnectLater();
                    }
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
                p.Data.Inventory.InitFirstSetup();
            }

            pd["inv"] = p.Data.Inventory.Serialize();
            pd["tutorialState"] = 3;
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

            string msg = bObj["msg"];

            string[] tokens = msg.Split(" ");
            int tokCount = tokens.Count();
            
            if (tokCount <= 0)
                return;

            if (tokens[0] == "")
                return;

            if (tokens[0][0] == '/')
            {
                string res = "Unknown command.";
                switch (tokens[0])
                {
                    case "/help":
                        res = "Commands >> /help /item /find";
                        break;

                    case "/find":
                        {
                            if (tokCount < 2)
                            {
                                res = "Usage: /find (ITEM NAME)";
                                break;
                            }

                            string item_query = tokens[1];

                            if (item_query.Length < 2)
                            {
                                res = "Please enter an item name with more than 2 characters!";
                                break;
                            }

                            var items = ItemDB.FindByAnyName(item_query);

                            if (items.Length > 0)
                            {
                                string found = "";

                                foreach (var it in items)
                                {
                                    found += $"\nItem Name: {it.name}   ID: {it.ID}";
                                }

                                res = $"Found items:{found}";
                            }
                            else
                            {
                                res = $"No item containing '{item_query}' was found.";
                            }
                            break;
                        }

                    case "/item":
                        if (tokCount < 2)
                        {
                            res = "Usage: /item (ITEM ID)";
                        }
                        else
                        {
                            int id;
                            int.TryParse(tokens[1], out id);

                            var it = ItemDB.GetByID(id);

                            if (it.ID < 0)
                            {
                                res = $"Item {id} not found!.";
                            }
                            else
                            {
                                BSONObject cObj = new BSONObject();
                                cObj["ID"] = "C";
                                cObj["CollectableID"] = 1;
                                cObj["BlockType"] = id;
                                cObj["Amount"] = 999; // HACK
                                cObj["InventoryType"] = it.type;
                                cObj["PosX"] = p.Data.PosX * 3.181d; /*cld.posX = world.spawnPointX / 3.125d;
            cld.posY = world.spawnPointY / 3.181d;*/
                                cObj["PosY"] = p.Data.PosY * 3.181d;
                                cObj["IsGem"] = false;
                                cObj["GemType"] = 0;
                                
                                p.Send(ref cObj);

                                p.Data.Inventory.Items.Add(
                                    new InventoryItem((short)id,
                                    ItemDB.IsWearable(id) ? (short)ItemFlags.IS_WEARABLE : (short)0,
                                    999));

                                res = @$"Given 9999 {it.name}  (ID {id}).";
                            }
                        }
                        break;

                    default:
                        break;
                }

                bObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#FF0000>System",
                    p.world.WorldID.ToString("X8"),
                    p.world.WorldName,
                    1,
                    res);

                p.Send(ref bObj);
            }
            else
            {
                bObj[MsgLabels.MessageID] = "WCM";
                bObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage(p.Data.Name, p.Data.UserID.ToString("X8"), "#" + p.world.WorldName, 0, msg);
                p.world.Broadcast(ref bObj, p);
            }
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


            bObj["U"] = p.Data.UserID.ToString("X8");
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
                if (player.Data.UserID == p.Data.UserID)
                    continue;

                Console.WriteLine("Got userID (rOP): " + player.Data.UserID.ToString("X8"));

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

            Console.WriteLine("UserID: " + p.Data.UserID.ToString("X8"));

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

        public void HandleGetActiveWorlds(Player p)
        {
            if (p == null)
                return;

            BSONObject resp = new BSONObject("GAW");

            List<string> worldIDs = new List<string>();
            List<string> worldNames = new List<string>();
            List<int> playerCounts = new List<int>();

            foreach (var world in pServer.GetWorldManager().GetWorlds())
            {
                int pC = world.Players.Count;
                if (pC > 0)
                {
                    worldIDs.Add(world.WorldID.ToString("X8"));
                    worldNames.Add(world.WorldName);
                    playerCounts.Add(pC);
                }
            }

            resp["W"] = worldIDs;
            resp["WN"] = worldNames;
            resp["Ct"] = playerCounts;
            p.Send(ref resp);
        }

        public void HandleRequestAIp(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            p.Send(ref bObj);
        }

        public void HandleWearableUsed(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            int id = bObj["hBlock"];

            if (id < 0 || id >= ItemDB.ItemsCount())
                return;

            Item it = ItemDB.GetByID(id);

            bObj[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleCollect(Player p, int colID)
        {

        }

        public void HandleWearableRemoved(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            int id = bObj["hBlock"];

            if (id < 0 || id >= ItemDB.ItemsCount())
                return;

            Item it = ItemDB.GetByID(id);


            bObj[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
            p.world.Broadcast(ref bObj, p);
        }

        public void HandleTryToJoinWorldRandom(Player p)
        {
            var worlds = pServer.GetWorldManager().GetWorlds();

            if (worlds.Count > 0)
            {
                var w = worlds[new Random().Next(worlds.Count)];

                BSONObject bObj = new BSONObject();
                bObj["W"] = w.WorldName;

                HandleTryToJoinWorld(p, bObj);
            }
        }

        public void HandleRespawn(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            BSONObject resp = new BSONObject();
            resp[MsgLabels.MessageID] = "UD";
            resp[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
            resp["x"] = w.SpawnPointX;
            resp["y"] = w.SpawnPointY;
            resp["DBl"] = 0;

            w.Broadcast(ref resp);
            p.Send(ref bObj);
        }
        public void HandleHitBackground(Player p, BSONObject bObj)
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
                if (tile.bg.id <= 0)
                    return;

                if (Util.GetSec() > tile.bg.lastHit + 4)
                {
                    tile.bg.damage = 0;
                }

                if (++tile.bg.damage > 2)
                {
                    resp[MsgLabels.DestroyBlockBlockType] = (int)tile.bg.id;
                    resp[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
                    resp["x"] = x;
                    resp["y"] = y;
                    w.Broadcast(ref resp);

                    tile.bg.id = 0;
                }

                tile.bg.lastHit = Util.GetSec();
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
                if (tile.fg.id <= 0 || tile.fg.id == 110)
                    return;

                if (Util.GetSec() > tile.fg.lastHit + 4)
                {
                    tile.fg.damage = 0;
                }

                if (++tile.fg.damage > 2)
                {
                    resp[MsgLabels.DestroyBlockBlockType] = (int)tile.fg.id;
                    resp[MsgLabels.UserID] = p.Data.UserID.ToString("X8");
                    resp["x"] = x;
                    resp["y"] = y;
                    w.Broadcast(ref resp);

                    tile.fg.id = 0;
                    tile.fg.damage = 0;
                }

                tile.fg.lastHit = Util.GetSec();
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

            if (blockType == 273)
                return;

            Item it = ItemDB.GetByID(blockType);
            bObj["U"] = p.Data.UserID.ToString("X8");

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

        public void HandleSetBackgroundBlock(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            int x = bObj["x"], y = bObj["y"];
            short blockType = (short)bObj["BlockType"];
            Item it = ItemDB.GetByID(blockType);

            bObj["U"] = p.Data.UserID.ToString("X8");


            var t = w.GetTile(x, y);
            t.bg.id = blockType;
            t.bg.damage = 0;
            t.bg.lastHit = 0;

            w.Broadcast(ref bObj);
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
