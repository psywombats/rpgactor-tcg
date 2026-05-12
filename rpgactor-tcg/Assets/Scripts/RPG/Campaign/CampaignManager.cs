using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapElitesTGC;

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

        public int RoundCount { get; private set; }

        public bool IsTutorial { get; set; }
        
        private HashSet<string> availableNPCPrefixes;
        private HashSet<string> availableNPCSuffixes;

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
            var constants = ConstantsData.Instance;
            for (var i = 0; i < constants.frontrunnerCount; i += 1) CreateNPC(NPCModel.BehaviorType.Frontrunner);
            for (var i = 0; i < constants.contentCount; i += 1)     CreateNPC(NPCModel.BehaviorType.Content);
            for (var i = 0; i < constants.copycatCount; i += 1)     CreateNPC(NPCModel.BehaviorType.Copycat);
            for (var i = 0; i < constants.scientistCount; i += 1)   CreateNPC(NPCModel.BehaviorType.Scientist);
            for (var i = 0; i < constants.chaffCount; i += 1)       CreateNPC(NPCModel.BehaviorType.Chaff);
        }

        private void CreateNPC(NPCModel.BehaviorType behaviorType)
        {
            var npc = new NPCModel(behaviorType);
            NPCs.Add(npc);
            AllEntrants.Add(npc);
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

            for (var i = 0; i < topEntrants.Count; i++)
            {
                topEntrants[i].CurrentRoundResult.Rank = i;
            }
            
            winningDecks = topEntrants.Take(ConstantsData.Instance.winningDeckCount)
                .Select(entrant => entrant.CurrentDeck).ToList();
            uniqueWinningDecks = new List<Deck>(winningDecks);
            
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
            var runner = new MapEliteRunner(ConstantsData.Instance.evolutionSettings,
                AllEntrants.Select(entrant => entrant.CurrentDeck).ToList(),
                GloballyAvailableHeroes,
                GloballyAvailableLeaders);
            var solutions = runner.RunAsync().Result;
            evolvedDecks = solutions.Select(sol => sol.Deck).ToList();
            uniqueEvolvedDecks = new List<Deck>(evolvedDecks);
        }

        public string GeneratePlayerName()
        {
            if (availableNPCPrefixes == null)
            {
                availableNPCPrefixes = new HashSet<string>(ConstantsData.Instance.oppoNames.prefixes);
                availableNPCSuffixes = new HashSet<string>(ConstantsData.Instance.oppoNames.suffixes);
            }

            var prefix = availableNPCPrefixes.Choose();
            var suffix = availableNPCSuffixes.Choose();
            availableNPCPrefixes.Remove(prefix);
            availableNPCSuffixes.Remove(suffix);
            return prefix + " " + suffix;
        }
        
        #region Claim next deck
        
        // claimables
        private List<Deck> evolvedDecks;
        private List<Deck> uniqueEvolvedDecks;
        private List<Deck> winningDecks;
        private List<Deck> uniqueWinningDecks;
        
        public Deck ClaimEvolvedDeck() => ClaimNextDeck(evolvedDecks, uniqueEvolvedDecks);
        public Deck ClaimUniqueEvolvedDeck() => ClaimNextUniqueDeck(evolvedDecks, uniqueEvolvedDecks);
        public Deck ClaimWinningDeck() => ClaimNextDeck(winningDecks, uniqueWinningDecks);
        public Deck ClaimUniqueWinningDeck() => ClaimNextUniqueDeck(winningDecks, uniqueWinningDecks);

        private Deck ClaimNextDeck(List<Deck> decks, List<Deck> uniqueDecks)
        {
            var result = decks.FirstOrDefault();
            return ClaimDeck(decks, uniqueDecks, result);
        }

        private Deck ClaimNextUniqueDeck(List<Deck> decks, List<Deck> uniqueDecks)
        {
            var result = uniqueDecks.FirstOrDefault();
            return ClaimDeck(decks, uniqueDecks, result);
        }

        private Deck ClaimDeck(List<Deck> decks, List<Deck> uniqueDecks, Deck deck)
        {
            if (deck == null) return null;
            decks.Remove(deck);
            RemoveDeckAndVariants(uniqueDecks, deck);
            return new Deck(deck);
        }
        
        private void RemoveDeckAndVariants(List<Deck> decks, Deck toRemove)
        {
            if (toRemove == null) return;

            // treat subsequent similar decks as variants and ignore for the sake of variety
            foreach (var deck in decks.ToList().Where(deck => deck.CalculateDistance(toRemove) <= 1))
            {
                decks.Remove(deck);
            }
        }
        
        public IEnumerable<Deck> GetTopNPCDecks(int count)
        {
            return NPCs.OrderBy(a => -1 * a.CurrentRoundResult.Wins).Take(count).Select(npc => npc.CurrentDeck);
        }
        
        #endregion
    }
}