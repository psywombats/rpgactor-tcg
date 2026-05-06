using System;
using System.Threading.Tasks;
using RpgActorTGC;

namespace MapElitesTGC
{
    /// <summary>
    /// A wrapper around a deck solution containing additional data about its performance within this problem space
    /// </summary>
    public class Solution
    {
        public Deck Deck { get; }
        private readonly MapEliteRunner runner;
        private Solution parent1, parent2;
        
        private Party party;
        private bool isPartyInUse;
        
        public int Fitness { get; private set; } = -1;
        
        public Solution(MapEliteRunner runner, Deck deck)
        {
            this.runner = runner;
            Deck = deck;
        }

        public Solution(MapEliteRunner runner, Solution parent1, Solution parent2)
            : this(runner, parent1.CrossWith(parent2))
        {
            this.parent1 = parent1;
            this.parent2 = parent2;
        }

        private Deck CrossWith(Solution other)
        {
            var newCards = new CharacterCard[4];
            var leaderLane = Deck.GetLaneForCard(Deck.Leader);
            newCards[(int)leaderLane] = RandomUtils.NextFloat() < runner.Settings.mutationRate
                ? runner.AvailableLeaders.Choose()
                : Deck.Leader;
            foreach (LaneType lane in Enum.GetValues(typeof(LaneType)))
            {
                if (lane == leaderLane) continue;

                if (RandomUtils.NextFloat() < runner.Settings.mutationRate)
                {
                    newCards[(int)lane] = runner.AvailableFollowers.Choose();
                }
                else if (Deck[lane].IsLeader)
                {
                    newCards[(int)lane] = other.Deck[lane];
                }
                else if (other.Deck[lane].IsLeader)
                {
                    newCards[(int)lane] = Deck[lane];
                }
                else
                {
                    newCards[(int)lane] = RandomUtils.Flip() ? Deck[lane] : other.Deck[lane];
                }
            }

            if (RandomUtils.NextFloat() < runner.Settings.mutationRate)
            {
                RandomUtils.Shuffle(newCards);
            }

            return new Deck("Crossed", newCards[0], newCards[1], newCards[2], newCards[3]);
        }

        public Party LockParty()
        {
            if (isPartyInUse)
            {
                return new Party(Deck);
            }
            else
            {
                isPartyInUse = true;
                return GetFreshParty();
            }
        }

        public void UnlockParty(Party returnedParty)
        {
            if (returnedParty == party)
            {
                isPartyInUse = false;
            }
        }
        
        public async Task<int> CalcFitnessAsync()
        {
            if (Fitness == -1)
            {
                Fitness = await runner.CalcFitnessAsync(this);
            }
            return Fitness;
        }

        private Party GetFreshParty()
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
    }
}