using Gao.Gurps.Dice;
using Gao.Gurps.Discord.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Gao.Gurps.Discord.Workflow
{
    public static class Probability
    {
        /// <summary>
        /// The odds of getting any one specific result on 3d6
        /// </summary>
        public static ReadOnlyDictionary<int, int> RollDistribution { get; } = new ReadOnlyDictionary<int, int>
            (
                new Dictionary<int, int>
                {
                    {3,1},
                    {4,3},
                    {5,6},
                    {6,10},
                    {7,15},
                    {8,21},
                    {9,25},
                    {10,27},
                    {11,27},
                    {12,25},
                    {13,21},
                    {14,15},
                    {15,10},
                    {16,6},
                    {17,3},
                    {18,1}
                }
            );



        /// <summary>
        /// The number of possible results.
        /// </summary>
        public const int PossibleOutcomes = 216;

        public static SuccessRollProbability CalculateOddsForSuccessRoll(int skillLevel)
        {
            return CalculateOddsForSuccessRoll(skillLevel, false);
        }

        public static SuccessRollProbability CalculateOddsForSuccessRoll(int skillLevel, bool resist)
        {
            var criticalSuccessOdds = GetProbabilityForRange(Roller.CriticalSuccesses(skillLevel, resist));
            var successOdds = GetProbabilityForRange(Roller.Successes(skillLevel));
            var failureOdds = GetProbabilityForRange(Roller.Failures(skillLevel, resist));
            var criticalFailureOdds = GetProbabilityForRange(Roller.CriticalFailures(skillLevel, resist));

            return new SuccessRollProbability
            {
                CriticalSuccessOdds = criticalSuccessOdds,
                SuccessOdds = successOdds,
                FailureOdds = failureOdds,
                CriticalFailureOdds = criticalFailureOdds
            };
        }

        private static decimal GetProbabilityForRange(IEnumerable<int> values)
        {
            return (decimal)RollDistribution.Join(values, rd => rd.Key, r => r, (rd, r) => rd.Value).Sum() / PossibleOutcomes;
        }

        public static Dictionary<EnergyAccumulationProbabilityKey, EnergyAccumulationProbability> CalculatedProbabilitiesForRitualSuccess { get; } = 
            new Dictionary<EnergyAccumulationProbabilityKey, EnergyAccumulationProbability>();

        public static Dictionary<EnergyAccumulationProbabilityKey, EnergyAccumulationProbability> CalculatedProbabilitiesForRitualNoQuirks { get; } =
            new Dictionary<EnergyAccumulationProbabilityKey, EnergyAccumulationProbability>();


        public static EnergyAccumulationProbability CalculateOddsOfRitualSuccess(int skillLevel, int energyToGather)
        {
            return CalculateEnergyGatheringOdds(skillLevel, energyToGather, Roller.CriticalFailures, CalculatedProbabilitiesForRitualSuccess);
        }

        public static EnergyAccumulationProbability CalculateOddsOfRitualSuccessWithNoQuirks(int skillLevel, int energyToGather)
        {
            return CalculateEnergyGatheringOdds(skillLevel, energyToGather, (i) => Roller.Failures(i).Union(Roller.CriticalFailures(i)), CalculatedProbabilitiesForRitualNoQuirks);
        }


        public static decimal CalculateOddsOfSuccessRoll(int skillLevel)
        {
            var effectiveMax = Math.Max(Math.Min(skillLevel, 16), 4);
            return RollDistribution.Where(r => r.Key <= effectiveMax).Select(r => r.Value).Sum() / ((decimal)PossibleOutcomes);
        }

        public static decimal CalculateOddsOfQuickContest(int mySkill, int theirSkill)
        {
            var runningScore = 0;
            foreach (var myRoll in Enumerable.Range(3, 16).Select(r => Roller.RollAgainst(mySkill, r)))
            {
                foreach(var theirRoll in Enumerable.Range(3,16).
                    Select(r => Roller.RollAgainst(theirSkill, r)).
                    Where(r => r.Margin < myRoll.Margin))
                {
                    runningScore += RollDistribution[myRoll.RollResult.Sum()] * RollDistribution[theirRoll.RollResult.Sum()];
                }
            }

            return runningScore / ((decimal)PossibleOutcomes * PossibleOutcomes);
        }

        public static RegularContestOdds CalculateOddsOfRegularContest(int mySkill, int theirSkill)
        {
            //First off, skills may need to be normalized if they are too high or too low.
            Contest.NormalizeRegularContestSkillLevels(ref mySkill, ref theirSkill);
            //Odds of me winning is odds that I succeed and they fail; likewise for them. It is a tie in all other cases.
            var myOdds = CalculateOddsOfSuccessRoll(mySkill) * (1-CalculateOddsOfSuccessRoll(theirSkill));
            var theirOdds = CalculateOddsOfSuccessRoll(theirSkill) * (1 - CalculateOddsOfSuccessRoll(mySkill));
            var oddsOfAnyWinning = myOdds + theirOdds;

            //Everything else is a tie.
            var tieOdds = 1 - oddsOfAnyWinning;

            //This is the odds of winning regardless of how many iterations end in a tie.
            var myTotalOdds = myOdds / oddsOfAnyWinning;
            var theirTotalOdds = theirOdds / oddsOfAnyWinning;

            //Finally the likely iterations to victory to achieve two sigma.
            const decimal twoSigma = 0.9545m;

            var likelyIterations = (int)Math.Ceiling(Math.Log(1 - (double)twoSigma) / Math.Log((double)tieOdds));
            var oddsOfWinningBylikelyCeiling = (decimal)Math.Pow((double)tieOdds, likelyIterations);

            return new RegularContestOdds
            {
                MySkill = mySkill,
                TheirSkill = theirSkill,
                MyOddsOfWinningAnIteration = myOdds,
                TheirOddsOfWinningAnIteration = theirOdds,
                OddsOfTyingAnIteration = tieOdds,
                MyOddsOfWinningTheContest = myTotalOdds,
                TheirOddsOfWinningTheContest = theirTotalOdds,
                LikelyIterationsToCompletion = likelyIterations,
                OddsOfCompletingByLikelyIterations = 1 - oddsOfWinningBylikelyCeiling,
            };
        }




        /// <summary>
        /// Starts the process of calculating energy gathering odds.
        /// </summary>
        /// <param name="skillLevel">Path Skill</param>
        /// <param name="energyToGather">Amount of energy to gather</param>
        /// <returns>Metrics about probability of success</returns>
        private static EnergyAccumulationProbability CalculateEnergyGatheringOdds(int skillLevel, int energyToGather, Func<int, IEnumerable<int>> calculateFailedRollsFunction, Dictionary<EnergyAccumulationProbabilityKey, EnergyAccumulationProbability> resultsDictionary)
        {
            const int turnCycle = 0;
            EnergyGatherStringBuilder.Clear();
            EnergyGatherStringBuilder.AppendLine("digraph G {");
            GraphedNodes.Clear();
            GraphedRelationships.Clear();
            var returnValue = CalculateEnergyGatheringOdds(
                new EnergyAccumulationProbabilityKey
                {
                    SkillLevel = skillLevel,
                    TurnCycle = turnCycle,
                    EnergyToGather = energyToGather
                },
                calculateFailedRollsFunction,
                resultsDictionary);
            EnergyGatherStringBuilder.AppendLine("}");
            var value = EnergyGatherStringBuilder.ToString();
            return returnValue;
        }

        public static StringBuilder EnergyGatherStringBuilder { get; } = new StringBuilder();
        public static List<int> GraphedNodes { get; } = new List<int>();
        public static List<KeyValuePair<int, int>> GraphedRelationships { get; } = new List<KeyValuePair<int, int>>();

        private static EnergyAccumulationProbability CalculateEnergyGatheringOdds(EnergyAccumulationProbabilityKey parameters, Func<int, IEnumerable<int>> calculateFailedRollsFunction, Dictionary<EnergyAccumulationProbabilityKey, EnergyAccumulationProbability> resultsDictionary)
        {
            EnergyAccumulationProbability returnValue;
            //Case 1, we just finished or this one has already been calculated.
            lock (resultsDictionary)
            {
                
                //The answer was just added, or already existed.
                if (resultsDictionary.ContainsKey(parameters))
                {
                    returnValue = resultsDictionary[parameters];
                   
                    GraphEnergyAccumulationNode(parameters, returnValue);
                    return resultsDictionary[parameters];
                }
            }
            //If we have no energy to gather, success rate is 100%.
            if (parameters.EnergyToGather <= 0)
            {
                returnValue = new EnergyAccumulationProbability { OddsOfSuccess = 1 };
                lock(resultsDictionary)
                {
                    resultsDictionary.Add(parameters, returnValue);
                }
                GraphEnergyAccumulationNode(parameters, returnValue);
                return returnValue;
            }
            //if skill is too low to roll, we have 0% success rate.
            if (parameters.SkillLevel < 3)
            {
                returnValue = new EnergyAccumulationProbability { OddsOfSuccess = 0 };
                lock (resultsDictionary)
                {
                    resultsDictionary.Add(parameters, returnValue);
                }
                GraphEnergyAccumulationNode(parameters, returnValue);
                return returnValue;
            }


            //Case 2, we need to actually solve the problem.
            //Step one figure out what numbers count as critical failures.
            var lowestFailRoll = calculateFailedRollsFunction(parameters.SkillLevel).Min();

            //Since those automatically fail, we don't even need to calculate those chances of success.
            var rollsWeCareAbout = Enumerable.Range(3, lowestFailRoll -3);
            decimal accumulatedSuccess = 0;
            foreach(var roll in rollsWeCareAbout)
            {
                var energyGathered = Math.Max(parameters.SkillLevel - roll, 1);

                //We still need to gather at a minimum, 0 energy.
                var childEnergyToGather = Math.Max(0, parameters.EnergyToGather - energyGathered);

                //Decrement skill every three cycles.
                var childTurnCycle = (parameters.TurnCycle + 1) % 3;
                var childSkill = (childTurnCycle == 0 ? parameters.SkillLevel - 1 : parameters.SkillLevel);
                var childParameter = new EnergyAccumulationProbabilityKey { EnergyToGather = childEnergyToGather, SkillLevel = childSkill, TurnCycle = childTurnCycle };
                var childProbability = CalculateEnergyGatheringOdds(childParameter, calculateFailedRollsFunction, resultsDictionary);
                //return $"\"{UniqueIdentifier}\"[label = \"(Paranthesis)\"]" + Environment.NewLine + Operand.PrintGraph() + $"\"{UniqueIdentifier}\"->\"{Operand.UniqueIdentifier}\"" + Environment.NewLine;
                GraphRelationship(parameters, childParameter);

                var probabilityOfRollingThisNumber = RollDistribution[roll] / ((decimal)PossibleOutcomes);
                accumulatedSuccess += (childProbability.OddsOfSuccess * probabilityOfRollingThisNumber);
            }

            //Now that we have calculated the probability of succeeding at this level
            //Let's memoize it.
            lock (resultsDictionary)
            {
                var result = new EnergyAccumulationProbability { OddsOfSuccess = accumulatedSuccess };
                GraphEnergyAccumulationNode(parameters, result);
                resultsDictionary.Add(parameters, result);
                return resultsDictionary[parameters];
            }

        }

        internal static EnergyAccumulationProbability MonteCarloEnergyGather(EnergyGatheringParameters parsedParameters)
        {
            const int iterationCount = 2000;
            var iterations = Enumerable.
                Range(1, iterationCount).
                Select(i => EnergyGather.Execute(parsedParameters)).
                ToArray();
            var successes = iterations.
                Where(i => !i.Failed).
                ToArray();
            var failures = iterations.
                Where(i => i.Failed).
                ToArray();
            var returnValue = new EnergyAccumulationProbability { OddsOfSuccess = (successes.Length * 1.0m) / iterationCount };
            returnValue.OddsOfSuccessWithoutQuirks = returnValue.OddsOfSuccess > 0m ? ((successes.Count(s => s.Quirks == 0) * 1.0m) / iterationCount) : 0m;



            if (successes.Any())
            {
                returnValue.AverageSuccessTime = successes.
                    Select(s => s.TimeSpent).
                    Aggregate((a, b) => a + b) / (successes.Length);
                returnValue.SuccessTimeSigma = 
                    new TimeSpan
                    (
                        (long)Math.Floor
                        (
                            Math.Sqrt
                            (
                                successes.
                                Select(s => Math.Pow(s.TimeSpent.Ticks - returnValue.AverageSuccessTime.Value.Ticks, 2)).
                                Average()
                            )
                        )
                    );

                returnValue.AverageQuirkCount = successes.Average(s => s.Quirks);
                returnValue.QuirkSigma = Math.Sqrt(successes.Select(s => Math.Pow(s.Quirks - returnValue.AverageQuirkCount.Value, 2)).Average());
            }

            if (failures.Any())
            {
                returnValue.AverageBotchEnergyLevel = failures.Average(f => f.BotchRitualEnergy);
                returnValue.BotchSigma = Math.Sqrt(failures.Select(s => Math.Pow(s.BotchRitualEnergy - returnValue.AverageBotchEnergyLevel.Value, 2)).Average());
                returnValue.AverageFailureTime = failures.
                    Select(s => s.TimeSpent).
                    Aggregate((a, b) => a + b) / (failures.Length);

                returnValue.FailureTimeSigma =
                    new TimeSpan
                    (
                        (long)Math.Floor
                        (
                            Math.Sqrt
                            (
                                failures.
                                Select(s => Math.Pow(s.TimeSpent.Ticks - returnValue.AverageFailureTime.Value.Ticks, 2)).
                                Average()
                            )
                        )
                    );
            }
            return returnValue;
        }

        private static void GraphRelationship(EnergyAccumulationProbabilityKey parameters, EnergyAccumulationProbabilityKey childParameter)
        {
            var key = new KeyValuePair<int, int>(parameters.GetHashCode(), childParameter.GetHashCode());
            if (!GraphedRelationships.Contains(key))
            {
                GraphedRelationships.Add(key);
                EnergyGatherStringBuilder.AppendLine($"\"{key.Key}\"->\"{key.Value}\"");
            }
        }

        private static void GraphEnergyAccumulationNode(EnergyAccumulationProbabilityKey parameters, EnergyAccumulationProbability probability)
        {
            var hashCode = parameters.GetHashCode();
            if (!GraphedNodes.Contains(hashCode))
            {
                GraphedNodes.Add(hashCode);
                EnergyGatherStringBuilder.AppendLine($"\"{parameters.GetHashCode()}\"[label = \"Energy: {parameters.EnergyToGather} Skill Level: {parameters.SkillLevel} Chance: {probability.OddsOfSuccess:P2}\"]");
            }
        }
    }
}
