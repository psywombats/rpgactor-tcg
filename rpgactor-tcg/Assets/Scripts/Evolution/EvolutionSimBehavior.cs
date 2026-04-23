using System.Linq;
using EditorAttributes;
using UnityEngine;

namespace RpgActorTGC
{
    public class EvolutionSimBehavior : MonoBehaviour
    {
        [SerializeField] private EvolutionRunner<DeckSolution>.EvolutionSettings settings;
        [SerializeField] private bool autoRun;

        public void Start()
        {
            if (autoRun)
            {
                Simulate();
            }
        }
        
        [Button]
        public void Simulate()
        {
            var runner = new DeckEvolutionRunner();
            var solutions = runner.RunEvolution(settings);
            Debug.Log($"Top solution: {solutions[0].Deck.CompositionString} ({solutions[0].Wins} wins)");
            
            var leaderString = "";
            var leaderList = CardCache.Instance.AllLeaderCards.ToList();
            leaderList.Sort((a, b) => b.LifetimeWins.CompareTo(a.LifetimeWins));
            foreach (var card in leaderList)
            {
                leaderString += card.CharacterName + " [" + card.LifetimeWins + "]\n";
            }
            Debug.Log("Leader list: " + leaderString);
            
            var followerString = "";
            var followerList = CardCache.Instance.AllHeroCards.ToList();
            followerList.Sort((a, b) => b.LifetimeWins.CompareTo(a.LifetimeWins));
            foreach (var card in followerList)
            {
                followerString += card.CharacterName + " [" + card.LifetimeWins + "]\n";
            }
            Debug.Log("Follower list: " + followerString);
        }
    }
}