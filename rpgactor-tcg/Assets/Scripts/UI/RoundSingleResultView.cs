using System;
using System.Threading.Tasks;
using MapElitesTGC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class RoundSingleResultView : MonoBehaviour
    {
        [SerializeField] private ListView spritesList;
        [SerializeField] private TMP_Text versusLabel;
        [SerializeField] private Image tint;
        [SerializeField] private Button replayButton;
        [Space]
        [SerializeField] private Color winColor;
        [SerializeField] private Color lossColor;
        [Space]
        [SerializeField] private TooltipSpawnComponent nameTooltip;

        private MainGameplayView gameplayView;
        private NPCModel npc;
        
        protected void Awake()
        {
            replayButton.onClick.AddListener(() => ReplayBattleAsync().Forget());
        }

        public void Populate(MainGameplayView mainView, NPCModel newNPC)
        {
            gameplayView = mainView;
            npc = newNPC;
            var myResult = CampaignManager.Instance.Player.CurrentRoundResult.ResultsByOpponent[npc];
            
            spritesList.Populate(npc.CurrentDeck, (obj, card) =>
            {
                obj.GetComponent<ResultsSpriteView>().Populate(card);
            });
            versusLabel.text = $"vs {npc.EntrantName}";
            tint.color = myResult ?  winColor : lossColor;

            nameTooltip.Message = $"{npc.CurrentRoundResult.Wins}-{npc.CurrentRoundResult.Losses}, " +
                                  $"{npc.LifetimeWins}-{npc.LifetimeLosses} overall";
        }

        private Task ReplayBattleAsync()
        {
            var myParty = new Party(CampaignManager.Instance.Player.CurrentDeck);
            var npcParty = new Party(npc.CurrentDeck);
            return gameplayView.PlaybackBattleAsync(new BattleModel(myParty, npcParty));
        }
    }
}