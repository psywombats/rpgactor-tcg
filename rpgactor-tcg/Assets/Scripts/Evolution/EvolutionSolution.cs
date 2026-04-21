namespace RpgActorTGC
{
    public abstract class EvolutionSolution<T> where T : EvolutionSolution<T>
    {
        // really only meant to be set in AssignFitness by the runner
        public int Fitness { get; set; }

        public abstract bool IsEquivalentTo(T other);
        
        public abstract T CrossWith(T other);

        public abstract void Mutate();
    }
}