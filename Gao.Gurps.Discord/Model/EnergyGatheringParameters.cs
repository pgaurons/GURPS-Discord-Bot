using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Model
{
    public class EnergyGatheringParameters
    {
        public int SkillLevel { get; set; }
        public int EnergyToGather { get; set; }
        public TimeSpan NormalTimeToGatherEnergy { get; set; }
        public bool Verbose { get; internal set; }
    }
}
