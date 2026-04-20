using System;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class Deck
    {
        public CharacterCard Leader => CardsByLane.Values.FirstOrDefault(c => c.IsLeader);
        public IEnumerable<CharacterCard> Followers => CardsByLane.Values.Where(c => !c.IsLeader);
        public CharacterCard this[LaneType lane] => CardsByLane.ContainsKey(lane) ? CardsByLane[lane] : null;
        
        public string DeckName { get; }
        
        public Dictionary<LaneType, CharacterCard> CardsByLane { get; } = new();

        private string compositionString;
        public string CompositionString
        {
            get
            {
                compositionString ??= $"(\"{DeckName}\" {this[LaneType.Back].CompositionString} / " +
                                      $"{this[LaneType.Left].CompositionString} / " +
                                      $"{this[LaneType.Center].CompositionString} / " +
                                      $"{this[LaneType.Right].CompositionString})";
                return compositionString;
            }
        }

        private int OrderFreeHash => this[LaneType.Back].GetHashCode()
                                    ^ this[LaneType.Left].GetHashCode()
                                    ^ this[LaneType.Center].GetHashCode()
                                    ^ this[LaneType.Right].GetHashCode();

        public Deck(string deckName)
        {
            DeckName = deckName;
        }

        public Deck(DeckData data) : this(data.name, data.backChara, data.leftChara, data.centerChara, data.rightChara) {}
        
        public Deck(string name, CharacterData back, CharacterData left,  CharacterData center, CharacterData right )
            : this(name,
                CardCache.Instance.GetOrCreateCard(back),
                CardCache.Instance.GetOrCreateCard(left),
                CardCache.Instance.GetOrCreateCard(center),
                CardCache.Instance.GetOrCreateCard(right)) { }

        public Deck(string name, CharacterCard back, CharacterCard left, CharacterCard center, CharacterCard right)
        {
            DeckName = name;
            CardsByLane[LaneType.Left] = left;
            CardsByLane[LaneType.Right] = right;
            CardsByLane[LaneType.Center] = center;
            CardsByLane[LaneType.Back] = back;
            
            if (Leader == null)
            {
                throw new ArgumentException($"No leader for deck {name}");
            }
        }

        public LaneType GetLaneForCard(CharacterCard card)
        {
            foreach (var kvp in CardsByLane)
            {
                if (kvp.Value == card)
                {
                    return kvp.Key;
                }
            }
            throw new KeyNotFoundException($"No lane for card {card}");
        }

        public bool IsEquivalentTo(Deck other)
        {
            return OrderFreeHash == other.OrderFreeHash;
        }

        public void Replace(LaneType lane, CharacterCard newCard)
        {
            CardsByLane[lane] = newCard;
        }
    }
}