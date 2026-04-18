using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class CardView : MonoBehaviour
    {
        [Header("Display Components")]
        [SerializeField] private CharaModelView chara;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private ListView statList;
        [SerializeField] private ListView abilList;
        [Space]
        [SerializeField] private List<GameObject> leaderObjects;
        [SerializeField] private List<GameObject> followerObjects;
        [Space]
        [SerializeField] private CanvasGroup canvas;

        [Header("Config")]
        [SerializeField] private float selectedAlpha = .8f;

        [Header("Wiring")]
        [SerializeField] private Button backerButton;

        private CharacterCard card;
        private Action<CardView> tapHandler;
        
        public CharacterCard Card => card;
        
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set 
            {
                isSelected = value;
                canvas.alpha = isSelected ? selectedAlpha : 1f;
            }
        }

        protected void Awake()
        {
            backerButton.onClick.AddListener(() =>
            {
                tapHandler?.Invoke(this);
            });
        }

        public void Populate(CharacterCard newCard, Action<CardView> newTapHandler = null)
        {
            tapHandler = newTapHandler;
            card = newCard;
            
            chara.Sprite = card.Sprite;
            nameText.text = card.CharacterName;

            var statTuples = card.Stats.ToTuples()
                .Where(tuple => tuple.Item1 is Stat.MHP or Stat.ATK or Stat.DEF or Stat.SPD 
                                || (tuple.Item1 == Stat.MP && card.IsLeader));
            statList.Populate(statTuples, (obj, statAndValue) =>
            {
                obj.GetComponent<StatView>().Populate(statAndValue);
            });
            abilList.Populate(card.AbilityCards, (obj, abil) =>
            {
                obj.GetComponent<AbilView>().Populate(abil, card);
            });

            foreach (var obj in leaderObjects)
            {
                obj.SetActive(card.IsLeader);
            }
            foreach (var obj in followerObjects)
            {
                obj.SetActive(!card.IsLeader);
            }
        }
    }
}
