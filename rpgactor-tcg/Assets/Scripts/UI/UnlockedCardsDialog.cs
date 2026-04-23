using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class UnlockedCardsDialog : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvas;
        [SerializeField] private TMP_Text label;
        [SerializeField] private ListView cardList;
        [SerializeField] private Button continueButton;
        [SerializeField] private GridLayoutGroup grid;
        [Space]
        [SerializeField] private string labelFormatString;
        [SerializeField] private float transitionDuration = .5f;
        [SerializeField] private int maxCols = 5;

        private TaskCompletionSource<bool> tcs;

        protected void Awake()
        {
            continueButton.onClick.AddListener(() => tcs.TrySetResult(true));
        }
        
        public async Task ShowDialogAsync(List<CharacterCard> newCards)
        {
            Populate(newCards);
            
            gameObject.SetActive(true);
            await canvas.DOFade(1f, transitionDuration).AsTask();
            
            tcs = new TaskCompletionSource<bool>();
            await tcs.Task;
            
            await canvas.DOFade(0f, transitionDuration).AsTask();
            gameObject.SetActive(false);
        }

        private void Populate(List<CharacterCard> newCards)
        {
            cardList.Populate(newCards, (obj, card) =>
            {
                obj.GetComponent<CardView>().Populate(card);
            });
            
            var count = newCards.Count;
            grid.constraintCount = count == 6 ? 3
                : count > 6 ? maxCols
                : count;

            label.text = string.Format(labelFormatString, newCards[0].UnlocksAt);
        }
    }
}