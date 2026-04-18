using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class HPSliderView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private AnchorPosTransBehavior anchorTrans;

        private int hp, max;
        
        public void Populate(int newHP, int newMax)
        {
            hp = newHP;
            max = newMax;
            label.text = $"{hp}/{max}";
            anchorTrans.JumpToLerp((float)hp / max);
        }

        public async Task TweenTo(int newHP, float duration)
        {
            anchorTrans.TweenToLerpAsync(hp, max).Forget();
            await DOTween.To(() => hp, val => hp = val, newHP, duration).Play().AsTask();
        }
    }
}