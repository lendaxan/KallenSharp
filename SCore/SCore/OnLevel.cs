using System;
using LeagueSharp;


namespace SCore
{
    class OnLevel : Base
    {
        private const int AbilitysSize = 18;
        private static int[] _sequence = new int[AbilitysSize - 1];
        private static float _lastTick;
        private static bool _enabled;
        public struct Abilitys // So you can refeer to spell to level by slot rather than 1,2,3,4
        {
            public const int Q = 1;
            public const int W = 2;
            public const int E = 3;
            public const int R = 4;
        }

        // ReSharper disable once UnusedParameter.Local
        public OnLevel(int[] abilitySequence, bool b = false)
        {
            if (abilitySequence.Length != _sequence.Length)
                throw new ArgumentException("Ability Size Invalid",String.Format("Size:{0}",abilitySequence.Length));//Does not contain required elements

            _sequence = abilitySequence;
            Initilize();


        }

        public void AutoLevel(bool b)
        {
            _enabled = b;
        }

        private static void Initilize()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            // ReSharper disable once PossibleLossOfFraction
            if (!(_lastTick < Time.TickCount + 1500 - Game.Ping/2)) return;//Check Every 1.5 Seconds
            _lastTick = Time.TickCount;
            if (!_enabled) return;
            Level();
        }

        private static void Level()
        {
            var qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            var wL = Player.Spellbook.GetSpell(SpellSlot.W).Level;
            var eL = Player.Spellbook.GetSpell(SpellSlot.E).Level;
            var rL = Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qL + wL + eL + rL >= Player.Level) return;


            int[] level = { 0, 0, 0, 0 };

            for (var i = 0; i < Player.Level; i++)
            {
                level[_sequence[i] - 1] = level[_sequence[i] - 1] + 1;
            }

            if (qL < level[0]) Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Player.Spellbook.LevelSpell(SpellSlot.R);
        }
    }
}
