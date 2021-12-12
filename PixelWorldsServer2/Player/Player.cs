using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using FeatherNet;
using Kernys.Bson;

namespace PixelWorldsServer2
{
    class Player
    {
        public bool isInGame = false; // when the player has logon and is inside.
        public struct PlayerData
        {
            public Player player;

            public uint UserID;
            public int Gems, Coins;
            public string Name;
            public PlayerInventory Inventory;

            public PlayerData(Player player = null)
            {
                this.player = player;
                UserID = 0;
                Gems = 0;
                Coins = 0;
                Name = "";
                Inventory = new PlayerInventory();
            }
        }

        private PlayerData pData; // basically acts like a save, this is not the data that is assigned to the FeatherNet session itself.
        private FeatherClient fClient = null;
        private List<BSONObject> packets = new List<BSONObject>();
        public Player(FeatherClient fClient = null)
        {
            this.fClient = fClient;

            if (fClient != null)
            {
                pData = new PlayerData(this);
                fClient.data = pData; // interlink
            }
        }

        public FeatherClient Client { get { return fClient; } }
        public ref dynamic Data { get { return ref fClient.data; } }
        public ref PlayerData GetData() => ref pData;

        public void Tick()
        {
            if (fClient != null)
            {
                while (packets.Count > 0)
                {
                    Client.Send(packets[0]);
                    packets.RemoveAt(0);
                }
            }
        }

        public void SetClient(FeatherClient fClient) => this.fClient = fClient;
        public void Send(ref BSONObject packet) => packets.Add(packet);
    }
}
