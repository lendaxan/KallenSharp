// <copyright file="Properties.cs" company="Kallen">
//   Copyright (C) 2015 LeagueSharp Kallen
//
//             This program is free software: you can redistribute it and/or modify
//             it under the terms of the GNU General Public License as published by
//             the Free Software Foundation, either version 3 of the License, or
//             (at your option) any later version.
//
//             This program is distributed in the hope that it will be useful,
//             but WITHOUT ANY WARRANTY; without even the implied warranty of
//             MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//             GNU General Public License for more details.
//
//             You should have received a copy of the GNU General Public License
//             along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Assembly to be use with LeagueSharp for champion Kalista
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using LeagueSharp;
using LeagueSharp.Common;
using System;

namespace S_Class_Kalista
{
    internal class Properties
    {
        public static readonly BuffType[] Bufftype =
        {
            BuffType.Snare,
            BuffType.Blind,
            BuffType.Charm,
            BuffType.Stun,
            BuffType.Fear,
            BuffType.Slow,
            BuffType.Taunt,
            BuffType.Suppression
        };
        public static void GenerateProperties()
        {
            PlayerHero = ObjectManager.Player;
        }

        #region Auto Properties

       // public static Orbwalking.Orbwalker LukeOrbWalker { get; set; }
        //public static SkyWalker.Orbwalker SkyWalker{ get; set; }
        public static Orbwalking.Orbwalker CommonWalker { get; set; }
        public static LandWalker.Orbwalker LandWalker { get; set; }

        public static Menu MainMenu { get; set; }
        public static Obj_AI_Hero PlayerHero { get; set; }
        public static Obj_AI_Hero SoulBoundHero { get; set; }

        #endregion Auto Properties


        internal class Time
        {
            private static readonly DateTime AssemblyLoadTime = DateTime.Now;

            public static float TickCount
            {
                get
                {
                    return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
                }
            }
           
            public static bool CheckRendDelay()
            {
                return Humanizer.Limiter.CheckDelay("RendDelay");
            }

            public static bool CheckNonKillable()
            {
                return Humanizer.Limiter.CheckDelay("NonKillableDelay");
            }
        }

       

        //internal class AutoLevel
        //{
        //    public static LeagueSharp.Common.AutoLevel LevelManager { get; set; }

        //    private struct Abilitys // So you can refeer to spell to level by slot rather than 1,2,3,4
        //    {
        //        public const int Q = 1;
        //        public const int W = 2;
        //        public const int E = 3;
        //        public const int R = 4;
        //    }

        //    private static readonly int[] AbilitySequence =
        //    {
        //        Abilitys.W, Abilitys.E, Abilitys.Q, Abilitys.E,
        //        Abilitys.E, Abilitys.R, Abilitys.E, Abilitys.Q,
        //        Abilitys.E, Abilitys.Q, Abilitys.R, Abilitys.Q,
        //        Abilitys.Q, Abilitys.W, Abilitys.W, Abilitys.R,
        //        Abilitys.W, Abilitys.W
        //    };

        //    public static void InitilizeAutoLevel()
        //    {
        //        LevelManager = new LeagueSharp.Common.AutoLevel(AbilitySequence);
        //    }
        //}

        internal class Champion
        {
            public static Spell Q { get; set; }
            public static Spell W { get; set; }
            public static Spell E { get; set; }
            public static Spell R { get; set; }
            public Obj_AI_Hero SoulBound { get; set; }
            //public static Spell D { get; set; }
            //public static Spell F { get; set; }

            public static void UseRend()
            {
                Humanizer.Limiter.UseTick("RendDelay");
                E.Cast();
#if DEBUG_MODE
                Console.WriteLine("Last Rend Tick:{0} Current Tick{1}", Time.LastRendTick, Time.TickCount);
#endif
            }

            public static void UseNonKillableRend()
            {
                Humanizer.Limiter.UseTick("NonKillableDelay");
                E.Cast();
#if DEBUG_MODE
                Console.WriteLine("Last Nonkillable Tick:{0} Current Tick{1}", Time.LastNonKillable, Time.TickCount);
#endif
            }

            public static void LoadSpells()
            {
                //Loads range and shit
                Q = new Spell(SpellSlot.Q, 1150);
                W = new Spell(SpellSlot.W, 5200);
                E = new Spell(SpellSlot.E, 950);
                R = new Spell(SpellSlot.R, 1200);

                Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
                R.SetSkillshot(0.50f, 1500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            }
        }
    }
}