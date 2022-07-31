using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// The results of all energy gathering
    /// </summary>
    public class EnergyGatherResults
    {
        /// <summary>
        /// All the iterations of an energy gather.
        /// </summary>
        public List<EnergyGatherIteration> EnergyGatherIterations { get; } = new List<EnergyGatherIteration>();

        /// <summary>
        /// Original Skill Level
        /// </summary>
        public int SkillLevel { get; set; }
        /// <summary>
        /// Total Energy to gather.
        /// </summary>
        public int EnergyToGather { get; set; }

        private Lazy<int> _quirks;
        /// <summary>
        /// Number of quirks generated.
        /// </summary>
        public int Quirks { get => _quirks.Value; }

        private Lazy<bool> _failed;
        /// <summary>
        /// If this Energy gathering failed.
        /// </summary>
        public bool Failed { get => _failed.Value; }

        private Lazy<int> _energyGathered;
        /// <summary>
        /// Cumulative energy gathered.
        /// </summary>
        public int EnergyGathered { get => _energyGathered.Value; }

        /// <summary>
        /// Sets backer properties for values that might be expensive to repeatedly calculate.
        /// </summary>
        public void InitializeLazy()
        {
            _quirks = new Lazy<int>(() => EnergyGatherIterations.Count(e => e.Quirked));
            _failed = new Lazy<bool>(() => EnergyGatherIterations.Any(e => e.Failed));
            _energyGathered = new Lazy<int>(() => EnergyGatherIterations.Sum(e => e.AmountOfEnergyGathered));
            _timeSpent = new Lazy<TimeSpan>(() => EnergyGatherIterations.
                Select(e => e.Length).
                Aggregate((a, b) => a + b));
            _botchRitualEnergy = new Lazy<int>(() => Math.Max(30, EnergyGathered * 2));
        }

        private Lazy<TimeSpan> _timeSpent;
        /// <summary>
        /// Total time spent gathering.
        /// </summary>
        public TimeSpan TimeSpent 
        { 
            get => _timeSpent.Value; 
        }

        private Lazy<int> _botchRitualEnergy;
        /// <summary>
        /// The amount of energy to use for a botch energy gather.
        /// </summary>
        public int BotchRitualEnergy
        {
            get => _botchRitualEnergy.Value;
        }

        /// <summary>
        /// The normal result
        /// </summary>
        /// <returns>The result of the gather attempt</returns>
        public override string ToString() => $"{(Failed ? "Failed" : "Succeeded" )} after {TimeSpent}. Gathered {EnergyGathered} energy. {(Failed? $"Use {BotchRitualEnergy} for the botch ritual." : Quirks > 0 ? $"{Quirks} quirk(s)." : "")}";

        /// <summary>
        /// Gets the verbose version of this energy gathering.
        /// </summary>
        /// <returns></returns>
        public string VerboseOutput()
        {
            var sb = new StringBuilder();
            foreach(var iteration in EnergyGatherIterations)
            {
                var timeElapsed = EnergyGatherIterations.
                    Where(e => e.Number <= iteration.Number).
                    Select(e => e.Length).
                    Aggregate((a, b) => a + b);
                var energyGathered = EnergyGatherIterations.
                    Where(e => e.Number <= iteration.Number).
                    Sum(e => e.AmountOfEnergyGathered);
                var quirks = EnergyGatherIterations.
                    Where(e => e.Number <= iteration.Number).
                    Count(e => e.Quirked);
                if (iteration.Failed)
                {
                    sb.AppendLine($"Critical failure after {timeElapsed}. Gathered {energyGathered}.");
                }
                var consecutiveGathers = iteration.Number;
                sb.AppendLine($"[{consecutiveGathers:00}] {timeElapsed} - {energyGathered}/{EnergyToGather}.{(quirks > 0 ? $" Total Quirks: {quirks}." : string.Empty)}");
                if(energyGathered >= EnergyToGather)
                {
                    sb.AppendLine("The ritual is ready.");
                    break;
                }
                var numberOfSkillDrops =
                    EnergyGatherIterations.
                    Where(e => e.Number <= iteration.Number).
                    Count(e => e.SkillDropped);
                var lowerSkill = iteration.SkillDropped;
                if (lowerSkill)
                {
                    sb.AppendLine($"Skill dropped to {SkillLevel - numberOfSkillDrops}.");
                }

            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// The result of one iteration of energy gathering.
    /// </summary>
    public class EnergyGatherIteration
    {
        /// <summary>
        /// Order of this iteration.
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Length of this particular iteration.
        /// </summary>
        public TimeSpan Length { get; set; }
        /// <summary>
        /// Amount of energy gathered on this specific iteration.
        /// </summary>
        public int AmountOfEnergyGathered { get; set; }
        /// <summary>
        /// Whether this iteration introduced a quirk.
        /// </summary>
        public bool Quirked { get; set; }
        /// <summary>
        /// Whether this iteration destroyed the ritual.
        /// </summary>
        public bool Failed { get; set; }
        /// <summary>
        /// Whether this iteration caused the skill to drop.
        /// </summary>
        public bool SkillDropped { get; set; }
        public SuccessRollResult Roll { get; set; }
    }
}
