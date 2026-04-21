using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RpgActorTGC
{
    public class RoundResultView : MonoBehaviour
    {
        [SerializeField] private ListView gridResultsList;
        [SerializeField] private TMP_Text topLabel;
        [SerializeField] private TMP_Text winLossLabel;
        [Space]
        [SerializeField] private ListView topPlayersView;
        [SerializeField] private int topPlayersCount;
        [SerializeField] private Button nextButton;
        [Space]
        [SerializeField] private CharaModelView backChara;
        [SerializeField] private CharaModelView leftChara;
        [SerializeField] private CharaModelView centerChara;
        [SerializeField] private CharaModelView rightChara;

        public MainGameplayView GameplayView { get; set; }
        
        public void Awake()
        {
            nextButton.onClick.AddListener(() => GameplayView.ExitTournamentAsync().Forget());
        }

        public void Populate(MainGameplayView mainView, TourneyRoundResult myResult)
        {
            GameplayView = mainView;
            
            gridResultsList.Populate(CampaignManager.Instance.NPCs, (obj, npc) =>
            {
                obj.GetComponent<RoundSingleResultView>().Populate(mainView, npc);
            });
            topLabel.text = $"End of round {CampaignManager.Instance.RoundCount}!";
            winLossLabel.text = $"Wins: {myResult.Wins}  Losses: {myResult.Losses}";

            var topPlayers = new List<EntrantModel>(CampaignManager.Instance.AllEntrants);
            topPlayers.Sort((a, b) => b.CurrentRoundResult.Wins.CompareTo(a.CurrentRoundResult.Wins));
            var rank = 1;
            topPlayersView.Populate(topPlayers.Take(topPlayersCount), (obj, entrant) =>
            {
                obj.GetComponent<TopPlayerView>().Populate((entrant, rank));
                rank += 1;
            });

            var myCards = CampaignManager.Instance.Player.CurrentDeck.CardsByLane;
            backChara.Sprite = myCards[LaneType.Back].Sprite;
            leftChara.Sprite = myCards[LaneType.Left].Sprite;
            centerChara.Sprite = myCards[LaneType.Center].Sprite;
            rightChara.Sprite = myCards[LaneType.Right].Sprite;
        }
    }
}