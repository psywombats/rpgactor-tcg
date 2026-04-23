using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
            foreach (var sol in solutions)
            {
                sol.Wins = 0;
            }
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                while (workers.Count < Environment.ProcessorCount)
                {
                    workers.Add(new DeckWorker());
                }

                var created = 0;
                foreach (var sol1 in solutions)
                {
                    foreach (var sol2 in oppositionSolutions)
                    {
                        workers[created % workers.Count].AssignTask(new DeckWorker.DeckTask(sol1, sol2));
                        created += 1;
                    }
                }
                Parallel.Invoke(workers.Select<DeckWorker, Action>(worker => worker.SimulateBattles).ToArray());
            }
            else
            {
                var battle = new BattleModel();
                foreach (var sol1 in solutions)
                {
                    foreach (var sol2 in oppositionSolutions)
                    {
                        var party1 = sol1.GetFreshParty();
                        var party2 = sol2.GetFreshParty();
                        var winner = battle.SimulateBattleAsync(party1, party2).Result;
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