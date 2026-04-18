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

        private CharacterCard currentlyReplacingCard;
        private CancellationTokenSource replaceCardCTS;

        protected void Start()
        {
            Populate(new Deck(defaultDeck));
        }
        
        public void Populate(Deck newDeck)
        {
            deck = newDeck;
            deckView.Populate(newDeck, OnCardSelect);
        }

        private void OnCardSelect(CardView cardView)
        {
            if (currentlyReplacingCard != null)
            {
                var startAgain = cardView.Card != currentlyReplacingCard;
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
            currentlyReplacingCard = cardView.Card;
            replaceCardCTS = new CancellationTokenSource();
            try
            {
                var newCard = await selectorView.SelectCardAsync(replaceCardCTS.Token, currentlyReplacingCard.IsLeader
                    ? CardCache.Instance.AllLeaderCards
                    : CardCache.Instance.AllHeroCards);
                deck.Replace(currentlyReplacingCard, newCard);
                Populate(deck);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            currentlyReplacingCard = null;
            await selectorView.HideAsync();
        }

        private void CancelReplacement()
        {
            if (currentlyReplacingCard == null) throw new ArgumentException("No replacement in progress");
            replaceCardCTS.Cancel();
            replaceCardCTS.Dispose();
            replaceCardCTS = null;
            currentlyReplacingCard = null;
        }
    }
}