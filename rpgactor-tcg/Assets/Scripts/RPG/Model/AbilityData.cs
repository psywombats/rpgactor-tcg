using System;
using UnityEngine;

namespace RpgActorTGC
{
    [Serializable]
    public class AbilityData
    {
        [SerializeField] public int mpCost;
        [SerializeField] public AbilityType type;
        [SerializeField] public int power;
    }
}