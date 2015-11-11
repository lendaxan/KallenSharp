// <copyright file="OrbWalkerManager.cs" company="Kallen">
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

using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Collision = LeagueSharp.Common.Collision;

namespace S_Class_Lucian
{
    internal class OrbWalkerManager
    {
        #region Public Functions

        public static void DoTheWalk()
        {
            switch (Properties.LukeOrbWalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        #endregion Public Functions

        #region Private Functions

        private static HitChance GetHitChance()
        {
            //switch (Properties.MainMenu.Item("slQprediction").GetValue<StringList>().SelectedIndex)
            //{
            //    case 0:
            //        return HitChance.VeryHigh;

            //    case 1:
            //        return HitChance.High;

            //    case 2:
            //        return HitChance.Dashing;
            //}
            return HitChance.VeryHigh;
        }

        private static void Combo()
        {
            //Console.WriteLine("Checking Delay");
            if (!Properties.Time.CheckLastDelay()) return;

            if (Properties.Champion.PassiveReady()) return;


            var target = TargetSelector.GetTarget(Properties.Champion.E.Range + Properties.Champion.Q.Range, TargetSelector.DamageType.Physical);
            if (target == null) return;


            if (Properties.Champion.Q.IsReady())
            {
                var predictionPosition = Properties.Champion.Q.GetPrediction(target);
                if (predictionPosition.Hitchance >= GetHitChance())
                {
                    if (target.IsValidTarget(Properties.Champion.Q.Range))
                    {
                        Console.WriteLine("Checking Q");
                        Properties.Champion.Q.Cast(target);
                        Properties.PlayerHero.IssueOrder(GameObjectOrder.AutoAttack, target);
                        Properties.Champion.UseTick();
                        return;
                    }
                }
            }

            if (Properties.Champion.W.IsReady())
            {
                var predictionPosition = Properties.Champion.W.GetPrediction(target);
                if (predictionPosition.Hitchance >= GetHitChance())
                {
                    if (target.IsValidTarget(Properties.Champion.W.Range))
                    {
                        Properties.Champion.W.Cast(predictionPosition.CastPosition);
                        Properties.PlayerHero.IssueOrder(GameObjectOrder.AutoAttack, target);
                        Properties.Champion.UseTick();
                        return;
                    }
                }
            }

            if (Properties.Champion.E.IsReady())
            {
                if (
                    target.IsValidTarget(Properties.Champion.E.Range + Properties.PlayerHero.AttackRange +
                                         Properties.PlayerHero.BoundingRadius))
                {
                    if (ObjectManager.Player.Distance(target) <= 450)
                        Properties.Champion.E.Cast(eAwayFrom());
                    else
                    Properties.Champion.E.Cast(target);

                    Properties.PlayerHero.IssueOrder(GameObjectOrder.AutoAttack, target);
                    Properties.Champion.UseTick();
                    return;

                }
            }

            if (Properties.Champion.E.IsReady() && Properties.Champion.Q.IsReady())
            {
                if (ObjectManager.Player.Distance(target) - 100 <= Properties.Champion.Q.Range)
                {
                    Properties.Champion.E.Cast(target);
                    Properties.PlayerHero.IssueOrder(GameObjectOrder.AutoAttack, target);
                    Properties.Champion.Q.Cast(target);
                    Properties.PlayerHero.IssueOrder(GameObjectOrder.AutoAttack, target);
                    Properties.Champion.UseTick();
                    return;
                }
            }

            if (ObjectManager.Player.Distance(target) <= 550) return;

            if (Properties.Champion.R.IsReady())
            {
                if (ObjectManager.Player.Distance(target) <= Properties.Champion.R.Range - 400)
                {
                    var predictionPosition = Properties.Champion.R.GetPrediction(target);
                    if (predictionPosition.Hitchance >= GetHitChance())
                    {
                        Properties.Champion.R.Cast(predictionPosition.CastPosition);
                    }
                }
            }

        }

        public static bool enemIsOnMe(Obj_AI_Base target)
        {
            if (!target.IsMelee() || target.IsAlly || target.IsDead)
                return false;

            var distTo = target.Distance(Properties.PlayerHero, true);
            var targetReack = target.AttackRange + target.BoundingRadius + Properties.PlayerHero.BoundingRadius + 100;
            if (distTo > targetReack * targetReack)
                return false;

            var per = target.Direction.To2D().Perpendicular();
            var dir = new Vector3(per, 0);
            var enemDir = target.Position + dir * 40;
            return !(distTo < enemDir.Distance(Properties.PlayerHero.Position, true));
        }

        public static Vector2 eAwayFrom()
        {
            Vector2 backTo = Properties.PlayerHero.Position.To2D();
            Obj_AI_Hero targ = null;
            int count = 0;
            foreach (var enem in ObjectManager.Get<Obj_AI_Hero>().Where(enemIsOnMe))
            {
                targ = enem;
                count++;
                backTo -= (enem.Position - Properties.PlayerHero.Position).To2D();
            }
            return Properties.PlayerHero.Position.To2D().Extend(backTo, 425);
        }

        private static void Mixed()
        {
           

        }

        private static void LaneClear()
        {

        }

        private static void LastHit()
        {
            // Fuck that
        }

        #endregion Private Functions
    }
}