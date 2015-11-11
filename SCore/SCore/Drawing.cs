using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace SCore
{
    internal class Drawing : Base
    {
        public class Monsters
        {
            public static bool EnableDraw { get; set; }
            public static bool EnableMonsterDraw { get; set; }
            public static bool EnableMonsterFill { get; set; }
            public static bool EnableText { get; set; }
            public static Color LineColor { get; set; }
            public static Color FillColor { get; set; }
            public static Color KillableColor { get; set; }
            private static DamageToUnitDelegate _damageToMonster;

            public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

            public static DamageToUnitDelegate DamageToMonster
            {
                get { return _damageToMonster; }

                set
                {
                    if (_damageToMonster == null)
                    {
                        LeagueSharp.Drawing.OnDraw += OnDrawMonster;
                    }
                    _damageToMonster = value;
                }
            }

            public Monsters(bool enableDraw = false, bool monsterDraw = false, bool monsterFill = false,bool enableText = false)
            {
                EnableDraw = enableDraw;
                EnableMonsterDraw = monsterDraw;
                EnableMonsterFill = monsterFill;
                EnableText = enableText;
                Initilize();
            }

            private static void Initilize()
            {
                LeagueSharp.Drawing.OnDraw += OnDrawMonster;
            }

            public static void OnDrawMonster(EventArgs args)
            {
                try
                {
                    if (EnableMonsterDraw || DamageToMonster == null) return;
                    if (Player.IsDead) return;
                    foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                    {
                        if (!minion.IsValidTarget(2000f)) continue;//If minion is outside 2000 range dont bother displaying the damage

                        var damage = new Damage().GetDamageToMonsters(minion);// Overwritten later

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
                        var percentHealthAfterDamage = Math.Max(0, minion.Health - damage)/minion.MaxHealth;
                        var yPos = barPos.Y + yOffset;
                        var xPosDamage = barPos.X + xOffset + barWidth*percentHealthAfterDamage;
                        var xPosCurrentHp = barPos.X + xOffset + barWidth*minion.Health/minion.MaxHealth;

                        if (EnableMonsterFill)
                        {
                            var differenceInHp = xPosCurrentHp - xPosDamage;
                            var pos1 = barPos.X + xOffset;

                            for (var i = 0; i < differenceInHp; i++)
                            {
                                LeagueSharp.Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1,FillColor);
                            }
                        }
                        else
                            LeagueSharp.Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1,LineColor);

                        if (damage < minion.Health) continue;
                        if (EnableText)
                        {

                            LeagueSharp.Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y, KillableColor, "Killable");
                        }
                    }
                }
                catch
                {
                    //Dumb color picker
                }
            }

        }
    }
}
