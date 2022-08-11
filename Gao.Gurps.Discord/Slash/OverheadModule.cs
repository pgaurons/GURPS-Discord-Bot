using Discord;
using Discord.Interactions;
using Gao.Gurps.Dice;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Discord.Workflow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommandRemarksAttribute = Discord.Commands.RemarksAttribute;
using CommandSummaryAttribute = Discord.Commands.SummaryAttribute;
using CommandService = Discord.Commands.CommandService;
using DiscordHlc = Gao.Gurps.Discord.Model.HitLocationTable;
using ModelHlc = Gao.Gurps.Model.HitLocationTable;
using System.Linq;

namespace Gao.Gurps.Discord.Slash
{
    /// <summary>
    /// Slash commands that deal with cross cutting concerns.
    /// </summary>
#if DEBUG
    [Group("debug-overhead", "test commands")]
#endif
    public class OverheadModule : GurpsInteractionModuleBase
    {
        private readonly CommandService _commandService;
        private readonly InteractionService _interactionService;
        public OverheadModule(CommandService commandService, InteractionService interactionService)
        {
            _commandService = commandService;
            _interactionService = interactionService;
        }

        [SlashCommand("dont-sue-me", "legal boilerplate, credits, et al.", runMode: RunMode.Async)]
        [CommandSummary(@"Reads legal boilerplate")]
        public async Task DontSueMe()
        {
            var builder = new EmbedBuilder().
               WithAuthor("Patricia Gauronskas, usually called Patricia Cake").
               WithTitle("Don't Sue Me!").
               WithDescription("Legal Boilerplate, Bot Invitation Link, and Credits").
               WithColor(new Color(0xB71E20)).
               WithThumbnailUrl("https://i.imgur.com/WhOXiFM.jpg").
               AddField("Invite Pseudobot", "If you want to add Pseudobot to another server follow this link (https://discord.com/api/oauth2/authorize?client_id=242855270076645377&permissions=309237680192&scope=applications.commands%20bot)").
               AddField("Gurps Character Sheet - http://gurpscharactersheet.com/", "Oh, and I borrow some stuff from GURPS Character sheet, specifically for the trait, skills, spells, techniques, and item lookups.").
               AddField("Dungeon Fantasy Treasure Generator - https://df-treasure-generator.herokuapp.com/", "I use this for the `/generate-treasure` command for DF loot.").
               AddField("Blog - http://pseudoboo.blogspot.com", "I post about GURPS and Pseudobot from time to time.").
               AddField("Patreon - https://www.patreon.com/user?u=6028839", "Support me on a recurring basis.").
               AddField("Google Pay", "Send money to `Patricia.Gauronskas@gmail.com` for one time contributions.").
               AddField("Venmo", "Send money to user `Patricia-Gauronskas` for one time contributions.").
               AddField("Pay Pal", "https://www.paypal.com/paypalme/patriciagauronskas").
               AddField("Contact Me", "You can find me in the official bot discord at https://discord.gg/cAQY85GXen").
               AddField("Legal - http://www.sjgames.com/general/online_policy.html", "GURPS is a trademark of Steve Jackson Games, and its rules and art are copyrighted by Steve Jackson Games . All rights are reserved by Steve Jackson Games. This game aid is the original creation of Tricia Gauronskas and is released for free distribution, and not for resale, under the permissions granted in the Steve Jackson Games Online Policy").
               AddField("Privacy Policy", "If you are concerned about privacy, the `/pboptout` command allows you to opt out of messages being read. The bot's privacy policy is available at: https://docs.google.com/document/d/17FiL-is65fZJBHxfdsX1Xjhe0h38jmkHBJv_zmdzFEQ/edit?usp=sharing")
               ;
            await Context.Interaction.RespondAsync(string.Empty, embed: builder.Build());
        }

