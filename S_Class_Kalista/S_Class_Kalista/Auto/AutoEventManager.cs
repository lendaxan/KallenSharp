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
//   Assembly to be use with LeagueSharp for champion Kalista
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    internal class AutoEventManager
    {
        #region Public Functions

        public static void Initilize()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Humanizer.Limiter.CheckDelay("AutoDelay")) return;
            Humanizer.Limiter.UseTick("AutoDelay");
            EventCheck();
        }
        private static void EventCheck()
        {
            try
            {
                //Sentinel
                if (Properties.MainMenu.Item("bSentinel").GetValue<KeyBind>().Active)
                {
                    if (Properties.MainMenu.Item("bSentinelDragon").GetValue<bool>())
                        AutoSentinels(true);
                    if (Properties.MainMenu.Item("bSentinelBaron").GetValue<bool>())
                        AutoSentinels(false);
                }

                if (!Properties.Time.CheckRendDelay()) return;

                if (!Properties.Champion.E.IsReady()) return;

                if (Properties.MainMenu.Item("bUseEToKillEpics").GetValue<bool>())
                    if (CheckEpicMonsters()) return;

                if (Properties.MainMenu.Item("bUseEToKillBuffs").GetValue<bool>())
                    if (CheckBuffMonsters()) return;

                if (Properties.MainMenu.Item("bUseEToAutoKill").GetValue<bool>())
                    if (CheckEnemies()) return;

                if (Properties.MainMenu.Item("bUseEToKillEpicMinions").GetValue<bool>())
                    if(CheckEpicMinions())return;

                if (Properties.MainMenu.Item("bUseEToAutoKillMinions").GetValue<bool>())
                    if (CheckMinions()) return;

                if (Properties.MainMenu.Item("bAutoEOnStacksAndMinions").GetValue<bool>())
                    if (AutoEOnStacksAndMinions()) return;

                if (Properties.MainMenu.Item("bEBeforeDeath").GetValue<bool>())
                {
                    if (EBeforeDeath()) return;
                }

                // ReSharper disable once RedundantJumpStatement
                if (Properties.MainMenu.Item("bUseEOnLeave").GetValue<bool>() && AutoEOnLeave()) return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool EBeforeDeath()
        {
            if (!Properties.Time.CheckRendDelay()) return false;

            var champs = 0;
            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValid) continue;
                if (!target.IsValidTarget(Properties.Champion.E.Range)) continue;
                if (!target.HasBuff("KalistaExpungeMarker")) continue;
                if (!Properties.Time.CheckRendDelay()) continue;
                if (ObjectManager.Player.HealthPercent > Properties.MainMenu.Item("sEBeforeDeathMaxHP").GetValue<Slider>().Value) continue;
                if (target.GetBuffCount("kalistaexpungemarker") < Properties.MainMenu.Item("sEBeforeDeathMinStacks").GetValue<Slider>().Value) continue;
                champs++;
                if (champs < Properties.MainMenu.Item("sEBeforeDeathChamps").GetValue<Slider>().Value) continue;

                Properties.Champion.UseRend();
#if DEBUG_MODE
                Console.WriteLine("Using E Before Death E:{0}", Environment.TickCount);
#endif
                return true;
            }
            return false;
        }

        public static void CheckNonKillables(AttackableUnit minion)
        {
            try
            {
                if (!Properties.Time.CheckNonKillable()) return;
                if (!Properties.MainMenu.Item("bUseENonKillables").GetValue<bool>() || !Properties.Champion.E.IsReady()) return;
                if (!(minion.Health <= DamageCalc.CalculateRendDamage((Obj_AI_Base)minion)) || minion.Health > 60) return;
                if(minion.IsValidTarget(Properties.Champion.E.Range))
#if DEBUG_MODE
            Console.WriteLine("Killing NonKillables");
#endif
                Properties.Champion.UseNonKillableRend();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static bool CheckEnemies()
        {
            if (!Properties.Time.CheckRendDelay()) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValid) continue;
                if (!target.IsValidTarget(Properties.Champion.E.Range)) continue;
                if (DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (!Properties.Time.CheckRendDelay()) continue;
                if (!Properties.Champion.E.IsInRange(target)) continue;
                if (DamageCalc.CalculateRendDamage(target) < target.Health) continue;
                if (target.IsDead) continue;
#if DEBUG_MODE
                Console.WriteLine("Using Killing Enemy E:{0}", Properties.Time.TickCount);
#endif
                Properties.Champion.UseRend();
                return true;
            }
            return false;
        }

        #endregion Public Functions

        #region Private Functions

        private static bool AutoEOnStacksAndMinions()
        {
            if (!Properties.Time.CheckRendDelay()) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValid) continue;
                if (!target.IsValidTarget(Properties.Champion.E.Range)) continue;
                if (DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (!Properties.Time.CheckRendDelay()) continue;
                var stacks = target.GetBuffCount("kalistaexpungemarker");
                if (stacks < Properties.MainMenu.Item("sUseEOnChampStacks").GetValue<Slider>().Value) continue;
                var minions = MinionManager.GetMinions(Properties.PlayerHero.ServerPosition, Properties.Champion.E.Range);
                var count = minions.Count(minion => minion.Health <= DamageCalc.CalculateRendDamage(minion) && minion.IsValid);
                if (Properties.MainMenu.Item("sUseEOnMinionKilled").GetValue<Slider>().Value > count) continue;
                Properties.Champion.UseRend();
#if DEBUG_MODE
                Console.WriteLine("Using Stacks And Minions E:{0}", Environment.TickCount);
#endif
                return true;
            }
            return false;
        }

        private static void AutoSentinels(bool dragon)
        {
            if (!Properties.Champion.W.IsReady()) return;

            if (dragon)
            {
                if (!(ObjectManager.Player.Distance(SummonersRift.River.Dragon) <= Properties.Champion.W.Range)) return; Properties.Champion.W.Cast(SummonersRift.River.Dragon);
            }
            else
            {
                if (!(ObjectManager.Player.Distance(SummonersRift.River.Baron) <= Properties.Champion.W.Range)) return;
                Properties.Champion.W.Cast(SummonersRift.River.Baron);
            }
        }

        private static bool CheckEpicMonsters()
        {
            // ReSharper disable once UnusedVariable

            if (!Properties.Time.CheckRendDelay()) return false;

            if (Properties.MainMenu.Item("bUseManaManager").GetValue<bool>())
                if (Properties.PlayerHero.ManaPercent < Properties.MainMenu.Item("sMinManaE").GetValue<Slider>().Value) return false;

            if (!MinionManager.GetMinions(Properties.PlayerHero.ServerPosition,
                Properties.Champion.E.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth)
                .Where(mob => mob.Name.Contains("Baron") || mob.Name.Contains("Dragon")).Any(mob => DamageCalc.CalculateRendDamage(mob) > mob.Health))
                return false;

            Properties.Champion.UseRend();
#if DEBUG_MODE
            Console.WriteLine("Using Baron and Dragon E:{0}", Properties.Time.TickCount);
#endif
            return true;
        }

        private static bool AutoEOnLeave()
        {
            if (!Properties.Time.CheckRendDelay()) return false;

            if (Properties.MainMenu.Item("bUseManaManager").GetValue<bool>())
                if (Properties.PlayerHero.ManaPercent < Properties.MainMenu.Item("sMinManaE").GetValue<Slider>().Value) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValid) continue;
                if (!target.IsValidTarget(Properties.Champion.E.Range)) continue;
                if (!Properties.Time.CheckRendDelay()) continue;
                if (DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (!Properties.Champion.E.IsInRange(target)) continue;
                if (target.IsDead) continue;
                if (target.Distance(Properties.PlayerHero) < Properties.Champion.E.Range - 100) continue;
                var stacks = target.GetBuffCount("kalistaexpungemarker");
                if (stacks <= Properties.MainMenu.Item("sStacksOnLeave").GetValue<Slider>().Value) continue;
#if DEBUG_MODE
                Console.WriteLine("Using Rend On Long  E:{0}", Properties.Time.TickCount);
#endif
                Properties.Champion.UseRend();
                return true;
            }
            return false;
        }

        private static bool CheckBuffMonsters()
        {
            // ReSharper disable once UnusedVariable
            if (!Properties.Time.CheckRendDelay()) return false;
            if (Properties.MainMenu.Item("bUseManaManager").GetValue<bool>())
                if (Properties.PlayerHero.ManaPercent < Properties.MainMenu.Item("sMinManaE").GetValue<Slider>().Value) return false;

            foreach (var monster in MinionManager.GetMinions(Properties.PlayerHero.ServerPosition,
                Properties.Champion.E.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth))
            {
                if (!monster.CharData.BaseSkinName.Equals("SRU_Red") &&
                    !monster.CharData.BaseSkinName.Equals("SRU_Blue"))
                    continue;

                if (!(DamageCalc.CalculateRendDamage(monster) > monster.Health)) continue;
#if DEBUG_MODE
                Console.WriteLine("Using Buff E:{0}", Properties.Time.TickCount);
#endif
                Properties.Champion.UseRend();
                return true;
            }
            return false;
        }

        private static bool CheckEpicMinions()
        {
            bool found = false;
            if (!Properties.Time.CheckRendDelay()) return false;

            if (Properties.MainMenu.Item("bUseManaManager").GetValue<bool>())
                if (Properties.PlayerHero.ManaPercent < Properties.MainMenu.Item("sMinManaE").GetValue<Slider>().Value) return false;

            foreach (var epic in MinionManager.GetMinions(Properties.PlayerHero.ServerPosition, Properties.Champion.E.Range).Where(epic => epic.IsValid))
            {
                if (epic.CharData.BaseSkinName.ToLower().Contains("siege"))
                {
                    if (DamageCalc.CalculateRendDamage(epic) < epic.Health) continue;
                    found = true;
                    break;
                }
                if (!epic.CharData.BaseSkinName.ToLower().Contains("super")) continue;
                if (DamageCalc.CalculateRendDamage(epic) < epic.Health) continue;
                found = true;
                break;
            }
            if (!found) return false;

            Properties.Champion.UseRend();
            return true;
        }

        private static bool CheckMinions()
        {
            if (!Properties.Time.CheckRendDelay()) return false;

            if (Properties.MainMenu.Item("bUseManaManager").GetValue<bool>())
                if (Properties.PlayerHero.ManaPercent < Properties.MainMenu.Item("sMinManaE").GetValue<Slider>().Value) return false;

            var count = 0;

            var minions = MinionManager.GetMinions(Properties.PlayerHero.ServerPosition, Properties.Champion.E.Range);
            count += minions.Count(minion => minion.Health <= DamageCalc.CalculateRendDamage(minion) && minion.IsValid);
            if (Properties.MainMenu.Item("sAutoEMinionsKilled").GetValue<Slider>().Value > count) return false;
#if DEBUG_MODE
            Console.WriteLine("Using Minion E:{0}", Properties.Time.TickCount);
#endif
            Properties.Champion.UseRend();
            return true;
        }

        #endregion Private Functions
    }
}