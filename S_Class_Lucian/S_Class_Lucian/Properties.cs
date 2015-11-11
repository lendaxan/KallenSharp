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
//   Assembly to be use with LeagueSharp for champion Lucian
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Linq;

namespace S_Class_Lucian
{
    class Properties
    {
        public static void GenerateProperties()
        {
            PlayerHero = ObjectManager.Player;
        }

        #region Auto Properties

        public static Orbwalking.Orbwalker LukeOrbWalker { get; set; }
        public static Menu MainMenu { get; set; }
        public static Obj_AI_Hero PlayerHero { get; set; }

        #endregion Auto Properties

        internal class Time
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

            public static bool CheckLastDelay()
            {
                return !(TickCount - LastTick < 100);
            }
        }

        internal class Champion
        {
            public static Spell Q { get; set; }
            public static Spell W { get; set; }
            public static Spell E { get; set; }
            public static Spell R { get; set; }

            public static bool PassiveReady()
            {
                return ObjectManager.Player.Buffs.Any(buff => buff.Name == "lucianpassivebuff");
            }

            public static void UseTick()
            {
                Time.LastTick = Time.TickCount;
            }

            public static void LoadSpells()
            {
                //Loads range and shit
                Q = new Spell(SpellSlot.Q, 675, TargetSelector.DamageType.Physical);
                W = new Spell(SpellSlot.W, 1000, TargetSelector.DamageType.Magical);
                E = new Spell(SpellSlot.E, 425, TargetSelector.DamageType.Physical);
                R = new Spell(SpellSlot.R, 1400, TargetSelector.DamageType.Physical);

                Q.SetTargetted(0.25f, float.MaxValue);
                W.SetSkillshot(0.4f, 150f, 1600, true, SkillshotType.SkillshotCircle);
                E.SetSkillshot(0.25f, 1f, float.MaxValue, false, SkillshotType.SkillshotLine);
                R.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);
            }
        }

    }

}

