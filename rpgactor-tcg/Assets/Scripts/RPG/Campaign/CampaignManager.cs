using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgActorTGC
{
    public class CampaignManager : SingletonBehaviour<CampaignManager>
    {
        public PlayerModel Player { get; private set; }
        public List<NPCModel> NPCs { get; } = new();
        public List<EntrantModel> AllEntrants { get; } = new();

        public HashSet<CharacterCard> MyLeaders { get; } = new();
        public HashSet<CharacterCard> MyHeroes { get; } = new();
        public HashSet<CharacterCard> MyCards { get; } = new();
        public List<CharacterCard> GloballyAvailableLeaders { get; } = new();
        public List<CharacterCard> GloballyAvailableHeroes { get; } = new();
        
        public List<Deck> EvolvedReplacementDecks { get; private set; }
        public List<Deck> WinningDecks { get; private set; } = new();

        public int RoundCount { get; private set; }

        public bool IsTutorial { get; set; }

        protected override void Init()
        {
            Player = new PlayerModel();
            foreach (var card in CardCache.Instance.AllHeroCards)
            {
                if (card.UnlocksAt == 0)
                {
                    GloballyAvailableHeroes.Add(card);
                    MyHeroes.Add(card);
                    MyCards.Add(card);
                }
            }
            foreach (var card in CardCache.Instance.AllLeaderCards)
            {
                if (card.UnlocksAt == 0)
                {
                    GloballyAvailableLeaders.Add(card);
                    MyLeaders.Add(card);
                    MyCards.Add(card);
                }
            }
            
            AllEntrants.Add(Player);
            for (var i = 0; i < ConstantsData.Instance.npcCount; i += 1)
            {
                var npc = new NPCModel();
                NPCs.Add(npc);
                AllEntrants.Add(npc);
            }
        }

        public async Task<TourneyRoundResult> SimulateRound(Deck myDeck)
        {
            if (RoundCount > 0)
            {
                RunEvoAlgorithm();
            }
            Player.SetDeckForCurrentRound(myDeck);
            
            foreach (var entrant in AllEntrants)
            {
                entrant.SetupForNewRound();
            }

            var battle = new BattleModel();
            for (var i = 0; i < AllEntrants.Count; i++)
            {
                var entrant1 = AllEntrants[i];
                for (var j = i + 1; j < AllEntrants.Count; j++)
                {
                    var entrant2 = AllEntrants[j];
                    var p1 = new Party(entrant1.CurrentDeck);
                    var p2 = new Party(entrant2.CurrentDeck);
                    var winner = await battle.SimulateBattleAsync(p1, p2);
                    entrant1.CurrentRoundResult.Score(p1, winner, entrant2);
                    entrant2.CurrentRoundResult.Score(p2, winner, entrant1);
                }
            }

            foreach (var entrant in AllEntrants)
            {
                entrant.TallyResults();
            }

            var topEntrants = new List<EntrantModel>(AllEntrants);
            topEntrants.Sort((a, b) => b.CurrentRoundResult.Wins.CompareTo(a.CurrentRoundResult.Wins));
            WinningDecks = topEntrants.Take(5).Select(entrant => entrant.CurrentDeck).ToList();
            
            RoundCount++;
            return Player.CurrentRoundResult;
        }

        public List<CharacterCard> CheckForNewUnlocks()
        {
            var unlocked = new List<CharacterCard>();
            foreach (var card in CardCache.Instance.AllCards)
            {
                if (card.UnlocksAt <= Player.LifetimeWins
                    && card.UnlocksAt > Player.LifetimeWins - (Player.CurrentRoundResult?.Wins ?? 0))
                {
                    unlocked.Add(card);
                    if (card.IsLeader)
                    {
                        MyLeaders.Add(card);
                        MyCards.Add(card);
                        GloballyAvailableLeaders.Add(card);
                    }
                    else
                    {
                        MyHeroes.Add(card);
                        MyCards.Add(card);
                        GloballyAvailableHeroes.Add(card);
                    }
                }
            }

            return unlocked;
        }

        private void RunEvoAlgorithm()
        {
            var runner = new DeckEvoVsFixedSetRunner(AllEntrants.Select(entrant => entrant.CurrentDeck),
                GloballyAvailableLeaders, GloballyAvailableHeroes);
            var solutions = runner.RunEvolution(ConstantsData.Instance.evolutionSettings);
            EvolvedReplacementDecks = solutions.Select(sol => sol.Deck).ToList();
        }
    }
}