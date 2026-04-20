using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "OpponentNameData", menuName = "Data/Opponent Names")]
    public class OpponentNameData : ScriptableObject
    {
        [SerializeField] private List<string> prefixes;
        [SerializeField] private List<string> suffixes;

        public string GeneratePlayerName()
        {
            return $"{prefixes.Choose()} {suffixes.Choose()}";
        }
    }
}