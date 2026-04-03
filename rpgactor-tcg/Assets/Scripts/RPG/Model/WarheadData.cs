using UnityEngine;

namespace RpgActorTGC
{
    public abstract class WarheadData :  ScriptableObject
    {
        [SerializeField] private string abilityName;
        [SerializeField][Tooltip("Use {X} for the power variable")] private string abilityDesc;
        [SerializeField] private bool isContinuous;
        
        public bool IsContinuous => isContinuous;
        
        public abstract void Activate(BattleModel battle, Unit caster, AbilityInstance instance, int power);
        
        public virtual string GetAbilityName(CharacterData owner) => abilityName;
        public virtual string GetAbilityDesc(CharacterData owner) => abilityDesc;
    }
}