using System.Collections.Generic;

namespace RpgActorTGC
{
    public class BattleModel
    {
        private Party Player1 { get; }
        private Party Player2 { get; }
        
        public BattleModel(Party player1, Party player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public Unit GetOppositeUnit(Unit unit)
        {
            if (unit.Lane == LaneType.Back)
            {
                return null;
            }
            var oppositeParty = unit.Party == Player1 ? Player2 : Player1;
            var opposite = oppositeParty[unit.Lane];
            if (opposite != null && !opposite.IsDead)
            {
                return opposite;
            }

            opposite = oppositeParty[LaneType.Center];
            if (opposite != null)
            {
                return opposite;
            }
            
            return oppositeParty[LaneType.Back];
        }
        
        #region Battle sim
        
        public int Turn { get; private set; }
        
        public bool IsOver => Player1.Leader.IsDead || Player2.Leader.IsDead;

        private readonly List<Unit> actedThisTurn = new();

        public Party SimulateBattle()
        {
            for (Turn = 1; Turn <= 10; Turn += 1)
            {
                SimulateNextActor();
                if (IsOver)
                {
                    break;
                }
                Player1.CheckPromotion();
                Player2.CheckPromotion();
            }

            return Player1;
        }

        private void SimulateNextActor()
        {
            actedThisTurn.Clear();
            Unit actor = null;
            foreach (var unit in Player1)
            {
                if (!actedThisTurn.Contains(unit)
                    && (actor == null
                        || unit[Stat.SPD] > actor[Stat.SPD]
                        || (unit[Stat.SPD] == actor[Stat.SPD] && unit.Lane < actor.Lane)))
                {
                    actor = unit;
                }
            }

            if (actor == null)
            {
                UnityEngine.Debug.LogError("No units have yet to move");
                return;
            }
            
            actedThisTurn.Add(actor);
            actor.SimulateTurn(this);
        }
        
        #endregion
    }
}