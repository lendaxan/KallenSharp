using System;
using LeagueSharp;
using LeagueSharp.Common;


namespace SCore
{
    internal class Base
    {
        public static Menu CoreMenu { get; set; }
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Orbwalking.OrbwalkingMode Mode { get; set; }
        public class Time
        {
            private static readonly DateTime AssemblyLoadTime = DateTime.Now;
            public static float LastTick { get; set; }

            public static float TickCount
            {
                get
                {
                    return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
                }
            }

            public static bool CheckLast()
            {
                return !(TickCount - LastTick < 1000 + Game.Ping / 2);
            }
        }
    }
}
