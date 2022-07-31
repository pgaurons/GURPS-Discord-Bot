using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Represents an alternate timeline a la Infinite Worlds.
    /// </summary>
    public class InfiniteWorld
    {
        public int Quantum { get; internal set; }
        public RandomWorldType WorldType { get; internal set; }
        public EmptyWorldType EmptyWorldType { get; internal set; }
        public int EchoWorldPresent { get; internal set; }
        public ParallelWorldType ParallelWorldType { get; internal set; }
        public CloseParallelWorldType CloseParallelWorldType { get; internal set; }
        public int TechLevel { get; internal set; }
        public TechnologyVariant TechnologyVariant { get; internal set; }
        public TechnologyVariantOpenness TechnologyVariantOpenness { get; internal set; }
        public ManaLevel ManaLevel { get; internal set; }
        public int TechLevelDivergencePoint { get; internal set; }
        public Dictionary<TechnologyCategory, int> SplitTechLevels { get; internal set; } = new Dictionary<TechnologyCategory, int>();
        public bool HasAirships { get; internal set; }
        public SuperscienceTechnology SuperscienceTechnology { get; internal set; }
        public bool TechnologyFromAncientAstronauts { get; internal set; }
        public InfiniteWorldCivilization[] MajorCivilizations { get; internal set; } = new InfiniteWorldCivilization[0];
        public DisasterWorldType DisasterType { get; internal set; }
    }
}
