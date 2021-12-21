using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net.Sockets;
using System.Numerics;
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
            public string CognitoID, Token;
            public string Name;
            public string LastIP;
            public PlayerInventory Inventory;
            public double PosX, PosY;
            public int Anim, Dir;
        }

        private PlayerData pData; // basically acts like a save, this is not the data that is assigned to the FeatherNet session itself.
        private FeatherClient fClient = null;
        private List<BSONObject> packets = new List<BSONObject>();
        public World.WorldSession world = null;
        public Player(FeatherClient fClient = null)
        {
            this.fClient = fClient;

            if (fClient != null)
            {
                pData.player = this;
                pData.UserID = 0;
                pData.Gems = 0;
                pData.Coins = 0;
                pData.CognitoID = "";
                pData.Token = "";
                pData.Name = "";
                pData.LastIP = "0.0.0.0";
                
                fClient.data = pData; // interlink
            }
        }
        public Player(SQLiteDataReader reader)
        {
            pData.player = this;
            pData.UserID = (uint)(long)reader["ID"];
            pData.Gems = (int)reader["Gems"];
            pData.Coins = (int)reader["ByteCoins"];
            pData.Name = (string)reader["Name"];
            pData.LastIP = (string)reader["IP"];
            pData.CognitoID = (string)reader["CognitoID"];
            pData.Token = (string)reader["Token"];
        }


        public FeatherClient Client { get { return fClient; } }
        public ref dynamic ClientData { get { return ref fClient.data; } }
        public ref PlayerData Data => ref pData;

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

        public void Ping()
        {
            if (Client.needsPing())
                return; // its gonna receive a ping anyway

            BSONObject p = new BSONObject("p");
            Send(ref p);
        }
    }
}
