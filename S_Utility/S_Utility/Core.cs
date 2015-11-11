using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace S_Utility
{
    class Core
    {


        private const string MenuName = "S Utility";

        public static Menu SMenu { get; set; } = new Menu(MenuName, MenuName, true);
        public static Obj_AI_Hero Player => ObjectManager.Player;


        public class Time
        {
            private static readonly DateTime AssemblyLoadTime = DateTime.Now;
            public static float LastTick { get; set; } = TickCount;
            public static float TickCount => (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
            public static bool CheckLast()
            {
                return !(TickCount - LastTick < 1000 + Game.Ping / 2);
            }
        }
    }
}
