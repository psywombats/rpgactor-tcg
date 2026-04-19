using System;
using UnityEngine;

namespace RpgActorTGC
{
    [Serializable]
    public class AbilityData
    {
        [SerializeField] private WarheadData warhead;
        [SerializeField] public int mpCost;
        [SerializeField] public int power;
        
         public bool IsContinuous => warhead.IsContinuous;

        public void Activate(BattleModel battle, Unit caster, AbilityInstance instance) =>
            warhead.Activate(battle, caster, instance, power);
        public string GetUseMessage(BattleModel battle, Unit caster, AbilityInstance instance) =>
            warhead.GetUseMessage(battle, caster, instance, power);
        
        public virtual string GetAbilityName(CharacterData owner) => warhead.GetAbilityName(owner);
        public virtual string GetAbilityDesc(CharacterData owner) => warhead.GetAbilityDesc(owner);
    }
}