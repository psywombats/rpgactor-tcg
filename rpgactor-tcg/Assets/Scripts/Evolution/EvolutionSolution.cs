using System.Collections.Generic;

namespace RpgActorTGC
{
    public abstract class EvolutionSolution<T> where T : EvolutionSolution<T>
    {
        public abstract int GetFitness(List<T> competitors);

        public abstract bool IsEquivalentTo(T other);
        
        public abstract T CrossWith(EvolutionSolution<T> other);

        public abstract void Mutate();
    }
}