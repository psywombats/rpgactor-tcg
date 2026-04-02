using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Data/StatInfo")]
    [DatabaseIndexed]
    public class StatInfo : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] private Stat associatedStat;
        [SerializeField] private string abbreviation;
        [SerializeField] private string statName;
        [SerializeField] public bool useBinaryEditor;

        public string Key => associatedStat.ToString();
    }
}