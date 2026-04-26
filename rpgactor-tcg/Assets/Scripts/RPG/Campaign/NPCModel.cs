using System.Linq;

namespace RpgActorTGC
{
    public class NPCModel : EntrantModel
    {
        public sealed override string EntrantName { get; }

        private BehaviorType type;
        
        public NPCModel(BehaviorType type)
        {
            this.type = type;
            EntrantName = CampaignManager.Instance.GeneratePlayerName();
            CurrentDeck = Deck.CreateRandom(EntrantName, CampaignManager.Instance.GloballyAvailableHeroes,
                CampaignManager.Instance.GloballyAvailableLeaders);
        }

        public override void SetupForNewRound()
        { 
            // if our deck sucks, take one that's proven to work against the current field
            if (CurrentRoundResult != null && CurrentRoundResult.Wins < CurrentRoundResult.Losses)
            {
                if (CampaignManager.Instance.EvolvedReplacementDecks.Any())
                {
                    CurrentDeck = CampaignManager.Instance.EvolvedReplacementDecks.First();
                    CampaignManager.Instance.EvolvedReplacementDecks.Remove(CurrentDeck);
                    CurrentDeck.DeckName = EntrantName;
                }
                else
                {
                    // fuck it, copy a winner
                    CurrentDeck = CampaignManager.Instance.WinningDecks.Choose();
                }
            }
            base.SetupForNewRound();
        }

        public enum BehaviorType
        {
            Frontrunner,    // intelligently builds decks to counter
            Content,        // keeps entering the same deck, and only innovates when the deck starts losing
            Copycat,        // enters winning decks from last round
            Scientist,      // builds unique decks, and keeps reentering them as long as they're unique and not terrible
            Chaff,          // generates random decks every round (or keeps them if they win)
            
        }
    }
}
