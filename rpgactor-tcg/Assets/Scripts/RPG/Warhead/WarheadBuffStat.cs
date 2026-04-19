using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadBuffStat", menuName = "Warhead/WarheadBuffStat")]
    public class WarheadBuffStat : WarheadSharedTargeting
    {
        [SerializeField] private Stat stat;

        protected override string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims, AbilityInstance instance, int power)
        {
            return $"Raised {stat.Info().StatName} of {GetVictimsString(victims)} by {power}.";
        }

        protected override void ApplyToVictim(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            victim[stat] += power;
            if (battle.UseVerboseLogging) battle.SimLog($"Raised {victim.CompositionString} {stat.Info().StatName} by {power} to {victim[stat]}");
            if (stat == Stat.SPD)
            {
                battle.InvalidateTurnOrder();
            }
        }
    }
}