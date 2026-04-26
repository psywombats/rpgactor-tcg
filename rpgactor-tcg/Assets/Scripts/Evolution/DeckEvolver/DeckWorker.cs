using System.Collections.Generic;

namespace RpgActorTGC
{
    public class DeckWorker
    {
        public record DeckTask(DeckSolution Solution1, DeckSolution Solution2);
        
        private readonly List<DeckTask> tasks = new List<DeckTask>();
        
        private readonly BattleModel battle = new();

        public void AssignTask(DeckTask task)
        {
            tasks.Add(task);
        }

        public void SimulateBattles()
        {
            foreach (var task in tasks)
            {
                // avoid the dining philosophers problem
                var party1 = task.Solution1.LockParty();
                var party2 = task.Solution2.LockParty();
                var winner = battle.SimulateBattleAsync(party1, party2).Result;
                task.Solution1.UnlockParty(party1);
                task.Solution2.UnlockParty(party2);
                ScoreResult(task.Solution1, task.Solution2, winner);
            }
            tasks.Clear();
        }

        public static void ScoreResult(DeckSolution solution1, DeckSolution solution2, Party winner)
        {
            if (winner != null)
            {
                // simple int32 addition is atomic
                solution1.Wins += winner.Deck == solution1.Deck ? 1 : 0;
                solution2.Wins += winner.Deck == solution2.Deck ? 1 : 0;

                if (winner.Deck == solution1.Deck)
                {
                    foreach (var card in solution1.Deck)
                    {
                        card.LifetimeWins += 1;
                    }
                }
                if (winner.Deck == solution2.Deck)
                {
                    foreach (var card in solution2.Deck)
                    {
                        card.LifetimeWins += 1;
                    }
                }
            }
        }
    }
}