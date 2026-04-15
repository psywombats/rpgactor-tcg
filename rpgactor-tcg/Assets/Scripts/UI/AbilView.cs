using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class AbilView : MonoBehaviour
    {
        [SerializeField] private TMP_Text desc;

        public void Populate(AbilityCard newCard, CharacterCard owner = null)
        {
            desc.text = newCard.GetShortDescription(owner, pretty: true);
        }
    }
}