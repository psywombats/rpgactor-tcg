using System;
using System.Collections.Generic;

namespace RpgActorTGC
{
    public class BattleModel
    {
        private Party Player1 { get; set; }
        private Party Player2 { get; set; }

        private readonly List<Unit> allUnits = new();
        
        public string LivenessString =>
            $"{Player1.Leader.LivenessString} ({Player1.Mp}) vs ({Player2.Mp}) {Player2.Leader.LivenessString}\n" +
            $"__{Player1[LaneType.Left].LivenessChar}____{Player2[LaneType.Left].LivenessChar}__\n" +
            $"_{Player1[LaneType.Back].LivenessChar}{Player1[LaneType.Center].LivenessChar}____{Player2[LaneType.Center].LivenessChar}{Player2[LaneType.Back].LivenessChar}_\n" +
            $"__{Player1[LaneType.Right].LivenessChar}____{Player2[LaneType.Right].LivenessChar}__";

        public BattleModel() {}

        public BattleModel(Party player1, Party player2) : this()
        {
            InitForParties(player1, player2);
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

        private readonly List<Unit> turnOrder = new();
        private readonly HashSet<Unit> actedThisTurn = new();

        private void InitForParties(Party player1, Party player2)
        {
            Turn = 0;
            
            Player1 = player1;
            Player2 = player2;
            
            Player1.Color = "pink";
            Player2.Color = "red";

            allUnits.Clear();
            foreach (var unit in Player1)
            {
                allUnits.Add(unit);
            }
            foreach (var unit in Player2)
            {
                allUnits.Add(unit);
            }
        }

        public Party SimulateBattle() => SimulateBattle(Player1, Player2);
        
        public Party SimulateBattle(Party player1, Party player2)
        {
            InitForParties(player1, player2);
            RecalculateTurnOrder();
            for (Turn = 1; Turn <= 10; Turn += 1)
            {
                actedThisTurn.Clear();
                if (UseVerboseLogging) Log($"Begin turn {Turn}\n{LivenessString}\n==========\n");

                while (actedThisTurn.Count < allUnits.Count)
                {
                    var actor = GetNextActor();
                    SimulateActor(actor);
                    
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

            var winner = Player1.Leader.IsDead ? Player2
                : Player2.Leader.IsDead ? Player1
                : Player1.Leader.Hp < Player2.Leader.Hp ? Player1 : Player2;
            if (UseVerboseLogging) Log($"\n\n<color={winner.Color}>{winner.ShortName}</color> won!"); 
            return winner;
        }

        private Unit GetNextActor()
        {
            foreach (var unit in turnOrder)
            {
                if (!actedThisTurn.Contains(unit))
                {
                    return unit;
                }
            }
            throw new ArgumentException("No actors have yet to act");
        }

        private void SimulateActor(Unit actor)
        {
            actedThisTurn.Add(actor);
            if (!actor.IsDead)
            {
                actor.SimulateTurn(this);
            }
        }

        public void InvalidateTurnOrder()
        {
            RecalculateTurnOrder();
        }

        private void RecalculateTurnOrder()
        {
            turnOrder.Clear();
            turnOrder.AddRange(allUnits);
            turnOrder.Sort(SpeedComparator);
        }

        private static int SpeedComparator(Unit p1, Unit p2) => (int)(p1[Stat.SPD] - p2[Stat.SPD]);

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