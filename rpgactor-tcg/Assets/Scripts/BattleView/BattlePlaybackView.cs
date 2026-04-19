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

        private BattleModel battle;
        private Dictionary<Unit, UnitView> viewForUnits = new();

        public void Populate(BattleModel newBattle)
        {
            battle = newBattle;
            player1View.Populate(newBattle.Player1);
            player2View.Populate(newBattle.Player2);
            RebuildViewCache();
        }

        public async Task PlayBattleAsync(Party player1, Party player2)
        {
            Populate(new BattleModel(player1, player2));
            await WriteLineAsync($"The battle begins: {player1.PrettyName} vs {player2.PrettyName}!", true);
            await battle.PlaybackBattleAsync(player1, player2, this);
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

        public Task GenerateMPAsync(Party party, Unit actor, int mp)
        {
            var partyView = party == player1View.Party ? player1View : player2View;
            var totalMP = party.MP + mp;
            return Task.WhenAll(WriteLineAsync($"{actor.PrettyName} generated {mp} MP (now {totalMP})."), 
                partyView.GenerateMPAsync(totalMP));
        }

        public async Task AnimateAttackAsync(Unit actor, Unit victim, int dmg)
        {
            var attackerView = viewForUnits[actor];
            var victimView = viewForUnits[victim];
            await Task.WhenAll(WriteLineAsync($"{actor.PrettyName} attacked {victim.PrettyName} for {dmg} damage.", !victim.IsDead),
                attackerView.AnimateAttackAsync(victimView, dmg));
        }
        
        public async Task AnimateSwapAsync(Tuple<Unit, Unit> promotion)
        {
            var unit1View = viewForUnits[promotion.Item1];
            var unit2View = viewForUnits[promotion.Item2];
            await Task.WhenAll(unit1View.SwapToUnitPosAsync(unit2View), unit2View.SwapToUnitPosAsync(unit1View));
            player1View.Repopulate();
            player2View.Repopulate();
            RebuildViewCache();
        }

        public void RepopulateUnit(Unit unit)
        {
            viewForUnits[unit].Repopulate();
        }

        public async Task EndBattleAsync(Party winner)
        {
            if (winner == null)
            {
                await WriteLineAsync("The parties are equivalent, so both lose.", true);
                return;
            }

            await WriteLineAsync($"The winner is {winner.PrettyName}!", true);
        }

        private void RebuildViewCache()
        {
            viewForUnits.Clear();
            foreach (var view in player1View.AllUnitViews)
            {
                viewForUnits.Add(view.Unit, view);
            }
            foreach (var view in player2View.AllUnitViews)
            {
                viewForUnits.Add(view.Unit, view);
            }
        }
    }
}