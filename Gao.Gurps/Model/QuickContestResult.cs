using System;

namespace Gao.Gurps.Model
{
    public class QuickContestResult
    {
        public SuccessRollResult FirstContestantResult { get; set; }
        public SuccessRollResult SecondContestantResult { get; set; }
        public int MarginOfVictory { get { return Math.Abs((FirstContestantResult?.Margin ?? 0) - (SecondContestantResult?.Margin ?? 0)); } }

        public bool FirstContestantIsWinner { get; set; }
        public bool SecondContestantIsWinner { get; set;}
        public bool IsTie { get; set; }


    }
}
