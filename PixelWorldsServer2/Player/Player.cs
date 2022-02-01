using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using FeatherNet;
using Kernys.Bson;
using PixelWorldsServer2.DataManagement;
using PixelWorldsServer2.Networking.Server;

namespace PixelWorldsServer2
{
    public class Player
    {
        private PWServer pServer = null;
        public PlayerSettings pSettings { get; set; }
        public bool isInGame = false; // when the player has logon and is inside.
        public bool sendPing = false;
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
            public BSONObject BSON;
        }

        private PlayerData pData; // basically acts like a save, this is not the data that is assigned to the FeatherNet session itself.
        private FeatherClient fClient = null;
        private List<BSONObject> packets = new List<BSONObject>();
        public World.WorldSession world = null;
        public string GetWorldName()
        {
            if (world == null)
                return "[WORLD MENU]";

            return world == null ? "<World Menu>" : world.WorldName;
        }
        public Player(FeatherClient fClient = null)
        {
            if (fClient != null)
            {
                this.fClient = fClient;
                pServer = fClient.link as PWServer;
                this.pSettings = new PlayerSettings();

                pData.player = this;
                pData.UserID = 0;
                pData.Gems = 0;
                pData.Coins = 0;
                pData.CognitoID = "";
                pData.Token = "";
                pData.Name = "";
                pData.LastIP = "0.0.0.0";
                pData.Inventory = new PlayerInventory();
                pData.BSON = new BSONObject();
                
                fClient.data = pData; // interlink
            }
        }
        public Player(SQLiteDataReader reader)
        {
            pData.player = this;
            this.pSettings = new PlayerSettings((int)reader["Settings"]);
            pData.UserID = (uint)(long)reader["ID"];
            pData.Gems = (int)reader["Gems"];
            pData.Coins = (int)reader["ByteCoins"];
            pData.Name = (string)reader["Name"];
            pData.LastIP = (string)reader["IP"];
           

            object inven = reader["Inventory"];
            byte[] invData = null;

            if (!Convert.IsDBNull(inven))
                invData = (byte[])inven;

            object bsonObj = reader["Inventory"];
            byte[] bsonData = null;

            if (!Convert.IsDBNull(bsonObj))
                bsonData = (byte[])bsonObj;

            try
            {
                pData.BSON = SimpleBSON.Load(bsonData);
            }
            catch
            {
                pData.BSON = new BSONObject();
                //Util.Log("Failed to read BSON extended data for User " + pData.UserID.ToString("X8") + "!");
            }

            pData.Inventory = new PlayerInventory(invData); // todo load inv from sql
        }

        public FeatherClient Client { get { return fClient; } }
        public ref dynamic ClientData { get { return ref fClient.data; } }
        public ref PlayerData Data => ref pData;

        public void Tick()
        {
            if (Client != null)
            {
                while (packets.Count > 0)
                {
                    Client.Send(packets[0]);
                    packets.RemoveAt(0);
                }
            }
        }

        public void SendPing()
        {
            foreach (var pac in packets)
            {
                if (pac["ID"] == "p")
                    return;
            }

            Send(ref MsgLabels.pingBson);
        }

        public void SetClient(FeatherClient fClient) 
        {
            this.fClient = fClient;

            if (fClient != null)
            {
                if (fClient.link != null)
                    pServer = fClient.link as PWServer;
            }
        }

        public void SelfChat(string txt)
        {
            BSONObject c = new BSONObject("WCM");
            c[MsgLabels.ChatMessageBinary] = Util.CreateChatMessage("<color=#FF0000>System",
                world.WorldName,
                world.WorldName,
                1,
                txt);

            Send(ref c);
        }

        public void Send(ref BSONObject packet) => packets.Add(packet);
        public void RemoveGems(int amt)
        {
            Data.Gems -= amt;
            BSONObject bObj = new BSONObject("RG");
            bObj["Amt"] = amt;

            Send(ref bObj);
        }

        public bool IsUnregistered()
        {
            return pData.Name.StartsWith("Subject_");
        }

        public void Save()
        {
            if (pServer == null)
                return; // No need to save, there has never been a client to perform any changes on the data anyway.

            Util.Log("Saving player...");

            var sql = pServer.GetSQL();
            var cmd = sql.Make("UPDATE players SET " +
                "Gems=@Gems, " +
                "ByteCoins=@ByteCoins, " +
                "IP=@IP, " +
                "Inventory=@Inventory, " +
                "Settings=@Settings, " +
                "BSON=@BSON " +
                "WHERE ID=@ID");

            cmd.Parameters.AddWithValue("@Gems", Data.Gems);
            cmd.Parameters.AddWithValue("@ByteCoins", Data.Coins);
            cmd.Parameters.AddWithValue("@IP", Data.LastIP);
            cmd.Parameters.AddWithValue("@Settings", pSettings.GetSettings());
            cmd.Parameters.AddWithValue("@BSON", SimpleBSON.Dump(Data.BSON));

            byte[] invData = Data.Inventory.Serialize();
            cmd.Parameters.Add("@Inventory", DbType.Binary);
            cmd.Parameters["@Inventory"].Value = invData;

            cmd.Parameters.AddWithValue("@ID", Data.UserID);

            if (sql.PreparedQuery(cmd) > 0)
            {
                Util.Log($"Player ID: {Data.UserID} ('{Data.Name}') saved.");
            }
        }
    }
}
