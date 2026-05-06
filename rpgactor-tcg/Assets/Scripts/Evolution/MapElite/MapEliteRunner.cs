using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgActorTGC;

namespace MapElitesTGC
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
        [Serializable]
        public class GenerationSettings
        {
            public int generationCount = 10;
            public int generationSize = 100;
            public float mutationRate = .2f;
            public int winnerCount = 10;
        }
        
        public List<Solution> Opposition { get; }
        public GenerationSettings Settings { get; }
        public List<CharacterCard> AvailableLeaders { get; }
        public List<CharacterCard> AvailableFollowers { get; }
        
        private readonly BattleModel battleSim = new();
        private NicheMap nicheMap;
        private Dictionary<(Deck d1, Deck d2), Deck> vsCache = new();
        
        /// <summary>
        /// Creates a solver for a given generation (or list of decks representing a generation)
        /// </summary>
        /// <remarks>
        /// It's important that the opposition is a list and not a set -- some decks appear more frequently than others.
        /// </remarks>
        public MapEliteRunner(GenerationSettings settings, List<Deck> opposingDecks, 
            List<CharacterCard> availableHeroes = null, List<CharacterCard> availableLeaders = null)
        {
            Opposition = opposingDecks.Select(d => new Solution(this, d)).ToList();
            Settings = settings;
            AvailableFollowers = availableHeroes ?? new List<CharacterCard>(CardCache.Instance.AllHeroCards);
            AvailableLeaders = availableLeaders ?? new List<CharacterCard>(CardCache.Instance.AllLeaderCards);
        }

        public async Task<IList<Solution>> RunAsync()
        {
            for (var generationIndex = 0; generationIndex < Settings.generationCount; generationIndex++)
            {
                if (nicheMap == null)
                {
                    await GenerateInitialNichesAsync();
                }
                else
                {
                    await EvaluateNextGeneration();
                }
            }

            return await nicheMap.GetOrderedElitesAsync(Settings.winnerCount);
        }

        public async Task<Solution> CalcBattleWinnerAsync(Solution sol1, Solution sol2)
        {
            if (sol1.Deck.GetHashCode() > sol2.Deck.GetHashCode())
            {
                (sol1, sol2) = (sol2, sol1);
            }
            if (vsCache.TryGetValue((sol1.Deck, sol2.Deck), out var winnerDeck))
            {
                if (Equals(winnerDeck, sol1.Deck)) return sol1;
                if (Equals(winnerDeck, sol2.Deck)) return sol2;
                return null;
            }
            
            var p1 = sol1.LockParty();
            var p2 = sol2.LockParty();
            var winningParty = await battleSim.SimulateBattleAsync(p1, p2);
            sol1.UnlockParty(p1);
            sol2.UnlockParty(p2);

            var winner = winningParty == p1 ? sol1
                : winningParty == p2 ? sol2
                : null;
            vsCache.TryAdd((sol1.Deck, sol2.Deck), winner?.Deck);
            return winner;
        }

        private async Task GenerateInitialNichesAsync()
        {
            var solutions = new List<Solution>();
            for (var i = 0; i < Opposition.Count && solutions.Count < Settings.generationSize; i++)
            {
                solutions.Add(Opposition[i]);
            }
            while (solutions.Count < Settings.generationSize)
            {
                solutions.Add(new Solution(this, Deck.CreateRandom("Seed Deck", AvailableFollowers, AvailableLeaders)));
            }
            nicheMap = new NicheMap(this, solutions);
            await nicheMap.PopulateMapAsync();
        }

        private async Task EvaluateNextGeneration()
        {
            var elites = await nicheMap.GetOrderedElitesAsync();
            for (var i = 0; i < Settings.generationSize; i++)
            {
                var parent1 = elites.WeightedChoose();
                var parent2 = elites.WeightedChoose();
                var solution = new Solution(this, parent1, parent2);
                await nicheMap.ClassifySolutionAsync(solution);
            }
        }

        public async Task<int> CalcFitnessAsync(Solution solution)
        {
            var fitness = 0;
            foreach (var oppo in Opposition)
            {
                if (await CalcBattleWinnerAsync(solution, oppo) == solution)
                {
                    fitness++;
                }
            }
            return fitness;
        }
    }
}