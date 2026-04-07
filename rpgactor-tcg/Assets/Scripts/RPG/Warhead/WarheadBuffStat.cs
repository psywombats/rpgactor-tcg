using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadBuffStat", menuName = "Warhead/WarheadBuffStat")]
    public class WarheadBuffStat : WarheadSharedTargeting
    {
        [SerializeField] private Stat stat;

        protected override void ApplyToVictim(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            victim[stat] += power;
            if (battle.UseVerboseLogging) battle.Log($"Raised {victim.CompositionString} {stat.Info().StatName} by {power} to {victim[stat]}");
            if (stat == Stat.SPD)
            {
                battle.InvalidateTurnOrder();
            }
        }
    }
}