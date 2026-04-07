using System;
using System.Linq;
using Random = UnityEngine.Random;

namespace RpgActorTGC
{
    public class DeckSolution : EvolutionSolution<DeckSolution>
    {
        public Deck Deck { get; private set; }
        private Party party;

        private DeckEvolutionRunner runner;
        
        public DeckSolution(DeckEvolutionRunner runner, Deck deck) : base(runner)
        {
            Deck = deck;
            this.runner = runner;
        }

        public Party GetFreshParty()
        {
            if (party == null)
            {
                party = new Party(Deck);
            }
            else
            {
                party.Reset();
            }

            return party;
        }

        public override bool IsEquivalentTo(DeckSolution other) => other.Deck.IsEquivalentTo(Deck);

        public override DeckSolution CrossWith(DeckSolution other)
        {
            var newCards = new CharacterCard[4];
            var leaderDeck = RandomUtils.Flip() ? Deck : other.Deck;
            var leader = leaderDeck.Leader;
            newCards[(int)leaderDeck.GetLaneForCard(leader)] = leader;
            foreach (LaneType lane in Enum.GetValues(typeof(LaneType)))
            {
                if (newCards[(int)lane] == null)
                {
                    if (Deck.CardsByLane[lane].IsLeader)
                    {
                        newCards[(int)lane] = other.Deck.CardsByLane[lane];
                    }
                    else if (other.Deck.CardsByLane[lane].IsLeader)
                    {
                        newCards[(int)lane] = Deck.CardsByLane[lane];
                    }
                    else
                    {
                        newCards[(int)lane] = RandomUtils.Flip()
                            ? Deck.CardsByLane[lane]
                            : other.Deck.CardsByLane[lane];
                    }
                }

                newCards[(int)lane] ??= (other.Deck.CardsByLane[lane].IsLeader && !Deck.CardsByLane[lane].IsLeader)
                    ? Deck.CardsByLane[lane] 
                    : other.Deck.CardsByLane[lane];
            }
            
            return new DeckSolution(runner, new Deck("Evolved Deck",
                newCards[0], newCards[1], newCards[2], newCards[3]));
        }

        public override void Mutate()
        {
            if (Random.Range(0f, 1f) < .1f)
            {
                var shuffled = Deck.CardsByLane.Values.ToList();
                RandomUtils.Shuffle(shuffled);
                Deck = new Deck("Shuffled Deck", shuffled[0],  shuffled[1], shuffled[2], shuffled[3]);
            }
            else
            {
                var newCards = new CharacterCard[4];
                var index = Random.Range(0, 4);
                var replacee = Deck.CardsByLane[(LaneType)index];
                newCards[index] = CardCache.Instance.GetRandomCharacter(replacee.IsLeader);
                foreach (LaneType lane in Enum.GetValues(typeof(LaneType)))
                {
                    newCards[(int)lane] ??= Deck.CardsByLane[lane];
                }

                Deck = new Deck("Mutated Deck",
                    newCards[0], newCards[1], newCards[2], newCards[3]);
            }
        }
    }
}