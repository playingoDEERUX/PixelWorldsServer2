using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.World
{
    public enum LayerType
    {
        Block,
        Background,
        Water,
        Wiring,
        Unknown
    }

    public struct WorldLayer
    {
        public ushort foregroundID, backgroundID;
        public byte blockType, damageNow;
        public object anotherSprite; // ?
        public bool activeAnimation;
        public int direction;
        public int classId;
        public long lastHit;
    }

    public class WorldTile
    {
        WorldLayer[] layers;

        public WorldLayer GetFG() => layers[(int)LayerType.Block];
        public WorldLayer GetBG() => layers[(int)LayerType.Background];
        public WorldLayer GetWater() => layers[(int)LayerType.Water];
        public WorldLayer GetWiring() => layers[(int)LayerType.Wiring];
    }
}
