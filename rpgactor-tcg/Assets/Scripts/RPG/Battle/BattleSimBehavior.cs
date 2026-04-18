using System;
using EditorAttributes;
using UnityEngine;
using Void = EditorAttributes.Void;

namespace RpgActorTGC
{
    public class BattleSimBehavior : MonoBehaviour
    {
        [SerializeField] private BattlePlaybackView view;
        [Space]
        [SerializeField, EnableField(nameof(UsePlayer1Deck))] private DeckData player1;
        [SerializeField, EnableField(nameof(UsePlayer1Card))] private CharacterData p1Back;
        [SerializeField, EnableField(nameof(UsePlayer1Card))] private CharacterData p1Center;
        [SerializeField, EnableField(nameof(UsePlayer1Card))] private CharacterData p1Left;
        [SerializeField, EnableField(nameof(UsePlayer1Card))] private CharacterData p1Right;
        [Space]
        [SerializeField, EnableField(nameof(UsePlayer2Deck))] private DeckData player2;
        [SerializeField, EnableField(nameof(UsePlayer2Card))] private CharacterData p2Back;
        [SerializeField, EnableField(nameof(UsePlayer2Card))] private CharacterData p2Center;
        [SerializeField, EnableField(nameof(UsePlayer2Card))] private CharacterData p2Left;
        [SerializeField, EnableField(nameof(UsePlayer2Card))] private CharacterData p2Right;
        [Space] 
        [SerializeField] private bool autoRun;
        [SerializeField, MessageBox(nameof(ResultString), nameof(ShouldShowResults), MessageMode.Log, StringInputMode.Dynamic)] private Void messageBoxHolder;
        [SerializeField, MessageBox("Can only simulate in play mode", nameof(IsNotRunnable), MessageMode.Warning)] private Void warningBoxHolder;
        
        private bool UsePlayer1Deck => p1Back == null && p1Center == null && p1Left == null && p1Right == null;
        private bool UsePlayer2Deck => p2Back == null && p2Center == null && p2Left == null && p2Right == null;
        private bool UsePlayer1Card => player1 == null;
        private bool UsePlayer2Card => player2 == null;
        
        private string ResultString { get; set; }
        private bool ShouldShowResults => !string.IsNullOrEmpty(ResultString);
        private bool IsRunnable => Application.isPlaying;
        private bool IsNotRunnable => !IsRunnable;

        public void Start()
        {
            if (autoRun)
            {
                Simulate();
            }
        }

        [Button]
        public void ClearPlayer1()
        {
            player1 = null;
            p1Back = p1Left = p1Center = p1Right = null;
        }
        
        [Button]
        public void ClearPlayer2()
        {
            player2 = null;
            p2Back = p2Left = p2Center = p2Right = null;
        }

        [Button(false, nameof(IsRunnable), ConditionResult.EnableDisable)]
        public void Simulate()
        {
            var p1 = UsePlayer1Deck ? new Party(player1) : new Party("Player1", p1Back, p1Left, p1Center, p1Right);
            var p2 = UsePlayer2Deck ? new Party(player2) : new Party("Player2", p2Back, p2Left, p2Center, p2Right);
            var model = new BattleModel(p1, p2)
            {
                UseVerboseLogging = true
            };
            model.SimulateBattle();
            Debug.Log(model.Report);
            ResultString = model.LivenessString;
        }
    }
}