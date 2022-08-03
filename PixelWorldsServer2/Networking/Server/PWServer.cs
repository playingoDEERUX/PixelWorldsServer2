using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.World;
using Timer = System.Timers.Timer;

namespace PixelWorldsServer2.Networking.Server
{
    public class PWServer
    {
        private readonly Timer tickTimer = new Timer(FeatherDefaults.PING_CLOCK_MS);
        public bool wantsShutdown = false;
        public int Version = 92;
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

        // return null if non existent:
        public Player GetPlayerByUserID(uint userID)
        {
            Player p = null;

            if (players.ContainsKey(userID))
            {
                p = players[userID];
            }
            else
            {
                var pSQL = GetSQL();

                var reader = pSQL.FetchQuery("SELECT * FROM players WHERE ID='" + userID.ToString() + "'");
                if (reader.Read())
                {
                    p = new Player(reader);
                    players[p.Data.UserID] = p;
                }
            }

            return p;
        }

        public string GetNameFromUserID(uint userID)
        {
            var p = GetPlayerByUserID(userID);

            return p == null ? "DeletedUser" : p.Data.Name;
        }

        public Player GetOnlinePlayerByName(string name)
        {
            string nameLower = name.ToLower();
            foreach (var p in players.Values)
            {
                if (!p.IsOnline())
                    continue;

                if (p.Data.Name.ToLower() == nameLower)
                    return p;
            }

            return null;
        }
       
        private void HandleConsoleSetRank(uint userID, Ranks rankType)
        {
            // duration is in secs here...

            if (!players.ContainsKey(userID))
            {
                Util.Log("This user isn't online. Use 'getinfo <name>' if you want to grab the userID of a player's name. (Aborted)");
                return;
            }

            Player p = players[userID];
            if (!p.IsOnline())
            {
                Util.Log("This user isn't online. Use 'getinfo <name>' if you want to grab the userID of a player's name. (Aborted)");
                return;
            }

            switch (rankType)
            {
                case Ranks.ADMIN:
                    p.pSettings.Set(PlayerSettings.Bit.SET_ADMIN);
                    break;

                case Ranks.INFLUENCER:
                    p.pSettings.Set(PlayerSettings.Bit.SET_INFLUENCER);
                    break;

                case Ranks.MODERATOR:
                    p.pSettings.Set(PlayerSettings.Bit.SET_MOD);
                    break;

                default:
                    break;
            }

            Util.Log("User rank has been set! Will request this user to reconnect...");
            
            BSONObject r = new BSONObject("DR");
            p.Send(ref r);
        }

        private void HandleConsoleGetInfo(string username)
        {
            Util.Log("Obtaining player info from username '" + username + "'...");

            var timeMs = Util.GetMs();
            var reader = sqlManager.FetchQuery("SELECT * FROM players WHERE Name='" + username + "'");

            if (reader.Read())
            {
                Player player = new Player(reader);

                uint userID = player.Data.UserID;
                if (players.ContainsKey(userID))
                {
                    player = players[userID];
                }

                Util.Log($"Result:  UserID: {userID}, Gems: {player.Data.Gems}, IP: {player.Data.LastIP}, Online: " + (player.isInGame ? $"yes (in '{player.GetWorldName()}')" : "no"));
            }
            else
            {
                Util.Log("No record(s) found.");
            }

            Util.Log($"Search took {Util.GetMs() - timeMs} milliseconds to perform.");
        }

        public void ConsoleCommand(string[] cmd)
        {
            // Process the console input command:

            lock (locker)
            {
                try
                {
                    switch (cmd[0])
                    {
                        case "?":
                        case "help":
                            Util.Log("Commands: setvip, setmod, setadmin, getinfo, stop");
                            break;

                        case "stop":
                            this.Shutdown();
                            break;

                        case "getinfo":
                            if (cmd.Length > 1)
                                HandleConsoleGetInfo(cmd[1]);

                            break;

                        case "setvip":
                            if (cmd.Length > 1)
                                HandleConsoleSetRank(uint.Parse(cmd[1]), Ranks.INFLUENCER);

                            break;

                        case "setmod":
                            if (cmd.Length > 1)
                                HandleConsoleSetRank(uint.Parse(cmd[1]), Ranks.MODERATOR);

                            break;

                        case "setadmin":

                            if (cmd.Length > 1)
                                HandleConsoleSetRank(uint.Parse(cmd[1]), Ranks.ADMIN);

                            break;

                        default:
                            Util.Log("Unknown command. Type 'help' for a list of commands.");
                            break;
                    }
                }
                catch { Util.Log("Invalid arguments!"); }
            }
        }

