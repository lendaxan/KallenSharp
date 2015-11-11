using System;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace S_Utility
{
    internal class MinionMarker : Core
    {
        private static bool _initilized;

        public static void Initilize()
        {
            if (_initilized) return; // already initilized

            LoadMenu();
            LeagueSharp.Drawing.OnDraw += OnMinionMarkerDraw;
            _initilized = !_initilized;
        }

        private static void LoadMenu()
        {
            var minionDamageMenu = new Menu("Minion Marker Menu", "minionMarkerMenu");
            minionDamageMenu.AddItem(new MenuItem("minionMarkerMenu.Enable", "Enable CS Marker").SetValue(true));
            minionDamageMenu.AddItem(
                new MenuItem("minionMarkerMenu.MarkerInnerColor", "Inner Color of Minion CS marker").SetValue(
                    new Circle(true, Color.DeepSkyBlue)));
            minionDamageMenu.AddItem(
                new MenuItem("minionMarkerMenu.MarkerOutterColor", "Outter Color of Minion CS marker").SetValue(
                    new Circle(true, Color.Crimson)));
            minionDamageMenu.AddItem(
                new MenuItem("minionMarkerMenu.Marker", "Color of CS marker").SetValue(new Circle(true,
                    Color.ForestGreen)));

            minionDamageMenu.AddItem(
                new MenuItem("minionMarkerMenu.Distance", "Render Distance").SetValue(new Slider(1000, 500, 2500)));


            SMenu.AddSubMenu(minionDamageMenu);
        }

        public static void OnMinionMarkerDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (!SMenu.Item("minionMarkerMenu.Enable").GetValue<bool>()) return;

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
            {

                if (minion.Distance(Player) > SMenu.Item("minionMarkerMenu.Distance").GetValue<Slider>().Value)continue; // Out of render range
                if(minion.IsAlly)continue; //This is not Dota2
                if(minion.IsDead)continue;//Dont poke the dead
                if(!minion.IsMinion)continue; //Differect Function
                
                if (Player.GetAutoAttackDamage(minion) > minion.Health) // Is killable
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + 50, SMenu.Item("minionMarkerMenu.Marker").GetValue<Circle>().Color, 2);
                }


                else // Not killable
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + 50,SMenu.Item("minionMarkerMenu.MarkerInnerColor").GetValue<Circle>().Color, 2);

                    var remainingHp = (int) 100 * (minion.Health - Player.GetAutoAttackDamage(minion))/minion.Health;

                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius + (float) remainingHp + 50, SMenu.Item("minionMarkerMenu.MarkerOutterColor").GetValue<Circle>().Color, 2);
                }
            }

        }
    }
}
