using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace RpgActorTGC
{
    public abstract class EvolutionRunner<T> where T : EvolutionSolution<T>
    {
        [Serializable]
        public class EvolutionSettings
        {
            public int generationCount = 100;
            public int generationSize = 100;
            public int generationWinnerCount = 10;
            public int generationParentCount = 40;
            public int generationChildCount = 90;
            public float mutationRate = .5f;
        }
        
        public IList<T> RunEvolution(EvolutionSettings settings)
        {
            var currentSolutions = new List<T>();
            for (var generation = 0; generation < settings.generationCount; generation += 1)
            {
                while (currentSolutions.Count < settings.generationSize)
                {
                    currentSolutions.Add(CreateRandomSolution());
                }

                AssignScoresToSolutions(currentSolutions);

                if (generation < settings.generationCount - 1)
                {
                    var orderedSolutions = currentSolutions.OrderBy( solution => solution.Fitness ).Reverse().ToList();
                    currentSolutions.Clear();
                    currentSolutions.AddRange(orderedSolutions.Take(settings.generationWinnerCount));
                
                    var parents = orderedSolutions.GetRange(settings.generationWinnerCount, settings.generationParentCount);
                    while (currentSolutions.Count < settings.generationChildCount)
                    {
                        var parent1 = RandomUtils.WeightedChoose(parents);
                        var parent2 = RandomUtils.WeightedChoose(parents);
                        var child = parent1.CrossWith(parent2);
                        currentSolutions.Add(child);
                    
                        if (Random.Range(0f, 1f) <= settings.mutationRate)
                        {
                            child.Mutate();
                        }
                    }
                }
            }

            return currentSolutions.Take(settings.generationWinnerCount).ToList();
        }

        protected abstract T CreateRandomSolution();
        
        protected abstract void AssignScoresToSolutions(List<T> solutions);
    }
}


