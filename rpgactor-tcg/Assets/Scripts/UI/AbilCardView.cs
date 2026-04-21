using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class AbilCardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text desc;
        [SerializeField] private TooltipSpawnComponent tooltip;

        public void Populate(AbilityCard newCard, CharacterCard owner = null)
        {
            desc.text = newCard.GetShortDescription(owner, pretty: true);
            tooltip.Message = newCard.GetLongDescription(owner, pretty: true);
        }
    }
}