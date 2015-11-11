using LeagueSharp;

namespace SCore
{
    class Damage
    {
        //Overwriten later
        public virtual float GetDamageToMonsters(Obj_AI_Minion minion)
        {
            return 0f;
        }
    }
}
