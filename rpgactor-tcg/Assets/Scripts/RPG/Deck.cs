using System;
using System.Collections.Generic;

namespace RpgActorTGC
{
    public class Deck
    {
        public CharacterCard Leader { get; }
        public CharacterCard this[LaneType lane] => CardsByLane[lane];
        
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
            if (!back.IsLeader)
            {
                throw new ArgumentException($"No leader for deck {name}");
            }

            DeckName = name;
            
            CardsByLane[LaneType.Left] = left;
            CardsByLane[LaneType.Right] = right;
            CardsByLane[LaneType.Center] = center;
            CardsByLane[LaneType.Back] = back;
        }
    }
}