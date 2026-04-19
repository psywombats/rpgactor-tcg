using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class DeckHeaderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text partyNameText;
        [SerializeField] private StatView mpView;

        public void Populate(Party party)
        {
            mpView.Populate(Stat.MP, party.MP);
            partyNameText.text = party.PartyName;
        }

        public Task GenerateMPAsync(int finalMP) => mpView.TweenToAsync(finalMP);
    }
}