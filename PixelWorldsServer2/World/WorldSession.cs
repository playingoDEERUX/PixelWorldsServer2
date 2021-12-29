using System;
using System.Collections.Generic;
using System.Text;
using PixelWorldsServer2.Networking.Server;
using System.IO;
using Kernys.Bson;
using PixelWorldsServer2.DataManagement;
using System.Linq;

namespace PixelWorldsServer2.World
{
    public  class WorldSession
    {
        private PWServer pServer = null;
        private List<Player> players = new List<Player>();
        public uint WorldID = 0;
        public short SpawnPointX = 36, SpawnPointY = 24;
        public string WorldName = string.Empty;
        private WorldTile[,] tiles = null;
        public int GetSizeX() => tiles.GetUpperBound(0) + 1;
        public int GetSizeY() => tiles.GetUpperBound(1) + 1;

        public List<Player> Players => players;

        public void AddPlayer(Player p)
        {
            if (HasPlayer(p) == -1)
                players.Add(p);

            p.world = this;
        }

        public int HasPlayer(Player p)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (p.Data.UserID == players[i].Data.UserID)
                    return i;
            }

            return -1;
        }

        public void RemovePlayer(Player p)
        {
            int idx = HasPlayer(p);

            if (idx >= 0)
                players.RemoveAt(idx);

            p.world = null;
        }

        public void Broadcast(ref BSONObject bObj, params Player[] ignored) // ignored player can be used to ignore packet being sent to player itself.
        {
            foreach (var p in players)
            {
                if (ignored.Contains(p))
                    continue;

                p.Send(ref bObj);
            }
        }

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

            WorldID = (uint)(long)res["ID"];
            WorldName = (string)res["Name"];

            string path = $"maps/{WorldID}.map";
            if (!File.Exists(path))
            {
                Console.WriteLine($"Cannot deserialize tile data, as map file ({path}) does not exist! Regenerating the world...");
                Generate(worldName);
                return;
            }
            Deserialize(File.ReadAllBytes(path));
        }

        public void Generate(string name)
        {
            // first, add new entry to sql:
            // todo filter the name from bad shit b4 release...

            var sql = pServer.GetSQL();
            if (sql.Query($"INSERT INTO worlds (Name) VALUES ('{name}')") > 0)
            {
                WorldID = (uint)sql.GetLastInsertID();
                WorldName = name;

                SetupTerrain();
            }
        }

        public void SetupTerrain()
        {
            Console.WriteLine("Setting up world terrain...");
            // empty world for now
            tiles = new WorldTile[80, 60];

            for (int i = 0; i < tiles.GetLength(1); i++)
            {
                for (int j = 0; j < tiles.GetLength(0); j++)
                {
                    tiles[j, i] = new WorldTile();
                }
            }

            for (int y = 0; y < SpawnPointY; y++)
            {
                for (int x = 0; x < GetSizeX(); x++)
                {
                    tiles[x, y].fg.id = 1;
                    tiles[x, y].bg.id = 2;
                }
            }
        }

        public WorldTile GetTile(int x, int y)
        {
            if (x >= GetSizeX() || y >= GetSizeY() || x < 0 || y < 0)
                return null;

            return tiles[x, y];
        }

        public BSONObject Serialize()
        {
            BSONObject wObj = new BSONObject();

            int tileLen = tiles.Length;
            int allocLen = tileLen * 2;

            byte[] blockLayerData = new byte[allocLen];
            byte[] backgroundLayerData = new byte[allocLen];
            byte[] waterLayerData = new byte[allocLen];
            byte[] wiringLayerData = new byte[allocLen];

            int width = GetSizeX();
            int height = GetSizeY();

            Console.WriteLine($"Serializing world '{WorldName}' with width: {width} and height: {height}.");

            int pos = 0;
            for (int i = 0; i < tiles.Length; ++i)
            {
                int x = i % width;
                int y = i / width;

                if (x == SpawnPointX && y == SpawnPointY)
                    tiles[x, y].fg.id = 110;


                if (tiles[x, y].fg.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].fg.id), 0, blockLayerData, pos, 2);
                if (tiles[x, y].bg.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].bg.id), 0, backgroundLayerData, pos, 2);
                if (tiles[x, y].water.id!= 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].water.id), 0, waterLayerData, pos, 2);
                if (tiles[x, y].wire.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].wire.id), 0, wiringLayerData, pos, 2);
                pos += 2;
            }

            wObj[MsgLabels.MessageID] = MsgLabels.Ident.GetWorld;
            wObj["World"] = WorldID.ToString("X8");
            wObj["BlockLayer"] = blockLayerData;
            wObj["BackgroundLayer"] = backgroundLayerData;
            wObj["WaterLayer"] = waterLayerData;
            wObj["WiringLayer"] = wiringLayerData;

            BSONObject cObj = new BSONObject();
            cObj["Count"] = 0;

            List<int>[] layerHits = new List<int>[4];
            for (int j = 0; j < layerHits.Length; j++)
            {
                layerHits[j] = new List<int>();
                layerHits[j].AddRange(Enumerable.Repeat(0, tileLen));
            }

            List<int>[] layerHitBuffers = new List<int>[4];
            for (int j = 0; j < layerHitBuffers.Length; j++)
            {
                layerHitBuffers[j] = new List<int>();
                layerHitBuffers[j].AddRange(Enumerable.Repeat(0, tileLen));
            }

            wObj["BlockLayerHits"] = layerHits[0];
            wObj["BackgroundLayerHits"] = layerHits[1];
            wObj["WaterLayerHits"] = layerHits[2];
            wObj["WiringLayerHits"] = layerHits[3];

            wObj["BlockLayerHitBuffers"] = layerHitBuffers[0];
            wObj["BackgroundLayerHitBuffers"] = layerHitBuffers[1];
            wObj["WaterLayerHitBuffers"] = layerHitBuffers[2];
            wObj["WiringLayerHits"] = layerHitBuffers[3];

            // change to template null count for optimization soon...
            BSONObject wLayoutType = new BSONObject();
            wLayoutType["Count"] = 0;
            BSONObject wBackgroundType = new BSONObject();
            wBackgroundType["Count"] = 0;
            BSONObject wMusicSettings = new BSONObject();
            wMusicSettings["Count"] = 0;

            BSONObject wStartPoint = new BSONObject();
            wStartPoint["x"] = (int)SpawnPointX; wStartPoint["y"] = (int)SpawnPointY;

            BSONObject wSizeSettings = new BSONObject();
            wSizeSettings["WorldSizeX"] = width; wSizeSettings["WorldSizeY"] = height;
            BSONObject wGravityMode = new BSONObject();
            wGravityMode["Count"] = 0;
            BSONObject wRatings = new BSONObject();
            wRatings["Count"] = 0;
            BSONObject wRaceScores = new BSONObject();
            wRaceScores["Count"] = 0;
            BSONObject wLightingType = new BSONObject();
            wLightingType["Count"] = 0;


            wObj["WorldLayoutType"] = wLayoutType;
            wObj["WorldBackgroundType"] = wBackgroundType;
            wObj["WorldMusicIndex"] = wMusicSettings;
            wObj["WorldStartPoint"] = wStartPoint;
            wObj["WorldItemId"] = 0;
            wObj["WorldSizeSettings"] = wSizeSettings;
            //wObj["WorldGravityMode"] = wGravityMode;
            wObj["WorldRatingsKey"] = wRatings;
            wObj["WorldItemId"] = 1;
            wObj["InventoryId"] = 1;
            wObj["RatingBoardCountKey"] = 0;
            wObj["QuestStarterItemSummerCountKey"] = 0;
            wObj["WorldRaceScoresKey"] = wRaceScores;
            wObj["WorldTagKey"] = 0;
            wObj["PlayerMaxDeathsCountKey"] = 0;
            wObj["RatingBoardDateTimeKey"] = DateTimeOffset.UtcNow.Date;
            wObj["WorldLightingType"] = wLightingType;
            wObj["WorldWeatherType"] = wLightingType;
            wObj["WorldItems"] = new BSONObject();

            BSONObject pObj = new BSONObject();

            wObj["PlantedSeeds"] = pObj;
            wObj["Collectables"] = cObj;

            return wObj;
        }

        public void Deserialize(byte[] binary)
        {
            // load binary from table
        }

        ~WorldSession()
        {

        }
    }
}
