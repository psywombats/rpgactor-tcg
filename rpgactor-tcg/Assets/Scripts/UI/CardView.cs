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
        [Header("Card Components")]
        [SerializeField] private CharaModelView chara;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private ListView statList;
        [SerializeField] private ListView abilList;
        [Space]
        [SerializeField] private List<GameObject> leaderObjects;
        [SerializeField] private List<GameObject> followerObjects;
        [Space]
        [SerializeField] private CanvasGroup canvas;
        [Space]
        [SerializeField] private Button backerButton;
        [SerializeField] private GameObject nullArea;
        [SerializeField] private GameObject nonNullArea;

        [Header("Config")]
        [SerializeField] private float selectedAlpha = .8f;
        [SerializeField] private LaneType lane;
        
        private Action<CardView> tapHandler;
        
        public CharacterCard Card { get; private set; }
        public LaneType Lane => lane;
        
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

        public void Populate(CharacterCard card, Action<CardView> newTapHandler = null)
        {
            tapHandler = newTapHandler;
            Card = card;
            
            nonNullArea.SetActive(Card != null);
            nullArea.SetActive(Card == null);

            if (Card != null)
            {
                chara.Sprite = Card.Sprite;
                nameText.text = Card.CharacterName;

                var statTuples = Card.Stats.ToTuples()
                    .Where(tuple => tuple.Item1 is Stat.MHP or Stat.ATK or Stat.DEF or Stat.SPD 
                                    || (tuple.Item1 == Stat.MP && Card.IsLeader));
                statList.Populate(statTuples, (obj, statAndValue) =>
                {
                    obj.GetComponent<StatView>().Populate(statAndValue);
                });
                abilList.Populate(Card.AbilityCards, (obj, abil) =>
                {
                    obj.GetComponent<AbilCardView>().Populate(abil, Card);
                });

                foreach (var obj in leaderObjects)
                {
                    obj.SetActive(Card.IsLeader);
                }
                foreach (var obj in followerObjects)
                {
                    obj.SetActive(!Card.IsLeader);
                }
            }
        }
    }
}
