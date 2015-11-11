using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Handlers;

namespace S_Plus_Class_Kalista.Libaries
{
    class RendCheck : Core
    {

        public static void Load()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Champion.E.IsReady()) return;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return;

            var used = false;
            short count = 0;
            while (!used)
            {
                used = GetUsed(count++);
            }
        }

        private static bool GetUsed(short count)
        {
            switch (count)
            {
                case 0:
                    return RendEpicMonsters();
                case 1:
                    return RendEnemies();
                case 2:
                    return RendBuffs();
                case 3:
                    return RendEpicsMinions();
                case 4:
                    return RendHarass();
                case 5:
                    return RendMinions();
                case 6:
                    return RendSmallMonsters();
                case 7:
                    return RendBeforeDeath();
                case 8:
                    return RendOnLeave();
                case 9:
                    return true;
            }
            return false;
        }

        public static void CheckNonKillables(AttackableUnit minion)
        {
                if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendNonKillables").GetValue<bool>()) return;
                if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.NonKillableDelay")) return;
                if (!(minion.Health <= Damage.DamageCalc.CalculateRendDamage((Obj_AI_Base)minion)) || minion.Health > 60) return;
                if (!minion.IsValidTarget(Champion.E.Range))return;
                Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.NonKillableDelay");
                Champion.E.Cast();
        }

        private static bool RendEpicMonsters()
        {
            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendEpics").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            if (!MinionManager.GetMinions(Player.ServerPosition,
                Champion.E.Range,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth)
                .Where(mob => mob.Name.Contains("Baron") || mob.Name.Contains("Dragon")).Any(mob => Damage.DamageCalc.CalculateRendDamage(mob) > mob.Health))
                return false;


            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }

        public static bool RendEnemies()
        {
            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendEnemyChampions").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValidTarget(Champion.E.Range)) continue;
                if (Damage.DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (Damage.DamageCalc.CalculateRendDamage(target) < target.Health) continue;
                if (target.IsDead) continue;

                Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();
                return true;
            }
            return false;
        }

        private static bool RendBuffs()
        {

            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendBuffs").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            if (
                !MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .Where(
                        monster =>
                            monster.CharData.BaseSkinName.Equals("SRU_Red") ||
                            monster.CharData.BaseSkinName.Equals("SRU_Blue"))
                    .Any(monster => Damage.DamageCalc.CalculateRendDamage(monster) > monster.Health))
                return false;


            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }

        private static bool RendEpicsMinions()
        {
            var found = false;
            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendEpicMinions").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            foreach (var epic in MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range).Where(epic => epic.IsValid))
            {
                if (epic.CharData.BaseSkinName.ToLower().Contains("siege"))
                {
                    if (Damage.DamageCalc.CalculateRendDamage(epic) < epic.Health) continue;
                    found = true;
                    break;
                }
                if (!epic.CharData.BaseSkinName.ToLower().Contains("super")) continue;
                if (Damage.DamageCalc.CalculateRendDamage(epic) < epic.Health) continue;
                found = true;
                break;
            }

            if (!found) return false;
            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }

        private static bool RendHarass()
        {
            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendMinions").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValidTarget(Champion.E.Range)) continue;
                if (Damage.DamageCalc.CheckNoDamageBuffs(target)) continue;
                var stacks = target.GetBuffCount("kalistaexpungemarker");
                if (stacks < SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendHarrassKill.Slider.Stacks").GetValue<Slider>().Value) continue;
                var minions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
                var count = minions.Count(minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);
                if (SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendHarrassKill.Slider.Killed").GetValue<Slider>().Value > count) continue;

                Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();
                return true;
            }
            return false;
        }

        private static bool RendMinions()
        {
            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendMinions").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;
            var minions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
            var count = minions.Count(minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);

            if (SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendMinions.Slider.Killed").GetValue<Slider>().Value >
                count)
                return false;

            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();

            return true;
        }

        private static bool RendSmallMonsters()
        {
            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendSmallMonster").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

            if (
                !MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth)
                    .Any(monster => Damage.DamageCalc.CalculateRendDamage(monster) > monster.Health))
                return false;


            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
            Champion.E.Cast();
            return true;
        }
        private static bool RendBeforeDeath()
        {
            
                if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendBeforeDeath").GetValue<bool>()) return false;
                if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;

                var champs = 0;
            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValidTarget(Champion.E.Range)) continue;
                if (!target.HasBuff("KalistaExpungeMarker")) continue;
                if (ObjectManager.Player.HealthPercent > SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendBeforeDeath.Slider.PercentHP").GetValue<Slider>().Value) continue;
                if (target.GetBuffCount("kalistaexpungemarker") < SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendBeforeDeath.Slider.Stacks").GetValue<Slider>().Value) continue;
                champs++;

                if (champs < SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendBeforeDeath.Slider.Enemies").GetValue<Slider>().Value) continue;

                Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();

                return true;
            }
            return false;
        }

        private static bool RendOnLeave()
        {

            if (!SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendOnLeave").GetValue<bool>()) return false;
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return false;


            foreach (var target in HeroManager.Enemies)
            {
                if (!target.IsValidTarget(Champion.E.Range)) continue;
                if (Damage.DamageCalc.CheckNoDamageBuffs(target)) continue;
                if (target.IsDead) continue;
                if (target.Distance(Player) < Champion.E.Range - 50) continue;
                var stacks = target.GetBuffCount("kalistaexpungemarker");
                if (stacks <= SMenu.Item(Handlers.RendHandler._MenuItemBase + "Boolean.RendOnLeave.Slider.Stacks").GetValue<Slider>().Value) continue;


                Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                Champion.E.Cast();

                return true;
            }
            return false;
        }
    }
}
