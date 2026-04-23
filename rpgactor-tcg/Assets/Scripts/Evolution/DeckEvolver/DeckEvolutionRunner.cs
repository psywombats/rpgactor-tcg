using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Action = System.Action;

namespace RpgActorTGC
{
    public class DeckEvolutionRunner : EvolutionRunner<DeckSolution>
    {
        private const string RandomDeckName = "Random Deck";
        
        private readonly List<DeckWorker> workers = new();
        
        protected override DeckSolution CreateRandomSolution()
        {
            return new DeckSolution(Deck.CreateRandom(RandomDeckName));
        }

        protected override void AssignScoresToSolutions(List<DeckSolution> solutions)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
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
            }
            else
            {
                var battle = new BattleModel();
                for (var i = 0; i < solutions.Count; i++)
                {
                    var sol1 =  solutions[i];
                    for (var j = i + 1; j < solutions.Count; j++)
                    {
                        var sol2 = solutions[j];
                        var winner = battle.SimulateBattleAsync(sol1.GetFreshParty(), sol2.GetFreshParty()).Result;
                        DeckWorker.ScoreResult(sol1, sol2, winner);
                    }
                }
            }
            foreach (var sol in solutions)
            {
                sol.Fitness = sol.Wins;
            }
        }
    }
}