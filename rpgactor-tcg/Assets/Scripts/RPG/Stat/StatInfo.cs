using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Data/StatInfo")]
    public class StatInfo : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] private Stat associatedStat;
        [SerializeField] private string abbreviation;
        [SerializeField] private string statName;

        public string Key => associatedStat.ToString();
    }
}