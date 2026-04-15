using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "Character", menuName = "Data/CharacterData")]
    [DatabaseIndexed]
    public class CharacterData : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] public string characterName;
        [SerializeField] public SpritesheetData sprite;
        [Space] 
        [SerializeField] public bool isLeader;
        [SerializeField] public StatSet stats;
        [SerializeField] public List<AbilityData> abilities;

        public string Key => name;
    }
}