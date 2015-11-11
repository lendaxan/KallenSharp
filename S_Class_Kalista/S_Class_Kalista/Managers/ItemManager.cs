using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Item = LeagueSharp.Common.Items.Item;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    class ItemManager
    {

        //Create Item's and set them inactive
        public struct Offensive
        {
            public static Item Botrk = new Item(ItemData.Blade_of_the_Ruined_King.GetItem().Id);
            public static Item Cutless = new Item(ItemData.Bilgewater_Cutlass.GetItem().Id);
            public static Item Hydra = new Item(ItemData.Ravenous_Hydra_Melee_Only.GetItem().Id);
            public static Item Tiamat = new Item(ItemData.Tiamat_Melee_Only.GetItem().Id);
            public static Item GunBlade = new Item(ItemData.Hextech_Gunblade.GetItem().Id);
            public static Item Muraman = new Item(ItemData.Muramana.GetItem().Id);
            public static Item GhostBlade = new Item(ItemData.Youmuus_Ghostblade.GetItem().Id);
        }

        public struct Defensive
        {
            public static Item Qss = new Item(ItemData.Quicksilver_Sash.GetItem().Id);
            public static Item Merc = new Item(ItemData.Mercurial_Scimitar.GetItem().Id);
        }

        public static void Initilize()
        {
            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
            Game.OnUpdate += OnUpdate;
        }
    
        private static void OnUpdate(EventArgs args)
        {
            #region Offensive

            if (!Humanizer.Limiter.CheckDelay("ItemDelay")) return;

            Humanizer.Limiter.UseTick("ItemDelay");

            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null) return;

            var inCombo = false;
            switch (Properties.MainMenu.Item("sOrbwalker").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    inCombo = Properties.LandWalker.ActiveMode == LandWalker.OrbwalkingMode.Combo;
                    break;

                case 1:
                    inCombo = Properties.CommonWalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;
                    break;
            }

            if (Properties.MainMenu.Item("bUseBork").GetValue<bool>() && Items.HasItem(Offensive.Botrk.Id))
                // If enabled and has item
            {
                if (Offensive.Botrk.IsReady())
                {
                    if (
                        target.IsValidTarget(Properties.PlayerHero.AttackRange +
                                             Properties.PlayerHero.BoundingRadius) ||
                        Properties.PlayerHero.HealthPercent <
                        Properties.MainMenu.Item("sMinPlayerHP").GetValue<Slider>().Value)
                    {
                        // In auto Range or about to die
                        if (Properties.MainMenu.Item("bUseOffensiveOnlyInCombo").GetValue<bool>() && inCombo &&
                            target.HealthPercent < Properties.MainMenu.Item("sEnemyBorkHP").GetValue<Slider>().Value
                            //in combo and target hp less then
                            ||
                            !Properties.MainMenu.Item("bUseOffensiveOnlyInCombo").GetValue<bool>() &&
                            target.HealthPercent < Properties.MainMenu.Item("sEnemyBorkHP").GetValue<Slider>().Value
                            //not in combo but target HP less then
                            ||
                            (Properties.PlayerHero.HealthPercent <
                             Properties.MainMenu.Item("sMinPlayerHP").GetValue<Slider>().Value))
                            //Player hp less then
                        {
                            Items.UseItem(Offensive.Botrk.Id, target);
                            return;
                        }

                    }
                }
            }

            if (Properties.MainMenu.Item("bUseBork").GetValue<bool>() && Items.HasItem(Offensive.Cutless.Id))
                // If enabled and has item
            {
                if (Offensive.Cutless.IsReady())
                {
                    if (
                        target.IsValidTarget(Properties.PlayerHero.AttackRange +
                                             Properties.PlayerHero.BoundingRadius) ||
                        Properties.PlayerHero.HealthPercent <
                        Properties.MainMenu.Item("sMinPlayerHP").GetValue<Slider>().Value)
                    {
                        // In auto Range or about to die
                        if (Properties.MainMenu.Item("bUseOffensiveOnlyInCombo").GetValue<bool>() && inCombo &&
                            target.HealthPercent <
                            Properties.MainMenu.Item("sEnemyBorkHP").GetValue<Slider>().Value
                            //in combo and target hp less then
                            ||
                            !Properties.MainMenu.Item("bUseOffensiveOnlyInCombo").GetValue<bool>() &&
                            target.HealthPercent <
                            Properties.MainMenu.Item("sEnemyBorkHP").GetValue<Slider>().Value
                            //not in combo but target HP less then
                            ||
                            (Properties.PlayerHero.HealthPercent <
                             Properties.MainMenu.Item("sMinPlayerHP").GetValue<Slider>().Value))
                            //Player hp less then
                        {
                            Items.UseItem(Offensive.Cutless.Id, target);
                            return;
                        }
                    }
                }
            }

            if (Properties.MainMenu.Item("bUseYoumuu").GetValue<bool>() && Items.HasItem(Offensive.GhostBlade.Id))
                // If enabled and has item
            {
                if (Offensive.GhostBlade.IsReady() &&
                    target.IsValidTarget(Properties.PlayerHero.AttackRange + Properties.PlayerHero.BoundingRadius))
                    // Is ready and target is in auto range 
                {
                    if (inCombo)
                    {
                        Items.UseItem(Offensive.GhostBlade.Id);
                        return;
                    }
                }
            }

            #endregion

            #region Defensive

            if (Properties.MainMenu.Item("bUseQSS").GetValue<bool>() && Items.HasItem(Defensive.Qss.Id))
            {
                if (Properties.MainMenu.Item("bUseDefensiveOnlyInCombo").GetValue<bool>() && inCombo ||
                    !Properties.MainMenu.Item("bUseDefensiveOnlyInCombo").GetValue<bool>())
                {
                    if (Defensive.Qss.IsReady())
                    {

                            foreach (var buff in Properties.Bufftype.Where(buff => Properties.MainMenu.Item("bUseQSS." + buff).GetValue<bool>()))
                            {
                                if (Properties.PlayerHero.HasBuffOfType(buff))
                                    Utility.DelayAction.Add(Properties.MainMenu.Item("sQSSDelay").GetValue<Slider>().Value, () => Items.UseItem(Defensive.Qss.Id));
                                
                            }
                    }
                }
            }


            if (Properties.MainMenu.Item("bUseMerc").GetValue<bool>() && Items.HasItem(Defensive.Merc.Id))
            {
                if (Properties.MainMenu.Item("bUseDefensiveOnlyInCombo").GetValue<bool>() && inCombo ||
                    !Properties.MainMenu.Item("bUseDefensiveOnlyInCombo").GetValue<bool>())
                {
                    if (Defensive.Merc.IsReady())
                    {
                        foreach (var buff in Properties.Bufftype.Where(buff => Properties.MainMenu.Item("bUseMerc." + buff).GetValue<bool>()))
                        {
                            if (Properties.PlayerHero.HasBuffOfType(buff))
                                Utility.DelayAction.Add(Properties.MainMenu.Item("sMercDelay").GetValue<Slider>().Value, () => Items.UseItem(Defensive.Qss.Id));

                        }
                    }
                }
            }

            #endregion
        }

        private static void Before_Attack(Orbwalking.BeforeAttackEventArgs args)
        {

        }

        private static void After_Attack(AttackableUnit unit, AttackableUnit target)
        {

        }
    }
}