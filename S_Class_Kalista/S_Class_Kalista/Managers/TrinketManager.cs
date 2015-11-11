// <copyright file="TrinketManager.cs" company="Kallen">
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
using LeagueSharp.Common;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    internal class TrinketManager
    {
        public static void Initilize()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if(Humanizer.Limiter.CheckDelay("TrinketDelay"))
            {
                if (Properties.MainMenu.Item("bAutoBuyOrb").GetValue<bool>() && Properties.PlayerHero.Level >= 6)
                    BuyOrb();
                Humanizer.Limiter.UseTick("TrinketDelay");
            }
        }
        private static void BuyOrb()
        {

            if (!ObjectManager.Player.InShop() ||
                Items.HasItem(ItemId.Scrying_Orb_Trinket.ToString()) ||
                Items.HasItem(ItemId.Farsight_Orb_Trinket.ToString()))
                return;
            ObjectManager.Player.BuyItem(ItemId.Scrying_Orb_Trinket);
        }
    }
}