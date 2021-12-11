using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.World
{
    class WorldManager
    {
        private List<WorldSession> worlds = new List<WorldSession>();
        public List<WorldSession> GetWorlds() => worlds;


        
        public WorldSession GetByName(string name, bool force = false)
        {
            string worldName = name.ToUpper();

            foreach (WorldSession s in worlds)
            {
                if (s.WorldName.ToUpper() == worldName)
                    return s;
            }

            if (!force)
                return null;

            var w = new WorldSession(name); // load or generate.
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
