using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public class RPGActorManager : SingletonBehaviour<RPGActorManager>
    {
        private const string ActorsPrefKey = "RPGActorManager_Actors";
        private const string CardsPrefKey = "RPGActorManager_Cards";

        public InitState State { get; private set; } = InitState.Startup;
        public string DebugMessage { get; private set; }

        public event Action<InitState, string> OnStateChange;
        
        private RPGActorFullCachedData fullCachedData;
        private readonly List<RPGActorModel> allActors = new();
        private readonly List<(RPGActorModel actor, CharacterCard card)> cardAssignments = new();
        private int loadedFromCacheCount;

        public async Task StartupAsync()
        {
            HttpResponseMessage initialFetchResponse;
            try
            {
                SetState(InitState.FetchingData, "Fetching initial rpg.actor data...");
                initialFetchResponse = await HTTPManager.Instance.Client.GetAsync("https://rpg.actor/api/actors/full");
                if (!initialFetchResponse.IsSuccessStatusCode)
                {
                    SetState(InitState.Error, $"Error fetching initial data: {initialFetchResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                SetState(InitState.Error, $"Error fetching initial data: {ex.Message}");
                Debug.LogException(ex);
                return;
            }

            try
            {
                SetState(InitState.ParsingResponse, "Parsing rpg.actor info...");
                await Task.Run(async () =>
                {
                    var json = await initialFetchResponse.Content.ReadAsStringAsync();
                    fullCachedData = Newtonsoft.Json.JsonConvert.DeserializeObject<RPGActorFullCachedData>(json);
                });
            }
            catch (Exception ex)
            {
                SetState(InitState.Error, $"Error parsing rpg.actor full data: {ex.Message}");
                Debug.LogException(ex);
                return;
            }

            try
            {
                allActors.Clear();
                allActors.AddRange(fullCachedData.actors.Where(actor => actor.IsValidForTCG())
                    .Select(actor => new RPGActorModel(actor)).ToList());
                var toAssign = new HashSet<CharacterCard>(CardCache.Instance.AllCards);
                ReadAssignmentsFromPlayerPrefs(toAssign);
                await Task.Run(() => AssignFittingCardsAsync(toAssign));
                if (toAssign.Any())
                {
                    SetState(InitState.Error, $"Could not find actors for {toAssign.Count} cards");
                    return;
                }
                
                WriteAssignmentsToPlayerPrefs();
            }
            catch (Exception ex)
            {
                SetState(InitState.Error, $"Error assigning cards: {ex.Message}");
                Debug.LogException(ex);
                return;
            }
            
            SetState(InitState.Ready, $"Ready! Character count: {cardAssignments.Count}, {loadedFromCacheCount} from cache");
        }

        public void ResetAssignments()
        {
            loadedFromCacheCount = 0;
            cardAssignments.Clear();
            CardCache.Instance.ResetAssignments();
            PlayerPrefs.DeleteKey(ActorsPrefKey);
            PlayerPrefs.DeleteKey(CardsPrefKey);
            foreach (var actor in allActors)
            {
                actor.Card = null;
            }
        }

        private void SetState(InitState state, string debugMessage)
        {
            State = state;
            DebugMessage = debugMessage;
            OnStateChange?.Invoke(state, debugMessage);
        }

        private void AssignFittingCardsAsync(HashSet<CharacterCard> toAssign)
        {
            var unassignedActors = allActors.Where(actor => actor.Card == null).ToList();
            foreach (var card in toAssign)
            {
                card.CalculateScores(unassignedActors);
            }
            
            // greedy algorithm assigns the best matches first
            var topScorers = new List<(RPGActorModel actor, CharacterCard card)>();
            var assignedThisRound = 0;
            do
            {
                // gather all the candidate assignments with same top score
                var topScore = int.MaxValue;
                foreach (var candidate in toAssign)
                {
                    var bestMatch = candidate.GetHighestUnasignedScore();
                    if (bestMatch.score < topScore)
                    {
                        topScore = bestMatch.score;
                        topScorers.Clear();
                    }

                    if (bestMatch.score == topScore)
                    {
                        topScorers.Add((bestMatch.actor, candidate));
                    }
                }

                // assign as many of the top scorers as possible
                foreach (var candidate in topScorers)
                {
                    if (candidate.actor.Card == null)
                    {
                        candidate.card.Actor = candidate.actor;
                        candidate.actor.Card = candidate.card;
                        cardAssignments.Add((candidate.actor, candidate.card));
                        assignedThisRound += 1;
                        toAssign.Remove(candidate.card);
                    }
                }
            } while (toAssign.Any() && assignedThisRound > 0);
        }

        private void WriteAssignmentsToPlayerPrefs()
        {
            var ordereredIdsString = string.Join(',', cardAssignments.Select(assign => assign.actor.DID));
            var cardKeysString = string.Join(',', cardAssignments.Select(assign => assign.card.Data.Key));
            PlayerPrefs.SetString(ActorsPrefKey, ordereredIdsString);
            PlayerPrefs.SetString(CardsPrefKey, cardKeysString);
        }

        private void ReadAssignmentsFromPlayerPrefs(HashSet<CharacterCard> toAssign)
        {
            if (!PlayerPrefs.HasKey(ActorsPrefKey) || !PlayerPrefs.HasKey(CardsPrefKey))
            {
                return;
            }
            var orderedIdsString = PlayerPrefs.GetString(ActorsPrefKey);
            var orderedCardsString = PlayerPrefs.GetString(CardsPrefKey);
            var dids = orderedIdsString.Split(',');
            var cards = orderedCardsString.Split(',');
            
            var actorsByDID = allActors.ToDictionary(actor => actor.DID, actor => actor);
            
            for (var i = 0; i < dids.Length; i++)
            {
                var cardKey = cards[i];
                if (!CardCache.Instance.TryGetCharacter(cardKey, out var card))
                {
                    // card no longer exists in this version
                    continue;
                }
                
                var did = dids[i];
                if (!actorsByDID.TryGetValue(did, out var actor))
                {
                    // actors can be deleted remotely, invalidating the assignment
                    continue;
                }

                toAssign.Remove(card);
                cardAssignments.Add((actor, card));
                actor.Card = card;
                card.Actor = actor;
            }

            loadedFromCacheCount = cardAssignments.Count;
        }
    }
}