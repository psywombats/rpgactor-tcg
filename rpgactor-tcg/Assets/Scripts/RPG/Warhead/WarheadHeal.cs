using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadHeal", menuName = "Warhead/WarheadHeal")]
    public class WarheadHeal : WarheadSharedTargeting
    {
        protected override void ApplyToVictim(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            if (victim.IsDead) return;
            victim[Stat.HP] += power;
            if (victim.Hp > victim[Stat.MHP]) victim[Stat.HP] =  victim[Stat.MHP];
            if (battle.UseVerboseLogging) battle.Log($"Healed {victim.CompositionString} by {power} to {victim.Hp}/{victim[Stat.MHP]}");
        }
    }
}