// <copyright file="DrawingManager.cs" company="Kallen">
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
using Color = System.Drawing.Color;

namespace S_Class_Lucian
{
    internal class DrawingManager
    {
        #region Public Functions

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Properties.PlayerHero.IsDead)
                return;
            //If User does not want drawing
            if (!Properties.MainMenu.Item("bDraw").GetValue<bool>())
                return;

            if (!Properties.PlayerHero.Position.IsOnScreen())
                return;
            try
            {
                if (Properties.MainMenu.Item("cDrawQRange").GetValue<Circle>().Active &&
                    Properties.Champion.Q.Level > 0)
                    Render.Circle.DrawCircle(Properties.PlayerHero.Position, Properties.Champion.Q.Range,
                        Properties.MainMenu.Item("cDrawQRange").GetValue<Circle>().Color, 2);

                if (Properties.MainMenu.Item("cDrawWRange").GetValue<Circle>().Active &&
                   Properties.Champion.W.Level > 0)
                    Render.Circle.DrawCircle(Properties.PlayerHero.Position, Properties.Champion.W.Range,
                        Properties.MainMenu.Item("cDrawWRange").GetValue<Circle>().Color, 2);

                if (Properties.MainMenu.Item("cDrawERange").GetValue<Circle>().Active &&
                   Properties.Champion.E.Level > 0)
                    Render.Circle.DrawCircle(Properties.PlayerHero.Position, Properties.Champion.E.Range,
                        Properties.MainMenu.Item("cDrawERange").GetValue<Circle>().Color, 2);

                if (Properties.MainMenu.Item("cDrawRRange").GetValue<Circle>().Active &&
                   Properties.Champion.R.Level > 0)
                    Render.Circle.DrawCircle(Properties.PlayerHero.Position, Properties.Champion.R.Range,
                        Properties.MainMenu.Item("cDrawRRange").GetValue<Circle>().Color, 2);
            }
            catch
            {
                // DUmb color picker
            }
        }

        private static Color GetColor(bool b)
        {
            return b ? Color.White : Color.SlateGray;
        }

        #endregion Public Functions
    }
}