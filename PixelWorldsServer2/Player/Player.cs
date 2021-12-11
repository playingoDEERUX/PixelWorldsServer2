using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using FeatherNet;

namespace PixelWorldsServer2
{
    class Player
    {
        public struct PlayerData
        {
            public uint ID;
            public int Gems, Coins;
            public string Name;
            public PlayerInventory Inventory;
        }

        private FeatherClient fClient = null;
        public Player(FeatherClient fClient = null)
        {
            this.fClient = fClient;
            
            if (fClient != null)
                this.Data = new PlayerData();
        }

        public FeatherClient Client { get { return fClient; } }
        public ref dynamic Data { get { return ref fClient.data; } }
    }
}
