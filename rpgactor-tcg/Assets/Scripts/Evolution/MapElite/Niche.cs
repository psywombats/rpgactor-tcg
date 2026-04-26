using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
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
        public DeckNicheSolution Elite { get; }

        private readonly MapEliteRunner runner;
        private HashSet<Niche> winSet;

        public Niche(MapEliteRunner runner, HashSet<Niche> winSet, DeckNicheSolution elite)
        {
            Elite = elite;
            this.winSet = winSet;
            this.runner = runner;
        }

        public bool CanWeBeat(DeckNicheSolution challenger)
        {
            var winner = runner.CalcWinner(Elite, challenger);
            return winner == challenger;
        }

        public bool IsSuperiorTo(DeckNicheSolution challenger, HashSet<Niche> otherWinset)
        {
            // if you can beat someone we can't, we aren't superior
            if (otherWinset.Any(win => !winSet.Contains(win)))
            {
                return false;
            }

            // if we can't beat you, we aren't superior
            if (!CanWeBeat(challenger))
            {
                return false;
            }
            
            // you can't beat someone that we can, and we can beat you -- we are superior
            return true;
        }

        public bool IsInferiorTo(DeckNicheSolution challenger, HashSet<Niche> otherWinset)
        {
            // if we can beat someone you can't, we're not inferior
            if (winSet.Any(win => !otherWinset.Contains(win)))
            {
                return false;
            }
            
            // if we can beat you, so we clearly can't be inferior lol
            if (CanWeBeat(challenger))
            {
                return false;
            }
            
            // you can beat everyone we can, and you can beat us -- you are superior
            return true;
        }
    }
}