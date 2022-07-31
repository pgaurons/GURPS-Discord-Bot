using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Model
{
    public class SuccessRollProbability
    {
        public decimal CriticalFailureOdds { get; internal set; }
        public decimal CriticalSuccessOdds { get; internal set; }
        public decimal FailureOdds { get; internal set; }
        public decimal SuccessOdds { get; internal set; }
    }
}
