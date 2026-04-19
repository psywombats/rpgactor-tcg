using System;
using System.Collections;
using System.Collections.Generic;

namespace RpgActorTGC
{
    public class Party : IEnumerable<Unit>
    {
        public Deck Deck { get; }
        
        public Unit Leader { get; }
        
        public List<Unit> AllUnits { get; } = new();
        public List<Unit> Heroes { get; } = new();
        
        public int MP { get; private set; }
        
        public Party(Deck deck)
        {
            Deck = deck;
            foreach (var kvp in deck.CardsByLane)
            {
                var unit = new Unit(this, kvp.Value, kvp.Key);
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

        public string PartyName => Deck.DeckName;
        public string Color { get; set; } = "grey";
        public string PrettyName => PartyName; // TODO: pretty names: $"<color=\"{Color}\">{PartyName}</color>";
        public string ShortName => Deck.DeckName;
        public string StateString => $"(\"{ShortName}\" {this[LaneType.Back].StateString} / " +
            $"{this[LaneType.Left].StateString} / " +
            $"{this[LaneType.Center].StateString} / " +
            $"{this[LaneType.Right].StateString})";

        public override string ToString() => StateString;

        #endregion
        
        #region Battle sim

        public void Reset()
        {
            MP = 0;
            
            foreach (var unit in AllUnits)
            {
                unit.Reset();
            }
        }

        public void GenerateMp(int mp)
        {
            MP += mp;
        }

        public Tuple<Unit, Unit> CheckPromotion()
        {
            var center = this[LaneType.Center];
            var back = this[LaneType.Back];
            var left = this[LaneType.Left];
            var right = this[LaneType.Right];
            Unit swapper = null, swappee = null;
            if (center.IsDead && !center.IsLeader)
            {
                swapper = center;
                if (!back.IsDead)
                {
                    swappee = back;
                }
                else if (!left.IsDead)
                {
                    swappee = left;
                }
                else if (!right.IsDead)
                {
                    swappee = right;
                }
            }

            if (left.IsDead && !left.IsLeader && !back.IsDead)
            {
                swapper = left;
                swappee = back;
            }
            if (right.IsDead && !right.IsDead && !back.IsDead)
            {
                swapper = right;
                swappee = back;
            }

            return swappee != null ? new Tuple<Unit, Unit>(swapper, swappee) : null;
        }

        public void SwapSpaces(Unit unit1, Unit unit2)
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