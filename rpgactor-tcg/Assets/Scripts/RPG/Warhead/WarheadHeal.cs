using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadHeal", menuName = "Warhead/WarheadHeal")]
    public class WarheadHeal : WarheadSharedTargeting
    {
        protected override string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims, AbilityInstance instance, int power)
        {
            return $"{caster.PrettyName} healed {GetVictimsString(victims)} by {power}.";
        }

        protected override Task ApplyToVictimAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            if (victim.IsDead) return Task.CompletedTask;
            victim[Stat.HP] += power;
            if (victim.HP > victim[Stat.MHP]) victim[Stat.HP] = victim[Stat.MHP];
            if (battle.UseVerboseLogging) battle.SimLog($"Healed {victim.CompositionString} by {power} to {victim.HP}/{victim[Stat.MHP]}");
            return battle.IsSim ? Task.CompletedTask : battle.View.ViewForUnit[victim].AnimateDamageAsync(power);
        }
    }
}