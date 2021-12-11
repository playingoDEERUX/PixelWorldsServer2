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
            PWServer pwServer = new PWServer(10001);

            Util.Log("Pixel Worlds Server by playingo (C) 2021");

            Util.Log("Checking config...");

            if (!File.Exists("config.txt"))
            {
                using (var fs = File.Create("config.txt"))
                {

                }
                
            }

            string conf = File.ReadAllText("config.txt");

            Util.TextScanner scanner = new Util.TextScanner(conf);

            try
            {
                Util.Log($"Logon port: {scanner.GetValueFromKey<int>("logon_port")}");
                Util.Log($"Beta Msg: {scanner.GetValueFromKey<string>("enable_beta_message")}");
            }
            catch
            {

            }

            Util.Log("Checking SQLite db...");

            var pSQL = pwServer.GetSQL();
            if (pSQL.Connect())
            {
                pSQL.Query("CREATE TABLE IF NOT EXISTS players (ID INTEGER PRIMARY KEY NOT NULL)");
                pSQL.Query("CREATE TABLE IF NOT EXISTS worlds (ID INTEGER PRIMARY KEY NOT NULL)");
            }
            else
            {
                Util.Log("Error SQLite database failed! Continuing... (saving of data may not work)");
            }

            Util.Log("Starting Pixel Worlds Server...");

            if (pwServer.Start())
            {
                Util.Log("Pixel Worlds Server has been started. Hosting now!");

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
