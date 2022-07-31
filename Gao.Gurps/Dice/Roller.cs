using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gao.Gurps.Dice
{

    public class Roller
    {
        public static Random  NumberGenerator = new Random();

        /// <summary>
        /// Count of successful yiffs.
        /// </summary>
        public static int YiffCount { get; private set; } = 0;

        /// <summary>
        /// Add a yiff.
        /// </summary>
        public static void IncrementYiffs()
        {
            lock(GurpsRollResults)
            {
                YiffCount++;
            }
        }

        private static Func<int, int, int> _integerGenerationFunction;
        public static Func<int, int, int> IntegerGenerationFunction
        {
            get
            {
                if (_integerGenerationFunction == null)
                    _integerGenerationFunction = DefaultIntegerGenerator;
                return _integerGenerationFunction;
            }
            set
            {
                _integerGenerationFunction = value;
            }
        }


        public static IEnumerable<int> Roll()
        {
            return Roll(3, 6);
        }
        public static IEnumerable<int> Roll(int quantity, int sides)
        {
            var result = Enumerable.Range(1, quantity).Select(a => IntegerGenerationFunction(1, sides)).ToList();
            if(quantity == 3 && sides ==6)
            {
                AddResultToList(result.Sum());
            }
            return result;
        }

        public static IEnumerable<int> Roll(ParsedDiceAdds diceAdds)
        {
            if(diceAdds.Quantity != 0 && diceAdds.Sides != 0)
                foreach (var result in Roll(diceAdds.Quantity, diceAdds.Sides))
                    yield return result;
            yield return diceAdds.Addend;
        }

        private static void AddResultToList(int result)
        {
            lock(GurpsRollResults)
            {
                GurpsRollResults.Add(result);
                if (GurpsRollResults.Count > 10000)
                    GurpsRollResults.RemoveAt(0);
            }
        }

        /// <summary>
        /// Performs a standard GURPS success roll with the given skill level.
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <returns></returns>
        public static SuccessRollResult RollAgainst(int targetNumber)
        {
            return RollAgainst(targetNumber, Roll().ToArray());
        }

        /// <summary>
        /// Performs the calculations for a standard success roll using a target number and the results from thrown dice.
        /// </summary>
        /// <param name="targetNumber"></param>
        /// <param name="diceThrown"></param>
        /// <returns></returns>
        public static SuccessRollResult RollAgainst(int targetNumber, params int[] diceThrown)
        {
            var result = diceThrown.Sum();
            var margin = targetNumber - result;

            var success = AllSuccesses(targetNumber).Contains(result);
            margin = success ? Math.Max(0, margin) : Math.Min(-1, margin);

            var critical =
                CriticalSuccesses(targetNumber).Contains(result) ||
                CriticalFailures(targetNumber).Contains(result);

            return new SuccessRollResult
            {
                Critical = critical,
                Margin = margin,
                RollResult = diceThrown,
                Success = success,
                Target = targetNumber
            };
        }

        public static List<int> GurpsRollResults = new List<int>();

        public static int DefaultIntegerGenerator(int minimum, int maximum)
        {
            return NumberGenerator.Next(minimum, maximum + 1);
        }

        /// <summary>
        /// Returns one of the provided items with equal likelihood of any.
        /// </summary>
        /// <typeparam name="T">type of the items</typeparam>
        /// <param name="items">Items to select from.</param>
        /// <returns>One random item from the list.</returns>
        public static T ChooseOne<T>(params T[] items)
        {
            return items[NumberGenerator.Next(items.Length)];
        }


        public static decimal RunningStandardDeviation
        {
            get
            {
                List<int> copiedList;
                lock (GurpsRollResults)
                {
                    copiedList = GurpsRollResults.ToList();
                }
                var standardDeviation = StandardDeviation(copiedList);
                return Convert.ToDecimal(standardDeviation);
            }
        }

        private static double StandardDeviation(List<int> copiedList)
        {
            var mean = copiedList.Average();
            return Math.Sqrt(
                    copiedList.
                    Select(r => Math.Pow(r - mean, 2)).
                    Average()
                );
        }

        public static decimal RunningAverage
        {
            get
            {
                List<int> copiedList;
                lock (GurpsRollResults)
                {
                    copiedList = GurpsRollResults.ToList();
                }
                return Convert.ToDecimal(copiedList.Average());
            }
        }

        public static decimal RunningCount
        {
            get
            {
                List<int> copiedList;
                lock (GurpsRollResults)
                {
                    copiedList = GurpsRollResults.ToList();
                }
                return Convert.ToDecimal(copiedList.Count);
            }
        }

        /// <summary>
        /// Add's a user to the daily user count if that user hasn't talked in the last 24 hours.
        /// </summary>
        /// <param name="id">the user id to add</param>
        public static void AddDailyUser(ulong id)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var oneWeekAgo = today.AddDays(-7);
            var yesterday = now.AddDays(-1);

            lock(GurpsRollResults)
            {
                //delete any old records
                _dailyUsers.RemoveAll(kvp => kvp.Item1 <= oneWeekAgo);
                //add this record if this user hasn't used the bot in the last 24 hours.
                if(!_dailyUsers.Any(kvp=> kvp.Item1 >= yesterday && kvp.Item2 == id))
                {
                    _dailyUsers.Add(new Tuple<DateTime, ulong>(now, id));
                }
            }
        }

        /// <summary>
        /// Gets the average daily users over the last week.
        /// </summary>
        public static double RollingDailyUserAverage
        {
            get
            {
                double result;
                lock(GurpsRollResults)
                {
                    result = _dailyUsers.
                        GroupBy(kvp => kvp.Item1.DayOfWeek).
                        Select(g => g.Count()).
                        Average();
                }
                return result;
            }
        }

        private static List<Tuple<DateTime, ulong>> _dailyUsers { get; } = new List<Tuple<DateTime, ulong>>();

        public static DateTimeOffset BotStartupTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// All the dice rolls that count as a critical failure.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static IEnumerable<int> CriticalFailures(int skillLevel)
        {
            return CriticalFailures(skillLevel, false);
        }

        /// <summary>
        /// All the dice rolls that count as a critical failure.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static IEnumerable<int> CriticalFailures(int skillLevel, bool resist)
        {
            var effectiveSkillLevel = skillLevel;
            if (effectiveSkillLevel <= 6) //If the margin is greater than 10 it is also a critical failure.
            {
                //3 and 4 are never critical failures for resistance rolls.
                effectiveSkillLevel = resist ? Math.Max(effectiveSkillLevel, -5) : effectiveSkillLevel;
                var start =  Math.Max(effectiveSkillLevel + 10, 3);
                var end = 16 - start + 1;
                foreach (var x in Enumerable.Range(start, end))
                    yield return x;
            }
            if (effectiveSkillLevel < 16) //If skill is 16 or less, then a 17 is a critical failure.
                yield return 17;
            //Always 18.
            yield return 18;
        }

        /// <summary>
        /// All the dice rolls that count as a normal failure.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static IEnumerable<int> Failures(int skillLevel)
        {
            return Failures(skillLevel, false);
        }

        /// <summary>
        /// All the dice rolls that count as a normal failure.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static IEnumerable<int> Failures(int skillLevel, bool resist)
        {
            
            //All numbers above skill level up to 16.
            var start = Math.Min( Math.Max(skillLevel + 1 + (skillLevel == 3 ? 1 : 0), resist ? 5 : 3), 17);
            var highestPossible = skillLevel < 7 ? skillLevel + 10 : skillLevel < 16 ? 16 + 1 : 17 + 1;
            var end = highestPossible - start;

            if (end <= 0) yield break;

            foreach (var missedRoll in Enumerable.Range(start, end))
            {
                yield return missedRoll;
            }

        }

        public static IEnumerable<int> AllFailures(int skillLevel, bool resist)
        {
            return Failures(skillLevel, resist).Union(CriticalFailures(skillLevel, resist));
        }

        public static IEnumerable<int> AllFailures(int skillLevel)
        {
            return AllFailures(skillLevel, false);
        }


        public static IEnumerable<int> AllSuccesses(int skillLevel, bool resist)
        {
            return Successes(skillLevel).Union(CriticalSuccesses(skillLevel, resist));
        }

        public static IEnumerable<int> AllSuccesses(int skillLevel)
        {
            return AllSuccesses(skillLevel, false);
        }

        /// <summary>
        /// All the dice rolls that count as a critical success.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <param name="resist">Whether this is a resistance or active defense roll</param>
        /// <returns></returns>
        public static IEnumerable<int> CriticalSuccesses(int skillLevel, bool resist)
        {
            //3 and 4 are always critical success for resistance rolls.
            if (resist || skillLevel >= 3) { yield return 3; yield return 4; }
            if (skillLevel >= 15) yield return 5;
            if (skillLevel >= 16) yield return 6;
        }

        /// <summary>
        /// All the dice rolls that count as a critical success.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static IEnumerable<int> CriticalSuccesses(int skillLevel)
        {
            return CriticalSuccesses(skillLevel, false);
        }

        /// <summary>
        /// All the dice rolls that count as a normal success.
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public static IEnumerable<int> Successes(int skillLevel)
        {
            if (skillLevel <= 4)
                yield break;
            var minimum = skillLevel < 15 ? 5 : skillLevel < 16 ? 6 : 7;
            const int maximum = 16 + 1;
            var end = Math.Min(Math.Max(skillLevel + 1 - minimum, 1), maximum - minimum);
            foreach (var result in Enumerable.Range(minimum, end))
                yield return result;
        }

        public static decimal RunningChiSquared
        {
            get
            {
                List<int> copiedList;
                lock (GurpsRollResults)
                {
                    copiedList = GurpsRollResults.ToList();
                }
                var standardDeviation = StandardDeviation(copiedList);
                var variance = Convert.ToDecimal(standardDeviation * standardDeviation);
                var degreesOfFreedom = Convert.ToDecimal(copiedList.Count - 1);
                const decimal approximatePopulationStandardDeviation = 2.96m;
                var populationVariance = approximatePopulationStandardDeviation * approximatePopulationStandardDeviation;

                return (degreesOfFreedom * variance) / populationVariance;
            }
        }
        /// <summary>
        /// Performs a quick contest between two contestants of given skill.
        /// </summary>
        /// <param name="firstContestantSkill"></param>
        /// <param name="secondContestantSkill"></param>
        /// <returns></returns>
        public static QuickContestResult NewQuickContest(int firstContestantSkill, int secondContestantSkill)
        {
            return NewQuickContest(firstContestantSkill, RollAgainst(firstContestantSkill), secondContestantSkill, RollAgainst(secondContestantSkill));
        }

        /// <summary>
        /// Does the calculations for a quick contest using pre-rolled numbers.
        /// </summary>
        /// <param name="firstContestantSkill"></param>
        /// <param name="firstContestantRoll"></param>
        /// <param name="secondContestantSkill"></param>
        /// <param name="secondContestantRoll"></param>
        /// <returns></returns>
        public static QuickContestResult NewQuickContest(int firstContestantSkill, int firstContestantRoll, int secondContestantSkill, int secondContestantRoll)
        {
            return NewQuickContest(firstContestantSkill, RollAgainst(firstContestantSkill, firstContestantRoll), secondContestantSkill, RollAgainst(secondContestantSkill, secondContestantRoll));
        }

        /// <summary>
        /// Does the calculations for a quick contest using the results of two success rolls.
        /// </summary>
        /// <param name="firstContestantSkill"></param>
        /// <param name="firstContestantRoll"></param>
        /// <param name="secondContestantSkill"></param>
        /// <param name="secondContestantRoll"></param>
        /// <returns></returns>
        public static QuickContestResult NewQuickContest(int firstContestantSkill, SuccessRollResult firstContestantRoll, int secondContestantSkill, SuccessRollResult secondContestantRoll)
        {
            var returnValue = new QuickContestResult
            {
                FirstContestantResult = firstContestantRoll,
                SecondContestantResult = secondContestantRoll,
                FirstContestantIsWinner = firstContestantRoll.Margin > secondContestantRoll.Margin
            };

            returnValue.IsTie = returnValue.MarginOfVictory == 0;
            returnValue.SecondContestantIsWinner = !(returnValue.IsTie || returnValue.FirstContestantIsWinner);

            return returnValue;
        }



    }
}
