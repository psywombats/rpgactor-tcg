using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class Party : IEnumerable<Unit>
    {
        private Deck deck;
        
        public Unit Leader { get; init; }
        public List<Unit> Heroes { get; } = new();

        private IEnumerable<Unit> allUnits;
        public IEnumerable<Unit> AllUnits => allUnits ??= Heroes.Union(new List<Unit> { Leader });
        
        public int Mp { get; private set; }
        
        public Party(Deck deck)
        {
            this.deck = deck;
            foreach (var kvp in deck.CardsByLane)
            {
                var unit = new Unit(this, kvp.Value, kvp.Key);
                if (deck.Leader == kvp.Value)
                {
                    Leader = unit;
                }
                else
                {
                    Heroes.Add(unit);
                }
            }
        }

        public Unit this[LaneType lane]
        {
            get
            {
                foreach (var unit in AllUnits)
                {
                    if (unit.Lane == lane)
                    {
                        return unit;
                    }
                }
                return null;
            }
        }
        
        #region Battle sim

        public void Reset()
        {
            Mp = 0;
            
            foreach (var unit in AllUnits)
            {
                unit.Reset();
            }
        }

        public void GenerateMp(int mp)
        {
            Mp += mp;
        }

        public void CheckPromotion()
        {
            var center = this[LaneType.Center];
            var back = this[LaneType.Back];
            var left = this[LaneType.Left];
            var right = this[LaneType.Right];
            if (center.IsDead)
            {
                if (!back.IsDead)
                {
                    SwapSpaces(center, back);
                }
                else if (left.IsDead)
                {
                    SwapSpaces(center, left);
                }
                else if (right.IsDead)
                {
                    SwapSpaces(center, right);
                }
            }

            if (left.IsDead && !back.IsDead)
            {
                SwapSpaces(back, left);
            }
            if (right.IsDead && !back.IsDead)
            {
                SwapSpaces(back, right);
            }
        }

        private void SwapSpaces(Unit unit1, Unit unit2)
        {
            (unit1.Lane, unit2.Lane) = (unit2.Lane, unit1.Lane);
        }
        
        #endregion

        #region IEnumerable

        public IEnumerator<Unit> GetEnumerator() => AllUnits.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => AllUnits.GetEnumerator();

        #endregion
    }
}