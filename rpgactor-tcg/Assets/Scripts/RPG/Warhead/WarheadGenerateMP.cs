using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadGenerateMP", menuName = "Warhead/WarheadGenerateMP")]
    public class WarheadGenerateMP : WarheadData
    {
        public override async Task ActivateAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power)
        {
            caster.Party.GenerateMp(power);
            if (battle.UseVerboseLogging) battle.SimLog($"Generated {power} MP to {caster.Party.MP}");
            if (!battle.IsSim) await battle.View.GenerateMPAsync(caster.Party, caster, power, true);
        }

        public override string GetUseMessage(BattleModel battle, Unit caster, AbilityInstance instance, int power)
        {
            return $"{caster.PrettyName} generates {power} MP.";
        }
    }
}