        [SlashCommand("help", "find out more about bot commands", runMode: RunMode.Async)]
        [CommandSummary(@"If no parameter is provided, lists all available functions. If a command name is provided, explains the given command.
Examples:
\help
\help roll")]
        public async Task HelpCommand([Summary("Command-Name")] string commandName = "")
        {
            if (string.IsNullOrEmpty(commandName))
            {
                await ListCommands();
            }
            else
            {
                await ShowHelpForGivenCommand(commandName);
            }

        }
        internal class ReducedParameter
        {
            internal bool IsOptional { get; set; }
            internal bool IsRemainder { get; set; }
            internal string Summary { get; set; }

        }
        internal class ReducedCommand
        {
            public string CommandType { get; internal set; }
            internal string Name { get; set; }
            internal ReducedParameter[] Parameters { get; set; }
            internal string Remarks { get; set; }
            internal string Summary { get; set; }
            internal string[] Aliases { get; set; }

        }
        private async Task ListCommands()
        {
            
            var embed = new EmbedBuilder()
                .WithTitle("Help");
            var commandNames = _commandService.Commands.
                Select(c => "`" + c.Name + "`").Distinct().OrderBy(s => s).ToArray();
            if(commandNames.Any())
                EmbedCommandNames(commandNames, embed, "Text Commands", "For more help on a specific command, use the help command on one of the following");
            commandNames = _interactionService.SlashCommands.
                Select(c => "`" + c.Name + "`").Distinct().OrderBy(s => s).ToArray();
            if(commandNames.Any())
                EmbedCommandNames(commandNames, embed, "Slash Commands", "For more help on a specific command, use the help command on one of the following");

            await Context.Interaction.RespondAsync("", embed: embed.Build());
        }
        private static void EmbedCommandNames(string[] commandNames, EmbedBuilder embed, string category, string detail)
        {
            const int maxCharLength = 1024;
            var joinedString = string.Join(", ", commandNames);
            if (joinedString.Length > maxCharLength)
            {
                EmbedTooBigCommandNames(commandNames, embed, category, detail);
            }
            else
            {
                embed.AddField($"{category} - {detail}", joinedString);
            }
        }

        private static void EmbedTooBigCommandNames(string[] commandNames, EmbedBuilder embed, string category, object detail)
        {
            const int maxCharLength = 1024;
            var divisor = 2; //start with the assumption we need to split the list in half, increment if half is not good enough.
            var stillTooBig = true;
            List<string[]> splitLists;
            do
            {
                splitLists = Split(commandNames, divisor);
                stillTooBig = (splitLists.Select(s => string.Join(", ", s).Length).Any(i => i > maxCharLength));
                if (stillTooBig) divisor++;
            } while (stillTooBig);
            for (var i = 1; i <= splitLists.Count; i++)
            {
                var currentList = splitLists[i - 1];
                var title = i == 1 ? $"{category} - {detail}" : $"{category} Continued, Part {i}";
                embed.AddField(title, string.Join(", ", currentList));
            }
        }

        private static List<string[]> Split(string[] items, int divisor)
        {

            var returnValue = new List<string[]>();
            var index = 0;
            var dividend = items.Length / divisor;

            while (index < items.Length - 1)
            {
                returnValue.Add(items.Skip(index).Take(dividend).ToArray());
                index += dividend;
            }

            return returnValue;
        }
        private async Task ShowHelpForGivenCommand(string commandName)
        {

            var result = _commandService.Commands.
                Select(c => new ReducedCommand { Name = c.Name, Parameters = c.Parameters.Select(p => new ReducedParameter { IsOptional = p.IsOptional, IsRemainder = p.IsRemainder, Summary = p.Summary }).ToArray(), Remarks = c.Remarks, Summary = c.Summary, Aliases = c.Aliases.ToArray(), CommandType = "Text" }).
                Union(_interactionService.SlashCommands.Select(c => new ReducedCommand { Name = c.Name, Parameters = c.Parameters.Select(p => new ReducedParameter { IsOptional = !p.IsRequired, IsRemainder = false, Summary = p.Description }).ToArray(), Remarks = c.Attributes.OfType<CommandRemarksAttribute>().FirstOrDefault()?.Text ?? String.Empty, Summary = (c.Description + Environment.NewLine + c.Attributes.OfType<CommandSummaryAttribute>().FirstOrDefault()?.Text ?? "").Trim(), Aliases = new string[0], CommandType = "Slash" })).
                Where(c => c.Name.ToUpperInvariant() == commandName.ToUpperInvariant() || c.Aliases.Select(a => a.ToUpperInvariant()).Contains(commandName.ToUpperInvariant())).
                OrderByDescending(c => c.Parameters.Length).
                FirstOrDefault();
            if (result == null)
            {
                await Context.Interaction.RespondAsync($"Command ({commandName}) not found");
                return;
            }
            var embed = new EmbedBuilder()
                .WithTitle($"{result.CommandType} Command - {result.Name}")
                .WithDescription(result.Summary);
            if (result.Parameters.Any())
            {
                embed.AddField("Parameters", string.Join(Environment.NewLine, result.Parameters.Select(p => {
                    var openBracket = p.IsOptional ? "[" : "<";
                    var closeBracket = p.IsOptional ? "]" : ">";
                    closeBracket = (p.IsRemainder ? "..." : string.Empty) + closeBracket;
                    return "`" + openBracket + p.Summary + closeBracket + "`";
                })));
            }
            if (!string.IsNullOrEmpty(result.Remarks))
            {
                embed.AddField("Additional Information", result.Remarks);
            }
            var aliases = result.Aliases.Where(a => a.ToUpperInvariant() != result.Name.ToUpperInvariant()).ToArray();
            if (aliases.Length > 0)
            {
                embed.AddField("Aliases", string.Join(", ", aliases.Select(a => "`" + a + "`")));
            }

            await Context.Interaction.RespondAsync("", embed: embed.Build());
        }

        [SlashCommand("lookup-refresh", "Owner only, refreshes lookup libraries", runMode: RunMode.Async), CommandSummary(@"Owner only command. Makes the bot reinitialize index libraries so that new data can be added to the index search without restarting the bot.")]
        [RequireOwner]
        public async Task LookupRefresh()
        {
            Workflow.Lookup.InitializeLazyCollections();
            await Context.Interaction.RespondAsync($"👍");
        }

        [SlashCommand("manage-guild-blocks", "block or unblock guilds", runMode: RunMode.Async)]
        [CommandSummary(@"Owner only command - Block or unblock a Guild
Examples:
\manage-guild-blocks 12345678")]
        [RequireOwner]
        public async Task ManageGuildBlocks([Summary("Guild-Id")] ulong guildId)
        {
            var result = GuildBlockUtility.Toggle(guildId);
            await Context.Interaction.RespondAsync($"{(result ? "Blocked" : "Unblocked")} {guildId}");
        }


        [SlashCommand("manage-user-blocks", "Block or unblock users", runMode: RunMode.Async)]
        [CommandSummary(@"Owner only command - Block or unblock a User
Examples:
\manage-user-blocks 12345678")]
        [RequireOwner]
        public async Task ManageUserBlocks([Summary("User-Id")] ulong userId)
        {
            var result = UserBlockUtility.Toggle(userId);
            await Context.Interaction.RespondAsync($"{(result ? "Blocked" : "Unblocked")} {userId}");
        }

        [SlashCommand("pb-opt-out", "Opt out or back into being visible to Pseudobot.", runMode: RunMode.Async)]
        [CommandSummary(@"Allows a user to opt out or opt back into features that require the ability to know more about them. Privacy conscious users may be interested.
Examples:
`/pb-opt-out All`
`/pb-opt-out None`")]
        public async Task OptOut(OptOutOptions options)
        {
            var user = Context.User;
            try
            {
                OptOutUtility.SetOptions(user.Id, options);
                await Context.Interaction.RespondAsync($"Set Opt-out to {options}.👍");

            }
            catch
            {
                await Context.Interaction.RespondAsync("👎");
                return;
            }
        }

        [SlashCommand("statistics", "Get stats about pseudobot", runMode: RunMode.Async)]
        [CommandSummary(@"Shows some statistical information about the dice roller if you are worried it is cheating.")]
        public async Task Statistics()
        {

            var runningCount = Roller.RunningCount;
            if (runningCount > 0)
                await Context.Interaction.RespondAsync(
                    string.Format("Current Stats:{0}" +
                    "Current Roll Count: {2}" +
                    "{0}Current Average: {3:0.00}" +
                    "{0}Current Standard Deviation: {4:0.00}" +
                    "{0}Current Chi Squared: {5:0.00}" +
                    "{0}Current number of Guilds Serviced: {7}" +
                    "{0}Average number of daily users over the last week: {8:0.00}" +
                    "{0}Last Service Restart: <t:{6}:F>" +
                    "{0}You're nor cleared for that: {9}",
                    Environment.NewLine,
                    string.Empty,
                    Roller.RunningCount,
                    Roller.RunningAverage,
                    Roller.RunningStandardDeviation,
                    Roller.RunningChiSquared,
                    Roller.BotStartupTime.ToUnixTimeSeconds(),
                    (await Context.Client.GetGuildsAsync()).Count,
                    Roller.RollingDailyUserAverage,
                    Roller.YiffCount));
            else
                await Context.Interaction.RespondAsync($"The roller function has not been used yet since last restarting the bot at {Roller.BotStartupTime}.");
        }

        [SlashCommand("timer", "Start a timer", runMode: RunMode.Async)]
        [CommandSummary(@"Starts a timer which when completed will alert the user.
Examples:
\timer 10:00
\timer 60:00 Used Luck
\timer 5")]
        public async Task Timer([Summary("Delay")] string timeSpan, [Summary("Message")] string message = ";3")
        {

            if (string.IsNullOrWhiteSpace(message))
            {
                message = ";3";
            }
            var validTimeSpan = TextUtility.TimeSpanRegularExpression.IsMatch(timeSpan);

            if (validTimeSpan)
            {
                var delay = TextUtility.ParseTimeSpanText(timeSpan, false);
                var thisChannel = await Context.Guild.GetTextChannelAsync(Context.Interaction.ChannelId.Value);
                var createdTimer = Workflow.Timer.Execute(async () =>
                {
                    var sb = new StringBuilder();

                    sb.AppendLine($"{Context.User.Mention}: {message} timer is up.");

                    await thisChannel.SendMessageAsync(sb.ToString());
                }, delay);


                await Context.Interaction.RespondAsync($"Timer is set to go off at approximately <t:{(DateTimeOffset.UtcNow + delay).ToUnixTimeSeconds()}:F>.");
                //await createdTimer;
            }
            else
            {
                await Context.Interaction.RespondAsync($" Can't parse '{timeSpan}'.");
            }
        }
    }
}
