using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RpgActorTGC
{
    public abstract class EvolutionRunner<T, U> where T : EvolutionSolution<U> where U : EvolutionSolution<U>
    {
        public class EvolutionSettings
        {
            public int GenerationCount { get; init; } = 100;
            public int GenerationSize { get; init; } = 100;
            public int GenerationWinnerCount { get; set; } = 10;
            public int GenerationParentCount { get; set; } = 40;
            public int GenerationChildCount { get; set; } = 80;
            public float MutationRate { get; set; } = .1f;
        }
        
        public IList<U> RunEvolution(EvolutionSettings settings)
        {
            var currentSolutions = new List<U>();
            for (var generation = 0; generation < settings.GenerationCount; generation += 1)
            {
                while (currentSolutions.Count < settings.GenerationSize)
                {
                    currentSolutions.Add(CreateRandomSolution());
                }

                var scores = new Dictionary<U, int>();
                foreach (var solution in currentSolutions)
                {
                    var fitness = EvaluateSolution(settings, solution);
                    scores.Add(solution, fitness);
                }
                
                var orderedSolutions = currentSolutions.OrderBy( solution => scores[solution] ).Reverse().ToList();
                currentSolutions.Clear();
                currentSolutions.AddRange(orderedSolutions.Take(settings.GenerationWinnerCount));
                
                var parents = orderedSolutions.GetRange(settings.GenerationWinnerCount, settings.GenerationParentCount);
                while (currentSolutions.Count < settings.GenerationChildCount)
                {
                    var parent1 = WeightedRandomSelect(parents);
                    var parent2 = WeightedRandomSelect(parents);
                    var child = parent1.CrossWith(parent2);
                    currentSolutions.Add(child);
                    
                    if (Random.Range(0f, 1f) <= settings.MutationRate)
                    {
                        child.Mutate();
                    }
                }
            }

            return currentSolutions.Take(settings.GenerationWinnerCount).ToList();
        }

        protected abstract U CreateRandomSolution();
        
        protected abstract int EvaluateSolution(EvolutionSettings settings, U solution);

        private static U WeightedRandomSelect(List<U> candidates)
        {
            var r = Random.Range(0f, 1f);
            r *= r; // parabolic weighting
            var index = Mathf.RoundToInt(r * candidates.Count + .5f);
            return candidates[index];
        }
    }
}


