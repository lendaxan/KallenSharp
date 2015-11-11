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
            SoulBoundDamage.Load();
            SMenu.AddSubMenu(_Menu());

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "souldBoundMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.IncomingDamage", "Auto-Save soulbound if incoming damage > HP").SetValue(true));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.Boolean.AutoSavePercent", "Auto-Save soulbound HP%").SetValue(true));
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.AutoSave.Slider.PercentHp", "Auto-Save soulbound when HP% less then").SetValue(new Slider(10, 1, 90)));
            return menu;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.SoulBoundDelay")) return;

            Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.SoulBoundDelay");
            if (SoulBoundHero == null) return;//SoulBoundDamage will bind when able

            if (SoulBoundHero.HealthPercent <
                SMenu.Item(_MenuItemBase + "Boolean.AutoSave.Slider.PercentHp").GetValue<Slider>().Value
                 && SoulBoundHero.CountEnemiesInRange(500) > 0 &&
                 SMenu.Item(_MenuItemBase + "Boolean.AutoSave.Boolean.AutoSavePercent").GetValue<bool>())
            {
                if (Champion.R.Range > SoulBoundHero.Distance(Player) && Champion.R.IsReady())
                    Champion.R.Cast();
            }

            if (SMenu.Item(_MenuItemBase + "Boolean.AutoSave.IncomingDamage").GetValue<bool>())
            {
                if (SoulBoundDamage.IncomingDamage > SoulBoundHero.Health)
                    if (Champion.R.Range > SoulBoundHero.Distance(Player) && Champion.R.IsReady())
                        Champion.R.Cast();
            }

        }

        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {

        }
    }
}
