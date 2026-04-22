using System.Linq;

namespace RpgActorTGC
{
    public class NPCModel : EntrantModel
    {
        public sealed override string EntrantName { get; }
        
        public NPCModel()
        {
            EntrantName = ConstantsData.Instance.oppoNames.GeneratePlayerName();
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
    }
}
