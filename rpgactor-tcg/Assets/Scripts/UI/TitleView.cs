using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loadingArea;
        [SerializeField] private TMP_Text startupStateText;
        [SerializeField] private TMP_Text loadingText;
        [Space]
        [SerializeField] private CanvasGroup readyArea;
        [SerializeField] private Button newCampaignButton;
        [SerializeField] private Button howToPlayButton;
        [Space]
        [SerializeField] private CanvasGroup errorArea;
        [SerializeField] private List<Button> resetButtons = new();
        [SerializeField] private Button reloadButton;
        [Space]
        [SerializeField] private CanvasGroup allUICanvas;
        [SerializeField] private AnchorPosTransBehavior shifter;
        [SerializeField] private float shiftDuration = 2.5f;
        [SerializeField] private float uiXfadeDuration = .2f;
        [Space]
        [SerializeField] private CanvasGroup fader;
        [SerializeField] private float fadeDuration = 1;
        [SerializeField] private string nextSceneName = "Gameplay";
        
        public void Start()
        {
            RPGActorManager.Instance.OnStateChange += RPGActorManagerOnStateChange;

            foreach (var button in resetButtons)
            {
                button.onClick.AddListener(ResetAssignments);
            }
            reloadButton.onClick.AddListener(Reload);
            newCampaignButton.onClick.AddListener(LaunchNewCampaign);
            howToPlayButton.onClick.AddListener(LaunchHowToPlay);
            
            Reload();
            StartCoroutine(ShowLoadOutputRoutine());

            allUICanvas.alpha = 1f;
            shifter.TweenToStateAsync(true, shiftDuration).Forget();
            allUICanvas.DOFade(1f, shiftDuration).Play();
        }

        private void RPGActorManagerOnStateChange(InitState state, string message)
        {
            startupStateText.text = message;
            UpdateForLoadState();
        }

        private void UpdateForLoadState()
        {
            var state = RPGActorManager.Instance.State;

            loadingArea.DOKill();
            readyArea.DOKill();
            errorArea.DOKill();

            loadingArea.DOFade(state != InitState.Error && state != InitState.Ready ? 1f : 0f, uiXfadeDuration).Play();
            readyArea.DOFade(state == InitState.Ready ? 1f : 0f, uiXfadeDuration).Play();
            errorArea.DOFade(state == InitState.Error ? 1f : 0f, uiXfadeDuration).Play();

            loadingArea.blocksRaycasts = state != InitState.Error && state != InitState.Ready;
            readyArea.blocksRaycasts = state == InitState.Ready;
            errorArea.blocksRaycasts = state == InitState.Error;
        }

        private void Reload()
        {
            RPGActorManager.Instance.StartupAsync().Forget();
            UpdateForLoadState();
        }

        private void LaunchNewCampaign()
        {
            TransitionSceneAsync().Forget();
        }

        private void LaunchHowToPlay()
        {
            TransitionSceneAsync().Forget();
        }

        private void ResetAssignments()
        {
            RPGActorManager.Instance.ResetAssignments();
            Reload();
        }
        
        private async Task TransitionSceneAsync()
        {
            allUICanvas.interactable = false;
            await fader.DOFade(1f, fadeDuration).AsTask();
            SceneManager.Instance.LoadSceneImmediate(nextSceneName);
        }

        private IEnumerator ShowLoadOutputRoutine()
        {
            while (gameObject.activeInHierarchy)
            {
                loadingText.text = "Loading.";
                yield return new WaitForSeconds(.5f);
                loadingText.text = "Loading..";
                yield return new WaitForSeconds(.5f);
                loadingText.text = "Loading...";
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}