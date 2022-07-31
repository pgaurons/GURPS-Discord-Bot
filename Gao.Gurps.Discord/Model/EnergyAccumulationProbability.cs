using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Model
{
    public class EnergyAccumulationProbability
    {
        public decimal OddsOfSuccess { get; set; }
        public decimal OddsOfSuccessWithoutQuirks { get; internal set; }
        public TimeSpan? AverageSuccessTime { get; internal set; }
        public double? AverageBotchEnergyLevel { get; internal set; }
        public double? AverageQuirkCount { get; internal set; }
        public TimeSpan? AverageFailureTime { get; internal set; }
        /// <summary>
        /// One standard deviation of success time. (σ)
        /// </summary>
        public TimeSpan? SuccessTimeSigma { get; internal set; }
        public double? QuirkSigma { get; internal set; }
        public double? BotchSigma { get; internal set; }
        public TimeSpan? FailureTimeSigma { get; internal set; }
    }
}
