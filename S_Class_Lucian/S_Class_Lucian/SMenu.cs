// <copyright file="SMenu.cs" company="Kallen">
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
using LeagueSharp.Common;
using Color = System.Drawing.Color;
namespace S_Class_Lucian
{
    class SMenu
    {
        private const string MenuName = "S Class Lucian";

        public static void GenerateMenu()
        {
            Properties.MainMenu = new Menu(MenuName, MenuName, true);
            Properties.MainMenu.AddSubMenu(new Menu("Created by Kallen", "ddsfhjsjhdfjhdsfjhdfsjhdf"));
            //Properties.MainMenu.AddSubMenu(HumanizerMenu());
            Properties.MainMenu.AddSubMenu(DrawingMenu());
            Properties.MainMenu.AddSubMenu(AutoEvents());
            Properties.MainMenu.AddSubMenu(OrbWalkingMenu());
            Properties.MainMenu.AddSubMenu(MixedMenu());
            Properties.MainMenu.AddSubMenu(ComboMenu());
            Properties.MainMenu.AddSubMenu(LaneClearMenu());
            Properties.MainMenu.AddSubMenu(MiscMenu());
            Properties.LukeOrbWalker = new Orbwalking.Orbwalker(Properties.MainMenu.SubMenu("Orbwalking"));
        }

        private static Menu OrbWalkingMenu()
        {
            var orbWalkingMenu = new Menu("Orbwalking", "lukeWalker");
            var targetSelectorMenu = new Menu("Target Selector", "tSelect");
            TargetSelector.AddToMenu(targetSelectorMenu);
            orbWalkingMenu.AddSubMenu(targetSelectorMenu);
            return orbWalkingMenu;
        }

        private static Menu MixedMenu()
        {
            var mixedMenu = new Menu("Mixed Options", "mixedOptions");


            return mixedMenu;
        }

        private static Menu ComboMenu()
        {
            var mixedMenu = new Menu("Combo Options", "comboMenu");
            return mixedMenu;
        }

        private static Menu LaneClearMenu()
        {
            var laneClearMenu = new Menu("Lane Clear Options", "laneClearOptions");
          
            return laneClearMenu;
        }

     

        private static Menu DrawingMenu()
        {
            var drawMenu = new Menu("Drawing Settings", "Drawings");

            drawMenu.AddItem(new MenuItem("bDraw", "Display Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("cDrawQRange", "Draw Q Range").SetValue(new Circle(true, Color.LightSkyBlue)));

            drawMenu.AddItem(new MenuItem("cDrawWRange", "Draw W Range").SetValue(new Circle(true, Color.BlueViolet)));

            drawMenu.AddItem(new MenuItem("cDrawERange", "Draw E Range").SetValue(new Circle(true, Color.Brown)));

            drawMenu.AddItem(new MenuItem("cDrawRRange", "Draw R ange").SetValue(new Circle(true, Color.Chocolate)));


            return drawMenu;
        }

        private static Menu AutoEvents()
        {
            var autoEventsMenu = new Menu("Auto Events", "autoEvents");
            autoEventsMenu.AddItem(new MenuItem("bAutoLevel", "Auto Level Skills").SetValue(true));
            autoEventsMenu.AddItem(new MenuItem("bAutoBuyOrb", "Auto Buy Orb at 6").SetValue(true));
            return autoEventsMenu;
        }

        private static Menu MiscMenu()
        {
            var autoEventsMenu = new Menu("Misc Crap", "miscMenu");
            return autoEventsMenu;
        }

    }
}
