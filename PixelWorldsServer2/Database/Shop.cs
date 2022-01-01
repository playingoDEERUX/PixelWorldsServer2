using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.Database
{
    public struct ShopResult
    {
        public int price;
        public List<int> items;
    }
    public class Shop
    {
        public static Dictionary<string, ShopResult> offers = new Dictionary<string, ShopResult>();

        public static void AddShopOffer(string name, int price, params int[] items)
        {
            ShopResult sr = new ShopResult();
            sr.items = new List<int>(items);
            sr.price = price;

            offers[name] = sr;
        }

        public static void Init()
        {
            AddShopOffer("WorldLock", 3500, 413);
        }
    }
}
