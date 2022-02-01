using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PixelWorldsServer2.DataManagement
{
    public enum ItemTileLockType
    {
        SMALL,
        BIG,
        LARGE,
        WORLD
    }
    public enum ItemType
    {
        BLOCK,
        BACKGROUND,
        FALL,
        LIQUID,
        CLOTHING,
        WEAPON,
        CONSUMABLE,
        ORB,
        SHARD,
        BLUEPRINT,
        FAMILIAR,
        SNACK,
        GATE,
    }

    public struct Item
    {
        public short hotspotType;
        public string name;
        public short ID;
        public short type;
        public short hitsRequired;

        public Item(string _name, short _ID, short _type, 
            short _hotspotType = -1, short _hitsRequired = 3)
        {
            name = _name;
            ID = _ID;
            type = _type;
            hotspotType = _hotspotType;
            hitsRequired = _hitsRequired;
        }
    }
    public class ItemDB
    {
        public static ItemTileLockType GetLockType(ref Item it)
        {
            switch (it.ID)
            {
                case 410:
                    return ItemTileLockType.SMALL;

                case 411:
                    return ItemTileLockType.BIG;

                case 412:
                    return ItemTileLockType.LARGE;

                default:
                    break;
            }
            return ItemTileLockType.WORLD;
        }

        private static List<Item> items = new List<Item>();

        public static bool IsWearable(int item)
        {
            Item it = GetByID(item);
            var type = (ItemType)it.type;

            return type == ItemType.CLOTHING || type == ItemType.WEAPON || type == ItemType.FAMILIAR;
        }

        public static Item GetByName(string name)
        {

            string s = name.ToLower();
            foreach (var item in items)
            {
                if (item.name.ToLower() == s)
                    return item;
            }

            return new Item("", -1, -1);
        }

        public static Item[] FindByName(string name)
        {
            List<Item> foundItems = new List<Item>();

            string s = name.ToLower();
            foreach (var item in items)
            {
                if (item.name.ToLower().Contains(s))
                    foundItems.Add(item);
            }

            return foundItems.ToArray();
        }

        public static Item[] FindByAnyName(string name)
        {
            List<Item> foundItems = new List<Item>();

            string s = name.ToLower();
            foreach (var item in items)
            {
                if (item.name.ToLower().StartsWith(s))
                    foundItems.Add(item);
            }

            return foundItems.ToArray();
        }

        public static int ItemsCount()
        {
            return items.Count;
        }
        public static Item GetByID(int id)
        {
            return (id >= items.Count || id < 0) ? new Item("", -1, 0) : items[id];
        }

        public static bool ItemIsLock(Item item)
        {
            return item.name.EndsWith("Lock"); // very easy, reliable and simple.
        }
        public static void Initialize()
        {
            string[] content = File.ReadAllLines("items.txt");
            foreach (string line in content)
            {
                string[] args = line.Split("|");

                Item item;
                item.ID = short.Parse(args[0]);
                item.name = args[1];
                item.type = short.Parse(args[2]);
                item.hotspotType = -1;
                item.hitsRequired = short.Parse(args[6]);

                items.Add(item);
            }
            Util.Log($"Initialized item database, {items.Count} entries!");
        }
    }
}
