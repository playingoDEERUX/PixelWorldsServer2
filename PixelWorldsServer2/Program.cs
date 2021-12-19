using System;
using System.IO;
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
                    "Settings int NOT NULL DEFAULT '0')");

                pSQL.Query("CREATE TABLE IF NOT EXISTS worlds (ID INTEGER PRIMARY KEY NOT NULL, Name varchar(32) NOT NULL DEFAULT '')");
            }
            else
            {
                Util.Log("Error SQLite database failed! Continuing... (saving of data may not work)");
            }

            Util.Log("Starting Pixel Worlds Server...");

            if (pwServer.Start())
            {
                Util.Log($"Pixel Worlds Server has been started. Hosting now at port {pwServer.Port}!");

                while (pwServer.GetServer() != null)
                    pwServer.Host();
            }
            else
            {
                Util.Log("ERROR! Quitting due to could not start Pixel Worlds Server!");
            }
        }
    }
}
