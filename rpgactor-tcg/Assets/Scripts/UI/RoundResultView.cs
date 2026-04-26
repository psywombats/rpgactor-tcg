using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private TooltipSpawnComponent winLossTooltip;
        [Space]
        [SerializeField] private ListView topPlayersView;
        [SerializeField] private int topPlayersCount;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button toggleButton;
        [Space]
        [SerializeField] private CharaModelView backChara;
        [SerializeField] private CharaModelView leftChara;
        [SerializeField] private CharaModelView centerChara;
        [SerializeField] private CharaModelView rightChara;

        public MainGameplayView GameplayView { get; set; }

        private bool useOverallTopPlayers;
        
        public void Awake()
        {
            nextButton.onClick.AddListener(() => GameplayView.ExitTournamentAsync().Forget());
            toggleButton.onClick.AddListener(() =>
            {
                useOverallTopPlayers = !useOverallTopPlayers;
                PopulateTopPlayers();
            });
        }

        public void Populate(MainGameplayView mainView, TourneyRoundResult myResult)
        {
            GameplayView = mainView;
            
            gridResultsList.Populate(CampaignManager.Instance.NPCs.OrderBy(npc => npc.CurrentRoundResult.Wins), 
                (obj, npc) =>
            {
                obj.GetComponent<RoundSingleResultView>().Populate(mainView, npc);
            });
            topLabel.text = $"End of round {CampaignManager.Instance.RoundCount}!";
            winLossLabel.text = $"Wins: {myResult.Wins}  Losses: {myResult.Losses}";

            PopulateTopPlayers();

            var player = CampaignManager.Instance.Player;
            backChara.Sprite = player.CurrentDeck[LaneType.Back].Sprite;
            leftChara.Sprite = player.CurrentDeck[LaneType.Left].Sprite;
            centerChara.Sprite = player.CurrentDeck[LaneType.Center].Sprite;
            rightChara.Sprite = player.CurrentDeck[LaneType.Right].Sprite;

            winLossTooltip.Message = $"{player.LifetimeWins}-{player.LifetimeLosses}";
        }

        private void PopulateTopPlayers()
        {
            var topPlayers = new List<EntrantModel>(CampaignManager.Instance.AllEntrants);
            if (!useOverallTopPlayers)
            {
                topPlayers.Sort((a, b) => b.CurrentRoundResult.Wins.CompareTo(a.CurrentRoundResult.Wins));
            }
            else
            {
                topPlayers.Sort((a, b) => b.LifetimeWins.CompareTo(a.LifetimeWins));
            }
            
            var rank = 1;
            topPlayersView.Populate(topPlayers.Take(topPlayersCount), (obj, entrant) =>
            {
                obj.GetComponent<TopPlayerView>().Populate((entrant, rank), useOverallTopPlayers);
                rank += 1;
            });
        }
    }
}