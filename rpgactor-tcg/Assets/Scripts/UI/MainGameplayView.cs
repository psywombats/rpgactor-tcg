using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class MainGameplayView : MonoBehaviour
    {
        [SerializeField] private DeckEditView editor;
        [SerializeField] private StateTransformBehavior editorTrans;
        [Space]
        [SerializeField] private BattlePlaybackView player;
        [SerializeField] private float transitionDuration = .8f;
        [SerializeField] private GameObject editorTouchBlocker;
        [Space]
        [SerializeField] private CanvasGroup fader;
        [SerializeField] private float fadeDuration = 1f;
        [Space]
        [SerializeField] private RoundResultView resultView;
        [SerializeField] private UnlockedCardsDialog cardsDialog;
        [SerializeField] private GameObject tourneyLoadingArea;
        [SerializeField] private Button tournamentButton;
        [SerializeField] private StateTransformBehavior resultTrans;
        [Space]
        [SerializeField] private CanvasGroup dialogGroup;
        [SerializeField] private TMP_Text dialogLabel;
        [SerializeField] private Button dialogCloseButton;
        [SerializeField] private float dialogTransitionDuration = .5f;

        public void Awake()
        {
            tournamentButton.onClick.AddListener(() => EnterTournamentAsync().Forget());
            dialogCloseButton.onClick.AddListener(() => CloseDialogAsync().Forget());
        }

        public void Start()
        {
            editor.Populate(this, CampaignManager.Instance.Player.GetDeck(0));
            
            fader.alpha = 1f;
            fader.DOFade(0f, fadeDuration).Play();
        }

        public async Task PlaybackBattleAsync(BattleModel battle)
        {
            editorTouchBlocker.SetActive(true);
            player.Populate(battle);
            await editorTrans.TweenToStateAsync(true, transitionDuration);
            editor.gameObject.SetActive(false);
            await player.PlayBattleAsync();
            editor.gameObject.SetActive(true);
            await editorTrans.TweenToStateAsync(false, transitionDuration);
            editorTouchBlocker.SetActive(false);
        }

        private async Task EnterTournamentAsync()
        {
            if (editor.TryPopIncompletionDialog())
            {
                return;
            }
            
            editorTouchBlocker.SetActive(true);
            
            tourneyLoadingArea.SetActive(true);
            await Task.Run(() => CampaignManager.Instance.SimulateRound(editor.Deck));
            tourneyLoadingArea.SetActive(false);

            resultView.Populate(this, CampaignManager.Instance.Player.CurrentRoundResult);
            await resultTrans.TweenToStateAsync(true, transitionDuration);
            
            editorTouchBlocker.SetActive(false);
        }

        public async Task ExitTournamentAsync()
        {
            editorTouchBlocker.SetActive(true);
            await resultTrans.TweenToStateAsync(false, transitionDuration);
            
            var newCards = CampaignManager.Instance.CheckForNewUnlocks();
            if (newCards.Count > 0)
            {
                await cardsDialog.ShowDialogAsync(newCards);
            }
            
            editorTouchBlocker.SetActive(false);
        }
        
        public async Task PopDialogAsync(string message)
        {
            dialogLabel.text = message;
            dialogGroup.gameObject.SetActive(true);
            await dialogGroup.DOFade(1f, dialogTransitionDuration).AsTask();
        }

        private async Task CloseDialogAsync()
        {
            await dialogGroup.DOFade(0f, dialogTransitionDuration).AsTask();
            dialogGroup.gameObject.SetActive(false);
        }
    }
}