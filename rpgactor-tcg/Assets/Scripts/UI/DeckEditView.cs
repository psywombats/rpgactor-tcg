using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public class DeckEditView : MonoBehaviour
    {
        [SerializeField] private DeckView deckView;
        [SerializeField] private CardSelectorView selectorView;

        [SerializeField] private DeckData defaultDeck;
        
        private Deck deck;
        private CharacterCard currentCard;

        private LaneType? currentlyReplacingLane;
        private CancellationTokenSource replaceCardCTS;
        
        public void Populate(Deck newDeck)
        {
            deck = newDeck;
            deckView.Populate(newDeck, OnCardSelect);
        }

        private void OnCardSelect(CardView cardView)
        {
            if (currentlyReplacingLane != null)
            {
                var startAgain = cardView.Lane != currentlyReplacingLane;
                CancelReplacement();
                if (!startAgain)
                {
                    return;
                }
            }
            ReplaceCardAsync(cardView).Forget();
        }

        private async Task ReplaceCardAsync(CardView cardView)
        {
            currentlyReplacingLane = cardView.Lane;
            replaceCardCTS = new CancellationTokenSource();
            var card = deck[currentlyReplacingLane.Value];
            var leaderMode = deck.Leader == null || (card != null && card.IsLeader);
            try
            {
                var newCard = await selectorView.SelectCardAsync(replaceCardCTS.Token, leaderMode
                    ? CardCache.Instance.AllLeaderCards
                    : CardCache.Instance.AllHeroCards);
                deck.Replace(currentlyReplacingLane.Value, newCard);
                Populate(deck);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            currentlyReplacingLane = null;
            await selectorView.HideAsync();
        }

        private void CancelReplacement()
        {
            if (currentlyReplacingLane == null) throw new ArgumentException("No replacement in progress");
            replaceCardCTS.Cancel();
            replaceCardCTS.Dispose();
            replaceCardCTS = null;
            currentlyReplacingLane = null;
        }
    }
}