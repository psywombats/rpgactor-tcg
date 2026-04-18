using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public class CardSelectorView : MonoBehaviour
    {
        [SerializeField] private ListView cardList;
        [SerializeField] private StateTransformBehavior stateTrans;
        [Space]
        [SerializeField] private float transitionDuration = .5f;
        
        private TaskCompletionSource<CharacterCard> tcs;

        public void Populate(IEnumerable<CharacterCard> cards, Action<CardView> tapHandler)
        {
            cardList.Populate(cards, (obj, card) =>
            {
                obj.GetComponent<CardView>().Populate(card, tapHandler);
            });
        }

        public Task ShowAsync() => stateTrans.TweenToStateAsync(true, transitionDuration);
        public Task HideAsync() => stateTrans.TweenToStateAsync(false, transitionDuration);

        public async Task<CharacterCard> SelectCardAsync(CancellationToken cts, IEnumerable<CharacterCard> cards)
        {
            if (cts.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            
            tcs = new TaskCompletionSource<CharacterCard>();
            
            Populate(cards, cardView =>
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
            
            await ShowAsync();
            return await tcs.Task;
        }
    }
}