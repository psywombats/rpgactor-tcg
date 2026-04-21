using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class MainGameplayView : MonoBehaviour
    {
        [SerializeField] private DeckEditView editor;
        [SerializeField] private StateTransformBehavior editorTrans;
        [SerializeField] private GameObject editorTouchBlocker;
        [Space]
        [SerializeField] private BattlePlaybackView player;
        [SerializeField] private float transitionDuration = .8f;
        [Space]
        [SerializeField] private CanvasGroup fader;
        [SerializeField] private float fadeDuration = 1f;
        [Space]
        [SerializeField] private RoundResultView resultView;
        [SerializeField] private GameObject tourneyLoadingArea;
        [SerializeField] private Button tournamentButton;
        [SerializeField] private StateTransformBehavior resultTrans;

        public void Awake()
        {
            tournamentButton.onClick.AddListener(() => EnterTournamentAsync().Forget());
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
            await player.PlayBattleAsync();
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
            editorTouchBlocker.SetActive(false);
        }
    }
}