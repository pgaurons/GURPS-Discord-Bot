using Gao.Gurps.Dice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Workflow
{
    public static class Contest
    {
        public static string Quick(string argument)
        {
            var levels = ContestParser.Parse(argument).ToArray();
            return $"Running contest: {argument}" + Environment.NewLine + Quick(levels[0], levels[1]);

        }


        public static string Quick(int firstContestantSkill, int secondContestantSkill)
        {
            var sb = new StringBuilder();
            var results = Roller.NewQuickContest(firstContestantSkill, secondContestantSkill);

            var firstMarginFormatted = TextUtility.FormatMargin(results.FirstContestantResult.Margin); //Specifically, ignore the normal margin rules.
            var firstRollFormatted = TextUtility.DiceThrownFormatter(results.FirstContestantResult.RollResult);
            sb.AppendLine($"Contestant #1 Rolls: [{firstRollFormatted}] = {results.FirstContestantResult.RollResult.Sum()}. {firstMarginFormatted}");

            var secondMarginFormatted = TextUtility.FormatMargin(results.SecondContestantResult.Margin);
            var secondRollFormatted = TextUtility.DiceThrownFormatter(results.SecondContestantResult.RollResult);
            sb.AppendLine($"Contestant #2 Rolls: [{secondRollFormatted}] = {results.SecondContestantResult.RollResult.Sum()}. {secondMarginFormatted}");

            if(results.IsTie)
            {
                sb.AppendLine($"The quick contest is a tie.");
            }
            else
            {
                var winningPlayer = results.FirstContestantIsWinner ? "1" : "2";
                sb.AppendLine($"Contestant #{winningPlayer} wins with a Margin of Victory of {results.MarginOfVictory}.");
            }

            return sb.ToString();
        }


        public static IEnumerable<string> Regular(int firstContestantSkill, int secondContestantSkill, TimeSpan iterationDuration)
        {
            var originalSkillOne = firstContestantSkill;
            var originalSkillTwo = secondContestantSkill;
            NormalizeRegularContestSkillLevels(ref firstContestantSkill, ref secondContestantSkill);

            if(originalSkillOne != firstContestantSkill || originalSkillTwo != secondContestantSkill)
            {
                yield return $"Normalized Contestant #1 Skill from {originalSkillOne} to {firstContestantSkill}.";
                yield return $"Normalized Contestant #2 Skill from {originalSkillTwo} to {secondContestantSkill}.";
            }

            var firstSuccesses = Roller.Successes(firstContestantSkill).Union(Roller.CriticalSuccesses(firstContestantSkill)).ToList();
            var firstFailures = Roller.Failures(firstContestantSkill).Union(Roller.CriticalFailures(firstContestantSkill)).ToList();

            var secondSuccesses = Roller.Successes(secondContestantSkill).Union(Roller.CriticalSuccesses(secondContestantSkill)).ToList();
            var secondFailures = Roller.Failures(secondContestantSkill).Union(Roller.CriticalFailures(secondContestantSkill)).ToList();

            var victory = false;
            var elapsedTime = iterationDuration;
            do
            {
                const string successString = "Success";
                const string failureString = "Failure";

                var firstRoll = Roller.Roll().Sum();
                var firstRollIsASuccess = firstSuccesses.Contains(firstRoll);
                var firstResult = firstRollIsASuccess ? successString : failureString;
                var secondRoll = Roller.Roll().Sum();
                var secondRollIsASuccess = secondSuccesses.Contains(secondRoll);
                var secondResult = secondRollIsASuccess ? successString : failureString;
                victory = firstRollIsASuccess != secondRollIsASuccess;
                var currentResult = victory ? firstRollIsASuccess ? "Contestant #1 wins" : "Contestant #2 wins" : "Tie";
                yield return $"{elapsedTime}: Contestant 1 rolled {firstRoll:00}({firstResult}). Contestant 2 rolled {secondRoll:00}({secondResult}). [{currentResult}]";

                elapsedTime += iterationDuration;
            } while (!victory);
        }

        public static void NormalizeRegularContestSkillLevels(ref int mySkill, ref int theirSkill)
        {
            var lowest = Math.Min(mySkill, theirSkill);
            var highest = Math.Max(mySkill, theirSkill);
            const int lowCutoff = 6;
            const int mediumHighCutoff = 14;
            const int veryHighCutoff = 20;
            if (highest <= lowCutoff) //Normalize lowest skill to 10, and increase other skill by same amount.
            {
                var addend = 10 - lowest;
                mySkill += addend;
                theirSkill += addend;
            }
            else if (lowest > veryHighCutoff) //Find the divisor that reduces the lowest to 10 and divide the higher by the same amount.
            {
                var divisor = lowest / 10m;
                mySkill = (int)Math.Floor(mySkill / divisor);
                theirSkill = (int)Math.Floor(theirSkill / divisor);
            }
            else if (lowest >= mediumHighCutoff)
            {
                var subtrahend = lowest - 10;
                mySkill -= subtrahend;
                theirSkill -= subtrahend;
            }
        }
    }
}
