using PixelWorldsServer2.DataManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorldsServer2
{
    public enum ItemFlags
    {
        IS_SEED = 1 << 9,
        IS_WEARABLE = 1 << 10
    }
    public struct InventoryItem
    {
        public short itemID;
        public short flags;
        public short amount;

        public InventoryItem(short itemID = 0, short flags = 0, short amount = 1)
        {
            this.itemID = itemID;
            this.flags = flags;
            this.amount = amount;
        }
    }
    public class PlayerInventory
    {
        private List<InventoryItem> itemList = new List<InventoryItem>();
        public List<InventoryItem> Items => itemList;

        public void Get(short id, short flags, out InventoryItem invItem)
        {
            foreach (InventoryItem i in itemList)
            {
                if (i.itemID == id && (i.flags & flags) != 0)
                {
                    invItem = i;
                    break;
                }
            }

            invItem = new InventoryItem(0, 0, 0);
        }

        public PlayerInventory(byte[] data = null)
        {
            if (data == null)
                return;

            Load(data);
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var item in Items)
                    {
                        bw.Write(item.itemID);
                        bw.Write(item.flags);
                        bw.Write(item.amount);
                    }
                }

                return stream.ToArray();
            }
        }

        public void Load(byte[] data)
        {
            if (data.Length % 6 != 0)
            {
                Util.Log("Inventory data doesn't have correct length?! May be corrupted!!");
                return;
            }

            int items = data.Length / 6;
            using (var stream = new MemoryStream(data))
            {
                using (var bw = new BinaryReader(stream))
                {
                    for (int i = 0; i < items; i++)
                    {
                        short id = bw.ReadInt16();
                        short flags = bw.ReadInt16();
                        short amount = bw.ReadInt16();

                        Console.WriteLine("ID: " + id + " FLAGS: " + flags + " AMOUNT: " + amount);
                    }
                }
            }
        }

        public void InitFirstSetup()
        {
            // bunch of cool items
            Items.Add(new InventoryItem(605, 0, 9999));
            Items.Add(new InventoryItem(869, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(870, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(871, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(890, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(1018, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(1019, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(4266, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(4267, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(4268, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(4269, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(4093, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(4266, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(2152, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(1412, (short)ItemFlags.IS_WEARABLE, 9999));
            Items.Add(new InventoryItem(3175, (short)ItemFlags.IS_WEARABLE, 9999));
        }
    }
}
