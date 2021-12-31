using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelWorldsServer2.Networking.Server;

namespace PixelWorldsServer2.World
{
    public class WorldManager
    {
        private PWServer pServer = null;
        private List<WorldSession> worlds = new List<WorldSession>();
        public List<WorldSession> GetWorlds() => worlds;

        public WorldManager(PWServer pServer)
        {
            this.pServer = pServer;
        }

        public void Clear() => worlds.Clear();

        public void SaveAll()
        {
            Util.Log("Saving all worlds...");

            foreach (var w in worlds)
            {
                Console.WriteLine($"Found cached world: {w.WorldName}, saving it...");
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.WriteByte(0x1); // version

                    for (int y = 0; y < w.GetSizeY(); y++)
                    {
                        for (int x = 0; x < w.GetSizeX(); x++)
                        {
                            var tile = w.GetTile(x, y);

                            ms.Write(BitConverter.GetBytes(tile.fg.id));
                            ms.Write(BitConverter.GetBytes(tile.bg.id));
                            ms.Write(BitConverter.GetBytes(tile.water.id));
                            ms.Write(BitConverter.GetBytes(tile.wire.id));
                        }
                    }

                    File.WriteAllBytes($"maps/{w.WorldName}.map", Util.LZMAHelper.CompressLZMA(ms.ToArray()));
                }
            }
        }

        public WorldSession GetByName(string name, bool forceGen = false)
        {
            string worldName = name.ToUpper();

            foreach (WorldSession s in worlds)
            {
                if (s.WorldName.ToUpper() == worldName)
                    return s;
            }

            if (!forceGen)
                return null;

            var w = new WorldSession(pServer, name); // load or generate.
            Add(w);

            return w;
        }

        public WorldSession GetByID(uint ID)
        {
            foreach (WorldSession s in worlds)
            {
                if (s.WorldID == ID)
                    return s;
            }
            
            return null;
        }

        public void Add(WorldSession world) => worlds.Add(world);
    }
}
