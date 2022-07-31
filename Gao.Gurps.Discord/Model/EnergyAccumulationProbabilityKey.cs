using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Model
{
    public class EnergyAccumulationProbabilityKey
    {
        public int SkillLevel { get; set; }
        public int EnergyToGather { get; set; }
        public int TurnCycle { get; set; }
        public override bool Equals(object obj)
        {
            bool returnValue;
            if (obj == null || obj.GetType() != typeof(EnergyAccumulationProbabilityKey))
            {
                returnValue = base.Equals(obj);
            }
            else
            {
                var other = obj as EnergyAccumulationProbabilityKey;
                returnValue =
                    SkillLevel == other.SkillLevel &&
                    EnergyToGather == other.EnergyToGather &&
                    TurnCycle == other.TurnCycle;
            }
            return returnValue;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(SkillLevel, EnergyToGather, TurnCycle).GetHashCode();
        }

        
    }
}
