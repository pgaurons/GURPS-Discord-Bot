using Gao.Gurps.Dice;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Workflow
{
    /// <summary>
    /// Parses the argument for the energy gathering command.
    /// </summary>
   public static class EnergyGather
    {

        const int NumberOfGathersBeforePenalty = 3;
        private static readonly TimeSpan NormalTimeSpan = new TimeSpan(0, 5, 0);
        private static readonly TimeSpan OneSecond = new TimeSpan(0, 0, 1);
        
        public static EnergyGatheringParameters Parse(string argument)
        {
            //The big thing I am looking for is a skill level, an energy amount, and optionally a time span.
            var regex = new Regex(@"[^0-9]*(\d+)[^0-9]+(\d+)\s*" + TextUtility.TimeSpanFormat + @"?\s*(VERBOSE)?", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(argument))
            {
                throw new ArgumentException("Argument formatted incorrectly");
            }
            var groups = regex.Match(argument).Groups;

            var skillLevel = int.Parse(groups[1].ToString());
            var energyToGather = int.Parse(groups[2].ToString());

            var timePeriod = groups[3].ToString();

            TimeSpan timeSpan = TextUtility.ParseTimeSpanText(timePeriod, NormalTimeSpan, true);

            var verbose = groups[4].Value.ToUpperInvariant() == "VERBOSE";

            return new EnergyGatheringParameters
            {
                EnergyToGather = energyToGather,
                SkillLevel = skillLevel,
                NormalTimeToGatherEnergy = timeSpan,
                Verbose = verbose

            };
        }

        public static EnergyGatherResults Execute(EnergyGatheringParameters parameters)
        {
            var energyGathered = 0;
            var timeElapsed = parameters.NormalTimeToGatherEnergy;
            var currentSkill = parameters.SkillLevel;
            var consecutiveGathers = 0;
            var quirks = 0;
            var returnValue = new EnergyGatherResults { EnergyToGather = parameters.EnergyToGather, SkillLevel = parameters.SkillLevel };
            while(energyGathered < parameters.EnergyToGather)
            {
                var newIteration = new EnergyGatherIteration();
                consecutiveGathers++;
                newIteration.Number = consecutiveGathers;

                //Roll to get the margin.
                var gatherRoll = Roller.RollAgainst(currentSkill);
                newIteration.Roll = gatherRoll;

                var iterationTime = consecutiveGathers != 1 ? 
                        returnValue.EnergyGatherIterations[consecutiveGathers - 2].Roll.Critical ?
                            new TimeSpan(Math.Min(OneSecond.Ticks, parameters.NormalTimeToGatherEnergy.Ticks)) :
                            parameters.NormalTimeToGatherEnergy :
                        parameters.NormalTimeToGatherEnergy;
                timeElapsed += iterationTime;
                newIteration.Length = iterationTime;

                //The gig is up on a critical failure.
                if (gatherRoll.Critical && !gatherRoll.Success)
                {
                    newIteration.Failed = true;
                    returnValue.EnergyGatherIterations.Add(newIteration);
                    break;
                }
                
                var energyGatheredByRoll = Math.Max(1, gatherRoll.Margin);
                newIteration.AmountOfEnergyGathered = energyGatheredByRoll;
                if (!gatherRoll.Success)
                {
                    quirks++;
                    newIteration.Quirked = true;
                }
                
                energyGathered += energyGatheredByRoll;

                if(energyGathered >= parameters.EnergyToGather)
                {
                    
                    returnValue.EnergyGatherIterations.Add(newIteration);
                    break;
                }
                //Skill drops every three gathers.
                var lowerSkill = consecutiveGathers % NumberOfGathersBeforePenalty == 0;
                if (lowerSkill)
                {
                    currentSkill--;
                    newIteration.SkillDropped = true;
                }
                returnValue.EnergyGatherIterations.Add(newIteration);

            }
            returnValue.InitializeLazy();
            return returnValue;
        }
    }
}
