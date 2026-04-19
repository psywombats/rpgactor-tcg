using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Party Party { get; private set; }
        
        public IEnumerable<UnitView> AllUnitViews => new[] {backUnit, leftUnit, centerUnit, rightUnit};
        
        public void Populate(Party party)
        {
            Party = party;
            
            backUnit.Populate(party[LaneType.Back]);
            leftUnit.Populate(party[LaneType.Left]);
            centerUnit.Populate(party[LaneType.Center]);
            rightUnit.Populate(party[LaneType.Right]);

            deckHeader.Populate(party);
        }
        
        public void Repopulate() => Populate(Party);

        public Task GenerateMPAsync(int mp) => deckHeader.GenerateMPAsync(mp);
    }
}