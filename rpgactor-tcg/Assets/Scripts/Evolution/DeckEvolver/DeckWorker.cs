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
                var winner = battle.SimulateBattle(party1, party2);
                task.Solution1.UnlockParty(party1);
                task.Solution2.UnlockParty(party2);
                
                // simple int32 addition is atomic
                task.Solution1.Wins += winner.Deck == task.Solution1.Deck ? 1 : 0;
                task.Solution2.Wins += winner.Deck == task.Solution2.Deck ? 1 : 0;

            }
            tasks.Clear();
        }
    }
}