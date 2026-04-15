using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class StatView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text valueText;

        public void Populate(Tuple<Stat, float> statAndValue)
        {
            icon.sprite = statAndValue.Item1.Info().Icon;
            valueText.text = ((int)statAndValue.Item2).ToString();
        }
    }
}