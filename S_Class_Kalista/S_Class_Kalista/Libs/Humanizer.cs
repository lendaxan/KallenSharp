// <copyright file="Humanizer.cs" company="Kallen">
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

using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    public class Humanizer
    {
        public class Blocker
        {
            // ReSharper disable once InconsistentNaming
            private static readonly Random _random = new Random();
            private static float _lastAttackTick,_lastMoveTick;
            private static float _currentAttackDelay = 500f;
            private static float _currentMoveDelay=500f;
            public static long BlockedCommands;

            public static void Initilize()
            {
               // Obj_AI_Base.OnIssueOrder += OnIssueOrder;
            }

            private static void OnIssueOrder(Obj_AI_Base _base, GameObjectIssueOrderEventArgs args)
            {
                if (!_base.IsMe) return; //Who cares about anyone else
                if (args.IsAttackMove) return; // Riot deals with this

                if (args.Order == GameObjectOrder.AttackUnit)
                {
                    if (_lastAttackTick - Properties.Time.TickCount < _currentAttackDelay)//Can attack
                    {
                        _lastAttackTick = Properties.Time.TickCount;
                        _currentAttackDelay = Properties.Time.TickCount +
                                            1000 / _random.Next(
                                                Properties.MainMenu.Item("sMinAttacks").GetValue<Slider>().Value,
                                                Properties.MainMenu.Item("sMaxAttacks").GetValue<Slider>().Value);
                    }
                    else
                    {
                        args.Process = false;
                        BlockedCommands++;
                    }
                }
                else if (args.Order == GameObjectOrder.MoveTo)
                {
                    if (!args.TargetPosition.IsValid()) return;
                    if (_lastMoveTick - Properties.Time.TickCount < _currentMoveDelay)//Can move
                    {
                        _lastMoveTick = Properties.Time.TickCount;
                        _currentMoveDelay = Properties.Time.TickCount +
                                            1000 / _random.Next(
                                                Properties.MainMenu.Item("sMinMoves").GetValue<Slider>().Value,
                                                Properties.MainMenu.Item("sMaxMoves").GetValue<Slider>().Value);
                    }
                    else
                    {
                        args.Process = false;
                        BlockedCommands++;
                    }
                }
                Game.PrintChat(BlockedCommands.ToString());
            }
           
        }
        public class Limiter
        {
            private static readonly Random Rand = new Random();
            private static readonly Dictionary<string, NewLevelShit> Delays = new Dictionary<string, NewLevelShit>();
            // ReSharper disable once RedundantDefaultMemberInitializer
            private static float _fMin = 0f, _fMax = 250f;

            private struct NewLevelShit
            {
                public readonly float Delay;
                public readonly float LastTick;

                public NewLevelShit(float delay, float lastTick)
                {
                    Delay = delay;
                    LastTick = lastTick;
                }
            }

            private static string[] sDelays = new String[]
            {"RendDelay", "NonKillableDelay", "LevelDelay", "SoulBoundDelay", "TrinketDelay", "ItemDelay", "AutoDelay"};

            public static void LoadDelays()
            {
                try
                {


                    foreach (var sDelay in sDelays)
                    {
                        if (Delays.ContainsKey(sDelay))
                            continue;

                        Delays.Add(sDelay,
                            new NewLevelShit(
                                Properties.MainMenu.Item(String.Format("s{0}", sDelay)).GetValue<Slider>().Value, 0f));
                    }

                    _fMin = Properties.MainMenu.Item("sMinRandom").GetValue<Slider>().Value;
                    _fMax = Properties.MainMenu.Item("sMaxRandom").GetValue<Slider>().Value;
                }
                catch 
                {
                    //Fuck her right in the pussy
                }
            }

            public static bool CheckDelay(String key)
            {
                if (Delays.ContainsKey(key))
                    return Delays[key].LastTick - Properties.Time.TickCount < Delays[key].Delay;

                LoadDelays();
                return false;
            }

            public static void UseTick(String key)
            {
                Delays[key] = new NewLevelShit(Delays[key].Delay,
                    Properties.Time.TickCount + Rand.NextFloat(_fMin, _fMax)); //Randomize delay
            }          
        }
    }
}

