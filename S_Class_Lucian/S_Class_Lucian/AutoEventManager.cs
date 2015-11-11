// <copyright file="AutoEventManager.cs" company="Kallen">
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
    class AutoEventManager
    {
        public static void EventCheck()
        {
            try
            {
                if (!Properties.Time.CheckLastDelay()) return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            switch (args.SData.Name)
            {
                case "LucianQ":
                case "LucianW":
                case "LucianE":
                    Utility.DelayAction.Add((int)(Math.Ceiling(Game.Ping / 2f) + 150),Orbwalking.ResetAutoAttackTimer);
                    break;
            }
        }

        public static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            
        }
        public static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R)
            {
                if (Items.HasItem(3142) && Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }


        }
    }
}
