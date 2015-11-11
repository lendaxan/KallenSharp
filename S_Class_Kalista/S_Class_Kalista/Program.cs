// <copyright file="Program.cs" company="Kallen">
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

//using AutoLevel = S_Class_Kalista.AutoLevel;

namespace S_Class_Kalista
{
    internal class Program
    {
        //readonly static Random Seeder = new Random();

        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            // So you can test if it in VS wihout crahses
#if !DEBUG
            CustomEvents.Game.OnGameLoad += OnLoad;
#endif
        }

        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedMember.Local
        private static void OnLoad(EventArgs args)
        {
            //They wanted comments so i added this line
            Console.WriteLine(@"Generating Auto Properties...");
            Properties.GenerateProperties();

            //Close If Assembly Not Needed
            if (Properties.PlayerHero.ChampionName != "Kalista")
                return;

            //Create Menu and Initialize spells
            Console.WriteLine(@"Generating Menu");
            SMenu.GenerateMenu();
            Console.WriteLine(@"Generating Spells...");
            Properties.Champion.LoadSpells();
            Properties.MainMenu.AddToMainMenu();
            Console.WriteLine(@"Linking Game Events...");

            //Link Game evernts to functions
            Game.OnUpdate += Game_OnUpdate;
            //Properties.AutoLevel.InitilizeAutoLevel();
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            SoulBound.Initialize();
            DrawingManager.Initilize();
            ItemManager.Initilize();
            AutoLevel.Initilize();
            TrinketManager.Initilize();
            AutoEventManager.Initilize();
            Humanizer.Blocker.Initilize();


            switch (Properties.MainMenu.Item("sOrbwalker").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Console.WriteLine(@"Activating Skywalker...");
                    SkyWalker.OnNonKillableMinion += AutoEventManager.CheckNonKillables;
                    SkyWalker.AfterAttack += OrbWalkerManager.AfterAttack;
                    break;

                case 1:
                    Console.WriteLine(@"Activating Common Walker...");
                    Orbwalking.OnNonKillableMinion += AutoEventManager.CheckNonKillables;
                    Orbwalking.AfterAttack += OrbWalkerManager.AfterAttack;
                    break;

            }
           // Limiter.LoadDelays();
            // Add Delays for later use
            //Humanizer needs to be reworked
            //Humanizer.AddAction("rendDelay",200);
            // Humanizer.AddAction("generalDelay",125);

            //Loaded yay
            Console.WriteLine(@"S Class Kalista Load Completed");

            Game.PrintChat("<b> <font color=\"#F88017\">S</font> Class <font color=\"#F88017\">Kalista</font></b> - <font color=\"#008080\">Loaded and ready!</font>");
            Net.CheckVersion();
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid) return;

            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
                Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (Properties.PlayerHero.IsDead)
                    return;
                if (Properties.PlayerHero.IsRecalling())
                    return;

                OrbWalkerManager.DoTheWalk();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}