using Discord;
using Discord.Interactions;
using Gao.Gurps.Dice;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Discord.Workflow;
using Gao.Gurps.Mechanic;
using Gao.Gurps.Model;
using Gao.Gurps.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandRemarksAttribute = Discord.Commands.RemarksAttribute;
using CommandSummaryAttribute = Discord.Commands.SummaryAttribute;
using DiscordHlc = Gao.Gurps.Discord.Model.HitLocationTable;
using ModelHlc = Gao.Gurps.Model.HitLocationTable;

namespace Gao.Gurps.Discord.Slash
{
#if DEBUG
    [Group("debug-dice", "test commands")]
#endif
    public class DiceRollingModule : GurpsInteractionModuleBase
    {
                [SlashCommand("gather-energy", "Gather energy via the Ritual Path Magic mechanics", runMode: RunMode.Async)]
        [CommandSummary(@"Gather Energy according to the rules of RPM. 
Example command usage: 
To gather 30 energy with 15 skill.
\gather-energy Path of Mind-15 30
To gather 30 energy with 15 skill and gathering occurs every 5 seconds.
\gather-energy Path of Body-15 30 00:00:05
If you are in a hurry to gather 30 energy with 15 skill.
\gather-energy 15 30 ")]
        public async Task GatherEnergy(string argument)
        {
            const int linesToDisplayAtOnce = 5;

            EnergyGatheringParameters parsedParameters;
            try
            {
                parsedParameters = EnergyGather.Parse(argument);
            }
            catch (Exception ex)
            {
                await DisplayErrorMessage(await Context.Guild.GetTextChannelAsync(Context.Interaction.ChannelId.Value), ex.Message);
                return;
            }

            var results = EnergyGather.Execute(parsedParameters);
            if (parsedParameters.Verbose)
            {
                await Context.Interaction.RespondAsync("Starting to gather energy.");
                await SendTextWithTimeBuffers(linesToDisplayAtOnce, results.VerboseOutput().Split(Environment.NewLine), false);
            }
            else
                await Context.Interaction.RespondAsync(results.ToString());

        }


        [SlashCommand("quick-contest", "Execute a quick contest", runMode: RunMode.Async), CommandSummary(@"Roll Quick Contest.
Examples:
\quick-contest 10 vs 12")]
        public async Task QuickContest(string argument = "")
        {

            var valid = ContestParser.Valid(argument);
            if (valid)
            {
                var result = Contest.Quick(argument);
                await Context.Interaction.RespondAsync(result);
            }
            else
            {
                await Context.Interaction.RespondAsync($"Can't parse '{argument}'");
            }
        }

        [SlashCommand("quick-contest-a-bunch", "Do a bunch of quick contests", runMode: RunMode.Async), CommandSummary(@"Rolls several Quick Contests. The first parameter is the skill of the instigator, this is then proceeded with the resisting skill levels of all targets, for example, if using an area of effect spell, the
caster's skill level of 16 would be the first parameter, and the wills of three targets would come next. Results are positive number for a success, negative for a loss, and 0 for a tie, those being context sensitive.
Examples:
\quick-contest-a-bunch 16 10 12 14
\quick-contest-a-bunch 16 16 15 14")]
        public async Task QuickContestABunch(int instigatorSkill, [Summary("defender-skill-levels")] string argument = "")
        {
            var opponentSkillLevels = new uint[0];
            bool valid;
            try
            {
                opponentSkillLevels = argument.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => uint.Parse(s)).ToArray();
                valid = opponentSkillLevels.Length > 0;
            }
            catch
            {
                valid = false;
            }
            if (valid)
            {
                var results = opponentSkillLevels.Select(osl => Roller.NewQuickContest(instigatorSkill, (int)osl)).ToList();

                await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateQuickContestABunchEmbed(results));
            }
            else
            {
                await Context.Interaction.RespondAsync($"Can't parse '{argument}'");
            }
        }

        [SlashCommand("quick-contest-a-bunch-identical", "Do a bunch of quick contests with contestants of the same skill level.", runMode: RunMode.Async), CommandSummary(@"Like Quick contest a bunch, except it assumes that the ability is being used against several opponents of the same resistance level.
Assuming a skill level of 16 versus 10 enemies with a will of 12.
Examples:
\quick-contest-a-bunch-identical 16 12 10
\quick-contest-a-bunch-identical 16 12 10")]
        public async Task QuickContestABunch(uint instigatorSkill, uint defenderSkillLevels, uint numberOfDefenders)
        {
            var results =
                Enumerable.Repeat(defenderSkillLevels, (int)numberOfDefenders).
                Select(osl => Roller.NewQuickContest((int)instigatorSkill, (int)osl)).
                ToList();

            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateQuickContestABunchEmbed(results));
        }

        [SlashCommand("regular-contest", "Do a regular contest", runMode: RunMode.Async), CommandSummary(@"Rolls a regular contest where each iteration defaults to a length of 1 second. 
Optional iteration lengths may be supplied.
Examples:
\regular-contest 10 10
\regular-contest 12 10 0:30")]
        public async Task RegularContest([Summary("contestant-one-skill", "First contestant's skill")] int contestantOneSkill, int contestantTwoSkill, [Summary("interval-duration", "Format as mm:ss e.g. 5:00 for five minutes")] string durationAsText = "0:01")
        {

            var duration = new TimeSpan(0, 0, 1);
            if (!string.IsNullOrWhiteSpace(durationAsText) && TextUtility.TimeSpanRegularExpression.IsMatch(durationAsText))
            {
                duration = TextUtility.ParseTimeSpanText(durationAsText, false);
            }

            await Context.Interaction.RespondAsync($"Beginning the regular contest.");
            var result = Contest.Regular(contestantOneSkill, contestantTwoSkill, duration).ToArray();
            await SendTextWithTimeBuffers(5, result, false);

        }


        [SlashCommand("roll", "Roll dice", runMode: RunMode.Async)]
        [CommandSummary(@"Roll dice using either the GURPS 'nd' syntax, assuming six sided dice, or using the common 'mdn' format to throw m quantity of n-sided dice. No parameter rolls 3d by default.
Examples:
`/roll`
`/roll 3d`
`/roll 2d+10`
`/roll 1d+1`")]
        [CommandRemarks(@"All algebra is currently integer based; rounding then follows the rules one would expect in most programming languages for such.
Accepted operators are as follows:
`+`: Addition
`-`: Subtraction
`*`: Multiplication (also, lowercase `x`)
`/`: Division
`()`: Parentheses
`^`: Exponent
You can also end a line with a comment by using `//`.
")]
        public async Task Roll([Summary("dice-adds", "Some arithmetic is allowed, add a comment with `//`")]string argument = "")
        {
            var toReturn = $"";
            var split = argument.Split("//", StringSplitOptions.None);
            var comment = split.Length > 1 ? split[1] : null;
            argument = split[0].Trim();
            if (argument == string.Empty)
                argument = "3d";

            var value = argument.Replace(" ", "");
            if (value.Length > 20)
            {

                await Context.Interaction.RespondAsync(toReturn + ": That's too much input, onii-chan!");
                return;
            }
            var roll = await GetDiceAdds(argument);

            toReturn += $"Rolled {argument.Replace("*", @"\*")}. Result: {roll.Replace("*", @"\*")}{(!string.IsNullOrEmpty(comment) ? " //" + comment : "")}";
            await Context.Interaction.RespondAsync(toReturn);
        }

        private async Task<string> GetDiceAdds(string diceAdds)
        {
            IExpression<int> result = null;
            string returnValue = string.Empty;
            try
            {
                result = DiceAddsParser.StartParseDiceAdds(diceAdds);
                result.Evaluate();
                returnValue = $"{result.ToString().Replace(" ", "").Replace("D", "d")} = {result.Result}";
            }
            catch (DivideByZeroException)
            {
                if (result != null)
                    returnValue = $"{result.ToString().Replace(" ", "")} = ";
                returnValue += "Divide By Zero";
            }
            catch (Exception)
            {
                if (result != null)
                {
                    returnValue = $"The value was parsed to {result.ToString().Replace(" ", "")}, but could not evaluate.";
                }
                else
                {
                    await Context.Interaction.RespondAsync($"Can't parse '{diceAdds}'");
                    throw new ArgumentException("Argument formatted incorrectly", diceAdds);
                }
            }

            return returnValue;
        }

        [SlashCommand("roll-a-bunch", "Make the same roll multiple times", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls as many times as told. It defaults to rolling 3d, but optionally, a different roll can
be provided.
Examples:
\roll-a-bunch 5
\roll-a-bunch 50
\roll-a-bunch 5 7d+1
You can also end a line with a comment by using `//`.")]
        public async Task RollABunch([Summary("number-of-rolls")] int times, string diceAdds = "3d")
        {
            diceAdds = diceAdds.Split("//", StringSplitOptions.None)[0];
            times = Math.Abs(times);
            var sb = new StringBuilder();
            if (diceAdds.Length > 20)
            {

                await Context.Interaction.RespondAsync("That's too much input, onii-chan!");
                return;
            }
            var rollExpression = DiceAddsParser.StartParseDiceAdds(diceAdds, true);
            foreach (var i in Enumerable.Range(1, times))
            {
                var roll = rollExpression.Evaluate();

                sb.Append($"{roll}, ");
            }
            var result = sb.ToString().Trim(',', ' ');
            if (result.Length > 1600)
            {
                await SendTextWithTimeBuffers(100, result.Split(", "), true, ", ");
            }
            else
                await Context.Interaction.RespondAsync(result);


        }

        [SlashCommand("roll-a-bunch-against", "Do multiple success rolls against the same value.", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls against a given target number a given number of times.
Examples:
\roll-a-bunch-against 10 5
\roll-a-bunch-against 10 50")]
        [CommandRemarks(@"Each result is shown as `[+-]\d+!?`. 
A plus or minus sign indicating success or failure respectively, followed by the margin of success or failure. An exclamation point means that the result is critical.")]
        public async Task RollABunchAgainst([Summary("Rolls")] int times, [Summary("Target-Number")] int target)
        {
            times = Math.Abs(times);
            var sb = new StringBuilder();

            foreach (var i in Enumerable.Range(1, times))
            {
                var roll = Roller.RollAgainst(target);

                sb.Append($"{roll.Margin:+0;-#}{(roll.Critical ? "!" : "")}, ");
            }
            var result = sb.ToString().Trim(',', ' ');
            if (result.Length > 1600)
            {
                await SendTextWithTimeBuffers(100, result.Split(", "), true, ", ");
            }
            else
                await Context.Interaction.RespondAsync(result);


        }

        [SlashCommand("roll-against", "Success rolls", runMode: RunMode.Async)]
        [CommandSummary(@"Does a success roll versus the target
Examples:
\roll-against 13
\roll-against Stealth-12
You can also end a line with a comment by using `//`.")]
        public async Task RollAgainst([Summary("Target-Number")] string argument)
        {
            
            argument = argument.Split("//", StringSplitOptions.None)[0].Trim();
            var valid = true;
            var evaluatedValue = -1;
            var targetParsed = string.Empty;
            try
            {
                targetParsed = TargetParser.Parse(argument);
                var value = DiceAddsParser.StartParseDiceAdds(targetParsed);
                evaluatedValue = value.Evaluate();
            }
            catch
            {
                valid = false;
            }
            if (valid)
            {
                var needsSimplifying = targetParsed.Replace(" ", string.Empty).ToUpperInvariant() != evaluatedValue.ToString();
                var rollingResult = Roller.RollAgainst(evaluatedValue);
                var diceThrown = rollingResult.RollResult.ToList();
                var result = diceThrown.Sum();

                var diceThrownAggregated = diceThrown.
                Aggregate
                (
                    string.Empty,
                    (accum, input) => accum + input + ",",
                    (accum) => accum.Trim(',')
                    );
                var marginFormatted = TextUtility.FormatMargin(rollingResult.Margin);
                var criticalText = rollingResult.Critical ? "Critical " : string.Empty;
                criticalText += rollingResult.Success ? "Success" : "Failure";
                var toReturn = $"Rolled against {argument}{(needsSimplifying ? $" ({evaluatedValue})" : "")}. Result: [{diceThrownAggregated}] = {result}. {marginFormatted}. {criticalText}".Trim();
                await Context.Interaction.RespondAsync(toReturn);

            }
            else
            {
                await Context.Interaction.RespondAsync($"Can't parse '{argument}'");
            }
        }


    }
}
