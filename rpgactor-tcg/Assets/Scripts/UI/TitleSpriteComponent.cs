using UnityEngine;

namespace RpgActorTGC
{
    public class TitleSpriteComponent : MonoBehaviour
    {
        [SerializeField] private CharaModelView charaModelView;
        [SerializeField] private TooltipSpawnComponent tooltip;
        
        protected void OnEnable()
        {
            RPGActorManager.Instance.OnStateChange += OnActorStateChange;
        }

        protected void OnDisable()
        {
            RPGActorManager.Instance.OnStateChange -= OnActorStateChange;
        }
        
        private void OnActorStateChange(InitState e, string msg)
        {
            if (e == InitState.Ready)
            {
                var chara = CardCache.Instance.AllHeroCards.Choose();
                charaModelView.Sprite = chara.Sprite;
                tooltip.Message = chara.CharacterName;
            }
        }
    }
}