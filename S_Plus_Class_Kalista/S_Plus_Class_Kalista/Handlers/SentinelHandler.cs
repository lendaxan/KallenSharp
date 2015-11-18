using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using S_Plus_Class_Kalista.Libaries;

namespace S_Plus_Class_Kalista.Handlers
{
    internal class SentinelHandler : Core
    {

        private const string _MenuNameBase = ".Sentinel Menu (Not DONE)";
        private const string _MenuItemBase = ".Sentinel.";


        private void LoadLocations()
        {
            // Use Monster Structures
        }
        public static void Load()
        {
            if (Game.MapId != GameMapId.SummonersRift) return;

            SMenu.AddSubMenu(_Menu());
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
        }

        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "sentinelMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel", "Use Auto Sentinal").SetValue(true));
            var menuSub = new Menu("Sentinel Locations", "sentinelMenu");
            menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnDragon", "Use On Dragon").SetValue(true));
            menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnBaron", "Use On Baron").SetValue(true));
            menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnRed", "Use On Red's").SetValue(true));
            menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnBlue", "Use On Blue's").SetValue(true));
            menuSub.AddItem(new MenuItem(_MenuItemBase + "Boolean.UseSentinel.OnMid", "Use On Mid").SetValue(true));
            menu.AddSubMenu(menuSub);
            return menu;
        }

        private static void OnUpdate(EventArgs args)
        {
           
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
     
        }

    }
}


