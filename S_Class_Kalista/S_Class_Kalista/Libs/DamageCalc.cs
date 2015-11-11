// <copyright file="DamageCalc.cs" company="Kallen">
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
using System.Linq;

namespace S_Class_Kalista
{
    internal class DamageCalc
    {
        #region Public Functions

        public static bool CheckNoDamageBuffs(Obj_AI_Hero target)// From Asuna
        {
            foreach (var b in target.Buffs.Where(b => b.IsValidBuff()))
            {
                switch (b.DisplayName)
                {
                    case "Chrono Shift":
                        return true;

                    case "JudicatorIntervention":
                        return true;

                    case "Undying Rage":
                        if (target.ChampionName == "Tryndamere")
                            return true;
                        continue;

                    //Spell Shields
                    case "bansheesveil":
                        return true;

                    case "SivirE":
                        return true;

                    case "NocturneW":
                        return true;
                        
                    case "kindredrnodeathbuff":
                            return true;
                }
            }
            if (target.ChampionName == "Poppy" && HeroManager.Allies.Any(
                o =>
                {
                    return !o.IsMe
                           && o.Buffs.Any(
                               b =>
                                   b.Caster.NetworkId == target.NetworkId && b.IsValidBuff()
                                   && b.DisplayName == "PoppyDITarget");
                }))
            {
                return true;
            }
           
            return (target.HasBuffOfType(BuffType.Invulnerability)
                    || target.HasBuffOfType(BuffType.SpellImmunity));
            // || target.HasBuffOfType(BuffType.SpellShield));
        }

        #endregion Public Functions

        #region Private Functions

        public static string[] ShieldBuffNames = new[]
        {
            ("EyeOfTheStorm"), ("KarmaSolKimShield"),
            ("blindmonkwoneshield"), ("lulufarieshield"),
            ("luxprismaticwaveshieldself"), ("orianaredactshield"),
            ("manabarrier"),("ShenStandUnited"),
            ("Shenstandunitedshield"),("RivenFeint"),
            ("udyrturtleactivation"),("malphiteshieldeffect"),
            ("JarvanIVGoeldenAegis"), ("evelynnrshield"),
            ("rumbleshieldbuff"),("sionwshieldstacks"),
            ("SkarnerExoskeleton"),("vipassivebuff"),
            ("tahmkencheshield"),("dianashield")
            , ("mordekaiserironman"), ("nautiluspiercinggazeshield"),
            ("UrgotTerrorCapacitorActive2"), ("ViktorPowerTransfer"),
            ("summonerbarrier"), ("ItemSeraphsEmbrace")
        };

        public static int GetRendCount(Obj_AI_Base target)
        {
            var count = target.GetBuffCount("kalistaexpungemarker");
            return target.GetBuffCount("kalistaexpungemarker") < 2 ? count + 1 : count;
        }

        private static readonly float[] RendBase = new float[]{ 20, 30, 40, 50, 60 };
        private const float RendBaseAdRate = .6f;
        private static readonly float[] RendStackBase = { 10, 14, 19, 25, 32 };
        private static readonly float[] RendStackAdRate = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        private static float GetRendDamage(Obj_AI_Base target)
        {
            if (target.IsMinion) return Properties.Champion.E.GetDamage(target) - 10;

            var rendCount = GetRendCount(target);
            var eLevel = Properties.Champion.E.Level - 1;
            var baseAd = Properties.PlayerHero.BaseAttackDamage + Properties.PlayerHero.FlatPhysicalDamageMod;
            if (rendCount <= 0) return 0f;

            var damage=(RendBase[eLevel] + RendBaseAdRate*baseAd) + ((GetRendCount(target) - 1)* (RendStackBase[eLevel] + RendStackAdRate[eLevel]*baseAd));

            damage += target.FlatHPRegenMod;
            return (float)Properties.PlayerHero.CalcDamage(target, Damage.DamageType.Physical, damage-10);

        }


        public static float GetShield(Obj_AI_Base target)
        {
            return ShieldBuffNames.Any(target.HasBuff) ? target.AllShield : 0;
        }

        public static float CalculateRendDamage(Obj_AI_Base target)
        {
            var defuffer = 1f;

            if (target.HasBuff("FerociousHowl") || target.HasBuff("GarenW"))
                defuffer *= .7f;

            if (target.HasBuff("Medidate"))
                defuffer *= .5f - target.Spellbook.GetSpell(SpellSlot.E).Level*.05f;
            
            if(target.HasBuff("gragaswself"))
                defuffer *= .9f - target.Spellbook.GetSpell(SpellSlot.W).Level * .02f;

            if (target.Name.Contains("Baron") && Properties.PlayerHero.HasBuff("barontarget"))
                defuffer *= 0.5f;

            if (target.Name.Contains("Dragon") && Properties.PlayerHero.HasBuff("s5test_dragonslayerbuff"))
                defuffer *= (1 - (.07f * Properties.PlayerHero.GetBuffCount("s5test_dragonslayerbuff")));

            if (Properties.PlayerHero.HasBuff("summonerexhaust"))
                defuffer *= .4f;


            if (!target.IsChampion()) return (GetRendDamage(target)*defuffer);

            var healthDebuffer = 0f;
            var hero = (Obj_AI_Hero) target;

            if (hero.ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") &&!target.HasBuff("ManaBarrier"))
                healthDebuffer += target.Mana/2;

            return (GetRendDamage(target) * defuffer) - (healthDebuffer + GetShield(target));
        }

        #endregion Private Functions
    }
}