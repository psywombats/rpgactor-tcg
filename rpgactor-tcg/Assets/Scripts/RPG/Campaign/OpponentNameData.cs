using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "OpponentNameData", menuName = "Data/Opponent Names")]
    public class OpponentNameData : ScriptableObject
    {
        [SerializeField] public List<string> prefixes;
        [SerializeField] public List<string> suffixes;
    }
}