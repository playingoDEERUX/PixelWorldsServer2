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
    class ItemDB
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

        public struct Item
        {
            public short hotspotType;
            public string name;
            public ushort ID;
            public short type;

            public Item (string _name, ushort _ID, short _type, short _hotspotType = -1) 
            {
                name = _name;
                ID = _ID;
                type = _type;
                hotspotType = _hotspotType;
            }
        }

        private static List<Item> items = new List<Item>();


        public static Item GetByName(string name)
        {
            lock (items)
            {
                string s = name.ToLower();
                foreach (var item in items)
                {
                    if (item.name.ToLower() == s)
                        return item;
                }
            }

            return new Item("", 0, -1);
        }

        public static Item[] FindByName(string name)
        {
            List<Item> foundItems = new List<Item>();
            lock (items)
            {
                string s = name.ToLower();
                foreach (var item in items)
                {
                    if (item.name.ToLower().StartsWith(s))
                        foundItems.Add(item);
                }
            }

            return foundItems.ToArray();
        }

        public static Item[] FindByAnyName(string name)
        {
            List<Item> foundItems = new List<Item>();
            lock (items)
            {
                string s = name.ToLower();
                foreach (var item in items)
                {
                    if (item.name.ToLower().Contains(s))
                        foundItems.Add(item);
                }
            }

            return foundItems.ToArray();
        }

        public static int ItemsCount()
        {
            return items.Count;
        }
        public static Item GetByID(int id)
        {
            lock (items)
            {
                if (id >= items.Count) return new Item("", 0, -1);
                return items[id];
            }
        }

        public static bool ItemIsLock(Item item)
        {
            return item.name.EndsWith("Lock"); // very easy, reliable and simple.
        }
        public static void Initialize()
        {
            string[] content = File.ReadAllLines("pwitems.txt");
            foreach (string line in content)
            {
                string[] args = line.Split("|");
                string aID = args[0]; string aName = args[1]; string aType = args[2];

                Item item;
                item.ID = ushort.Parse(aID);
                item.name = aName;
                item.type = short.Parse(aType);
                item.hotspotType = -1;
                items.Add(item);
            }
            Console.WriteLine($"Initialized item database, {items.Count} entries!");
        }
    }
}
