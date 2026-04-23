using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Data/StatInfo")]
    [DatabaseIndexed]
    public class StatInfo : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] private Stat associatedStat;
        [SerializeField] private bool isPrimary;
        [SerializeField] private bool isFlag;
        [Space]
        [SerializeField] private string abbreviation;
        [SerializeField] private string statName;
        [SerializeField] private Sprite icon;
        [SerializeField, Tooltip("Use {0} for stat value")] public string description;

        public string Key => associatedStat.ToString();
        public string StatName => statName;
        public Sprite Icon => icon;
        public string Description => description;
        public bool IsFlag => isFlag;
        public bool IsPrimary => isPrimary;
    }
}