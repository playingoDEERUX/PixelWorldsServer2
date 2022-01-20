using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PixelWorldsServer2.World.WorldInterface;

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

#if RELEASE
            try
            {
#endif
                int messageCount = bObj["mc"];


                Player p = client.data == null ? null : ((Player.PlayerData)client.data).player;
                for (int i = 0; i < messageCount; i++)
                {
                    if (!bObj.ContainsKey($"m{i}"))
                        throw new Exception($"Non existing message object failed to be accessed by index '{i}'!");

                    BSONObject mObj = bObj[$"m{i}"] as BSONObject;
                    string mID = mObj[MsgLabels.MessageID];

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

                        case "BIPack":
                            HandleShopPurchase(p, mObj);
                            break;

                        case "RenamePlayer":
                            HandleRenamePlayer(p, mObj);
                            break;

                        case "rOP": // request other players
                            HandleSpawnPlayer(p, mObj);
                            HandleRequestOtherPlayers(p, mObj);
                            break;

                        case "GM":
                            HandleGlobalMessage(p, mObj);
                            break;

                        case "RtP":
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
                        case "XPCl":
                            break;
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

                        case MsgLabels.Ident.SyncTime:
                            HandleSyncTime(client);
                            break;

                        default:
                            pServer.onPing(client, 1);
                            break;

                    }
                }
