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
            max = newMax;
            SetHP(newHP);
            anchorTrans.JumpToLerp((float)hp / max);
        }

        private void SetHP(int newHP)
        {
            hp = newHP;
            label.text = $"{hp}/{max}";
        }

        public async Task TweenTo(int newHP, float duration)
        {
            if (newHP < 0) newHP = 0;
            var ratio = (float)newHP / max;
            await Task.WhenAll(anchorTrans.TweenToLerpAsync(duration, ratio),
                DOTween.To(() => hp, SetHP, newHP, duration).Play().AsTask());
        }
    }
}