using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class AbilCardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text desc;

        public void Populate(AbilityCard newCard, CharacterCard owner = null)
        {
            desc.text = newCard.GetShortDescription(owner, pretty: true);
        }
    }
}