using System;
using System.IO;
using System.Threading;
using PixelWorldsServer2.Database;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.Networking.Server;

namespace PixelWorldsServer2
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            Util.Log("Pixel Worlds Server by playingo (C) 2021");
            Util.Log("Checking config...");

            if (!File.Exists("config.txt"))
            {
                using (var fs = File.Create("config.txt"))
                {
                    Util.Log("NOTE: Created config file as it wasn't present.");
                }
            }

            string conf = File.ReadAllText("config.txt");

            Util.TextScanner scanner = new Util.TextScanner(conf);

            int port = 10001;
            try
            {
                port = scanner.GetValueFromKey<int>("port");
                Util.Log($"Server port: {port}");
                Util.Log($"Beta Msg: {scanner.GetValueFromKey<string>("enable_beta_message")}");
            }
            catch
            {

            }

            PWServer pwServer = new PWServer(port);
            Util.StartLogger(pwServer);

            Util.Log("Checking SQLite db...");

            var pSQL = pwServer.GetSQL();
            if (pSQL.Connect())
            {
                pSQL.Query("CREATE TABLE IF NOT EXISTS players (ID INTEGER PRIMARY KEY NOT NULL, " +
                    "Name varchar(24) NOT NULL DEFAULT '', " +
                    "CognitoID varchar(64) DEFAULT NULL, " +
                    "Token varchar(64) DEFAULT NULL, " +
                    "IP char(20) NOT NULL DEFAULT '0.0.0.0'," +
                    "Country char(3) NOT NULL DEFAULT '00'," +
                    "Gems int NOT NULL DEFAULT '0'," +
                    "ByteCoins int NOT NULL DEFAULT '0'," +
                    "Settings int NOT NULL DEFAULT '0'," +
                    "Inventory varbinary(6144) DEFAULT NULL," +
                    "Pass varchar(32) NOT NULL DEFAULT ''," +
                    "OPStatus int NOT NULL DEFAULT '0'," +
                    "BSON MEDIUMBLOB DEFAULT NULL)");
            }
            else
            {
                Util.Log("Error SQLite database failed! Continuing... (saving of data may not work)");
            }

            if (!Directory.Exists("maps"))
                Directory.CreateDirectory("maps");

            ItemDB.Initialize();
            Util.Log("ItemDB initialized!");

            Util.Log("Starting Pixel Worlds Server...");

            if (pwServer.Start())
            {
                Util.Log($"Pixel Worlds Server (0.1.2) has been started. Hosting now at port {pwServer.Port}!");

                Console.CancelKeyPress += delegate 
                {
                    pwServer.Shutdown();
                };
#if DEBUG
#else
                Util.Log("Initializing Discord Bot...");
                DiscordBot.Init();
                Util.Log("Discord Bot OK!");
#endif

                Util.Log("Initializing Shop...");
                Shop.Init();
                Util.Log("Shop OK!");

                while (pwServer.GetServer() != null && !pwServer.wantsShutdown)
                    pwServer.Host();

                Util.StopLogger();
            }
            else
            {
                Util.Log("ERROR! Quitting due to could not start Pixel Worlds Server!");
            }
        }
    }
}
