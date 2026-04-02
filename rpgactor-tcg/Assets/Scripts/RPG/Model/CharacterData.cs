using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class CharacterData : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] public string characterName;
        [Space] 
        [SerializeField] public StatSet stats;
        [SerializeField] private List<AbilityData> abilities;

        public string Key => name;
    }
}