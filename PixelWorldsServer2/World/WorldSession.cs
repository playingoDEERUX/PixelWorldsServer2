using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.World
{
    internal class WorldSession
    {
        private List<Player> players = new List<Player>();
        public uint WorldID = 0;
        public string WorldName = string.Empty;
        private WorldTile[,] tiles;
        public int GetSizeX() => tiles.GetUpperBound(0) + 1;
        public int GetSizeY() => tiles.GetUpperBound(1) + 1;

        public WorldSession(string worldName = "")
        {
            if (worldName == "")
                return;

            // otherwise load from SQL, if it doesn't exist, then generate:

        }
    }
}
