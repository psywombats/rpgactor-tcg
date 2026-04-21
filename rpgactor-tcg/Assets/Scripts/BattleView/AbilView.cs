using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class AbilView : MonoBehaviour
    {
        [SerializeField] private TMP_Text desc;
        [SerializeField] private TooltipSpawnComponent tooltip;
        [SerializeField] private float unactivatedAlpha = .66f;

        public void Populate(AbilityInstance abil)
        {
            desc.text = abil.GetShortDescription(pretty: true);
            desc.color = new Color(desc.color.r, desc.color.g, desc.color.b, abil.HasActivated ? 1f : unactivatedAlpha);
            tooltip.Message = abil.GetLongDescription(pretty: true);
        }
    }
}