using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class DeckEditView : MonoBehaviour
    {
        [SerializeField] private DeckView deckView;
        [SerializeField] private CardSelectorView selectorView;
        [SerializeField] private StateTransformBehavior mainMenuTrans;
        [Space]
        [SerializeField] private List<StateTransformBehavior> deckShowTrans = new();
        [SerializeField] private float transitionDuration = .33f;
        [Space]
        [SerializeField] private Button clearButton;
        [SerializeField] private ListView saveLoadEntries;
        [Space]
        [SerializeField] private Button practiceButton;
        [SerializeField] private PlaybackSelectionView practiceMenu;
        [Space]
        [SerializeField] private CanvasGroup incompleteGroup;
        [SerializeField] private TMP_Text incompleteLabel;
        [SerializeField] private Button incompleteCloseButton;
        [SerializeField] private float incompleteTransitionDuration = .5f;
        
        public Deck Deck { get; private set; }
        public MainGameplayView GameplayView { get; private set; }

        private LaneType? currentlyReplacingLane;
        private CancellationTokenSource replaceCardCTS;

        public void Awake()
        {
            clearButton.onClick.AddListener(ClearDeck);
            incompleteCloseButton.onClick.AddListener(() => CloseDialogAsync().Forget());
            practiceButton.onClick.AddListener(() => PracticeAsync().Forget());
        }

        public void Populate(MainGameplayView gameplayView, Deck newDeck)
        {
            GameplayView = gameplayView;
            Deck = newDeck;
            deckView.Populate(newDeck, OnCardSelect);
            saveLoadEntries.Populate(Enumerable.Range(0, ConstantsData.Instance.deckCount), (obj, i) =>
            {
                obj.GetComponent<SaveLoadView>().Populate(i, this);
            });
        }

        public Task ShowMainMenuAsync() => mainMenuTrans.TweenToStateAsync(false, transitionDuration);
        
        public void Populate(Deck newDeck) => Populate(GameplayView, newDeck);
        
        public void Repopulate() => Populate(Deck);

        private void OnCardSelect(CardView cardView)
        {
            TryRestartReplacementAsync(cardView).Forget();
        }

        private async Task TryRestartReplacementAsync(CardView cardView)
        {
            var startAgain = currentlyReplacingLane != cardView.Lane;
            await CancelSubmenusAsync();
            if (!startAgain)
            {
                return;
            }

            await ReplaceCardAsync(cardView);
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
                /*if (Deck.ContainsCard(newCard) && Deck.CardsByLane[currentlyReplacingLane.Value] != newCard)
                {
                    
                }
                else
                {
                    
                }*/
                Deck.Replace(currentlyReplacingLane.Value, newCard);
                Repopulate();
            }
            catch (OperationCanceledException) {}

            cardView.IsSelected = false;
            currentlyReplacingLane = null;
            replaceCardCTS = null;
            await Task.WhenAll(deckShowTrans.Select(trans 
                => trans.TweenToStateAsync(false, transitionDuration)));
        }

        private async Task CancelSubmenusAsync()
        {
            await Task.WhenAll(practiceMenu.HideAsync(), ShowMainMenuAsync());
            
            if (replaceCardCTS != null)
            {
                replaceCardCTS.Cancel();
                replaceCardCTS.Dispose();
                replaceCardCTS = null;
                currentlyReplacingLane = null;
            }
        }

        private void ClearDeck()
        {
            CancelSubmenusAsync().Forget();
            selectorView.Cancel();
            Deck.Replace(LaneType.Back, null);
            Deck.Replace(LaneType.Left, null);
            Deck.Replace(LaneType.Center, null);
            Deck.Replace(LaneType.Right, null);
            Repopulate();
        }

        private async Task PracticeAsync()
        {
            await CancelSubmenusAsync();
            await mainMenuTrans.TweenToStateAsync(true, transitionDuration);
            practiceMenu.Populate(CampaignManager.Instance.Player.MyDecks, this);
            await practiceMenu.ShowAsync();
        }
        
        #region Dialog
        
        public bool TryPopIncompletionDialog()
        {
            if (Deck.IsIncomplete)
            {
                var message = Deck.Leader == null
                    ? "This party needs a leader first!"
                    : "This party has empty spaces. Fill those first!";
                PopDialogAsync(message).Forget();
                return true;
            }

            return false;
        }

        public async Task PopDialogAsync(string message)
        {
            incompleteLabel.text = message;
            incompleteGroup.gameObject.SetActive(true);
            await incompleteGroup.DOFade(1f, incompleteTransitionDuration).AsTask();
        }

        private async Task CloseDialogAsync()
        {
            await incompleteGroup.DOFade(0f, incompleteTransitionDuration).AsTask();
            incompleteGroup.gameObject.SetActive(false);
        }
        
        #endregion
    }
}