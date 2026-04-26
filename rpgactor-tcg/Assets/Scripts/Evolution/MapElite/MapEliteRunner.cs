using System;
using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    /// <summary>
    /// Runs the MAP-Elites (sorta) algorithm against a particular problem space.
    /// </summary>
    /// <remarks>
    /// In practical terms, this is a manager class that runs our algorithm to find the best diverse decks that perform
    /// well against a set of other decks. Once at least one tournament generation has been run, this algorithm can be
    /// run and find decks with unique performance characteristics the effectively counter the previous tournament
    /// generation.
    /// </remarks>
    public class MapEliteRunner
    {
        private NicheMap niches = new();
        private Dictionary<Tuple<DeckNicheSolution, DeckNicheSolution>, DeckNicheSolution> battleVsCache = new();
        private BattleModel battleSim = new();
        
        /// <summary>
        /// Creates a solver for a given generation (or list of decks representing a generation)
        /// </summary>
        /// <remarks>
        /// It's important that the opposition is a list and not a set -- some decks appear more frequently than others.
        /// The set of strongest decks is probably the elites from previous generations. We'll use it to initialize the
        /// set of niches.
        /// </remarks>
        /// <param name="opposition">The set of decks we are optimizing to beat</param>
        /// <param name="knownStrongDecks">A set of decks known to perform well</param>
        public MapEliteRunner(List<Deck> opposition, HashSet<Deck> knownStrongDecks)
        {
            foreach (var deck in knownStrongDecks)
            {
                var solutions = knownStrongDecks.Select(d => new DeckNicheSolution(d)).ToHashSet();
            }
        }

        public DeckNicheSolution CalcWinner(DeckNicheSolution sol1, DeckNicheSolution sol2)
        {
            if (sol1.Deck.GetHashCode() > sol2.GetHashCode())
            {
                (sol2, sol1) = (sol1, sol2);
            }

            if (battleVsCache.TryGetValue(new Tuple<DeckNicheSolution, DeckNicheSolution>(sol1, sol2), out var result))
            {
                return result;
            }

            var p1 = sol1.LockParty();
            var p2 = sol2.LockParty();
            var winningParty = battleSim.SimulateBattleAsync(p1, p2).Result;
            var winner = winningParty == p1 ? sol1 : sol2;
            battleVsCache.Add(new Tuple<DeckNicheSolution, DeckNicheSolution>(sol1, sol2), winner);
            return winner;
        }
    }
}