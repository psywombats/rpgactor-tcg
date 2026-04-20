using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class DeckEditView : MonoBehaviour
    {
        [SerializeField] private DeckView deckView;
        [SerializeField] private CardSelectorView selectorView;
        [Space]
        [SerializeField] private List<StateTransformBehavior> deckShowTrans = new();
        [SerializeField] private float transitionDuration = .5f;
        [Space]
        [SerializeField] private Button clearButton;
        [SerializeField] private ListView saveLoadEntries;
        
        public Deck Deck { get; private set; }
        private CharacterCard currentCard;

        private LaneType? currentlyReplacingLane;
        private CancellationTokenSource replaceCardCTS;

        public void Awake()
        {
            clearButton.onClick.AddListener(ClearDeck);
        }

        public void Populate(Deck newDeck)
        {
            Deck = newDeck;
            deckView.Populate(newDeck, OnCardSelect);
            saveLoadEntries.Populate(Enumerable.Range(0, ConstantsData.Instance.deckCount), (obj, i) =>
            {
                obj.GetComponent<SaveLoadView>().Populate(i, this);
            });
        }
        
        public void Repopulate() => Populate(Deck);

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
            var card = Deck[currentlyReplacingLane.Value];
            cardView.IsSelected = true;
            var leaderMode = Deck.Leader == null || (card != null && card.IsLeader);
            try
            {
                selectorView.Populate(CardCache.Instance.AllCards, leaderMode, replaceCardCTS.Token);
                await Task.WhenAll(deckShowTrans.Select(trans 
                    => trans.TweenToStateAsync(true, transitionDuration)));
                if (replaceCardCTS.Token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
                
                var newCard = await selectorView.SelectCardAsync();
                Deck.Replace(currentlyReplacingLane.Value, newCard);
                Repopulate();
            }
            catch (OperationCanceledException) {}

            cardView.IsSelected = false;
            currentlyReplacingLane = null;
            await Task.WhenAll(deckShowTrans.Select(trans 
                => trans.TweenToStateAsync(false, transitionDuration)));
        }

        private void CancelReplacement()
        {
            if (currentlyReplacingLane == null) return;
            replaceCardCTS.Cancel();
            replaceCardCTS.Dispose();
            replaceCardCTS = null;
            currentlyReplacingLane = null;
        }

        private void ClearDeck()
        {
            CancelReplacement();
            selectorView.Cancel();
            Deck.Replace(LaneType.Back, null);
            Deck.Replace(LaneType.Left, null);
            Deck.Replace(LaneType.Center, null);
            Deck.Replace(LaneType.Right, null);
            Repopulate();
        }
    }
}