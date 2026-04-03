using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    public class WHBuffStat : WarheadData
    {
        [SerializeField] private Stat stat;
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
                victim[stat] += power;
            }
        }

        public override string GetAbilityName(CharacterData owner)
        {
            var baseName = base.GetAbilityName(owner);
            if (targetsLeader && !owner.isLeader)
            {
                return "Leader " + baseName;
            }
            if (!targetsLeader && owner.isLeader)
            {
                return "Follower " + baseName;
            }
            return baseName;
        }
    }
}