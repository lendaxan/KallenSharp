﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S_Plus_Class_Kalista.Structures
{
    class Monster
    {
        public struct MonsterBar
        {
            public int BarWidth;
            public int XOffset;
            public int YOffsetBegin;
            public int YOffsetEnd;

            public MonsterBar(int barWidth, int xoffset, int yOffsetBegin, int yOffsetEnd)
            {
                BarWidth = barWidth;
                XOffset = xoffset;
                YOffsetBegin = yOffsetBegin;
                YOffsetEnd = yOffsetEnd;
            }
        }

        public static Dictionary<string, MonsterBar> MonsterBarDictionary = new Dictionary<string, MonsterBar>()
        {
            {"SRU_Red",new MonsterBar(145,3,18,10)},
            {"SRU_Blue",new MonsterBar(145,3,18,10)},
            {"SRU_Dragon",new MonsterBar(145,3,18,10)},
            {"SRU_Baron",new MonsterBar(194,-22,13,16)},
            {"SRU_Crab",new MonsterBar(61,45,34,3)},
            {"SRU_Krug",new MonsterBar(81,58,18,4)},
            {"SRU_Gromp",new MonsterBar(87,62,18,4)},
            {"SRU_Murkwolf",new MonsterBar(75,54,19,4)},
            {"SRU_Razorbeak",new MonsterBar(75,54,18,4)}
        };

    }
}
