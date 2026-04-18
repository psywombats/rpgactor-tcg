using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class PartyView : MonoBehaviour
    {
        [SerializeField] private UnitView backUnit;
        [SerializeField] private UnitView leftUnit;
        [SerializeField] private UnitView centerUnit;
        [SerializeField] private UnitView rightUnit;
        [Space]
        [SerializeField] private DeckHeaderView deckHeader;

        public void Populate(Party party)
        {
            backUnit.Populate(party[LaneType.Back]);
            leftUnit.Populate(party[LaneType.Left]);
            centerUnit.Populate(party[LaneType.Center]);
            rightUnit.Populate(party[LaneType.Right]);

            deckHeader.Populate(party);
        }
    }
}