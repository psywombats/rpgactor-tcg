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
        [SerializeField] private MPView mp;
        [Space]
        [SerializeField] private List<GameObject> leaderObjects;
        [SerializeField] private List<GameObject> followerObjects;
        [Space]
        [SerializeField] private CanvasGroup canvas;
        [Space]
        [SerializeField] private Button backerButton;
        [SerializeField] private GameObject nullArea;
        [SerializeField] private GameObject nonNullArea;
        [Space]
        [SerializeField] private GameObject lockedArea;
        [SerializeField] private GameObject unlockedArea;
        [SerializeField] private TMP_Text lockedLabel;
        [SerializeField] private TooltipSpawnComponent lockedTooltip;

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
            
            foreach (var obj in leaderObjects)
            {
                obj.SetActive(Card != null && Card.IsLeader);
            }
            foreach (var obj in followerObjects)
            {
                obj.SetActive(Card != null && !Card.IsLeader);
            }

            if (Card != null)
            {
                chara.Sprite = Card.Sprite;
                nameText.text = Card.CharacterName;
                
                unlockedArea.SetActive(Card.IsUnlocked);
                lockedArea.SetActive(!Card.IsUnlocked);
                lockedTooltip.Message = $"This character is a reward for reaching {card.UnlocksAt} wins";
                lockedLabel.text = card.UnlocksAt.ToString();

                mp.Populate((int)Card[Stat.MP]);
                var statTuples = Card.Stats.ToTuples()
                    .Where(tuple => tuple.Item1 is Stat.MHP or Stat.ATK or Stat.SPD 
                                    || (tuple.Item1 == Stat.DEF && Card[Stat.DEF] > 0));
                statList.Populate(statTuples, (obj, statAndValue) =>
                {
                    obj.GetComponent<StatView>().Populate(statAndValue);
                });
                abilList.Populate(Card.AbilityCards, (obj, abil) =>
                {
                    obj.GetComponent<AbilCardView>().Populate(abil, Card);
                });
            }
        }
    }
}
