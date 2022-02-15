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
    public class InventoryItem
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

        public Animation.HotSpots[] AnimHotSpots;

        public InventoryItem Get(int id, short flags = 0)
        {
            foreach (InventoryItem i in itemList)
            {
                if (i.itemID == id && i.flags == flags)
                    return i;
            }

            return null;
        }

        public PlayerInventory(byte[] data = null)
        {
            if (data == null)
                return;

            AnimHotSpots = new Animation.HotSpots[(int)Animation.HotSpots.END_OF_THE_ENUM + 1];

            Load(data);
        }

        // 0: success, -1 any error, higher than 0: left to be handled.
        public int Add(InventoryItem invItem)
        {
            var item = Get(invItem.itemID, invItem.flags);

            if (item == null)
            {
                Items.Add(invItem);
                return 0;
            }

            item.amount += invItem.amount;

            if (item.amount > 999)
            {
                int h = item.amount - 999;
                item.amount = 999;
                return h;
            }

            return 0;
        }

        public int Remove(InventoryItem invItem)
        {
            var item = Get(invItem.itemID, invItem.flags);

            if (item == null)
                return -1;

            if (item.amount <= 1)
            {
                Items.Remove(item);
                return 0;
            }

            item.amount -= invItem.amount;
            return invItem.amount;
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

                        Items.Add(new InventoryItem(id, flags, amount));
                    }
                }
            }
        }

        public void InitFirstSetup()
        {
            // bunch of cool items
            Items.Add(new InventoryItem(605, 0, 999));
            Items.Add(new InventoryItem(869, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(870, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(871, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(890, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(1018, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(1019, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(4266, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(4267, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(4268, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(4269, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(4093, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(4266, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(2152, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(1412, (short)ItemFlags.IS_WEARABLE, 999));
            Items.Add(new InventoryItem(3175, (short)ItemFlags.IS_WEARABLE, 999));
        }
    }
}
