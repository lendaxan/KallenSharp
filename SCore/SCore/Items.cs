using System;
using LeagueSharp.Common;
using LeagueSharp;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Item = LeagueSharp.Common.Items.Item;

namespace SCore
{
    internal class Items : Base
    {
        public class _Item
        {
            private static Item Item;
            private static bool _Enabled;

            public _Item(Item _item,bool __Enabled )
            {
                Item = _item;
                _Enabled = __Enabled;
            }

            public bool Enabled
            {
                get { return _Enabled; }
                set { _Enabled = value; }
            }

            public Item GetItem()
            {
                return Item;
            }
        }

        //Create Item's and set them inactive
        public struct Offensive
        {
            public static _Item Botrk = new _Item(ItemData.Blade_of_the_Ruined_King.GetItem(),false);
            public static _Item Cutless = new _Item(ItemData.Bilgewater_Cutlass.GetItem(), false);
            public static _Item Hydra = new _Item(ItemData.Ravenous_Hydra_Melee_Only.GetItem(), false);
            public static _Item Tiamat = new _Item(ItemData.Tiamat_Melee_Only.GetItem(), false);
            public static _Item GunBlade = new _Item(ItemData.Hextech_Gunblade.GetItem(), false);
            public static _Item Muraman = new _Item(ItemData.Muramana.GetItem(), false);
            public static _Item GhostBlade = new _Item(ItemData.Youmuus_Ghostblade.GetItem(), false);
        }

        public struct Defensive
        {
            public static _Item Qss = new _Item(ItemData.Quicksilver_Sash.GetItem(), false);
            public static _Item Merc = new _Item(ItemData.Mercurial_Scimitar.GetItem(), false);
        }

        public static void Initilize()
        {
            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
            Game.OnUpdate += OnUpdate;
        }

        private static int GetDangerLevel()
        {
            var count = 0;
            //Bad
            if (Player.HasBuffOfType(BuffType.Suppression))
                count += 3;
            if (Player.HasBuffOfType(BuffType.Taunt))
                count += 3;
            if (Player.HasBuffOfType(BuffType.Stun))
                count += 3;

            //Not good
            if (Player.HasBuffOfType(BuffType.Snare))
                count += 2;
            if (Player.HasBuffOfType(BuffType.Polymorph))
                count += 2;
            if (Player.HasBuffOfType(BuffType.Charm))
                count += 2;
            if (Player.HasBuffOfType(BuffType.Fear))
                count += 2;

            //Meh
            if (Player.HasBuffOfType(BuffType.Blind))
                count += 1;
            if (Player.HasBuffOfType(BuffType.Flee))
                count += 1;
            if (Player.HasBuffOfType(BuffType.Sleep))
                count += 1;
            if (Player.HasBuffOfType(BuffType.Slow))
                count += 1;

            return count;
        }

        private static void OnUpdate(EventArgs args)
        {
            #region Offensive
            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null) return;
            if (Offensive.Botrk.Enabled && LeagueSharp.Common.Items.HasItem(Offensive.Botrk.GetItem().Id) ) // If enabled and has item
            {
                if (Offensive.Botrk.GetItem().IsReady() && Offensive.Botrk.GetItem().IsInRange(target))
                {
                    if (Mode == Orbwalking.OrbwalkingMode.Combo // If in combo mode
                        ||(Player.HealthPercent < 15)) // Player Hp less then 15%
                    {
                        LeagueSharp.Common.Items.UseItem(Offensive.Botrk.GetItem().Id, target);
                    }
                }
            }

            if (Offensive.GhostBlade.Enabled && LeagueSharp.Common.Items.HasItem(Offensive.GhostBlade.GetItem().Id) ) // If enabled and has item
            {
                if (Offensive.GhostBlade.GetItem().IsReady() && target.IsValidTarget(Player.AttackRange + Player.BoundingRadius)) // Is ready and target is in auto range 
                {
                    if (Mode == Orbwalking.OrbwalkingMode.Combo)//In combo mode
                    {
                        LeagueSharp.Common.Items.UseItem(Offensive.GhostBlade.GetItem().Id);
                    }
                }
            }
            #endregion

            #region Defensive

            if (Defensive.Qss.Enabled && LeagueSharp.Common.Items.HasItem(Defensive.Qss.GetItem().Id))
            {
                if (Defensive.Qss.GetItem().IsReady())
                {
                    if (GetDangerLevel() >= 3)
                            LeagueSharp.Common.Items.UseItem(Defensive.Qss.GetItem().Id);
                    }
                }


            if (Defensive.Merc.Enabled && LeagueSharp.Common.Items.HasItem(Defensive.Merc.GetItem().Id))
            {
                if (Defensive.Merc.GetItem().IsReady())
                {
                    if (GetDangerLevel() >= 3)
                        LeagueSharp.Common.Items.UseItem(Defensive.Merc.GetItem().Id);
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
