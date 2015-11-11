// <copyright file="SoulBound.cs" company="Kallen">
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
using System.Linq;

// ReSharper disable once CheckNamespace
namespace S_Class_Kalista
{
    internal class SoulBound
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<float, float> _incomingDamage = new Dictionary<float, float>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<float, float> _instantDamage = new Dictionary<float, float>();

 


        private static float IncomingDamage
        {
            get { return _incomingDamage.Sum(e => e.Value) + _instantDamage.Sum(e => e.Value); }
        }

        public static void Initialize()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (!Properties.MainMenu.Item("bAutoSaveSoul").GetValue<bool>()) return;

            if (Humanizer.Limiter.CheckDelay("SoulBoundDelay") && Properties.Champion.R.IsReady())
            {
                Humanizer.Limiter.UseTick("SoulBoundDelay");

            try
                {

                    if (Properties.SoulBoundHero == null)
                        Properties.SoulBoundHero =
                            HeroManager.Allies.Find(
                                ally =>
                                    ally.Buffs.Any(
                                        user => user.Caster.IsMe && user.Name.Contains("kalistacoopstrikeally")));

                        if (Properties.SoulBoundHero.HealthPercent <
                            Properties.MainMenu.Item("sSoulBoundPercent").GetValue<Slider>().Value
                            && Properties.SoulBoundHero.CountEnemiesInRange(500) > 0
                            || IncomingDamage > Properties.SoulBoundHero.Health && Properties.SoulBoundHero.Distance(Properties.PlayerHero) < Properties.Champion.R.Range)
                            Properties.Champion.R.Cast();
                    


                    foreach (var entry in _incomingDamage.Where(entry => entry.Key < Game.Time))
                    {
                        _incomingDamage.Remove(entry.Key);
                    }

                    foreach (var entry in _instantDamage.Where(entry => entry.Key < Game.Time))
                    {
                        _instantDamage.Remove(entry.Key);
                    }
                }
                catch
                {
                    //
                }
            }
        }

        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsEnemy) return;
            if (Properties.SoulBoundHero == null) return;
            if (!Properties.MainMenu.Item("bAutoSaveSoul").GetValue<bool>()) return;

                // Calculate Damage
                if ((!(sender is Obj_AI_Hero) || args.SData.IsAutoAttack()) && args.Target != null && args.Target.NetworkId == Properties.SoulBoundHero.NetworkId)
            {
                // Calculate arrival time and damage
                _incomingDamage.Add(Properties.SoulBoundHero.ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time, (float)sender.GetAutoAttackDamage(Properties.SoulBoundHero));
            }
            // Sender is a hero
            else if (sender is Obj_AI_Hero)
            {
                var attacker = (Obj_AI_Hero)sender;
                var slot = attacker.GetSpellSlot(args.SData.Name);

                if (slot != SpellSlot.Unknown)
                {
                    if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null && args.Target.NetworkId == Properties.SoulBoundHero.NetworkId)
                        _instantDamage.Add(Game.Time + 2, (float)attacker.GetSummonerSpellDamage(Properties.SoulBoundHero, Damage.SummonerSpell.Ignite));
                    
                    // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                    else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                        ((args.Target != null && args.Target.NetworkId == Properties.SoulBoundHero.NetworkId) ||
                        args.End.Distance(Properties.SoulBoundHero.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                        _instantDamage.Add(Game.Time + 2, (float)attacker.GetSpellDamage(Properties.SoulBoundHero, slot));
                    
                }
            }

            if (!Properties.MainMenu.Item("bBST").GetValue<bool>() || !Properties.Champion.R.IsReady()) return;

            var buffName = "";
            switch (Properties.SoulBoundHero.ChampionName)
            {
                case "Blitzcrank":
                    buffName = "rocketgrab2";
                    break;

                case "Skarner":
                    buffName = "skarnerimpale";
                    break;

                case "TahmKench":
                    buffName = "tahmkenchwdevoured";
                    break;
            }

            if (buffName.Length <= 0) return;
       
            if (Properties.SoulBoundHero.Distance(Properties.PlayerHero) > Orbwalking.GetRealAutoAttackRange(Properties.PlayerHero)) return;
            
            foreach (var target in HeroManager.Enemies.Where(t => t.IsValid && t.HasBuff(buffName)))
            {
                if (target.Distance(Properties.PlayerHero) < Properties.MainMenu.Item("sBST").GetValue<Slider>().Value) continue;
                Properties.Champion.R.Cast();
                return;
            }
        }
    }
}