using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorldsServer2.DataManagement
{
    interface ConfigManager
    {
        struct ServerConfiguration
        {
            public ushort serverPort;
            public ushort gameVersion; // is necessary to even allow for login
            public short playerLimit;
        }
        static string GetAsPrintable(ServerConfiguration config)
        {
            string toPrint = string.Empty;

            toPrint += $"\n\tServer Port: {config.serverPort}\n";
            toPrint += $"\n\tGame Version: {config.gameVersion}\n";
            toPrint += $"\n\tPlayer Max Limit: {config.playerLimit}\n";

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
                            Util.Log($"Invalid conversion from config value '{value}' by key '{key}'.");

                        break;

                    case "gameversion":
                        if (!ushort.TryParse(value, out config.gameVersion))
                            Util.Log($"Invalid conversion from config value '{value}' by key '{key}'.");

                        break;

                    case "maxplayers":
                        if (!short.TryParse(value, out config.playerLimit))
                            Util.Log($"Invalid conversion from config value '{value}' by key '{key}'.");

                        break;

                    default:
                        Util.Log("Unknown config key: " + key);
                        break;
                }
            }

            return config;
        }
    }
}
