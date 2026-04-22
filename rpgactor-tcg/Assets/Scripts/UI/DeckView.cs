using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] private TMP_Text deckNameText;
        [SerializeField] private List<CardView> cardViews;

        private Dictionary<LaneType, CardView> cardViewsByLane = new();

        protected void Awake()
        {
            foreach (var cardView in cardViews)
            {
                cardViewsByLane.Add(cardView.Lane, cardView);
            }
        }
        
        public void Populate(Deck deck, Action<CardView> onSelect = null)
        {
            deckNameText.text = deck.DeckName;
            foreach (var cardView in cardViews)
            {
                cardView.Populate(deck[cardView.Lane], onSelect);
            }
        }

        public void Unselect()
        {
            foreach (var view in cardViews)
            {
                view.IsSelected = false;
            }
        }
    }
}