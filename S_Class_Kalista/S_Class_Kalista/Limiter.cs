using System;
using System.Collections.Generic;
using LeagueSharp.Common;
using SharpDX;

namespace S_Class_Kalista
{
    public class Limiter
    {

        private static readonly Random Rand = new Random();
        private static readonly Dictionary<string, NewLevelShit> Delays = new Dictionary<string, NewLevelShit>();
        private static float _fMin = 0f, _fMax = 250f;
        struct NewLevelShit
        {
            public readonly float Delay;
            public readonly float LastTick;

            public NewLevelShit(float delay, float lastTick)
            {
                Delay = delay;
                LastTick = lastTick;
            }
        }

        private static string[] sDelays = new String[] {"RendDelay", "NonKillableDelay", "LevelDelay", "SoulBoundDelay", "TrinketDelay", "ItemDelay", "AutoDelay" };
        public static void LoadDelays()
        {
            foreach (var sDelay in sDelays)
            {
                if(Delays.ContainsKey(sDelay))
                    continue;

                Delays.Add(sDelay, new NewLevelShit(Properties.MainMenu.Item(String.Format("s{0}",sDelay)).GetValue<Slider>().Value,0f));
            }

            _fMin = Properties.MainMenu.Item("sMinRandom").GetValue<Slider>().Value;
            _fMax = Properties.MainMenu.Item("sMaxRandom").GetValue<Slider>().Value;
        }

        public static bool CheckDelay(String key)
        {
            if(Delays.ContainsKey(key))
                return Delays[key].LastTick - Properties.Time.TickCount < Delays[key].Delay;

            LoadDelays();
            return false;
        }

        public static void UseTick(String key)
        {
                Delays[key] = new NewLevelShit(Delays[key].Delay,Properties.Time.TickCount + Rand.NextFloat(_fMin, _fMax));//Randomize delay
        }
    }

}
