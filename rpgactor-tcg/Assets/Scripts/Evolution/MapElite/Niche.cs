using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapElitesTGC
{
    /// <summary>
    /// A niche represents the competition space of a deck versus the top known solution decks.
    /// </summary>
    /// <remarks>
    /// For instance, any deck that wins against an arbitrary high-performing deck A and deck B but loses to a deck C
    /// defines a niche. A deck that wins against A but loses to B and C does not define a niche because it is strictly
    /// worse than the former, but a deck that loses to A and B but wins against C is considered a unique niche because
    /// it is still the only deck that wins against C.
    ///
    /// Niches are only valid within the context of a single generation.
    ///
    /// A niche has an "elite" deck that represents the best-performing deck that matches the niche criteria. This is
    /// the deck that can be used to measure the performance characteristics of a new deck. If a deck has a unique win
    /// pattern against the elites of existing niches, it is the elite of a new niche. If a deck has the same win
    /// pattern but fails against the niche's elite, or its win set is a subset of a niche, it is part of that niche but
    /// not an elite.
    /// </remarks>
    public class Niche
    {
        public Solution Elite { get; }

        public int Fitness => winSet.Count;

        private readonly MapEliteRunner runner;
        private readonly HashSet<Solution> winSet;

        public Niche(MapEliteRunner runner, HashSet<Solution> winSet, Solution elite)
        {
            Elite = elite;
            this.winSet = winSet;
            this.runner = runner;
        }

        public async Task<bool> CheckIfWeCanBeatAsync(Solution challenger)
        {
            var winner = await runner.CalcBattleWinnerAsync(Elite, challenger);
            return winner == challenger;
        }

        public async Task<bool> CheckIfSuperiorToAsync(Solution challenger, HashSet<Solution> otherWinset)
        {
            // if you can beat someone we can't, we aren't superior
            if (otherWinset.Any(win => !winSet.Contains(win)))
            {
                return false;
            }

            // if we can't beat you, we aren't superior
            if (!await CheckIfWeCanBeatAsync(challenger))
            {
                return false;
            }
            
            // you can't beat anyone we can't, and we can beat you -- we are superior
            return true;
        }

        public async Task<bool> CheckIfInferiorToAsync(Solution challenger, HashSet<Solution> otherWinset)
        {
            // if we can beat someone you can't, we're not inferior
            if (winSet.Any(win => !otherWinset.Contains(win)))
            {
                return false;
            }
            
            // if we can beat you, we clearly can't be inferior lol
            if (await CheckIfWeCanBeatAsync(challenger))
            {
                return false;
            }
            
            // you can beat everyone we can, and you can beat us -- you are superior
            return true;
        }
    }
}