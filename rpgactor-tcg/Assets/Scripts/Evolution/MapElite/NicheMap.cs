using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class NicheMap
    {
        private readonly MapEliteRunner runner;
        private HashSet<Niche> niches = new();

        public NicheMap(MapEliteRunner runner, IEnumerable<DeckSolution> initialSet)
        {
            this.runner = runner;
            foreach (var deck in initialSet)
            {
                var niche = ClassifyDeck(deck);
            }
        }

        /// <summary>
        /// Determines the niches to which this deck belongs.
        /// </summary>
        /// <remarks>
        /// If this is a particularly novel deck, it could define a new niche. If this deck is a particularly
        /// good deck, it might not define a new niche, but instead replace the elite that currently dominates a
        /// particular niche. In these cases, the returned set will contain only a single niche.
        ///
        /// If this deck is neither novel nor top-class, it will likely return several niches. This means that this deck
        /// is strictly inferior to the elites of either niche, and it could be classified as either.
        /// </remarks>
        /// <param name="deck">The deck to classify</param>
        /// <returns>The set of niches to which this deck is a member</returns>
        public HashSet<Niche> ClassifyDeck(DeckNicheSolution deck)
        {
            var qualifyingNiches = new HashSet<Niche>();
            var winSet = new HashSet<Niche>();
            foreach (var niche in niches)
            {
                if (!niche.CanWeBeat(deck))
                {
                    winSet.Add(niche);
                }
            }

            foreach (var niche in niches)
            {
                if (niche.IsSuperiorTo(deck, winSet))
                {
                    qualifyingNiches.Add(niche);
                }
            }

            // we are a subset of some amount of other niches
            if (qualifyingNiches.Any())
            {
                return qualifyingNiches;
            }
            
            // at this point, we are known to be either a unique niche, or strictly superior to at least one other
            var newNiche = new Niche(runner, winSet, deck);
            var inferiorNiches = new HashSet<Niche>();
            foreach (var niche in niches)
            {
                if (niche.IsInferiorTo(deck, winSet))
                {
                    inferiorNiches.Add(niche);
                }
            }

            niches.Add(newNiche);
            // consolider all inferio niches under us
            foreach (var inferiorNiche in inferiorNiches)
            {
                // we would be the elite of this new niche if it were to continue to exist
                niches.Remove(inferiorNiche);
            }
            
            return new HashSet<Niche> { newNiche };
        }
    }
}