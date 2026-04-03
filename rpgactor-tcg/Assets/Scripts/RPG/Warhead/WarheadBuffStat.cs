using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "WarheadBuffStat", menuName = "Warhead/WarheadBuffStat")]
    public class WarheadBuffStat : WarheadData
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
                if (battle.UseVerboseLogging) battle.Log($"Raised {victim.CompositionString} {stat.Info().StatName} by {power} to {victim[stat]}");
            }
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

        public override string GetAbilityDesc(CharacterData owner)
        {
            var targetString = "the user";
            if (targetsLeader && (owner == null || !owner.isLeader))
            {
                targetString = "the party leader";
            }
            if (!targetsLeader && owner != null && owner.isLeader)
            {
                targetString = "all heroes";
            }
            return abilityDesc.Replace("{1}", targetString);
        }
    }
}