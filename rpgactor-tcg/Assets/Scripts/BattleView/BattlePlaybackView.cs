using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace RpgActorTGC
{
    public class BattlePlaybackView : MonoBehaviour
    {
        [SerializeField] private PartyView player1View;
        [SerializeField] private PartyView player2View;
        [Space]
        [SerializeField] private BattleBox battlebox;
        [SerializeField] private TMP_Text turnCounter;
        [SerializeField] private TMP_Text roundCounter;
        [Space]
        [SerializeField] private TutorialDialogSpawner endTutorial;

        private BattleModel battle;
        public Dictionary<Unit, UnitView> ViewForUnit { get; } = new();

        public void Populate(BattleModel newBattle)
        {
            battlebox.Clear();
            battle = newBattle;
            player1View.Populate(newBattle.Player1);
            player2View.Populate(newBattle.Player2);
            RebuildViewCache();
            
            roundCounter.gameObject.SetActive(!newBattle.IsPractice);
            roundCounter.text = $"Round {CampaignManager.Instance.RoundCount + 1}";
            turnCounter.text = "Turn 1";
        }

        public async Task PlayBattleAsync()
        {
            await WriteLineAsync($"The battle begins: {battle.Player1.PrettyName} vs {battle.Player2.PrettyName}!", true);
            await battle.PlaybackBattleAsync(this);
        }

        public async Task WriteLineAsync(string line, bool requiresConfirm = false)
        {
            await battlebox.WriteLineRoutine(line);
            if (requiresConfirm)
            {
                await InputManager.Instance.ConfirmAsync();
            }
        }

        public async Task NextTurnAsync()
        {
            turnCounter.text = "Turn " + battle.Turn;
            await WriteLineAsync($"Turn {battle.Turn} begins!");
            await WriteLineAsync("");
        }

        public Task GenerateMPAsync(Party party, Unit actor, int mp, bool silent = false)
        {
            var partyView = party == player1View.Party ? player1View : player2View;
            if (silent)
            {
                return partyView.GenerateMPAsync(party.MP);
            }
            else
            {
                return Task.WhenAll(WriteLineAsync($"{actor.PrettyName} generated {mp} MP (now {party.MP})."), 
                    partyView.GenerateMPAsync(party.MP));
            }
        }

        public async Task AnimateAttackAsync(Unit actor, Unit victim, int dmg)
        {
            var attackerView = ViewForUnit[actor];
            var victimView = ViewForUnit[victim];
            await Task.WhenAll(WriteLineAsync($"{actor.PrettyName} attacked {victim.PrettyName} for {dmg} damage.", !victim.IsDead),
                attackerView.AnimateAttackAsync(victimView, dmg));
        }
        
        public async Task AnimateSwapAsync(Tuple<Unit, Unit> promotion)
        {
            var unit1View = ViewForUnit[promotion.Item1];
            var unit2View = ViewForUnit[promotion.Item2];
            await Task.WhenAll(unit1View.SwapToUnitPosAsync(unit2View), unit2View.SwapToUnitPosAsync(unit1View));
            player1View.Repopulate();
            player2View.Repopulate();
            RebuildViewCache();
        }

        public void RepopulateUnit(Unit unit)
        {
            ViewForUnit[unit].Repopulate();
        }

        public async Task EndBattleAsync(Party winner)
        {
            endTutorial.TrySpawn();
            if (winner == null)
            {
                await WriteLineAsync("The parties are equivalent, so both lose.", true);
                return;
            }

            await WriteLineAsync($"The winner is {winner.PrettyName}!", true);
        }

        private void RebuildViewCache()
        {
            ViewForUnit.Clear();
            foreach (var view in player1View.AllUnitViews)
            {
                ViewForUnit.Add(view.Unit, view);
            }
            foreach (var view in player2View.AllUnitViews)
            {
                ViewForUnit.Add(view.Unit, view);
            }
        }
    }
}