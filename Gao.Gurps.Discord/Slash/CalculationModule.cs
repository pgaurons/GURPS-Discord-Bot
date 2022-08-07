using Discord;
using Discord.Interactions;
using Gao.Gurps.Dice;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Discord.Workflow;
using Gao.Gurps.Mechanic;
using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandRemarksAttribute = Discord.Commands.RemarksAttribute;
using CommandSummaryAttribute = Discord.Commands.SummaryAttribute;

namespace Gao.Gurps.Discord.Slash
{
#if DEBUG
    [Group("debug-calculation", "test commands")]
#endif
    public class CalculationModule : InteractionModuleBase
    {
        [SlashCommand("basic-lift", "Calculate basic lift", runMode: RunMode.Async), CommandSummary(@"Calculates the Basic Lift of a character given an effective strength. Add Kyos at the end to use Know your own strength calculations.
Examples:
\basic-lift 10 
\basic-lift 12
\basic-lift 13 Kyos")]
        public async Task BasicLift([Summary("Strength")] uint strength, [Summary("normal-or-kyos", "Whether to use default strength rules or KYOS strength rules"), Choice("Normal", ""), Choice("Kyos", "kyos")] string kyosOptionAsText = "", [Summary("imperial-or-metric"), Choice("Imperial", "IMPERIAL"), Choice("Metric", "METRIC")] string imperialOrMetric = "IMPERIAL")
        {
            var isMetric = imperialOrMetric.ToUpperInvariant().StartsWith("M");
            var weightUnit = isMetric ? "kg" : "lbs.";
            var kyosOption = !string.IsNullOrWhiteSpace(kyosOptionAsText) && kyosOptionAsText.ToUpperInvariant().StartsWith("K");

            var result = LookupTables.GetBasicLiftCalculations((int)strength, kyosOption);
            result.BasicLift /= isMetric ? 2 : 1;
            var numberFormatting = result.BasicLift < 10 ? "N1" : "N0";

            var firstColumnWidth = result.ExtraHeavy.ToString(numberFormatting).Length;
            var secondColumnWidth = result.ShiftSlightly.ToString(numberFormatting).Length;
            var firstColumnFormatString = "{0,16} {1," + firstColumnWidth + "} {2, 4}";
            var firstColumnOutput = "```" + Environment.NewLine +
                string.Format(firstColumnFormatString, "None (0):", result.BasicLift.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(firstColumnFormatString, "Light (1):", result.Light.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(firstColumnFormatString, "Medium (2):", result.Medium.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(firstColumnFormatString, "Heavy (3):", result.Heavy.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(firstColumnFormatString, "Extra-Heavy (4):", result.ExtraHeavy.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                "```";

            var secondColumnFormatString = "{0,30} {1," + secondColumnWidth + "} {2,4}";

            var secondColumnOutput = "```" + Environment.NewLine +
                string.Format(secondColumnFormatString, "Basic Lift:", result.BasicLift.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(secondColumnFormatString, "One-Handed Lift:", result.OneHandedLift.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(secondColumnFormatString, "Two-Handed Lift:", result.TwoHandedLift.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(secondColumnFormatString, "Shove and Knock Over:", result.ShoveAndKnockOver.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(secondColumnFormatString, "Running Shove and Knock Over:", result.RunningShoveAndKnockOver.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(secondColumnFormatString, "Carry On Back:", result.CarryOnBack.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                string.Format(secondColumnFormatString, "Shift Slightly:", result.ShiftSlightly.ToString(numberFormatting), weightUnit) + Environment.NewLine +
                "```";

            var builder = new EmbedBuilder()
.WithTitle("Basic Lift Table")
.WithDescription($"Here's the data nyaa~")
.WithColor(new Color(0x822451))
.WithFooter(footer =>
{
    footer
        .WithText(kyosOption ? "Know Your Own Strength" : "Normal");
})
.WithThumbnailUrl("https://i.imgur.com/OUtgQCV.png")
.AddField("Encumbrance Thresholds", firstColumnOutput)
.AddField("Common Actions", secondColumnOutput);
            var embed = builder.Build();



            await Context.Interaction.RespondAsync(string.Empty, embed: embed);
        }


        [SlashCommand("broken-blade", "Calculate broken blade stats", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates a number of specs about a melee weapon using the Broken Blade system from Pyramid #3/87
Examples:
`\broken-blade 12` for a weapon with minimum ST of 12
`\broken-blade 12 cheap` if it is cheap
`\broken-blade 12 good hafted` for a good quality hafted weapon.
`\broken-blade 12 symmetrical very fine` for a symmetrical very fine weapon
`\broken-blade 12 symmetrical very fine $3500.00 -3` for a symmetrical very fine weapon that costs $3500.00 and has accrued -3 HT damage.")]
        [CommandRemarks(@"Valid qualities are `cheap` `good` `fine` and `very fine`.
Extra options include `hafted` for hafted weapons and `symmetrical` for weapons that are symmetrical.
The stats generated from these equations are at times more like ""good defaults"" so adjust as reasonable.
The mechanics for using these stats are described in Pyramid #3/87: `http://www.warehouse23.com/products/pyramid-number-3-slash-87-low-tech-iii`
Current HT damage can be provided with a negative number. If a weapon has accrued -5 HT, you can provide -5 as a modifier.
Full cost of the weapon can be provided as a dollar amount like $500. If both damage and cost are provided, cost of repairs are included in the output.
The health damage figures are extrapolated past -10. The original table only goes up to -10, but it is mechanically possible to go to -15 with the best quality weapon in the worst possible shape.
")]
        public async Task BrokenBlade([Summary("weapon-strength")] uint strength, [Summary("other-options")] string otherOptions = "")
        {
            otherOptions = otherOptions.ToUpperInvariant();
            var quality =
                otherOptions.Contains("VERY FINE") ? EquipmentQuality.VeryFine :
                otherOptions.Contains("FINE") ? EquipmentQuality.Fine :
                otherOptions.Contains("GOOD") ? EquipmentQuality.Good :
                otherOptions.Contains("CHEAP") ? EquipmentQuality.Cheap : EquipmentQuality.Good;
            var isHafted = otherOptions.Contains("HAFTED");
            var isSymmetrical = otherOptions.Contains("SYMMETRICAL");

            var dollarAmount = 0m;
            var dollarAmountExpression = new Regex(@"\$\d+(\.\d{2})?");
            if (dollarAmountExpression.IsMatch(otherOptions))
            {
                dollarAmount = decimal.Parse(dollarAmountExpression.Match(otherOptions).Value.Trim('$'));
            }
            var healthDamage = 0;
            var negativeIntegerExpression = new Regex(@"\-\d+");
            if (negativeIntegerExpression.IsMatch(otherOptions))
            {
                healthDamage = int.Parse(negativeIntegerExpression.Match(otherOptions).Value);
            }
            if (healthDamage < -18)
            {
                await Context.Interaction.RespondAsync("Too much health damage.");
                return;
            }
            try
            {
                var result = Mechanic.BrokenBlade.GenerateStatistics(strength, quality, isHafted, isSymmetrical, dollarAmount, healthDamage);

                await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateBrokenBlade(result));
            }
            catch (ArgumentOutOfRangeException)
            {
                await Context.Interaction.RespondAsync("Too much health damage.");
                return;
            }
        }

        [SlashCommand("dollars-to-coins", "finds the weight of Dungeon Fantasy dollar amounts.", runMode: RunMode.Async)]
        [CommandSummary(@"Converts dollar amount given to coins for Dungeon Fantasy RPG.
Examples:
For $40:
\dollars-to-coins 40
For $40.12
\dollars-to-coins 40.12")]
        public async Task DollarsToCoins( decimal amount, [Summary("Unused")] string mode = "Dungeon Fantasy RPG")
        {
            await Context.Interaction.RespondAsync($"Converting {amount:C} to coins.");
            var coins = Mechanic.Money.ConvertDollarAmountToCoins(amount);
            await SendTextWithTimeBuffers(
                5,
                    new[] { $"Total Value : {coins.Value:C} Total Weight: {coins.Weight:N2} lbs." }.
                    Union(coins.Coins.Select(c => $"{(c.Name != "Remainder" ? c.Quantity + " " : string.Empty)}{c.Name + (c.Quantity > 1 ? "s" : "")}. {c.Value * c.Quantity:C}, {c.Weight * c.Quantity:N2} lbs.")),
                false);
        }


        [SlashCommand("grappling-encumbrance-table", "Calculates Technical Grappling encumbrance.", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates Grappling Weight Modifier and Encumbrance Penalty from Basic Lift (in lbs or kg) and Grappling Encumbrance (Defined on Technical Grappling p. 8)
\grappling-encumbrance-table 12.4 100
\grappling-encumbrance-table 24 180
\grappling-encumbrance-table 33.3 40 metric")]
        public async Task GrapplingEncumbranceTable([Summary("basic-lift-rational", "Basic lift, as a rational number")] decimal basicLift, [Summary("encumbrance-in-lbs")] uint encumbrance)
        {
            if (basicLift <= 0m)
            {
                await Context.Interaction.RespondAsync("You know what you are doing, and it's not funny.");
                return;
            }
            var results = LookupTables.LookupGrapplingEncumbrance(basicLift, encumbrance);
            await Context.Interaction.RespondAsync($"Grappling Weight Modifier: `{results.Item1:+#;-#;0}` Encumbrance Penalty: `{results.Item2:+#;-#;0}`");
        }

        [SlashCommand("jump", "Calculate jump distance and height", runMode: RunMode.Async)]
        [CommandSummary(@"Given a character's maximum move, calculates maximum vertical and horizontal jump. Optionally levels of super jump and enhanced move, in that order, can be provided.
Example usage:
`\jump 5
\jump 6 1
\jump 6 1 4
\jump 6 1 4 metric`")]
        public async Task Jump([Summary("Basic-Move")] int basicMove, [Summary("Super-Jump-Level")] int superJumpLevel = 0, [Summary("Enhanced-Move-Level", "Accepts decimals for half levels.")] decimal enhancedMoveLevel = 0, [Summary("imperial-or-metric"), Choice("Imperial", "IMPERIAL"), Choice("Metric", "METRIC")] string metricOrImperial = "IMPERIAL")
        {
            if (!new[] { 0m, 0.5m }.Contains(enhancedMoveLevel % 1.0m))
            {
                await Context.Interaction.RespondAsync(@"Stop.");
                return;
            }
            var isImperial = metricOrImperial.ToUpperInvariant().StartsWith('I');
            var results = Jumping.CalculateJumpMetrics(basicMove, Math.Max(0, superJumpLevel), Math.Max(0, enhancedMoveLevel));

            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateJumpEmbed(results, isImperial));
        }

        [SlashCommand("last-gasp", "Calculate fatigue recovery rates for last gasp rules", runMode: RunMode.Async), CommandSummary(@"Calculates the penalties for Last Gasp Long Term Fatigue.
Examples:
Nominally you need your remaining and your total FP
`\last-gasp 9 10`
You can also supply ST, DX, IQ, and HT in that order if you want the aritmetic done for you
`\last-gasp 9 12 10 11 10 12`
Finally, if you want High-Resolution ST Loss rules turned on, you can supply it as an optional parameter, but look at `LastGaspHighResolution` for a shortcut.
`\last-gasp 9 12 10 11 10 12 true`
")]
        public async Task LastGasp([Summary("Current-FP")] int currentFatiguePoints, [Summary("Total-FP")] int totalFatiguePoints, [Summary("ST")] int strength = -1, [Summary("DX")] int dexterity = -1, [Summary("IQ")] int intelligence = -1, [Summary("HT")] int health = -1, [Summary("high-resolution-mode", "More detailed rules make smaller increments.")] bool highDefinition = false)
        {
            var attributes = new[] { strength, dexterity, intelligence, health };
            if (attributes.Any(a => a > -1) && attributes.Any(a => a < 0))//All attributes must be supplied, or none of them.
            {
                await Context.Interaction.RespondAsync($"Define everything, or nothing, UwU.");
                return;
            }
            if (attributes.Any(a => a < -1))
            {
                await Context.Interaction.RespondAsync($"Don't be so negative, UwU.");
                return;
            }
            if (currentFatiguePoints > totalFatiguePoints)
            {
                await Context.Interaction.RespondAsync($"**(╯｀□′)╯**UNACCEPTABLE!");
                return;
            }



            var result = LookupTables.GetLongTermFatigueStats(currentFatiguePoints, totalFatiguePoints, strength, dexterity, intelligence, health, highDefinition);

            var embed = EmbedUtility.LastGaspEmbed(result);

            await Context.Interaction.RespondAsync(
                string.Empty,
                embed: embed);

        }


        [SlashCommand("lifting-strength", "Calculate minimum lifting strength from basic lift", runMode: RunMode.Async), CommandSummary(@"Calculates the effective lifting strength for a given basic lift in pounds. Add Knowing Your Own Strength at the end to use Know your own strength calculations.
Examples:
\lifting-strength 20 
\lifting-strength 24.5
\lifting-strength 81.2 Knowing Your Own Strength")]
        public async Task LiftingStrength([Summary("Basic-Lift")] decimal basicLift, [Summary("normal-or-kyos"), Choice("Normal", ""), Choice("Knowing Your Own Strength", "kyos")] string kyosOptionAsText = "", [Summary("imperial-or-metric"), Choice("Imperial", "IMPERIAL"), Choice("Metric", "METRIC")] string imperialOrMetric = "IMPERIAL")
        {
            var isMetric = imperialOrMetric.ToUpperInvariant().StartsWith("M");
            var kyosOption = !string.IsNullOrWhiteSpace(kyosOptionAsText) && kyosOptionAsText.ToUpperInvariant().StartsWith("K");

            if (basicLift <= 0)
            {
                await Context.Interaction.RespondAsync($"Don't be so negative, UwU.");
                return;
            }

            var result = LookupTables.GetLiftingStrength(isMetric ? basicLift * 2 : basicLift, kyosOption);


            await Context.Interaction.RespondAsync($"Minimum effective lifting ST is {result}.");
        }

        [SlashCommand("long-distance", "Look up long distance penalties", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates long range penalties for a given distance with an optional unit of measure. Default is miles.
Examples:
\long-distance 10
\long-distance 0.1 inches
\long-distance 10e+01 miles")]
        [CommandRemarks(@"Range accepts an optional designator for unit, assuming by default a mile. The conversion factor is based on the not 100% accurate, but simpler factors provided by the basic set.
The allowed units are as follows:
`CM` (or `Centimeter`) - Centimeters
`FT` (or `Feet`, `Foot`) - Feet 
`IN` (or `Inch`, `Inches`) - Inches
`KM` (or `Kilometer`) - Kilometers
`M` (or `Meter`) - Meters
`MI` (or `Mile`) - Miles
`YD` (or `Yard`) - Yards
Even when not specified as valid input, technically plural terms should work because everything is ignored after a valid match.
EG: Mi, Mile, and Miles (and even MiIdontcareanymore) are all equivalent.
")]
        public async Task LongDistance([Summary("Speed-or-Range", "check help for units.")] string argument)
        {
            try
            {
                var result = LookupTables.GetLongDistancePenalty(argument);
                await Context.Interaction.RespondAsync($"Long Distance Penalty for {argument} is {result}.");
            }
            catch
            {
                await Context.Interaction.RespondAsync($"Can't parse '{argument}'");
            }
        }


        [SlashCommand("lookup", "look up values in e.g. indices and GCS data", runMode: RunMode.Async)]
        [CommandSummary(@"Looks up GURPS information from several different resources.
Example command usage: 
\lookup spell fireball")]
        [CommandRemarks(@"The first parameter tells what type of index to search. Valid values are as follows:
`Advantage`, `Book`, `Disadvantage`, `Equipment`, `FAQ`, `Hitlocation`, `Index`, `Item`, `Perk`, `Pyramid`, `Quirk`, `Skill`, `Spell`, `Technique`
The second parameter is a search query. It accepts regular expressions for all searches but hit location.
The final parameter is an override to return more results than normal.
")]
        public async Task Lookup([Summary("Lookup-Type")] LookupType lookupType, [Summary("Lookup-Value")] string lookupValue, uint rowsToReturn = 10)
        {



            const uint searchThreshold = 10;

            var valid = true;

            var returnValue = $"";
            LookupResult searchResults = new LookupResult { OriginalCount = 0, Results = new string[0] };
            try
            {
                var regex = new Regex(lookupValue);
            }
            catch (Exception)
            {
                valid = false;
                searchResults = new LookupResult { OriginalCount = 0, Results = new[] { "Invalid Search expression." } };
            }
            try
            {
                if (valid)
                    switch (lookupType.ToString().ToUpperInvariant())
                    {
                        case "BOOK": searchResults = Workflow.Lookup.FindBook(lookupValue, (int)rowsToReturn); break;
                        case "SKILL": searchResults = Workflow.Lookup.FindSkill(lookupValue, (int)rowsToReturn); break;
                        case "EQUIPMENT":
                        case "ITEM": searchResults = Workflow.Lookup.FindEquipment(lookupValue, (int)rowsToReturn); break;
                        case "SPELL": searchResults = Workflow.Lookup.FindSpell(lookupValue, (int)rowsToReturn); break;
                        case "TECHNIQUE": searchResults = Workflow.Lookup.FindTechnique(lookupValue, (int)rowsToReturn); break;
                        case "ADVANTAGE":
                        case "DISADVANTAGE":
                        case "QUIRK":
                        case "PERK":
                            searchResults = Workflow.Lookup.FindAdvantage(lookupValue, (int)rowsToReturn);
                            break;
                        case "PYRAMID":
                            searchResults = Workflow.Lookup.FindInPyramid(lookupValue, (int)rowsToReturn);
                            break;
                        case "INDEX":
                            searchResults = Workflow.Lookup.FindInIndex(lookupValue, (int)rowsToReturn); //Using the non uppercased version because of regex.
                            break;
                        case "HITLOCATION":
                            var results = LookupTables.FindHitLocation(lookupValue).ToArray();
                            searchResults = new LookupResult { OriginalCount = results.Length, Results = results };
                            break;
                        case "FAQ":
                            searchResults = await Workflow.Lookup.FindInFrequentlyAskedQuestions(lookupValue, (int)rowsToReturn);
                            break;
                        default: await Context.Interaction.RespondAsync(returnValue + "Invalid Lookup Arguments"); break;
                    }
                if (searchResults.Results.Count() > 0)
                {
                    var count = searchResults.OriginalCount;
                    await Context.Interaction.RespondAsync(returnValue + $"Found {count} result(s). Showing {Math.Min(count, rowsToReturn)}");
                    await SendTextWithTimeBuffers((int)searchThreshold, searchResults.Results.ToArray(), true);

                }
                else
                {
                    await Context.Interaction.RespondAsync(returnValue + $"Found nothing!");
                }

            }
            catch (Exception ex)
            {

                returnValue += $"{Environment.NewLine}{ex.GetType()}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}";
                Console.WriteLine(returnValue);
                //returnValue += $"{Environment.NewLine}{ex.GetType()}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}";
                //var task = ReplyAsync(returnValue);

            }


        }

        [SlashCommand("normalize-to-dice", "Simplifies dice down.", runMode: RunMode.Async)]
        [CommandSummary(@"Converts an integer to a number of dice, or normalizes dice adds using the rules from B269
Examples:
\normalize-to-dice 12
\normalize-to-dice 11d-22")]
        [CommandRemarks(@"This uses the formula from B269")]
        public async Task NormalizeToDice([Summary("Integer-or-Dice-Value", "Value to simplify")] string value)
        {
            if (!int.TryParse(value, out _) &&
                !DiceAddsParser.ValidDiceAdds.IsMatch(value))
            {
                await Context.Interaction.RespondAsync(@"This is garbage. You can do better.");
                return;
            }
            var parsedValue = DiceAddsParser.Parse(value).Normalize();
            var factoredValue = parsedValue.Factor();
            var returnString = parsedValue.ToString();
            if (!factoredValue.Equals(parsedValue))
            {
                returnString += $" (or {factoredValue})";
            }
            await Context.Interaction.RespondAsync(returnString);
        }

        [SlashCommand("range", "get speed/range penalties", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates speed/range penalties for a given distance with an optional unit of measure. Default is yards.
Examples:
\range 10
\range 0.1 inches
\range 10e+01 miles")]
        [CommandRemarks(@"Range accepts an optional designator for unit, assuming by default a yard. The conversion factor is based on the not 100% accurate, but simpler factors provided by the basic set.
The allowed units are as follows:
`CM` (or `Centimeter`) - Centimeters
`FT` (or `Feet`, `Foot`) - Feet 
`IN` (or `Inch`, `Inches`) - Inches
`KM` (or `Kilometer`) - Kilometers
`M` (or `Meter`) - Meters
`MI` (or `Mile`) - Miles
`YD` (or `Yard`) - Yards
Even when not specified as valid input, technically plural terms should work because everything is ignored after a valid match.
EG: Mi, Mile, and Miles (and even MiIdontcareanymore) are all equivalent.
")]
        public async Task Range([Summary("Speed-Range", "rational number, can include a unit too, check help for formatting and unit help.")] string argument)
        {
            try
            {
                var result = LookupTables.GetDistancePenalty(argument);
                await Context.Interaction.RespondAsync($"Range Penalty for {argument} is {result}.");
            }
            catch
            {
                await Context.Interaction.RespondAsync($"Can't parse '{argument}'");
            }
        }


        [SlashCommand("rate-of-fire-bonus", "give the rate of fire, find out the skill bonus", runMode: RunMode.Async), CommandSummary(@"Calculates bonus to skill for shooting really fast.
Examples:
\rate-of-fire-bonus 10
\rate-of-fire-bonus 100")]
        public async Task RateOfFireBonus([Summary("Rate-of-Fire")] uint rateOfFire)
        {
            var bonus = LookupTables.GetRateOfFireBonus((int)rateOfFire);

            await Context.Interaction.RespondAsync($"The bonus for an ROF of {rateOfFire} is {bonus}.");
        }

        [SlashCommand("robustness-threshold", "Give HP to get robustness threshold or wound potential", runMode: RunMode.Async)]
        [CommandSummary(@"Looks up the robustness threshold (or wound potential) given a number of hit points (or penetrating damage).")]
        public async Task RobustnessThreshold([Summary("Hit-Points", "Or penetrating damage")] int hitPoints)
        {
            try
            {
                var result = LookupTables.GetRobustnessThreshold(hitPoints);
                await Context.Interaction.RespondAsync($"Robustness Threshold: {result}");
            }
            catch
            {
                await Context.Interaction.RespondAsync(@"You were raised by a very naughty wizard!");
            }
        }


        [SlashCommand("size-modifier", "Give size, get size modifier", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates unmodified size modifier for a given length or height with an optional unit designator
Examples:
\size-modifier 10
\size-modifier 0.1 inches
\size-modifier 10e+01 miles")]
        [CommandRemarks(@"Size accepts an optional designator for unit, assuming by default a yard. The conversion factor is based on the not 100% accurate, but simpler factors provided by the basic set.
The allowed units are as follows:
`CM` (or `Centimeter`) - Centimeters
`FT` (or `Feet`, `Foot`) - Feet 
`IN` (or `Inch`, `Inches`) - Inches
`KM` (or `Kilometer`) - Kilometers
`M` (or `Meter`) - Meters
`MI` (or `Mile`) - Miles
`YD` (or `Yard`) - Yards
Even when not specified as valid input, technically plural terms should work because everything is ignored after a valid match.
EG: Mi, Mile, and Miles (and even MiIdontcareanymore) are all equivalent.
")]
        public async Task SizeModifier([Summary("Length-or-Height", "e.g. 100 yards, see help for more")] string argument)
        {
            try
            {
                var result = LookupTables.GetSizeModifier(argument);
                await Context.Interaction.RespondAsync($"Size Modifier is {result}.");
            }
            catch
            {
                await Context.Interaction.RespondAsync($"Can't parse '{argument}'");
            }
        }

        [SlashCommand("striking-strength", "Find out swing and thrust damage from strength", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates the amount of damage done for a given strength level. Optionally you can request the results using kyos or reducedSwing to get results from two popular alternative striking strength tables
Examples:
\striking-strength 10
\striking-strength 12
\striking-strength 13 Knowing your Own Strength
\striking-strength 14 Reduced Swing")]
        public async Task StrikingStrength([Summary("Strength")] uint strength, [Summary("Strength-Mode"), Choice("Normal", ""), Choice("Reduced Swing", "R"), Choice("Knowing Your Own Strength", "kyos")] string kyosOptionAsText = "")
        {


            if (string.IsNullOrEmpty(kyosOptionAsText)) kyosOptionAsText = "B"; //B for basic.
            var kyosOption = kyosOptionAsText.ToUpperInvariant().First() switch
            {
                'K' => StrengthMode.KnowYourOwnStrength,
                'R' => StrengthMode.ReducedSwing,
                _ => StrengthMode.Basic,
            };
            var result = LookupTables.GetStrikingStrengthCalculations((int)strength, kyosOption);

            await Context.Interaction.RespondAsync($"{result}");
        }

        private async Task SendTextWithTimeBuffers(int linesToDisplayAtOnce, IEnumerable<string> results, bool privateMessageOverflow, string lineDivider = "")
        {
            if (lineDivider == string.Empty) lineDivider = Environment.NewLine;
            const int maxMessageSize = 2000 - 30; //Let's give some buffer room.
            results = results.SelectMany(r => r.Length > maxMessageSize ? r.Split(lineDivider) : new[] { r }).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            IMessageChannel overflowChannel;
            var thisChannel = await Context.Guild.GetTextChannelAsync(Context.Interaction.ChannelId.Value);
            if (privateMessageOverflow) overflowChannel = await Context.User.CreateDMChannelAsync();
            else overflowChannel = thisChannel;
            var remainingResults = results.ToArray();
            var overflow = false;
            IMessageChannel channel = thisChannel;

            while (remainingResults.Length > 0)
            {
                channel = overflow ? overflowChannel : thisChannel;
                var sb = new StringBuilder();
                sb.AppendLine("```");
                var linesLeftToAppend = Math.Min(linesToDisplayAtOnce, remainingResults.Length);
                var linesTaken = 0;
                while (sb.Length + (remainingResults.FirstOrDefault() ?? "").Length < maxMessageSize && linesLeftToAppend > 0)
                {
                    sb.Append(remainingResults.First() + lineDivider);
                    remainingResults = remainingResults.Skip(1).ToArray();
                    linesLeftToAppend--;
                    linesTaken++;
                }
                var displayValue = sb.ToString();
                displayValue = displayValue.Trim(lineDivider.ToCharArray()) + "```";

                displayValue = displayValue.Replace("``````", string.Empty);
                await channel.SendMessageAsync(displayValue);
                if (remainingResults.Length == 0)
                    break;

                await Task.Delay(1500);

                overflow = true;
            }
        }
    }

}
