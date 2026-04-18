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
        private Task replaceCardTask;
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
            if (replaceCardTask != null)
            {
                var startAgain = cardView.Card != currentlyReplacingCard;
                CancelReplacement();
                if (!startAgain)
                {
                    return;
                }
            }
            replaceCardTask = ReplaceCardAsync(cardView);
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

            await selectorView.HideAsync();
            replaceCardTask = null;
        }

        private void CancelReplacement()
        {
            if (replaceCardTask == null) throw new ArgumentException("No replacement in progress");
            replaceCardTask = null;
            replaceCardCTS.Cancel();
            replaceCardCTS.Dispose();
            replaceCardCTS = null;
            currentlyReplacingCard = null;
        }
    }
}