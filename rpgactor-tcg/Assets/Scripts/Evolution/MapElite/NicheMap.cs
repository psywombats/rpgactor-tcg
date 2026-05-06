using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpgActorTGC;

namespace MapElitesTGC
{
    public class NicheMap
    {
        private readonly MapEliteRunner runner;
        private readonly HashSet<Niche> niches = new();
        private readonly IEnumerable<Solution> uncategorizedSolutions;
        private readonly HashSet<Deck> seenDecks = new();

        /// <summary>
        /// Creates a new niche map by classifying the provided solutions. The solutions aren't graded against each
        /// other, but instead how they perform against the fixed opposition held in the runner. Populating the map is
        /// an async operation as it involves simulating battles.
        /// </summary>
        /// <param name="runner">The context of this niche map</param>
        /// <param name="solutions">The list of solutions to populate the map</param>
        public NicheMap(MapEliteRunner runner, IEnumerable<Solution> solutions)
        {
            this.runner = runner;
            uncategorizedSolutions = solutions;
        }

        public async Task PopulateMapAsync()
        {
            foreach (var sol in uncategorizedSolutions)
            {
                await ClassifySolutionAsync(sol);
            }
        }

        /// <summary>
        /// Determines the niches to which this deck belongs, then mutates the map to accomdate any new niches.
        /// </summary>
        /// <remarks>
        /// If this is a particularly novel deck, it could define a new niche. If this deck is a particularly
        /// good deck, it might not define a new niche, but instead replace the elite that currently dominates a
        /// particular niche. In these cases, the returned set will contain only a single niche.
        ///
        /// If this deck is neither novel nor top-class, it will likely return several niches. This means that this deck
        /// is strictly inferior to the elites of either niche, and it could be classified as either.
        /// </remarks>
        /// <param name="solution">The deck to classify</param>
        public async Task ClassifySolutionAsync(Solution solution)
        {
            if (!seenDecks.Add(solution.Deck)) return;

            var winSet = new HashSet<Solution>();
            foreach (var opponent in runner.Opposition)
            {
                if (await runner.CalcBattleWinnerAsync(solution, opponent) == solution)
                {
                    winSet.Add(solution);
                }
            }

            foreach (var niche in niches)
            {
                if (await niche.CheckIfSuperiorToAsync(solution, winSet))
                {
                    // we are a subset of some amount of other niches
                    return;
                }
            }
            
            // at this point, we are known to be either a unique niche, or strictly superior to at least one other
            var newNiche = new Niche(runner, winSet, solution);
            var inferiorNiches = new HashSet<Niche>();
            foreach (var niche in niches)
            {
                if (await niche.CheckIfInferiorToAsync(solution, winSet))
                {
                    inferiorNiches.Add(niche);
                }
            }

            niches.Add(newNiche);
            
            // consolidate all inferior niches under us
            foreach (var inferiorNiche in inferiorNiches)
            {
                // we would be the elite of this new niche if it were to continue to exist
                niches.Remove(inferiorNiche);
            }
        }

        /// <summary>
        /// Sorts the elites in this map by fitness, and returns the cream of the crop. Fitness is defined as how the
        /// elites perform against the fixed opposition, not each other.
        /// </summary>
        /// <param name="limit">A max of this many elites will be returned</param>
        public async Task<IList<Solution>> GetOrderedElitesAsync(int limit = 0)
        {
            var elites = niches.Select(niche => niche.Elite);
            var eliteList = elites.ToList();
            foreach (var elite in eliteList) await elite.CalcFitnessAsync();
            var orderedElites = eliteList.OrderBy(elite => -1 * elite.Fitness);
            return limit > 0 ? orderedElites.Take(limit).ToList() : orderedElites.ToList();
        }
    }
}