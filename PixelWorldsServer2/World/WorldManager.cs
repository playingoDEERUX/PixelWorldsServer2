using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelWorldsServer2.Networking.Server;
using System.Linq;
using System.Threading;

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
                string path = $"maps/{w.WorldName}.map";
                Console.WriteLine($"Found cached world: {w.WorldName}, saving it...");
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.WriteByte(0x1); // version
                    ms.Write(BitConverter.GetBytes(w.OwnerID));

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

                    ms.Write(BitConverter.GetBytes(w.collectables.Values.Count));
                    for (int i = 0; i < w.collectables.Values.Count; i++)
                    {
                        var col = w.collectables.ElementAt(i).Value;
                        ms.Write(BitConverter.GetBytes(col.item));
                        ms.Write(BitConverter.GetBytes(col.amt));
                        ms.Write(BitConverter.GetBytes(col.posX));
                        ms.Write(BitConverter.GetBytes(col.posY));
                        ms.Write(BitConverter.GetBytes(col.gemType));
                    }

                    File.WriteAllBytes(path, Util.LZMAHelper.CompressLZMA(ms.ToArray()));
                    SpinWait.SpinUntil(() => Util.IsFileReady(path));
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
