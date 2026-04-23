using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadStatSwap", menuName = "Warhead/WarheadStatSwap")]
    public class WarheadStatSwap : WarheadSharedTargeting
    {
        [SerializeField] private Stat stat1;
        [SerializeField] private Stat stat2;

        public override bool HasPower => false;

        protected override string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims, AbilityInstance instance, int power)
        {
            return $"{GetVictimsString(victims)}'s {stat1.Info().StatName} swapped with {stat2.Info().StatName}";
        }

        protected override Task ApplyToVictimAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim)
        {
            if (!battle.IsSim) battle.View.ViewForUnit[victim].FlashAsync(instance.Element.Info().PrimaryColor).Forget();
            if (!battle.IsSim)
            {
                battle.View.ViewForUnit[victim].AnimateCastAsync(instance);
            }
            (victim[stat1], victim[stat2]) = (victim[stat2], victim[stat1]);
            return Task.CompletedTask;
        }
    }
}