using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadDamage", menuName = "Warhead/WarheadDamage")]
    public class WarheadDamage : WarheadSharedTargeting
    {
        protected override string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims, AbilityInstance instance, int power)
        {
            return $"{caster.PrettyName} struck {GetVictimsString(victims)} for {power} damage.";
        }

        protected override async Task ApplyToVictimAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            power = await victim.TakeDamageAsync(battle, power);
            if (!battle.IsSim) await battle.View.ViewForUnit[victim].AnimateDamageAsync(power);
            victim.CleanupPostAttack(battle);
            
            if (!battle.IsSim) battle.View.RepopulateUnit(victim);
            if (battle.UseVerboseLogging) battle.SimLog($"Damaged {victim.PrettyName} by {power} to {victim.HP}/{victim[Stat.MHP]}");
            
            if (victim.IsDead && !battle.IsSim) await battle.View.WriteLineAsync($"{victim.PrettyName} has fallen!", true);
        }
    }
}