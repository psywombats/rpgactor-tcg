using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class PlaybackSelectionView : MonoBehaviour
    {
        [SerializeField] private ListView opponentButtons;
        [SerializeField] private Button cancelButton;
        [SerializeField] private StateTransformBehavior showHideTrans;
        [SerializeField] private float transitionDuration = .5f;

        private DeckEditView editor;
        
        protected void Awake()
        {
            cancelButton.onClick.AddListener(() =>
            {
                HideAsync().Forget();
                editor.ShowMainMenuAsync().Forget();
            });
        }
        
        public void Populate(IEnumerable<Deck> opponents, DeckEditView newEditor)
        {
            editor = newEditor;
            opponentButtons.Populate(opponents, (obj, deck) =>
            {
                obj.GetComponent<OpponentButtonView>().Populate(deck, editor);
            });
        }

        public Task ShowAsync() => showHideTrans.TweenToStateAsync(true, transitionDuration);
        public Task HideAsync() => showHideTrans.TweenToStateAsync(false, transitionDuration);
    }
}