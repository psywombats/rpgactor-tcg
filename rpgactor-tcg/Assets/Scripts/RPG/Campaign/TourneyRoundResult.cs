using System.Collections.Generic;

namespace RpgActorTGC
{
    public class TourneyRoundResult
    {
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Rank { get; set; }

        public Dictionary<EntrantModel, bool> ResultsByOpponent { get; } = new();

        public void Score(Party myParty, Party winner, EntrantModel opponent)
        {
            if (myParty == winner) Wins += 1;
            if (myParty != winner) Losses += 1;
            ResultsByOpponent.Add(opponent, myParty == winner);
        }
    }
}