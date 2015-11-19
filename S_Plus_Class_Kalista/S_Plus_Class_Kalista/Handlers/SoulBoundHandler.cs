using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Libaries;

namespace S_Plus_Class_Kalista.Handlers
{
    class SoulBoundHandler : Core
    {
        private const string _MenuNameBase = ".Soulbound Menu";
        private const string _MenuItemBase = ".Soulbound.";

        public static void Load()
        {
            SMenu.AddSubMenu(_Menu());

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "souldBoundMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.IncomingDamage", "Auto-Save soulbound if incoming damage >= HP").SetValue(true));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.IncomingDamage.RemainingHPPercent", ">> max remaining HP% after incming damage").SetValue(new Slider(10, 0, 40)));
            //menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.Boolean.AutoSavePercent", "Auto-Save soulbound HP%").SetValue(true));
            //menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.Slider.PercentHp", "Auto-Save soulbound when HP% less then").SetValue(new Slider(10, 1, 90)));
            return menu;
        }


        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<float, float> _incomingDamage = new Dictionary<float, float>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<float, float> _instantDamage = new Dictionary<float, float>();

        public static float IncomingDamage
        {
            get { return _incomingDamage.Sum(e => e.Value) + _instantDamage.Sum(e => e.Value); }
        }

        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy) return;
            if (SoulBoundHero == null) return;
            // Calculate Damage
            if ((!(sender is Obj_AI_Hero) || args.SData.IsAutoAttack()) && args.Target != null && args.Target.NetworkId == SoulBoundHero.NetworkId)
            {
                // Calculate arrival time and damage
                _incomingDamage.Add(SoulBoundHero.ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time, (float)sender.GetAutoAttackDamage(SoulBoundHero));
            }
            // Sender is a hero
            else if (sender is Obj_AI_Hero)
            {
                var attacker = (Obj_AI_Hero)sender;
                var slot = attacker.GetSpellSlot(args.SData.Name);

                if (slot == SpellSlot.Unknown) return;

                if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null && args.Target.NetworkId == SoulBoundHero.NetworkId)
                    _instantDamage.Add(Game.Time + 2, (float)attacker.GetSummonerSpellDamage(SoulBoundHero, LeagueSharp.Common.Damage.SummonerSpell.Ignite));

                else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                         ((args.Target != null && args.Target.NetworkId == SoulBoundHero.NetworkId) ||
                          args.End.Distance(SoulBoundHero.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                    _instantDamage.Add(Game.Time + 2, (float)attacker.GetSpellDamage(SoulBoundHero, slot));
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.SoulBoundDelay")) return;

            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.SoulBoundDelay");

            if (!Champion.R.IsReady()) return;

            if (SoulBoundHero == null)
            {
                SoulBoundHero =
                    HeroManager.Allies.Find(
                        ally => ally.Buffs.Any(user => user.Caster.IsMe && user.Name.Contains("kalistacoopstrikeally")));
                return;
            }

            foreach (var entry in _incomingDamage.Where(entry => entry.Key < Game.Time))
            {
                _incomingDamage.Remove(entry.Key);
            }

            foreach (var entry in _instantDamage.Where(entry => entry.Key < Game.Time))
            {
                _instantDamage.Remove(entry.Key);
            }


            //if (SoulBoundHero.HealthPercent <
            //    SMenu.Item(_MenuItemBase + "Boolean.AutoSave.Slider.PercentHp").GetValue<Slider>().Value
            //     && SoulBoundHero.CountEnemiesInRange(500) > 0 &&
            //     SMenu.Item(_MenuItemBase + "Boolean.AutoSave.Boolean.AutoSavePercent").GetValue<bool>())
            //{
            //    if (Champion.R.Range > SoulBoundHero.Distance(Player) && Champion.R.IsReady())
            //        Champion.R.Cast();
            //}
            var soulHealth = SoulBoundHero.Health;
            if (SoulBoundHero.ChampionName == "Blitzcrank" && !SoulBoundHero.HasBuff("BlitzcrankManaBarrierCD") && !SoulBoundHero.HasBuff("ManaBarrier"))
                soulHealth += SoulBoundHero.Mana / 2;

            if (!SMenu.Item(_MenuItemBase + "Boolean.AutoSave.IncomingDamage").GetValue<bool>()) return;
            if (SoulBoundHero.Distance(Player) > Champion.R.Range) return;
            if (IncomingDamage > soulHealth || IncomingDamage > soulHealth * (100 - SMenu.Item(_MenuItemBase + "Boolean.AutoSave.Boolean.AutoSavePercent").GetValue<Slider>().Value) / 100)
                Champion.R.Cast();
        }
    }
}
