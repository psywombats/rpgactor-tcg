using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "StatusData", menuName = "Data/StatusData")]
    public class StatusData : ScriptableObject
    {
        [SerializeField] public string statusName;
        [SerializeField] public string statusTooltip;
        [SerializeField] public Sprite icon;
        [SerializeField, Tooltip("Use {0} for victim name")] public string inflictMessage;
    }
}