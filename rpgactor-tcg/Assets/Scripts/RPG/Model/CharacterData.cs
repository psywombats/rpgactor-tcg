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
        [SerializeField] public int rarity;
        [SerializeField] public StatSet stats;
        [SerializeField] public List<AbilityData> abilities;
        [Space]
        [SerializeField] public string actorPreferredName;
        [SerializeField] public List<string> actorPreferredClasses;
        [SerializeField] public string actorPreferredDID;

        public string Key => name;
    }
}