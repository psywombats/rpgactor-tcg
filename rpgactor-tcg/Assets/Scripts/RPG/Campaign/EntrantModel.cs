using System.Collections.Generic;

namespace RpgActorTGC
{
    public abstract class EntrantModel
    {
        public abstract string EntrantName { get; }
        
        public List<TourneyRoundResult> HistoricalResults { get; } = new();
        public TourneyRoundResult CurrentRoundResult { get; private set; }
        public Deck CurrentDeck { get; protected set; }
        public int LifetimeWins { get; protected set; }
        public int LifetimeLosses { get; protected set; }

        public virtual void SetupForNewRound()
        {
            if (CurrentRoundResult != null)
            {
                HistoricalResults.Add(CurrentRoundResult);
                LifetimeWins += CurrentRoundResult.Wins;
                LifetimeLosses += CurrentRoundResult.Losses;
            }
            CurrentRoundResult = new TourneyRoundResult();
        }
    }
}