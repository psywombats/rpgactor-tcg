using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class WarheadSharedTargeting : WarheadData
    {
        [SerializeField] private bool targetsLeader;

        public override void Activate(BattleModel battle, Unit caster, AbilityInstance instance, int power)
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

            foreach (var victim in victims)
            {
                ApplyToVictim(battle, caster, instance, power, victim);
            }
        }
        
        protected abstract void ApplyToVictim(BattleModel battle, Unit caster, AbilityInstance instance, int power, Unit victim);
        
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