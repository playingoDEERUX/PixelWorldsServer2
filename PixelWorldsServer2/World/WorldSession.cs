using System;
using System.Collections.Generic;
using System.Text;
using PixelWorldsServer2.Networking.Server;
using System.IO;
using Kernys.Bson;
using PixelWorldsServer2.DataManagement;
using System.Linq;
using static PixelWorldsServer2.World.WorldInterface;

namespace PixelWorldsServer2.World
{
    public class WorldSession
    {
        private PWServer pServer = null;
        private byte version = 0x1;
        private List<Player> players = new List<Player>();
        public Dictionary<int, Collectable> collectables = new Dictionary<int, Collectable>();
        public int colID = 0;
        public uint WorldID = 0;
        public short SpawnPointX = 36, SpawnPointY = 24;
        public string WorldName = string.Empty;
        private WorldTile[,] tiles = null;
        public int GetSizeX() => tiles.GetUpperBound(0) + 1;
        public int GetSizeY() => tiles.GetUpperBound(1) + 1;

        public uint OwnerID = 0;
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

        public void RemoveCollectable(int colID, Player toIgnore = null)
        {
            collectables.Remove(colID);
            BSONObject bObj = new BSONObject("RC");
            bObj["CollectableID"] = colID;

            Broadcast(ref bObj, toIgnore);
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

        public void Drop(int id, int amt, double posX, double posY, int gem = -1)
        {
            int cId = ++colID;
            BSONObject cObj = new BSONObject("nCo");
            cObj["CollectableID"] = cId;
            cObj["BlockType"] = id;
            cObj["Amount"] = amt;
            cObj["InventoryType"] = gem < 0 ? ItemDB.GetByID(id).type : 0;

            Collectable c = new Collectable();
            c.amt = (short)amt;
            c.item = (short)id;
            c.posX = posX * Math.PI;
            c.posY = posY * Math.PI;
            c.gemType = (short)gem;

            cObj["PosX"] = c.posX;
            cObj["PosY"] = c.posY;
            cObj["IsGem"] = c.gemType > -1;
            cObj["GemType"] = c.gemType < 0 ? 0 : c.gemType;
            
            collectables[cId] = c;

            Broadcast(ref cObj);
        }

        public WorldSession(PWServer pServer, string worldName = "")
        {
            if (worldName == "")
                return;

            // load from SQL and File, if it doesn't exist, then generate.
            // first retrieve worldID, name, metadata... if fail, then generate world.
            this.pServer = pServer;
            string path = $"maps/{worldName}.map";

            if (!File.Exists(path))
            {
#if DEBUG
                Util.Log("Generating new world with name: " + worldName);
#endif
                // generate world
                Generate(worldName);
                return;
            }

            Util.Log("Attempting to load world from DB...");
            Deserialize(Util.LZMAHelper.DecompressLZMA(File.ReadAllBytes(path)));
            this.WorldName = worldName;
        }

        public void Generate(string name)
        {
            // first, add new entry to sql:
            // todo filter the name from bad shit b4 release...
            SpawnPointX = (short)(1 + new Random().Next(79));
            WorldName = name;

            SetupTerrain();
        }

        public void SetupTerrain()
        {
            Util.Log("Setting up world terrain...");
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

            Util.Log($"Serializing world '{WorldName}' with width: {width} and height: {height}.");

            int pos = 0;
            for (int i = 0; i < tiles.Length; ++i)
            {
                int x = i % width;
                int y = i / width;

                if (x == SpawnPointX && y == SpawnPointY)
                    tiles[x, y].fg.id = 110;

                if (tiles[x, y].fg.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].fg.id), 0, blockLayerData, pos, 2);
                if (tiles[x, y].bg.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].bg.id), 0, backgroundLayerData, pos, 2);
                if (tiles[x, y].water.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].water.id), 0, waterLayerData, pos, 2);
                if (tiles[x, y].wire.id != 0) Buffer.BlockCopy(BitConverter.GetBytes(tiles[x, y].wire.id), 0, wiringLayerData, pos, 2);
                pos += 2;
            }

            wObj[MsgLabels.MessageID] = MsgLabels.Ident.GetWorld;
            wObj["World"] = WorldName;
            wObj["BlockLayer"] = blockLayerData;
            wObj["BackgroundLayer"] = backgroundLayerData;
            wObj["WaterLayer"] = waterLayerData;
            wObj["WiringLayer"] = wiringLayerData;

            BSONObject cObj = new BSONObject();
            cObj["Count"] = collectables.Values.Count;

            for (int i = 0; i < collectables.Values.Count; i++)
            {
                var col = collectables.ElementAt(i).Value.GetAsBSON();
                var kv = collectables.ElementAt(i);

                col["CollectableID"] = kv.Key;
                cObj[$"C{i}"] = col;
            }

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
            // load binary from file
            tiles = new WorldTile[80, 60]; // only this dimension is supported anyways
            for (int i = 0; i < tiles.GetLength(1); i++)
            {
                for (int j = 0; j < tiles.GetLength(0); j++)
                {
                    tiles[j, i] = new WorldTile();
                }
            }

            version = binary[0];
            OwnerID = BitConverter.ToUInt32(binary, 1);

            int pos = 5;
            for (int y = 0; y < GetSizeY(); y++)
            {
                for (int x = 0; x < GetSizeX(); x++)
                {
                    var tile = tiles[x, y];

                    tile.fg.id = BitConverter.ToInt16(binary, pos);
                    tile.bg.id = BitConverter.ToInt16(binary, pos + 2);
                    tile.water.id = BitConverter.ToInt16(binary, pos + 4);
                    tile.wire.id = BitConverter.ToInt16(binary, pos + 6);

                    if (tile.fg.id == 110)
                    {
                        SpawnPointX = (short)x;
                        SpawnPointY = (short)y;
                    }

                    pos += 8;
                }
            }

            int dropCount = BitConverter.ToInt16(binary, pos); pos += 4;
            for (int i = 0; i < dropCount; i++)
            {
                Collectable c = new Collectable();
                c.item = BitConverter.ToInt16(binary, pos);
                c.amt = BitConverter.ToInt16(binary, pos + 2);
                c.posX = BitConverter.ToDouble(binary, pos + 4);
                c.posY = BitConverter.ToDouble(binary, pos + 12);
                c.gemType = BitConverter.ToInt16(binary, pos + 20);
                collectables[++colID] = c;
                pos += 22;
            }
        }

        ~WorldSession()
        {

        }
    }
}
