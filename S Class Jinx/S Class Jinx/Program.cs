// <copyright file="Program.cs" company="Kallen">
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


namespace S_Class_Jinx
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            // So you can test if it in VS wihout crahses
            #if !DEBUG
                        CustomEvents.Game.OnGameLoad += OnLoad;
            #endif
        }

        private static void OnLoad(EventArgs args)
        {
            ClassBase.Initialize();
        }
    }
}
