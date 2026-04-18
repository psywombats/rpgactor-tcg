using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class Party : IEnumerable<Unit>
    {
        public Deck Deck { get; }
        
        public Unit Leader { get; }
        public Dictionary<LaneType, Unit> UnitsByLane { get; } = new();
        
        public List<Unit> AllUnits { get; } = new();
        public List<Unit> Heroes { get; } = new();
        
        public int Mp { get; private set; }

        public string PartyName => Deck.DeckName;
        
        public Party(Deck deck)
        {
            Deck = deck;
            foreach (var kvp in deck.CardsByLane)
            {
                var unit = new Unit(this, kvp.Value, kvp.Key);
                UnitsByLane[kvp.Key] = unit;
                AllUnits.Add(unit);
                if (kvp.Value.IsLeader)
                {
                    Leader = unit;
                }
                else
                {
                    Heroes.Add(unit);
                }
            }
        }

        public Party(DeckData data) : this(new Deck(data)) {}

        public Party(string name, CharacterData back, CharacterData left, CharacterData center, CharacterData right)
            : this(new Deck(name, back, left, center, right)) {}

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
        
        #region ToString

        public string Color { get; set; } = "grey";

        public string ShortName => Deck.DeckName;
        public string CompositionString => Deck.CompositionString;
        public string StateString => $"(\"{ShortName}\" {this[LaneType.Back].StateString} / " +
            $"{this[LaneType.Left].StateString} / " +
            $"{this[LaneType.Center].StateString} / " +
            $"{this[LaneType.Right].StateString})";

        public override string ToString() => StateString;

        #endregion
        
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
                else if (!left.IsDead)
                {
                    SwapSpaces(center, left);
                }
                else if (!right.IsDead)
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