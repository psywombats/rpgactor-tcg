using System;
using System.Globalization;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class StatView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text valueText;
        [Space]
        [SerializeField] private float tweenDeltaPerSecond = 10;

        private Stat stat;
        private int value;
        
        public void Populate(Stat newStat, int newValue)
        {
            stat = newStat;
            icon.sprite = newStat.Info().Icon;
            valueText.text = newValue.ToString(CultureInfo.InvariantCulture);
        }
        
        public void Populate(Tuple<Stat, float> statAndValue) =>  Populate(statAndValue.Item1, (int)statAndValue.Item2);

        public async Task TweenToAsync(int newValue)
        {
            var duration = Math.Abs(newValue - value) / tweenDeltaPerSecond;
            await DOTween.To(() => value, val => Populate(stat, val), newValue, duration).AsTask();
        }
    }
}