#if RELEASE
            }

            catch (Exception ex)
            {
                Util.Log("Server exception, force disconnect! (ex: " + ex.Message + ")");
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

            if (p.Client == null)
            {
                Util.Log("Client was null, so setting it here!");
                p.SetClient(client);
                client.data = p.Data;
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
                        {
                            BSONObject r = new BSONObject("DR");
                            p.Client.Send(r);
                            p.Client.Flush();

                            p.Client.DisconnectLater();
                        }
                    }
                }
            }

            p.Data.CognitoID = cogID;
            p.Data.Token = cogToken;

            BSONObject pd = new BSONObject("pD");
            pd[MsgLabels.PlayerData.ByteCoinAmount] = p.Data.Coins;
            pd[MsgLabels.PlayerData.GemsAmount] = p.Data.Gems;
            pd[MsgLabels.PlayerData.Username] = p.Data.Name.ToUpper();
            pd[MsgLabels.PlayerData.PlayerOPStatus] = 2;
            pd[MsgLabels.PlayerData.InventorySlots] = 400;

            
           // pd["experienceAmount"] = 180000;
           // pd["xpAmount"] = 180000;

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
            resp["EmailVerified"] = true;
            resp["Email"] = p.IsUnregistered() ? "Register via /register!" : "Registered!";

            p.SetClient(client); // override client...
            client.data = p.Data;
            p.isInGame = true;

            client.Send(resp);
        }

        public string HandleCommandRegister(Player p, string[] args)
        {
            if (args.Length < 3)
                return "Usage: /register (NAME) (PASS)";

            string name = args[1], pass = args[2];

            if (SQLiteManager.HasIllegalChar(name) || SQLiteManager.HasIllegalChar(pass))
                return "Username or password has illegal character! Only letters and numbers.";

            if (pass.Length > 24 || name.Length > 24 || pass.Length < 3 || name.Length < 3)
                return "Username or Password too long or too short!";

            if (!p.IsUnregistered())
                return "You are registered already!";

            var sql = pServer.GetSQL();

            using (var read = sql.FetchQuery($"SELECT * FROM players WHERE Name='{name}'"))
            {
                if (read.HasRows)
                    return "An account with this name already exists.";
            }

            if (sql.Query($"UPDATE players SET Name='{name}', Pass='{pass}' WHERE ID='{p.Data.UserID}'") > 0)
            {
                p.Data.Name = name;
                BSONObject r = new BSONObject("DR");

                p.Send(ref r);
                return "";
            }

            return "Couldn't register right now, try again!";
        }

        public string HandleCommandLogin(Player p, string[] args)
        {
            if (args.Length < 3)
                return "Usage: /login (NAME) (PASS)";

            string name = args[1], pass = args[2];

            if (SQLiteManager.HasIllegalChar(name) || SQLiteManager.HasIllegalChar(pass))
                return "Username or password has illegal character! Only letters and numbers.";

            if (pass.Length > 24 || name.Length > 24 || pass.Length < 3 || name.Length < 3)
                return "Username or Password too long or too short!";

            if (!p.IsUnregistered())
                return "You are logged on already!";

            var sql = pServer.GetSQL();
            using (var read = sql.FetchQuery($"SELECT * FROM players WHERE Name='{name}' AND Pass='{pass}'"))
            {
                uint uID = 0;

                if (!read.HasRows)
                    return "Account does not exist or password is wrong!";

                if (!read.Read())
                    return "Account does not exist or password is wrong!";


                uID = (uint)(long)read["ID"];

                Console.WriteLine("CognitoID: " + p.Data.CognitoID + " Token: " + p.Data.Token + " UID: " + uID + " UserID: " + p.Data.UserID);

                var cmd = sql.Make("UPDATE players SET CognitoID=@CognitoID, Token=@Token WHERE ID=@ID");
                cmd.Parameters.AddWithValue("@CognitoID", p.Data.CognitoID);
                cmd.Parameters.AddWithValue("@Token", p.Data.Token);
                cmd.Parameters.AddWithValue("@ID", uID);

                if (sql.PreparedQuery(cmd) > 0 && sql.Query($"DELETE FROM players WHERE ID='{p.Data.UserID}'") > 0)
                {
                    BSONObject r = new BSONObject("DR");
                    p.Client.Send(r);
                    p.Client.Flush();

                    pServer.players.Remove(p.Data.UserID);
                    return "";
                }
            }

            return "Couldn't login right now, try again!";
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
                        res = "Commands >> /help /item /find /register /login";
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

                    case "/register":
                        res = HandleCommandRegister(p, tokens);
                        break;

                    case "/login":
                        res = HandleCommandLogin(p, tokens);
                        break;

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
                                res = $"Item {id} not found!";
                            }
                            else
                            {
                                if (id == 413)
                                {
                                    res = "This item must be bought!";
                                    break;
                                }
                                p.world.Drop(id, 999, p.Data.PosX, p.Data.PosY);

                                res = @$"Given 999 {it.name}  (ID {id}).";
                            }
                        }
                        break;

                    default:
                        break;
                }

                if (res != "")
                {
                    bObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#FF0000>System",
                        p.world.WorldName,
                        p.world.WorldName,
                        1,
                        res);

                    p.Send(ref bObj);
                }
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

        public void HandleShopPurchase(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            var id = bObj["IPId"];
            bObj["S"] = "PS";

            if (Shop.offers.ContainsKey(id))
            {
                var s = Shop.offers[id];

                if (s.items != null)
                {
                    if (p.Data.Gems >= s.price)
                    {
                        bObj["IPRs"] = s.items;

                        foreach (var item in s.items)
                        {
                            p.Data.Inventory.Add(new InventoryItem((short)item));
                        }

                        p.RemoveGems(s.price);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            bObj["IPRs2"] = new List<int>();

            p.Send(ref bObj);
        }

        public void HandleRenamePlayer(Player p, BSONObject bObj)
        {
            string username = bObj["UN"];

            p.Send(ref bObj);
        }

        public void HandleTryToJoinWorld(Player p, BSONObject bObj)
        {
            if (p == null)
            {
                Console.WriteLine("p is null");
                return;
            }

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

            HandleLeaveWorld(p, null);

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

            long kukTime = Util.GetKukouriTime();
            foreach (var player in p.world.Players)
            {
                if (player.Data.UserID == p.Data.UserID)
                    continue;

                pObj["x"] = player.Data.PosX;
                pObj["y"] = player.Data.PosY;
                pObj["t"] = Util.GetKukouriTime();
                pObj["a"] = player.Data.Anim;
                pObj["d"] = player.Data.Dir;
                List<int> spotsList = new List<int>();
                //spotsList.AddRange(player.GetSpots());

                pObj["spots"] = spotsList;
                pObj["familiar"] = 0;
                pObj["familiarName"] = "";
                pObj["familiarLvl"] = 0;
                pObj["familiarAge"] = kukTime;
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
                pObj["D"] = 0;
                pObj["VIPEndTimeAge"] = kukTime;
                pObj["IsVIP"] = false;

                p.Send(ref pObj);
            }

            p.Send(ref bObj);
        }

        public enum GlobalMessageResult
        {
            Unknown,
            Timeout,
            ConnectionFailed,
            AuthenticationFailed,
            NoMessage,
            NoSender,
            NoGems,
            Success,
        }


        public void HandleGlobalMessage(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (p.Data.Gems >= 2500)
            {
                p.RemoveGems(2500);

                var cmb = SimpleBSON.Load(Convert.FromBase64String(bObj["msg"]));

                string msg = cmb["message"].stringValue;
                if (msg.Length > 256)
                    return;

                BSONObject gObj = new BSONObject(MsgLabels.Ident.BroadcastGlobalMessage);
                gObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage($"<color=#00FFFF>Broadcast from {p.Data.Name}", p.world.WorldName, p.world.WorldName, 1, 
                   msg);

                pServer.Broadcast(ref gObj);
            }
        }

        public void HandleSpawnPlayer(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            long kukTime = Util.GetKukouriTime();
            BSONObject pObj = new BSONObject();
            pObj[MsgLabels.MessageID] = "AnP";
            pObj["x"] = p.Data.PosX;
            pObj["y"] = p.Data.PosY;
            pObj["t"] = kukTime;
            pObj["a"] = p.Data.Anim;
            pObj["d"] = p.Data.Dir;
            List<int> spotsList = new List<int>();
            //spotsList.AddRange(Enumerable.Repeat(0, 35));

            pObj["spots"] = spotsList;
            pObj["familiar"] = 0;
            pObj["familiarName"] = "";
            pObj["familiarLvl"] = 0;
            pObj["familiarAge"] = kukTime;
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
            pObj["VIPEndTimeAge"] = kukTime;
            pObj["IsVIP"] = false;

            p.world.Broadcast(ref pObj, p);

            BSONObject cObj = new BSONObject("WCM");
            
            cObj[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#00FF00>Credits",
                    p.world.WorldName,
                    p.world.WorldName,
                    1,
                    "PWPS by RynioBrothers, discord.gg/bxF65jx7Vs");

            p.Send(ref cObj);
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
            List<string> worldNames = new List<string>();
            List<int> playerCounts = new List<int>();

            foreach (var world in pServer.GetWorldManager().GetWorlds())
            {
                int pC = world.Players.Count;
                if (pC > 0)
                {
                    worldNames.Add(world.WorldName);
                    playerCounts.Add(pC);
                }
            }

            resp["W"] = worldNames;
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
            if (p == null)
                return;

            if (p.world == null)
                return;

            if (!p.world.collectables.ContainsKey(colID))
                return;

            BSONObject resp = new BSONObject();
            resp["ID"] = "C";
            resp["CollectableID"] = colID;

            var c = p.world.collectables[colID];
            resp["BlockType"] = c.item;
            resp["Amount"] = c.amt; // HACK
            resp["InventoryType"] = ItemDB.GetByID(c.item).type;
            resp["PosX"] = c.posX;
            resp["PosY"] = c.posY;
            resp["IsGem"] = c.gemType > -1;
            resp["GemType"] = c.gemType < 0 ? 0 : c.gemType;

            if (c.gemType < 0)
            {
                p.Data.Inventory.Add(new InventoryItem((short)c.item,
                ItemDB.IsWearable(c.item) ? (short)ItemFlags.IS_WEARABLE : (short)0, c.amt));
            }
            else
            {
                int gemsToGive = 0;
                switch ((GemType)c.gemType)
                {
                    case GemType.Gem1:
                        gemsToGive = 1;
                        break;

                    case GemType.Gem2:
                        gemsToGive = 5;
                        break;

                    case GemType.Gem3:
                        gemsToGive = 20;
                        break;

                    case GemType.Gem4:
                        gemsToGive = 50;
                        break;

                    case GemType.Gem5:
                        gemsToGive = 100;
                        break;

                    default:
                        break;
                }

                p.Data.Gems += gemsToGive;
            }

            p.world.RemoveCollectable(colID, p);
            p.Send(ref resp);
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

                if ((w.OwnerID > 0 && w.OwnerID != p.Data.UserID))
                {
                    p.SelfChat("World is owned by someone else!");
                    return;
                }

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
                    tile.fg.damage = 0;

                    double pX = x / Math.PI, pY = y / Math.PI;

                    for (int i = 0; i < 5; i++)
                        w.Drop(0, 1, pX - 0.1 + Util.rand.NextDouble(0, 0.2), pY - 0.1 + Util.rand.NextDouble(0, 0.2), Util.rand.Next(5));
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

                if ((p.world.OwnerID > 0 && p.world.OwnerID != p.Data.UserID))
                {
                    p.SelfChat("World is owned by someone else!");
                    return;
                }

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

                    double pX = x / Math.PI, pY = y / Math.PI;

                    if (tile.fg.id == (short)WorldInterface.BlockType.LockWorld)
                    {
                        w.OwnerID = 0;
                        w.Drop(tile.fg.id, 1, pX, pY);
                        HandleCollect(p, w.colID);
                    }

                    for (int i = 0; i < 5; i++)
                        w.Drop(0, 1, pX - 0.1 + Util.rand.NextDouble(0, 0.2), pY - 0.1 + Util.rand.NextDouble(0, 0.2), Util.rand.Next(5));

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

            var invIt = p.Data.Inventory.Get(blockType);
            if (invIt == null)
                return;

            if (invIt.amount <= 0)
                return;

            if ((p.world.OwnerID > 0 && p.world.OwnerID != p.Data.UserID))
            {
                p.SelfChat("World is owned by someone else!");
                return;
            }

            if ((BlockType)blockType == BlockType.LockWorld)
                p.world.OwnerID = p.Data.UserID; // set world owner!

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

            p.Data.Inventory.Remove(new InventoryItem(blockType));
        }

        public void HandleSetBackgroundBlock(Player p, BSONObject bObj)
        {
            if (p == null)
                return;

            if (p.world == null)
                return;

            var w = p.world;

            if ((w.OwnerID > 0 && w.OwnerID != p.Data.UserID))
            {
                p.SelfChat("World is owned by someone else!");
                return;
            }

            int x = bObj["x"], y = bObj["y"];
            short blockType = (short)bObj["BlockType"];
            Item it = ItemDB.GetByID(blockType);

            var invIt = p.Data.Inventory.Get(blockType);
            if (invIt == null)
                return;

            if (invIt.amount <= 0)
                return;

            bObj["U"] = p.Data.UserID.ToString("X8");


            var t = w.GetTile(x, y);
            t.bg.id = blockType;
            t.bg.damage = 0;
            t.bg.lastHit = 0;

            w.Broadcast(ref bObj);

            p.Data.Inventory.Remove(new InventoryItem(blockType));
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
