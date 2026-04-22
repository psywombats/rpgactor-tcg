using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public class TutorialDialogSpawner : MonoBehaviour
    {
        [SerializeField, TextArea(8, 24)] private string message;
        [SerializeField] private MainGameplayView mainView;
        [Space]
        [SerializeField] private bool popOnEnable;
        [SerializeField] private StateTransformBehavior popOnTrans;
        [SerializeField] private float requiredPopLerp;
        [SerializeField] private float delay;

        private bool hasPopped;

        protected void Start()
        {
            if (popOnTrans != null)
            {
                popOnTrans.OnTransition += OnStateTransition;
            }
        }

        private void OnStateTransition(float t)
        {
            if (Mathf.Approximately(t, requiredPopLerp))
            {
                popOnTrans.OnTransition -= OnStateTransition;
                TrySpawn();
            }
        }

        protected void OnEnable()
        {
            if (popOnEnable)
            {
                TrySpawn();
            }
        }

        public void TrySpawn()
        {
            if (CampaignManager.Instance.IsTutorial && !hasPopped)
            {
                SpawnAsync().Forget();
            }
        }

        private async Task SpawnAsync()
        {
            await Task.Delay((int)(delay * 1000f));
            mainView.PopDialogAsync(message).Forget();
            hasPopped = true;
        }
    }
}