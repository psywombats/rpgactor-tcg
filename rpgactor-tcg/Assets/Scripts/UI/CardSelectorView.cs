using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class CardSelectorView : MonoBehaviour
    {
        [SerializeField] private ListView cardList;
        [SerializeField] private Button modeToggleButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text toggleButtonLabel;

        public IEnumerable<CharacterCard> AllCards { get; private set; }
        
        private bool isLeaderMode;
        public bool IsLeaderMode
        {
            get => isLeaderMode;
            set 
            {
                if (isLeaderMode != value)
                {
                    isLeaderMode = value;
                    PopulateFilteredCards();
                    toggleButtonLabel.text = IsLeaderMode ? "Switch to Followers" : "Switch to Leaders";
                }
            }
        }

        private Action<CardView> tapHandler;
        private TaskCompletionSource<CharacterCard> tcs;

        protected void Awake()
        {
            modeToggleButton.onClick.AddListener(ToggleMode);
            cancelButton.onClick.AddListener(Cancel);
        }

        public void Populate(IEnumerable<CharacterCard> cards, bool leaderMode, Action<CardView> handler)
        {
            tapHandler = handler;
            AllCards = cards;
            IsLeaderMode = leaderMode;
            PopulateFilteredCards();
        }
        
        public void Populate(IEnumerable<CharacterCard> cards, bool leaderMode, CancellationToken cts) 
            => Populate(cards, leaderMode, cardView =>
        {
            if (!cts.IsCancellationRequested)
            {
                tcs.SetResult(cardView.Card);
            }
            else
            {
                tcs.SetCanceled();
            }
        });

        public async Task<CharacterCard> SelectCardAsync()
        {
            tcs = new TaskCompletionSource<CharacterCard>();
            return await tcs.Task;
        }

        public void Cancel()
        {
            tcs?.TrySetCanceled();
        }

        private void PopulateFilteredCards()
        {
            var filteredCards = AllCards.Where(card => card.IsLeader ^ !IsLeaderMode);
            cardList.Populate(filteredCards, (obj, card) =>
            {
                obj.GetComponent<CardView>().Populate(card, tapHandler);
            });
        }

        private void ToggleMode()
        {
            IsLeaderMode = !IsLeaderMode;
        }
    }
}