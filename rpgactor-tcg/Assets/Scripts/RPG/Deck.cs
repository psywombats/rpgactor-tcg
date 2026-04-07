using System;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class Deck
    {
        public CharacterCard Leader { get; }
        public CharacterCard this[LaneType lane] => CardsByLane[lane];
        
        public string DeckName { get; set; }
        
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

        public Deck(DeckData data) : this(data.name, data.backChara, data.leftChara, data.centerChara, data.rightChara) {}
        
        public Deck(string name, CharacterData back, CharacterData left,  CharacterData center, CharacterData right )
            : this(name,
                CardCache.Instance.GetOrCreateCard(back),
                CardCache.Instance.GetOrCreateCard(left),
                CardCache.Instance.GetOrCreateCard(center),
                CardCache.Instance.GetOrCreateCard(right)) { }

        public Deck(string name, CharacterCard back, CharacterCard left, CharacterCard center, CharacterCard right)
        {
            Leader = left.IsLeader ? left :
                    center.IsLeader ? center :
                    right.IsLeader ? right : back;
            if (!Leader.IsLeader)
            {
                throw new ArgumentException($"No leader for deck {name}");
            }

            DeckName = name;
            
            CardsByLane[LaneType.Left] = left;
            CardsByLane[LaneType.Right] = right;
            CardsByLane[LaneType.Center] = center;
            CardsByLane[LaneType.Back] = back;
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
            return other.CardsByLane[LaneType.Back].Equals(CardsByLane[LaneType.Back])
                && other.CardsByLane[LaneType.Left].Equals(CardsByLane[LaneType.Left])
                && other.CardsByLane[LaneType.Center].Equals(CardsByLane[LaneType.Center])
                && other.CardsByLane[LaneType.Back].Equals(CardsByLane[LaneType.Back]);
        }
    }
}