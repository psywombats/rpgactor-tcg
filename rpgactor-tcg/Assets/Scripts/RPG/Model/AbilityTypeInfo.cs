using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "AbilityType", menuName = "Data/AbilityTypeInfo")]
    [DatabaseIndexed]
    public class AbilityTypeInfo : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] private AbilityType associatedAbilityType;
        [SerializeField] private string abilityName;
        [SerializeField][Tooltip("Use {X} for the power variable")] private string abilityDesc;

        public string Key => associatedAbilityType.ToString();
    }
}