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
//   Assembly to be use with LeagueSharp for champion Kalista
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Globalization;
using System.Linq;
using Color = System.Drawing.Color;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    internal class DrawingManager
    {
        #region Public Functions

        static class Drawing
        {
            private static DamageToUnitDelegate _damageToUnit, _damageToMonster;

            public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

            public static DamageToUnitDelegate DamageToUnit
            {
                get { return _damageToUnit; }

                set
                {
                    if (_damageToUnit == null)
                    {
                        LeagueSharp.Drawing.OnDraw += DrawingManager.Drawing_OnDrawChamp;
                    }
                    _damageToUnit = value;
                }
            }

            public static DamageToUnitDelegate DamageToMonster
            {
                get { return _damageToMonster; }

                set
                {
                    if (_damageToMonster == null)
                    {
                        LeagueSharp.Drawing.OnDraw += DrawingManager.Drawing_OnDrawMonster;
                    }
                    _damageToMonster = value;
                }
            }
        }
        public static void Initilize()
        {
            LeagueSharp.Drawing.OnDraw += DrawingManager.Drawing_OnDrawChamp;
            Drawing.DamageToUnit = DamageCalc.CalculateRendDamage;
            Drawing.DamageToMonster = DamageCalc.CalculateRendDamage;
            LeagueSharp.Drawing.OnDraw += Drawing_OnDraw;
            LeagueSharp.Drawing.OnDraw += Drawing_OnDrawChamp;
            LeagueSharp.Drawing.OnDraw += Drawing_OnDrawMonster;
        }
        private static void Drawing_OnDraw(EventArgs args)
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
                if (Properties.MainMenu.Item("cDrawRendRange").GetValue<Circle>().Active &&
                    Properties.Champion.E.Level > 0)
                    Render.Circle.DrawCircle(Properties.PlayerHero.Position, Properties.Champion.E.Range,
                        Properties.MainMenu.Item("cDrawRendRange").GetValue<Circle>().Color, 2);
            }
            catch
            {
                // DUmb color picker
            }
        }

        private static void Drawing_OnDrawMonster(EventArgs args)
        {
            try
            {
                if (!Properties.MainMenu.Item("cDrawOnMonsters").GetValue<Circle>().Active ||
                    Drawing.DamageToMonster == null)
                    return;

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    
                    if (minion.Team != GameObjectTeam.Neutral || !minion.IsValidTarget() || !minion.IsHPBarRendered)
                        continue;

                    var rendDamage = DamageCalc.CalculateRendDamage(minion);
                   // Game.PrintChat(minion.CharData.BaseSkinName.ToString());
                    // Monster bar widths and offsets from ElSmite
                    var barWidth = 0;
                    var xOffset = 0;
                    var yOffset = 0;
                    var yOffset2 = 0;
                    var display = true;
                    switch (minion.CharData.BaseSkinName)
                    {
                        case "SRU_Red":
                        case "SRU_Blue":
                        case "SRU_Dragon":
                            barWidth = 145;
                            xOffset = 3;
                            yOffset = 18;
                            yOffset2 = 10;
                            break;

                        case "SRU_Baron":
                            barWidth = 194;
                            xOffset = -22;
                            yOffset = 13;
                            yOffset2 = 16;
                            break;

                        case "Sru_Crab":
                            barWidth = 61;
                            xOffset = 45;
                            yOffset = 34;
                            yOffset2 = 3;
                            break;

                        case "SRU_Krug":
                            barWidth = 81;
                            xOffset = 58;
                            yOffset = 18;
                            yOffset2 = 4;
                            break;

                        case "SRU_Gromp":
                            barWidth = 87;
                            xOffset = 62;
                            yOffset = 18;
                            yOffset2 = 4;
                            break;

                        case "SRU_Murkwolf":
                            barWidth = 75;
                            xOffset = 54;
                            yOffset = 19;
                            yOffset2 = 4;
                            break;

                        case "SRU_Razorbeak":
                            barWidth = 75;
                            xOffset = 54;
                            yOffset = 18;
                            yOffset2 = 4;
                            break;

                        default:
                            display = false;
                            break;
                    }
                    if (!display) continue;

                    var barPos = minion.HPBarPosition;
                    var percentHealthAfterDamage = Math.Max(0, minion.Health - rendDamage) / minion.MaxHealth;
                    var yPos = barPos.Y + yOffset;
                    var xPosDamage = barPos.X + xOffset + barWidth * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + xOffset + barWidth * minion.Health / minion.MaxHealth;

                    if (Properties.MainMenu.Item("bDisplayRemainingHealth").GetValue<bool>()  && minion.Health > rendDamage || !Properties.MainMenu.Item("cKillableText").GetValue<Circle>().Active)
                    {

                        if (rendDamage > 0)
                        {
                            switch (minion.CharData.BaseSkinName)
                            {

                                case "SRU_Red":
                                case "SRU_Blue":
                                case "SRU_Dragon":
                                case "SRU_Baron":
                                    var playerPos = LeagueSharp.Drawing.WorldToScreen(Properties.PlayerHero.Position);
                                    String mobString = String.Format("{0}:Remaining HP :{1}",
                                        minion.CharData.BaseSkinName, Math.Ceiling(minion.Health - rendDamage));
                                    LeagueSharp.Drawing.DrawText(
                                        playerPos.X - LeagueSharp.Drawing.GetTextExtent(mobString).Width + 50,
                                        playerPos.Y - LeagueSharp.Drawing.GetTextExtent(mobString).Height + 60,
                                        Color.DimGray, mobString);
                                    break;
                                default:
                                    LeagueSharp.Drawing.DrawText(minion.HPBarPosition.X + xOffset,
                                        minion.HPBarPosition.Y,
                                        Properties.MainMenu.Item("cKillableText").GetValue<Circle>().Color,
                                        Math.Ceiling(minion.Health - rendDamage).ToString(CultureInfo.CurrentCulture));
                                    break;
                            }
                        }
                    }
                        if (Properties.MainMenu.Item("cFillMonster").GetValue<Circle>().Active)
                    {
                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + xOffset;

                        for (var i = 0; i < differenceInHp; i++)
                        {
                            LeagueSharp.Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1,
                                Properties.MainMenu.Item("cFillMonster").GetValue<Circle>().Color);
                        }
                    }
                    else
                        LeagueSharp.Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1,
                            Properties.MainMenu.Item("cDrawOnMonsters").GetValue<Circle>().Color);
        
                    if (!(rendDamage > minion.Health)) continue;
                    if (!Properties.MainMenu.Item("cKillableText").GetValue<Circle>().Active) return;

                    LeagueSharp.Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y,
                        Properties.MainMenu.Item("cKillableText").GetValue<Circle>().Color, "Killable");
                }
            }
            catch
            {
                //Dumb color picker
            }
        }

        private static Color GetColor(bool b)
        {
            return b ? Color.White : Color.SlateGray;
        }


        private static void Drawing_OnDrawChamp(EventArgs args)
        {
            try
            {
                if (!Properties.MainMenu.Item("bDrawOnChamp").GetValue<bool>() ||
                    Drawing.DamageToUnit == null)
                    return;

                // For every enemis in E range
                foreach (
                    var unit in
                        HeroManager.Enemies.Where(
                            unit => unit.IsValid && unit.IsHPBarRendered && Properties.Champion.E.IsInRange(unit)))
                {
                    const int xOffset = 10;
                    const int yOffset = 20;
                    const int width = 103;
                    const int height = 8;

                    var barPos = unit.HPBarPosition;
                    var damage = DamageCalc.CalculateRendDamage(unit);
                    var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                    var yPos = barPos.Y + yOffset;
                    var xPosDamage = barPos.X + xOffset + width * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + xOffset + width * unit.Health / unit.MaxHealth;

                    if (Properties.MainMenu.Item("cDrawTextOnChamp").GetValue<Circle>().Active && damage > unit.Health)
                        LeagueSharp.Drawing.DrawText(barPos.X + xOffset, barPos.Y + yOffset - 13,
                            Properties.MainMenu.Item("cDrawTextOnChamp").GetValue<Circle>().Color, "Killable");

                    LeagueSharp.Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + height, 1, Color.LightGray);

                    if (!Properties.MainMenu.Item("cDrawFillOnChamp").GetValue<Circle>().Active) return;

                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (var i = 0; i < differenceInHp; i++)
                    {
                        LeagueSharp.Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + height, 1,
                            Properties.MainMenu.Item("cDrawFillOnChamp").GetValue<Circle>().Color);
                    }
                }

                if (!Properties.MainMenu.Item("bDrawTextOnSelf").GetValue<bool>())
                    return;

                var playerPos = LeagueSharp.Drawing.WorldToScreen(Properties.PlayerHero.Position);
                var jungleBool = Properties.MainMenu.Item("bUseJungleClear").GetValue<KeyBind>().Active
                    ? "Enabled"
                    : "Disabled";
                var jungleClear = string.Format("Jungle Clear: {0}", jungleBool);
                var vColor = GetColor(Properties.MainMenu.Item("bUseJungleClear").GetValue<KeyBind>().Active);
                LeagueSharp.Drawing.DrawText(playerPos.X - LeagueSharp.Drawing.GetTextExtent(jungleClear).Width + 50,
                    playerPos.Y - LeagueSharp.Drawing.GetTextExtent(jungleClear).Height + 30, vColor, jungleClear);
            }
            catch
            {
                //Dumb Color Picker
            }
        }

        #endregion Public Functions
    }
}