using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class StatView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text valueText;

        public void Populate(Stat stat, float value)
        {
            icon.sprite = stat.Info().Icon;
            valueText.text = value.ToString(CultureInfo.InvariantCulture);
        }
        
        public void Populate(Tuple<Stat, float> statAndValue) =>  Populate(statAndValue.Item1, statAndValue.Item2);
    }
}