using System;
using System.Collections.Generic;
using System.Text;
using PixelWorldsServer2.Networking.Server;
using System.IO;

namespace PixelWorldsServer2.World
{
    internal class WorldSession
    {
        private PWServer pServer = null;
        private List<Player> players = new List<Player>();
        public uint WorldID = 0;
        public string WorldName = string.Empty;
        private WorldTile[,] tiles;
        public int GetSizeX() => tiles.GetUpperBound(0) + 1;
        public int GetSizeY() => tiles.GetUpperBound(1) + 1;

        public WorldSession(PWServer pServer, string worldName = "")
        {
            if (worldName == "")
                return;

            // load from SQL and File, if it doesn't exist, then generate.
            // first retrieve worldID, name, metadata... if fail, then generate world.
            this.pServer = pServer;

            var sql = pServer.GetSQL();
            var res = sql.FetchQuery(string.Format("SELECT * FROM worlds WHERE Name='{0}'",
                worldName));

            if (!res.Read())
            {
#if DEBUG
                Console.WriteLine("Generating new world with name: " + worldName);
#endif
                // generate world
                Generate(worldName);
                return;
            }

            Console.WriteLine("Attempting to load world from DB...");

            WorldID = (uint)res["ID"];
            WorldName = (string)res["Name"];
            Deserialize(File.ReadAllBytes($"maps/{WorldID}.map"));
        }

        public void Generate(string name)
        {
            // first, add new entry to sql:
            // todo filter the name from bad shit b4 release...

            var sql = pServer.GetSQL();
            if (sql.Query($"INSERT INTO worlds (Name) VALUES ('{name}')") > -1)
            {
                WorldID = (uint)sql.GetLastInsertID();
                WorldName = name;

                SetupStructure();
            }
        }

        public void SetupStructure()
        { 

        }

        public void Deserialize(byte[] binary)
        {

        }
    }
}
