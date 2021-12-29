using System;
using System.Collections.Generic;
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
