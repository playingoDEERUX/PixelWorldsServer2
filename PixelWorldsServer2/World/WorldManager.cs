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

        private readonly int UNLOAD_WORLDS_MB_THRESHOLD = 768;

        public void CheckAll()
        {
            // Unload the worlds that don't have players if we can.

            long totalMemMB = GC.GetTotalMemory(false) / 100000;
            
            if (totalMemMB >= UNLOAD_WORLDS_MB_THRESHOLD)
            {
                List<WorldSession> worldSessionsToRemove = new List<WorldSession>();
                foreach (var w in worlds)
                {
                    if (w.Players.Count > 0)
                        continue;

                    w.Save();
                }

                foreach (var w in worldSessionsToRemove)
                    Remove(w);

                GC.Collect();
            }
        }

        public void Clear() => worlds.Clear();

        public void SaveAll()
        {
            Util.Log("Saving all worlds...");

            foreach (var w in worlds)
            {
                Util.Log($"Found cached world: {w.WorldName}, saving it...");
                w.Save();
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

        public void Remove(WorldSession world) => worlds.Remove(world);

        public void Add(WorldSession world) => worlds.Add(world);
    }
}
