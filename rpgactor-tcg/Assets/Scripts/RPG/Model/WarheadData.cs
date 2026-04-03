using JetBrains.Annotations;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class WarheadData :  ScriptableObject
    {
        [SerializeField] private string abilityName;
        [SerializeField][Tooltip("Use {0} for the power variable")] protected string abilityDesc;
        [SerializeField] private bool isContinuous;
        
        public bool IsContinuous => isContinuous;
        
        public abstract void Activate(BattleModel battle, Unit caster, AbilityInstance instance, int power);
        
        public virtual string GetAbilityName([CanBeNull] CharacterData owner) => abilityName;
        public virtual string GetAbilityDesc([CanBeNull] CharacterData owner) => abilityDesc;

        public override string ToString() => GetAbilityName(null);
    }
}