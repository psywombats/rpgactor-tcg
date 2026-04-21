using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgActorTGC
{
    // we want to find the best deck that works against a specific set of other decks, not a game theory optimal one
    public class DeckEvoVsFixedSetRunner : EvolutionRunner<DeckSolution>
    {
        private const string RandomDeckName = "Random Deck";
        
        private readonly List<DeckWorker> workers = new();

        private readonly List<DeckSolution> oppositionSolutions;
        private readonly List<CharacterCard> availableLeaders, availableHeroes;

        public DeckEvoVsFixedSetRunner(IEnumerable<Deck> opposition, List<CharacterCard> availableLeaders,
            List<CharacterCard> availableHeroes)
        {
            oppositionSolutions = opposition.Select(oppo => new DeckSolution(oppo)).ToList();
            this.availableLeaders = availableLeaders;
            this.availableHeroes = availableHeroes;
        }
        
        protected override DeckSolution CreateRandomSolution()
        {
            return new DeckSolution(Deck.CreateRandom(RandomDeckName, availableHeroes, availableLeaders));
        }

        protected override void AssignScoresToSolutions(List<DeckSolution> solutions)
        {
            while (workers.Count < Environment.ProcessorCount)
            {
                workers.Add(new DeckWorker());
            }
            
            foreach (var sol in solutions)
            {
                sol.Wins = 0;
            }

            var created = 0;
            for (var i = 0; i < solutions.Count; i++)
            {
                var sol1 =  solutions[i];
                for (var j = i + 1; j < oppositionSolutions.Count; j++)
                {
                    var sol2 = oppositionSolutions[j];
                    workers[created % workers.Count].AssignTask(new DeckWorker.DeckTask(sol1, sol2));
                    created += 1;
                }
            }
            Parallel.Invoke(workers.Select<DeckWorker, Action>(worker => worker.SimulateBattles).ToArray());
            
            foreach (var sol in solutions)
            {
                sol.Fitness = sol.Wins;
            }
        }
    }
}