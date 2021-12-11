using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorldsServer2.DataManagement
{
    interface ConfigManager
    {

        // 8 bit settings:
        // 1st bit -> host server locally (127.0.0.1) yes/no
        // 2nd bit -> toggle subserver support
        // 3rd bit -> toggle locking support
        // 4th bit -> toggle NPC support
        // 5th bit -> toggle world generation support
        // 6th bit -> reserved
        // 7th bit -> reserved
        // 8th bit -> reserved
        struct ServerConfiguration 
        {
            public ushort serverPort;
            public ushort gameVersion; // is necessary to even allow for login
            public short playerLimit; // if -1, allow infinite / have no block, but will be capped by general IPV4/TCP standards to 65535 anyway and bottlenecked even earlier by hardware
            public string dbHost;
            public string dbPass;
        };

        public enum SettingsFlag
        {
            HOST_LOCALLY,
            MULTI_SERVER_SUPPORT,
            TILE_LOCKING,
            NPC_SUPPORT,
            UNIQUE_WORLD
        }

        static string GetAsPrintable(ServerConfiguration config)
        {
            string toPrint = string.Empty;

            toPrint += $"\n\tServer Port: {config.serverPort}\n";
            toPrint += $"\n\tGame Version: {config.gameVersion}\n";
            toPrint += $"\n\tPlayer Max Limit: {config.playerLimit}\n";
            toPrint += $"\n\tDatabase host: {config.dbHost}\n";

            return toPrint;
        }
        static string GetAsString(ServerConfiguration config)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"port={config.serverPort}\n"); // default 10001
            sb.Append($"gameversion={config.gameVersion}\n");
            sb.Append($"maxplayers={config.playerLimit}\n");
            
            return sb.ToString();
        }

        static ServerConfiguration LoadFromFile(string path)
        {
            ServerConfiguration config = new ServerConfiguration();

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                int index = line.IndexOf('=');
                if (index < 0) continue;

                string key = line.Substring(0, index);
                string value = line.Substring(index + 1);

                switch (key.ToLower())
                {
                    case "port":
                        if (!ushort.TryParse(value, out config.serverPort))
                            Console.WriteLine($"Invalid conversion from config value '{value}' by key '{key}'.");

                        break;

                    case "gameversion":
                        if (!ushort.TryParse(value, out config.gameVersion))
                            Console.WriteLine($"Invalid conversion from config value '{value}' by key '{key}'.");

                        break;

                    case "maxplayers":
                        if (!short.TryParse(value, out config.playerLimit))
                            Console.WriteLine($"Invalid conversion from config value '{value}' by key '{key}'.");

                        break;

                    case "database":
                        {
                            config.dbHost = value;
                        }
                        break;

                    case "database_pass":
                        {
                            config.dbPass = value;
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown config key: " + key);
                        break;
                }
            }

            return config;
        }
    }
}
