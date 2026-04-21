using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class WarheadSharedTargeting : WarheadData
    {
        [SerializeField] private bool targetsLeader;
        [SerializeField] private bool targetsHeroes;
        [SerializeField] private bool targetsLocal;
        [SerializeField] private bool targetsFriendly;
        [SerializeField] private bool targetsHostile;

        public override async Task ActivateAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power)
        {
            var victims = GetVictims(battle, caster);
            foreach (var victim in victims)
            {
                await ApplyToVictimAsync(battle, caster, instance, power, victim);
                if (!battle.IsSim) battle.View.RepopulateUnit(victim);
            }
        }

        public override string GetUseMessage(BattleModel battle, Unit caster, AbilityInstance instance, int power)
            => GetUseMessage(battle, caster, GetVictims(battle, caster), instance, power);

        protected abstract string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims,
            AbilityInstance instance, int power);

        protected abstract Task ApplyToVictimAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim);

        private List<Unit> GetVictims(BattleModel battle, Unit caster)
        {
            var victims = new List<Unit>();
            if (targetsLeader)
            {
                if (targetsFriendly) victims.Add(caster.Party.Leader);
                if (targetsHostile) victims.Add(battle.GetOppositeParty(caster).Leader);
            }
            if (targetsHeroes)
            {
                if (targetsFriendly) victims.AddRange(caster.Party.Heroes);
                if (targetsHostile) victims.AddRange(battle.GetOppositeParty(caster).Heroes);
            }
            if (targetsLocal)
            {
                if (targetsFriendly) victims.Add(caster);
                if (targetsHostile) victims.Add(battle.GetOppositeUnit(caster));
            }
            return victims;
        }
    }
}