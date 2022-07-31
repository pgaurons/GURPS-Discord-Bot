using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Model
{
    public class RegularContestOdds
    {
        public int MySkill { get; set; }
        public int TheirSkill { get; set; }
        public decimal MyOddsOfWinningAnIteration { get; set; }
        public decimal TheirOddsOfWinningAnIteration { get; set; }
        public decimal OddsOfTyingAnIteration { get; set; }
        public decimal MyOddsOfWinningTheContest { get; set; }
        public decimal TheirOddsOfWinningTheContest { get; set; }

        public int LikelyIterationsToCompletion { get; set; }

        public decimal OddsOfCompletingByLikelyIterations { get; set; }
    }
}
