using PixelWorldsServer2.DataManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2
{
    public class PlayerSettings
    {
        private int Settings = 0;

        public enum Bit
        {
            NONE = 0,
            SET_VIP,
            SET_MOD,
            SET_ADMIN,
            SET_INFLUENCER
        }

        private void Load(int s) => Settings = s;
        public int GetSettings() => Settings;
        public bool isSet(Bit pBit) => (Settings & (1 << (int)pBit)) != 0;
        public void Set(Bit pBit) => Settings |= (1 << (int)pBit);
        public void Unset(Bit pBit) => Settings &= ~(1 << (int)pBit);

        public PlayerSettings(int s = 0) => Load(s);

        public Ranks GetHighestRank()
        {
            Ranks rank = Ranks.PLAYER;

            if (isSet(Bit.SET_VIP))
                rank = Ranks.INFLUENCER;

            if (isSet(Bit.SET_MOD))
                rank = Ranks.MODERATOR;

            if (isSet(Bit.SET_ADMIN))
                rank = Ranks.ADMIN;

            return rank;
        }
    }
}
