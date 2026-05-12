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
            CurrentDeck = Deck.CreateRandom(EntrantName,
                CampaignManager.Instance.GloballyAvailableHeroes,
                CampaignManager.Instance.GloballyAvailableLeaders);
        }

        public override void SetupForNewRound()
        {
            if (CurrentRoundResult != null)
            {
                switch (type)
                {
                    case BehaviorType.Frontrunner: CheckForNewFrontrunnerDeck(); break;
                    case BehaviorType.Content:     CheckForNewContentDeck();     break;
                    case BehaviorType.Copycat:     CheckForNewCopycatDeck();     break;
                    case BehaviorType.Scientist:   CheckForNewScientistDeck();   break;
                    case BehaviorType.Chaff:       CheckForNewChaffDeck();       break;
                }
            }
            base.SetupForNewRound();
        }

        private void CheckForNewFrontrunnerDeck()
        {
            if (CurrentRoundResult.Rank > 0)
            {
                CurrentDeck = null;
            }
            CurrentDeck ??= CampaignManager.Instance.ClaimEvolvedDeck();
            if (CurrentDeck == null)
            {
                CheckForNewCopycatDeck();
            }
        }

        private void CheckForNewCopycatDeck()
        {
            if (CurrentRoundResult.Rank > ConstantsData.Instance.winningDeckCount)
            {
                CurrentDeck = null;
            }
            CurrentDeck ??= CampaignManager.Instance.ClaimWinningDeck();
            if (CurrentDeck == null)
            {
                CheckForNewChaffDeck();
            }
        }

        private void CheckForNewScientistDeck()
        {
            if (CurrentRoundResult.Losses > CurrentRoundResult.Wins)
            {
                CurrentDeck = null;
            }
            CurrentDeck ??= CampaignManager.Instance.ClaimUniqueEvolvedDeck();
            if (CurrentDeck == null)
            {
                CheckForNewContentDeck();
            }
        }

        private void CheckForNewChaffDeck()
        {
            if (CurrentRoundResult.Losses > CurrentRoundResult.Wins + 6)
            {
                CurrentDeck = null;
            }
            CurrentDeck = Deck.CreateRandom(EntrantName,
                CampaignManager.Instance.GloballyAvailableHeroes,
                CampaignManager.Instance.GloballyAvailableLeaders);
        }
        
        private void CheckForNewContentDeck()
        {
            if (CurrentRoundResult.Losses > CurrentRoundResult.Wins + 3)
            {
                CurrentDeck = null;
            }
            CurrentDeck ??= CampaignManager.Instance.ClaimUniqueWinningDeck();
            if (CurrentDeck == null)
            {
                CheckForNewChaffDeck();
            }
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
