using Gao.Gurps.Discord.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gao.Gurps.Discord.Workflow
{
    public static class TextUtility
    {
        /// <summary>
        /// A regular expression that represents a valid timespan.
        /// </summary>
        public const string TimeSpanFormat = @"((?:\d+\.)?(?:\d{1,2}\:)?(?:\d{1,2}(?:\:\d{1,2})?)(?:\.\d+)?)";

        public static readonly Regex TimeSpanRegularExpression = new Regex(TimeSpanFormat);
        public static TimeSpan ParseTimeSpanText(string timePeriod, TimeSpan defaultTimeSpan, bool preferSeconds)
        {
            var timeSpan = defaultTimeSpan;
            if (!string.IsNullOrEmpty(timePeriod))
            {
                timeSpan = ParseTimeSpanText(timePeriod, preferSeconds);
            }

            return timeSpan;
        }

        public static readonly Regex HourMinuteSeconds = new Regex(@"^(\d+):(\d+):(\d+)$");

        public static TimeSpan ParseTimeSpanText(string timePeriod, bool preferSeconds)
        {
            TimeSpan timeSpan;
            if (Regex.IsMatch(timePeriod, @"^\d+$"))
            {
                var intValue = int.Parse(timePeriod);
                timeSpan = preferSeconds ? new TimeSpan(0, 0, intValue) : new TimeSpan(0, intValue, 0);
            }
            else if(HourMinuteSeconds.IsMatch(timePeriod))
            {
                var groups = HourMinuteSeconds.Match(timePeriod).Groups;
                timeSpan = new TimeSpan(int.Parse(groups[1].Value), int.Parse(groups[2].Value), int.Parse(groups[3].Value));
            }
            else if
            (
                !TimeSpan.TryParseExact(timePeriod, @"m\:s", CultureInfo.InvariantCulture, out timeSpan) &&
                !TimeSpan.TryParse(timePeriod, out timeSpan)
            )
            {
                throw new ArgumentException($"The found time span {timePeriod} looks malformed");
            }
            return timeSpan;
        }


        public static string FormatMargin(int margin)
        {
            return string.Format("{0}: {1}", margin < 0 ? "Margin of Failure" : "Margin of Success", Math.Abs(margin));
        }
        public static string DiceThrownFormatter(IEnumerable<int> results)
        {
            return results.Aggregate
            (
                string.Empty,
                (accum, input) => accum + input + ",",
                (accum) => accum.Trim(',')
                );
        }


        public static IEnumerable<string> FormatExplosionResults(IEnumerable<ExplosionResult> results)
        {
            var targetCount = 0;
            foreach(var result in results)
            {
                targetCount++;
                if (!result.InExplosionRange)
                {
                    yield return $"Target {targetCount} is not in range of the explosion.";
                }
                else
                {
                    var returnValue = $"Target {targetCount}: [{result.ExplosionDamageParsed} = {result.ExplosionDamageEvaluated}]/{Math.Max(1, result.Distance * (3 - (result.ExplosionLevel - 1)))} = {result.ExplosionDamageEvaluated / Math.Max(1, result.Distance * (3 - (result.ExplosionLevel - 1)))} damage from explosion.";
                    if (!string.IsNullOrWhiteSpace(result.ShrapnelDamageParsed) && result.InShrapnelRange)
                    {
                        returnValue += $" Shrapnel Roll of {result.ShrapnelRoll}.";
                    }
                    yield return returnValue;
                }
                if(!string.IsNullOrWhiteSpace(result.ShrapnelDamageParsed))
                {
                    if(!result.InShrapnelRange)
                    {
                        yield return $"\tTarget {targetCount} is not in range of the shrapnel.";
                        continue;
                    }
                    if(!result.ShrapnelHits.Any())
                    {
                        yield return $"\tTarget {targetCount} was not hit by the shrapnel.";
                        continue;
                    }
                    foreach(var hit in result.ShrapnelHits)
                    {
                        var potentialText = hit.IsAutomatic ? string.Empty : "[Potential] ";
                        yield return $"\t{potentialText}{result.ShrapnelDamageParsed} = {hit.Damage} to {hit.Location}({hit.LocationRoll}) from shrapnel.";
                    }
                }

            }
        }
    }
}
