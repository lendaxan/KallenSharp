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
//   Assembly to be use with LeagueSharp for champion Lucian
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using LeagueSharp;
using LeagueSharp.Common;
using System;


namespace S_Class_Lucian
{
    internal class Program
    {
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
            // Copied from S CLass Kalista LOL

            Console.WriteLine(@"Generating Auto Properties...");
            Properties.GenerateProperties();

            //Close If Assembly Not Needed
            if (Properties.PlayerHero.ChampionName != "Lucian")
                return;

            //Create Menu and Initialize spells
            Console.WriteLine(@"Generating Menu");
            SMenu.GenerateMenu();
            Console.WriteLine(@"Generating Spells...");
            Properties.Champion.LoadSpells();
            Properties.MainMenu.AddToMainMenu();


            Console.WriteLine(@"Linking Game Events...");
            //Link Game evernts to methods
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += DrawingManager.Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += AutoEventManager.OnProcessSpellCast;
            Spellbook.OnCastSpell += AutoEventManager.OnCastSpell;
            AntiGapcloser.OnEnemyGapcloser += AutoEventManager.OnGapcloser;
            //Loaded yay
            Console.WriteLine(@"S Class Lucian Load Completed");
            Properties.Champion.UseTick();
            Game.PrintChat(
                "<b> <font color=\"#F88017\">S</font> Class <font color=\"#F88017\">Lucian</font></b> - <font color=\"#008080\">Loaded and ready!</font>");
            Net.CheckVersion();

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (Properties.MainMenu.Item("bAutoLevel").GetValue<bool>())
                    AutoLevel.LevelUpSpells();

                if (Properties.MainMenu.Item("bAutoBuyOrb").GetValue<bool>() && Properties.PlayerHero.Level >= 6)
                    TrinketManager.BuyOrb();

                if (Properties.PlayerHero.IsDead)
                    return;
                if (Properties.PlayerHero.IsRecalling())
                    return;

                AutoEventManager.EventCheck();
                OrbWalkerManager.DoTheWalk();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
