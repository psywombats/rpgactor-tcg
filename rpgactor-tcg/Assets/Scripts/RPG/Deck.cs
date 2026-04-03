using System;
using System.Collections.Generic;

namespace RpgActorTGC
{
    public class Deck
    {
        public CharacterCard Leader { get; }
        public CharacterCard this[LaneType lane] => CardsByLane[lane];
        
        public Dictionary<LaneType, CharacterCard> CardsByLane { get; } = new();

        public Deck(DeckData data) : this(CardCache.Instance.GetOrCreateCard(data.leftChara),
            CardCache.Instance.GetOrCreateCard(data.centerChara),
            CardCache.Instance.GetOrCreateCard(data.rightChara),
            CardCache.Instance.GetOrCreateCard(data.backChara)) { }

        public Deck(CharacterCard left, CharacterCard center, CharacterCard right, CharacterCard back)
        {
            Leader = left.IsLeader ? left :
                    center.IsLeader ? center :
                    right.IsLeader ? right : back;
            if (!back.IsLeader)
            {
                throw new ArgumentException("No leader for deck");
            }
            
            CardsByLane[LaneType.Left] = left;
            CardsByLane[LaneType.Right] = right;
            CardsByLane[LaneType.Center] = center;
            CardsByLane[LaneType.Back] = back;
        }
    }
}