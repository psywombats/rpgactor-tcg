using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadBuffStat", menuName = "Warhead/WarheadBuffStat")]
    public class WarheadBuffStat : WarheadSharedTargeting
    {
        [SerializeField] private Stat stat;
        [SerializeField] private bool invert;

        public override bool HasPower => !stat.Info().IsFlag;

        protected override string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims, AbilityInstance instance, int power)
        {
            var verb = !invert ? "Raised" : "Lowered";
            return stat.Info().IsFlag 
                ? $"{GetVictimsString(victims)} is now {stat.Info().StatName}." 
                : $"{verb} {stat.Info().StatName} of {GetVictimsString(victims)} by {power}.";
        }

        protected override Task ApplyToVictimAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            if (!battle.IsSim) battle.View.ViewForUnit[victim].FlashAsync(instance.Element.Info().PrimaryColor).Forget();
            victim[stat] += power * (invert ? -1 : 1);
            if (victim[stat] < 0) victim[stat] = 0;
            if (battle.UseVerboseLogging) battle.SimLog($"Raised {victim.CompositionString} {stat.Info().StatName} by {power} to {victim[stat]}");
            if (stat == Stat.SPD)
            {
                battle.InvalidateTurnOrder();
            }

            return Task.CompletedTask;
        }
    }
}