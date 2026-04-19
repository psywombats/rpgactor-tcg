using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class WarheadSharedTargeting : WarheadData
    {
        [SerializeField] private bool targetsLeader;

        public override void Activate(BattleModel battle, Unit caster, AbilityInstance instance, int power)
        {
            foreach (var victim in GetVictims(caster))
            {
                ApplyToVictim(battle, caster, instance, power, victim);
            }
        }

        public override string GetUseMessage(BattleModel battle, Unit caster, AbilityInstance instance, int power)
            => GetUseMessage(battle, caster, GetVictims(caster), instance, power);

        protected abstract string GetUseMessage(BattleModel battle, Unit caster, List<Unit> victims,
            AbilityInstance instance, int power);

        protected abstract void ApplyToVictim(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim);

        protected List<Unit> GetVictims(Unit caster)
        {
            var victims = new List<Unit>();
            if (targetsLeader)
            {
                victims.Add(caster.Party.Leader);
            }
            else if (caster.IsLeader)
            {
                victims.AddRange(caster.Party.Heroes);
            }
            else
            {
                victims.Add(caster);
            }
            return victims;
        }
        
        public override string GetAbilityName(CharacterData owner)
        {
            var baseName = base.GetAbilityName(owner);
            if (targetsLeader && (owner == null || !owner.isLeader))
            {
                return "Leader " + baseName;
            }
            if (!targetsLeader && owner != null && owner.isLeader)
            {
                return "Follower " + baseName;
            }
            return baseName;
        }
    }
}