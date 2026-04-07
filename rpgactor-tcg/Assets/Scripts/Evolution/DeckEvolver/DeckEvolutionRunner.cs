using System.Collections.Generic;
using UnityEngine;

namespace RpgActorTGC
{
    public class DeckEvolutionRunner : EvolutionRunner<DeckSolution>
    {
        private BattleModel battle = new();
        
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

            return new DeckSolution(this, new Deck("Random Deck", cards[0], cards[1], cards[2], cards[3]));
        }

        protected override void AssignScoresToSolutions(List<DeckSolution> solutions)
        {
            foreach (var sol in solutions)
            {
                sol.Wins = 0;
            }

            for (var i = 0; i < solutions.Count; i++)
            {
                var sol1 =  solutions[i];
                for (var j = i + 1; j < solutions.Count; j++)
                {
                    var sol2 = solutions[j];
                    var winner = battle.SimulateBattle(sol1.GetFreshParty(), sol2.GetFreshParty());
                    sol1.Wins += winner.Deck == sol1.Deck ? 1 : 0;
                    sol2.Wins += winner.Deck == sol2.Deck ? 1 : 0;
                }
            }
            
            foreach (var sol in solutions)
            {
                sol.Fitness = sol.Wins;
            }
        }
    }
}