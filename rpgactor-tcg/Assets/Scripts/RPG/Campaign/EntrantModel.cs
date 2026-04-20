using System.Collections.Generic;

namespace RpgActorTGC
{
    public abstract class EntrantModel
    {
        public List<TourneyRoundResult> HistoricalResults { get; } = new();
        
        public TourneyRoundResult CurrentRoundResult { get; private set; }
        
        public Deck CurrentDeck { get; protected set; }
        
        public abstract string EntrantName { get; }

        public virtual void SetupForNewRound()
        {
            if (CurrentRoundResult != null)
            {
                HistoricalResults.Add(CurrentRoundResult);
            }
            CurrentRoundResult = new TourneyRoundResult();
        }
    }
}