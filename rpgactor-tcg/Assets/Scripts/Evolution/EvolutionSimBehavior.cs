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
        }
    }
}