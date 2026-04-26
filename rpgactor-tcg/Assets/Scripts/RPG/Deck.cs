using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class Deck : IEnumerable<CharacterCard>
    {
        public CharacterCard Leader => cardsByLane.Values.FirstOrDefault(c => c != null && c.IsLeader);
        public IEnumerable<CharacterCard> Followers => cardsByLane.Values.Where(c => c != null && !c.IsLeader);
        public CharacterCard this[LaneType lane] => cardsByLane.ContainsKey(lane) ? cardsByLane[lane] : null;
        
        public string DeckName { get; set; }
        public int DeckIndex { get; set; } = -1;

        public bool IsIncomplete => Leader == null || Followers.Count() < 3;

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
        
        private readonly Dictionary<LaneType, CharacterCard> cardsByLane = new();

        public Deck(string deckName)
        {
            DeckName = deckName;
            foreach (LaneType lane in Enum.GetValues(typeof(LaneType))) cardsByLane[lane] = null;
        }

        public Deck(Deck other) : this(other.DeckName, other[LaneType.Back], other[LaneType.Left],
            other[LaneType.Center], other[LaneType.Right]) { }

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
            cardsByLane[LaneType.Left] = left;
            cardsByLane[LaneType.Right] = right;
            cardsByLane[LaneType.Center] = center;
            cardsByLane[LaneType.Back] = back;
            
            if (Leader == null)
            {
                throw new ArgumentException($"No leader for deck {name}");
            }
        }

        public bool ContainsCard(CharacterCard card) => cardsByLane.Values.Contains(card);

        public LaneType GetLaneForCard(CharacterCard card)
        {
            foreach (var kvp in cardsByLane)
            {
                if (kvp.Value == card)
                {
                    return kvp.Key;
                }
            }
            throw new KeyNotFoundException($"No lane for card {card}");
        }

        public void Replace(LaneType lane, CharacterCard newCard)
        {
            // remove other leaders if we're assigning another one
            if (newCard != null && newCard.IsLeader)
            {
                var lanesToClear = new HashSet<LaneType>();
                foreach (var asgn in cardsByLane)
                {
                    if (asgn.Value != null && asgn.Value.IsLeader && lane != asgn.Key)
                    {
                        lanesToClear.Add(asgn.Key);
                    }
                }
                foreach (var toClear in lanesToClear)
                {
                    cardsByLane.Remove(toClear);
                }
            }

            cardsByLane[lane] = newCard;
        }

        public static Deck CreateRandom(string deckName,
            List<CharacterCard> availableHeroes = null, List<CharacterCard> availableLeaders = null)
        {
            var cards = new[]
            {
                CardCache.Instance.GetRandomCharacter(isLeader: true, availableHeroes, availableLeaders),
                CardCache.Instance.GetRandomCharacter(isLeader: false, availableHeroes, availableLeaders),
                CardCache.Instance.GetRandomCharacter(isLeader: false, availableHeroes, availableLeaders),
                CardCache.Instance.GetRandomCharacter(isLeader: false, availableHeroes, availableLeaders),
            };
            var leaderIndex = RandomUtils.Range(0, 4);
            (cards[leaderIndex], cards[0]) = (cards[0], cards[leaderIndex]);
            return new Deck(deckName, cards[0], cards[1], cards[2], cards[3]);
        }
        
        #region Equals

        public bool HasSameCardsAs(Deck deck) => OrderFreeHash == deck.OrderFreeHash;
        
        public override bool Equals(object obj)
        {
            var other = obj as Deck;
            if (other == null) return false;

            return this[LaneType.Back] == other[LaneType.Back]
                && this[LaneType.Center] == other[LaneType.Center]
                && ((this[LaneType.Left] == other[LaneType.Left] && this[LaneType.Right] == other[LaneType.Right])
                    || (this[LaneType.Left] == other[LaneType.Right] && this[LaneType.Right] == other[LaneType.Left]));
        }

        public override int GetHashCode()
        {
            // we don't distinguish between left/right positions for equivalence checks
            return HashCode.Combine(
                this[LaneType.Back].GetHashCode(),
                this[LaneType.Center].GetHashCode(),
                this[LaneType.Left].GetHashCode() ^ this[LaneType.Right].GetHashCode());
        }

        #endregion

        #region IEnumerable

        public IEnumerator<CharacterCard> GetEnumerator() => cardsByLane.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}