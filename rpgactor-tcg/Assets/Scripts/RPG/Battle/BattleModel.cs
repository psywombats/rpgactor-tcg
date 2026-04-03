using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class BattleModel
    {
        private Party Player1 { get; }
        private Party Player2 { get; }
        
        public string LivenessString =>
            $"{Player1.Leader.LivenessString} ({Player1.Mp}) vs ({Player2.Mp}) {Player2.Leader.LivenessString}\n" +
            $"__{Player1[LaneType.Left].LivenessChar}____{Player2[LaneType.Left].LivenessChar}__\n" +
            $"_{Player1[LaneType.Back].LivenessChar}{Player1[LaneType.Center].LivenessChar}____{Player2[LaneType.Center].LivenessChar}{Player2[LaneType.Back].LivenessChar}_\n" +
            $"__{Player1[LaneType.Right].LivenessChar}____{Player2[LaneType.Right].LivenessChar}__";

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

        public string Report { get; private set; }
        public bool UseVerboseLogging { get; init; }

        public int Turn { get; private set; }
        
        public bool IsOver => Player1.Leader.IsDead || Player2.Leader.IsDead;

        private readonly List<Unit> actedThisTurn = new();

        public Party SimulateBattle()
        {
            Reset();
            for (Turn = 1; Turn <= 10; Turn += 1)
            {
                actedThisTurn.Clear();
                if (UseVerboseLogging)
                {
                    Log($"Begin turn {Turn}\n{LivenessString}\n==========\n");
                }

                for (var actor = GetNextActor(); actor != null; actor = GetNextActor())
                {
                    SimulateActor(actor);
                    Player1.CheckPromotion();
                    Player2.CheckPromotion();
                    if (IsOver)
                    {
                        break;
                    }
                }
                if (IsOver)
                {
                    break;
                }
                
                if (UseVerboseLogging) Log("");
            }

            var winner = Player1.Leader.IsDead ? Player2 : Player1;
            if (UseVerboseLogging) Log($"\n\n{winner.ShortName} won!"); 
            return winner;
        }

        private void Reset()
        {
            Player1.Reset();
            Player2.Reset();
            Turn = 0;
        }

        private Unit GetNextActor()
        {
            Unit actor = null;
            foreach (var unit in Player1.Union(Player2))
            {
                if (!actedThisTurn.Contains(unit)
                    && (actor == null
                        || unit[Stat.SPD] > actor[Stat.SPD]
                        || (unit[Stat.SPD] == actor[Stat.SPD] && unit.Lane < actor.Lane)))
                {
                    actor = unit;
                }
            }
            return actor;
        }

        private void SimulateActor(Unit actor)
        {
            actedThisTurn.Add(actor);
            actor.SimulateTurn(this);
        }

        public void Log(string message)
        {
            LogPartial(message + '\n');
        }

        public void LogPartial(string partialMessage)
        {
            Report += partialMessage;
        }
        
        #endregion
    }
}