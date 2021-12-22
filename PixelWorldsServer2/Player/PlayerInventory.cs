using PixelWorldsServer2.DataManagement;
using System;
using System.Collections.Generic;
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
    }
}