        public void Shutdown()
        {
            try
            {
                Util.Log("Server is shutting down...");

                // will call destructors:
                long ms = Util.GetMs();

                //fServer.Stop();
                sqlManager.Close();
                worldManager.SaveAll();
                worldManager.Clear();

                foreach (var p in players.Values)
                    p.Save();

                players.Clear();
                tickTimer.Stop();

                Util.Log($"Shutdown finished in {Util.GetMs() - ms} ms.");

                wantsShutdown = true;
                GC.KeepAlive(this);
            }
            catch (Exception ex)
            {
                Util.Log(ex.Message);
            }
        }

        public int GetPlayersIngameCount()
        {
            int c = 0;

            foreach (Player player in players.Values)
            {
                if (player.isInGame)
                    c++;
            }

            return c; // should be a more optimized version of returning the entire list.
        }

        public List<Player> GetPlayersIngame()
        {
            List<Player> ingame = new List<Player>();

            foreach (Player player in players.Values)
            {
                if (player.isInGame)
                    ingame.Add(player);
            }

            return ingame;
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
            bool started = fServer == null ? false : fServer.Start();

            if (started)
            {
                tickTimer.AutoReset = true;
                tickTimer.Elapsed += Tick;
                tickTimer.Start();
            }

            return started;
        }

        public void Broadcast(ref BSONObject bObj, params Player[] ignored)
        {
            foreach (var p in players.Values)
            {
                if (ignored.Contains(p))
                    continue;

                if (p.isInGame)
                {
                    p.Send(ref bObj);
                }
            }
        }

        public void Tick(object obj, ElapsedEventArgs e)
        {
            lock (locker)
            {
                int playersOn = 0;
                foreach (var p in players.Values)
                {
                    if (p.isInGame)
                    {
                        playersOn++;
                        
                        if (!p.isLoadingWorld)
                            p.Tick();
                    }
                }

                var clients = fServer.GetClients();
                foreach (var client in clients)
                {
                    if (client.areWeSending)
                    {
                        OnPing(client, 1);
                        client.Flush();
                    }
                }

                //worldManager.CheckAll();

                if (Util.GetSec() > lastDiscordUpdateTime + 29)
                {
                    _ = DiscordBot.UpdateStatus($"Join {playersOn} other players!");
                    lastDiscordUpdateTime = Util.GetSec();
                }
            }
        }

        public bool Poll(int duration = 1)
        {
            return fServer.GetListener().Server.Poll(duration * 1000, SelectMode.SelectRead);
        }

        public void Host()
        {
            bool sleep = false;
            lock (locker)
            {
                var evs = fServer.Service(1);
                if (evs.Length == 0)
                    sleep = true;

                foreach (var ev in evs)
                {
                    switch (ev.type)
                    {
                        case FeatherEvent.Types.CONNECT:
                            OnConnect(ev.client, ev.flags);
                            break;

                        case FeatherEvent.Types.DISCONNECT:
                            OnDisconnect(ev.client, ev.flags);
                            break;

                        case FeatherEvent.Types.RECEIVE:
                            try
                            {
                                OnReceive(ev.client, SimpleBSON.Load(ev.packetData), ev.flags);
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("Don't know elementType"))
                                    ev.client.DisconnectLater();
                            }
                            break;

                        case FeatherEvent.Types.PING_NOW:
                            break;

                        default:
                            break;
                    }
                }
            }

            if (sleep)
                Thread.Sleep(1);
        }

        // onPing is used for other stuff too so it's public here...
        public void OnPing(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            Player p = client.data == null ? null : ((Player.PlayerData)client.data).player;
            
            if (p == null)
                return;

            if (flags == 0)
            {
                p.sendPing = true; // unused for now
            }
            else
            {
                p.SendPing();
            }
        }

        private void OnDisconnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            if (client.data == null)
                return;

            var pData = (Player.PlayerData)client.data;
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

            Player p = pData.player;
            p.isInGame = instances > 0;

            if (!p.isInGame)
            {
                Util.Log("Player nowhere ingame anymore, unregistering session...");

                GetMessageHandler().HandleLeaveWorld(p, null);
                p.SetClient(null);
            }
        }

        private void OnReceive(FeatherClient client, BSONObject packet, int flags)
        {
            if (client == null)
                return;

            msgHandler.ProcessBSONPacket(client, packet);
            client.areWeSending = true;
        }

        private void OnConnect(FeatherClient client, int flags)
        {
            if (client == null)
                return;

            client.StartReading(this);
        }
    }
}
