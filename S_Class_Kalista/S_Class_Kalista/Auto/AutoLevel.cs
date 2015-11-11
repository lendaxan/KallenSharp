// <copyright file="AutoLevel.cs" company="Kallen">
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

using System;
using LeagueSharp;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    internal class AutoLevel
    {
        #region Structures

        public static void Initilize()
        {
            Game.OnUpdate += OnUpdate;
        }
        
        private static void OnUpdate(EventArgs args)
        {
            if (!Humanizer.Limiter.CheckDelay("LevelDelay")) return;

            Humanizer.Limiter.UseTick("LevelDelay");

            if (Properties.MainMenu.Item("bAutoLevel").GetValue<bool>())
                LevelUpSpells();
        }
        private struct Abilitys // So you can refeer to spell to level by slot rather than 1,2,3,4
        {
            public const int Q = 1;
            public const int W = 2;
            public const int E = 3;
            public const int R = 4;
        }
        #endregion Structures

        #region Variable Declaration

        private static readonly int[] AbilitySequence ={
            Abilitys.W,Abilitys.E,Abilitys.Q,Abilitys.E,
            Abilitys.E,Abilitys.R,Abilitys.E,Abilitys.Q,
            Abilitys.E,Abilitys.Q,Abilitys.R,Abilitys.Q,
            Abilitys.Q,Abilitys.W,Abilitys.W,Abilitys.R,
            Abilitys.W,Abilitys.W
        };

        private static readonly int[] AbilitySequence2 ={
            Abilitys.E,Abilitys.W,Abilitys.Q,Abilitys.E,
            Abilitys.E,Abilitys.R,Abilitys.E,Abilitys.Q,
            Abilitys.E,Abilitys.Q,Abilitys.R,Abilitys.Q,
            Abilitys.Q,Abilitys.W,Abilitys.W,Abilitys.R,
            Abilitys.W,Abilitys.W
        };

        private static int QOff = 0;
        private static int WOff = 0;
        private static int EOff = 0;
        private static int ROff = 0;

        #endregion Variable Declaration

        #region Public Functions

        private static void LevelUpSpells()
        {
            var qL = Properties.PlayerHero.Spellbook.GetSpell(Properties.Champion.Q.Slot).Level + QOff;
            var wL = Properties.PlayerHero.Spellbook.GetSpell(Properties.Champion.W.Slot).Level + WOff;
            var eL = Properties.PlayerHero.Spellbook.GetSpell(Properties.Champion.E.Slot).Level + EOff;
            var rL = Properties.PlayerHero.Spellbook.GetSpell(Properties.Champion.R.Slot).Level + ROff;

            if (qL + wL + eL + rL >= Properties.PlayerHero.Level) return;

            int[] level = { 0, 0, 0, 0 };
            if (Properties.MainMenu.Item("bStartE").GetValue<bool>())
            {
                for (var i = 0; i < Properties.PlayerHero.Level; i++)
                {
                    level[AbilitySequence2[i] - 1] = level[AbilitySequence2[i] - 1] + 1;
                }
            }
            else
            {
                for (var i = 0; i < Properties.PlayerHero.Level; i++)
                {
                    level[AbilitySequence[i] - 1] = level[AbilitySequence[i] - 1] + 1;
                }
            }
            if (qL < level[0]) Properties.PlayerHero.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Properties.PlayerHero.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Properties.PlayerHero.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Properties.PlayerHero.Spellbook.LevelSpell(SpellSlot.R);
        }

        #endregion Public Functions
    }
}