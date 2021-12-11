/* Creation Date: 27.11.2020 */
/* Author: @playingo */
/* Note: (may be unused for now) */

using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.DataManagement // misc packet & internal server data long-term storage stuff (file saving/reloading etc.)
{
    class StorageLabels
    {
        public const string PlayerName = "PlayerName";
        public const string CognitoToken = "Token";
        public const string OperatorLevel = "OPLevel"; // "admin" state
        public const string GemAmount = "Gems";
        public const string CoinAmount = "ByteCoins";
    }
}
