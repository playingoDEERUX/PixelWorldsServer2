/* Creation Date: 27.11.2020 */
/* Author: @playingo */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixelWorldsServer2.DataManagement
{
    class Randomizer
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static int RandomInt(int start, int end)
        {
            return random.Next(start, end);
        }
    }
}
