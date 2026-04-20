using System;
using DG.Tweening;
using UnityEngine;

namespace RpgActorTGC
{
    public class MainGameplayView : MonoBehaviour
    {
        [SerializeField] private DeckEditView editor;
        [Space]
        [SerializeField] private CanvasGroup fader;
        [SerializeField] private float fadeDuration = 1f;

        public void Start()
        {
            editor.Populate(CampaignManager.Instance.GetDeck(0));
            
            fader.alpha = 1f;
            fader.DOFade(0f, fadeDuration).Play();
        }
    }
}