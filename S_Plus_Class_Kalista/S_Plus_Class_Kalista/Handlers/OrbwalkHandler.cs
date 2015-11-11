using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Libaries;
using Damage = S_Plus_Class_Kalista.Libaries.Damage;

namespace S_Plus_Class_Kalista.Handlers
{
    internal class OrbwalkHandler : Core
    {
        private const string _MenuNameBase = ".LukeSkywalker.Mode Menu";
        private const string _MenuItemBase = "LukeSkywalker.Mode.";

        public static void Load()
        {
            SMenu.AddSubMenu(new Menu(".LukeSkywalker", ".LukeSkywalker"));
            LukeOrbwalker = new LukeSkywalker.Orbwalker(SMenu.SubMenu(".LukeSkywalker"));
            Game.OnUpdate += OnUpdate;
            SMenu.AddSubMenu(_Menu());
            LukeSkywalker.OnNonKillableMinion += RendCheck.CheckNonKillables;
        }

        private static void OnUpdate(EventArgs args)
        {
            HandleMode();
        }
        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "lukeskywalkerModeMenu");

            var subMenuCombo = new Menu(".Combo", "comboMenu");
            subMenuCombo.AddItem(new MenuItem(_MenuItemBase + "Combo.Boolean.UseQ", "Use Q").SetValue(true));
            subMenuCombo.AddItem(
                new MenuItem(_MenuItemBase + "Combo.Boolean.UseQ.Reset", "Use Q AA reset(Safe Exploit)").SetValue(false));
            subMenuCombo.AddItem(
                new MenuItem(_MenuItemBase + "Combo.Boolean.UseQ.Prediction", "Q prediction").SetValue(
                    new StringList(new[] {"Very High", "High", "Dashing"})));
            //subMenuCombo.AddItem(new MenuItem(_MenuItemBase + "Combo.Boolean.Rend.KillEnemies", "Use Rend to Kill Enemies").SetValue(false));

            var subMenuMixed = new Menu(".Mixed", "mixedMenu");
            subMenuMixed.AddItem(new MenuItem(_MenuItemBase + "Mixed.Boolean.UseQ", "Use Q").SetValue(true));
            subMenuMixed.AddItem(
                new MenuItem(_MenuItemBase + "Mixed.Boolean.UseQ.Reset", "Use Q AA reset(Exploit)").SetValue(false));
            subMenuMixed.AddItem(
                new MenuItem(_MenuItemBase + "Mixed.Boolean.UseQ.Prediction", "Q prediction").SetValue(
                    new StringList(new[] {"Very High", "High", "Dashing"})));
            subMenuMixed.AddItem(new MenuItem(_MenuItemBase + "Mixed.Boolean.Rend", "Use Rend on stacks").SetValue(false));
            subMenuMixed.AddItem(
                new MenuItem(_MenuItemBase + "Mixed.Boolean.Rend.Stacks", ">> Required E stacks").SetValue(new Slider(
                    4, 2, 15)));


            var subMenuClear = new Menu(".Clear", "clearMenu");
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.UseQ.Minions", "Use Q to kill minions").SetValue(true));
            subMenuClear.AddItem(new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions", "Rend Minions").SetValue(true));
            subMenuClear.AddItem(
                new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions.Killed", ">> Required minions killed").SetValue
                    (new Slider(2, 1, 5)));
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.UseQ.Prediction", "Q prediction").SetValue(
            //        new StringList(new[] {"Very High", "High", "Dashing"})));
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions", "Use Rend to kill minions").SetValue(false));
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions.Killed", ">> Required minions killed").SetValue
            //        (new Slider(4, 2, 15)));


            menu.AddSubMenu(subMenuCombo);
            menu.AddSubMenu(subMenuMixed);
            menu.AddSubMenu(subMenuClear);
            return menu;
        }

        private static void HandleMode()
        {
            switch (LukeOrbwalker.ActiveMode)
            {
                case LukeSkywalker.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case LukeSkywalker.OrbwalkingMode.Mixed:
                    Mixed();
                    break;

                case LukeSkywalker.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case LukeSkywalker.OrbwalkingMode.LastHit:
                    //LastHit();
                    break;
            }
        }


        private static HitChance GetHitChance(int index)
        {
            switch (index)
            {
                case 0:
                    return HitChance.VeryHigh;

                case 1:
                    return HitChance.High;

                case 2:
                    return HitChance.Dashing;
            }
            return HitChance.VeryHigh;
        }

        private static void LaneClear()
        {

            if (SMenu.Item(_MenuItemBase + "Clear.Boolean.Rend.Minions").GetValue<bool>() && Champion.E.IsReady())
            {
                if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay"))
                {
                    var minions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
                    var count =
                        minions.Count(
                            minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);

                    if (SMenu.Item(_MenuItemBase + "Clear.Boolean.Rend.Minions.Killed").GetValue<Slider>().Value > count)
                    {

                        Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                        Champion.E.Cast();
                    }
                }
            }
        }
        private static void Mixed()
        {

            if (SMenu.Item(_MenuItemBase + "Mixed.Boolean.UseQ").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Physical);
                var predictionPosition = Champion.Q.GetPrediction(target);
                var collisionObjects = predictionPosition.CollisionObjects;
                if (0 >= collisionObjects.Count && predictionPosition.Hitchance >=
                    GetHitChance(
                        SMenu.Item(_MenuItemBase + "Mixed.Boolean.UseQ.Prediction").GetValue<StringList>().SelectedIndex))
                {
                    if (SMenu.Item(_MenuItemBase + "Mixed.Boolean.UseQ.Reset").GetValue<bool>())
                    {
                        if (Player.IsWindingUp || Player.IsDashing())
                            Champion.Q.Cast(predictionPosition.CastPosition);
                    }

                    else if (!Player.IsWindingUp && !Player.IsDashing())
                        Champion.Q.Cast(predictionPosition.CastPosition);
                }

            }
            if (SMenu.Item(_MenuItemBase + "Mixed.Boolean.Rend").GetValue<bool>())
            {
                foreach (var stacks in from target in HeroManager.Enemies
                    where target.IsValid
                    where target.IsValidTarget(Champion.E.Range)
                    where !Damage.DamageCalc.CheckNoDamageBuffs(target)
                    select target.GetBuffCount("kalistaexpungemarker")
                    into stacks
                    where stacks >= SMenu.Item(_MenuItemBase + "Mixed.Boolean.Rend.Stacks").GetValue<Slider>().Value
                    select stacks)
                {
                    if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return;
                    Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                }
            }

        }
        private static void Combo()
        {

            if (SMenu.Item(_MenuItemBase + "Combo.Boolean.UseQ").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Physical);
                var predictionPosition = Champion.Q.GetPrediction(target);
                var collisionObjects = predictionPosition.CollisionObjects;
                if (0 >= collisionObjects.Count
                 && predictionPosition.Hitchance >=
                    GetHitChance(
                        SMenu.Item(_MenuItemBase + "Combo.Boolean.UseQ.Prediction").GetValue<StringList>().SelectedIndex))
                {
                    if (SMenu.Item(_MenuItemBase + "Combo.Boolean.UseQ.Reset").GetValue<bool>())
                    {
                        if (Player.IsWindingUp || Player.IsDashing())
                            Champion.Q.Cast(predictionPosition.CastPosition);
                    }

                    else if (!Player.IsWindingUp && !Player.IsDashing())
                        Champion.Q.Cast(predictionPosition.CastPosition);
                }

            }


        }
    }
}