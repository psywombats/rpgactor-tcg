using System.Collections.Generic;
using System.Threading.Tasks;
using Effekseer;
using JetBrains.Annotations;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class WarheadData : ScriptableObject
    {
        [SerializeField] private string abilityName;
        [SerializeField][Tooltip("Use {0} for the power variable")] protected string abilityDesc;
        [SerializeField] private EffekseerEffectAsset anim;
        
        public virtual bool HasPower => true;
        
        public EffekseerEffectAsset Anim => anim;
        
        public abstract Task ActivateAsync(BattleModel battle, Unit caster, AbilityInstance instance, int power);
        public abstract string GetUseMessage(BattleModel battle, Unit caster, AbilityInstance instance, int power);
        
        public virtual string GetAbilityName([CanBeNull] CharacterData owner) => abilityName;
        public virtual string GetAbilityDesc([CanBeNull] CharacterData owner) => abilityDesc;

        public override string ToString() => GetAbilityName(null);
        
        protected static string GetVictimsString(List<Unit> victims)
        {
            if (victims.Count > 0)
            {
                var result = victims[0].PrettyName;
                for (var i = 1; i < victims.Count; i++)
                {
                    result += ", " + victims[i].PrettyName;
                }

                return result;
            }

            return "nobody";
        }
    }
}