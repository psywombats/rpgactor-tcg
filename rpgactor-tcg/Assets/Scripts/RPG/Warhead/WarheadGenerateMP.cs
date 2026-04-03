using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadGenerateMP", menuName = "Warhead/WarheadGenerateMP")]
    public class WarheadGenerateMP : WarheadData
    {
        public override void Activate(BattleModel battle, Unit caster, AbilityInstance instance, int power)
        {
            caster.Party.GenerateMp(power);
            if (battle.UseVerboseLogging) battle.Log($"Generated {power} MP to {caster.Party.Mp}");
        }
    }
}