using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

namespace RpgActorTGC
{
    public class DeckEvolutionRunner : EvolutionRunner<DeckSolution>
    {
        private const string RandomDeckName = "Random Deck";
        
        private readonly List<DeckWorker> workers = new List<DeckWorker>();
        
        protected override DeckSolution CreateRandomSolution()
        {
            var cards = new[]
            {
                CardCache.Instance.GetRandomCharacter(isLeader: true),
                CardCache.Instance.GetRandomCharacter(isLeader: false),
                CardCache.Instance.GetRandomCharacter(isLeader: false),
                CardCache.Instance.GetRandomCharacter(isLeader: false),
            };
            var leaderIndex = Random.Range(0, 4);
            (cards[leaderIndex], cards[0]) = (cards[0], cards[leaderIndex]);

            return new DeckSolution(this, new Deck(RandomDeckName, cards[0], cards[1], cards[2], cards[3]));
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
                for (var j = i + 1; j < solutions.Count; j++)
                {
                    var sol2 = solutions[j];
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