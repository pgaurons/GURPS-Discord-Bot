using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using InteractionService = Discord.Interactions.InteractionService;

namespace Gao.Gurps.Discord.Workflow
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly InteractionService _interactionService;
        public HelpModule(CommandService commandService, InteractionService interactionService) : base()
        {
            _commandService = commandService;
            _interactionService = interactionService;
        }



        [Command("Announce", RunMode = RunMode.Async)]
        [Summary(@"Owner only command - Broadcast to all servers new changes.
Examples:
.Announce This is an announcement, there has been an update")]
        [RequireOwner]
        public async Task AnnounceCommand([Summary("Announcement"), Remainder]string announcement)
        {
            var botUser = Context.Client.CurrentUser;
            var guilds = Context.Client.Guilds;
            var channels = new List<SocketGuildChannel>();
            foreach(var guild in guilds)
            {
                var settings = AnnouncementUtility.GetAnnouncementSettingsFromGuild(guild, botUser.Id);
                if(settings.Enabled)
                {
                    var channel = guild.TextChannels.FirstOrDefault(c => c.Id == settings.ChannelId);
                    if (channel != null)
                        channels.Add(channel);
                }
            }

            var acknowledge = Context.Message.AddReactionAsync(new Emoji("👍"));
            foreach (IMessageChannel channel in channels)
            {
                await channel.SendMessageAsync(announcement);
                //await ReplyAsync($"Would send a message to {channel.Name}");
                await Task.Delay(300);
            }
            await acknowledge;
            await Context.Message.AddReactionAsync(new Emoji("🛑"));

        }

        [Command("DumpLog", RunMode = RunMode.Async)]
        [Summary(@"Dumps the log from the date provided to when the command is executed.
An end date can be provided if the dates are divided by a `->`
Examples:
.DumpLog 2008-05-01 7:34:42
.dl 2008-05-01
.dl 2008-05-01 -> 2008-07-01")]
        [Alias("DL")]
        public async Task DumpLogCommand([Summary("Date (and time) in the UTC timezone of the start of the dump"),Remainder]string unsplitDateText = "")
        {
            var dateTexts = unsplitDateText.Split("->").Select(s => s.Trim()).ToArray();
            const int blockSize = 1000;
            var endDate = DateTime.UtcNow;
            if(!DateTime.TryParse(dateTexts[0], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,  out DateTime startDate))
            {
                await Context.Channel.SendMessageAsync($"Can't parse the start date - {dateTexts[0]}");
                return;
            }
            if (dateTexts.Length > 1 && !DateTime.TryParse(dateTexts[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out endDate))
            {
                await Context.Channel.SendMessageAsync($"Can't parse the end date - {dateTexts[1]}");
                return;
            }
            if (startDate > endDate)
            {
                await Context.Channel.SendMessageAsync("Start date is greater than end date.");
                return;
            }
            if(startDate < endDate.AddMonths(-3))
            {
                await Context.Channel.SendMessageAsync("Dates must be within three months of each other.");
                return;
            }
            using (Context.Channel.EnterTypingState())
            {
                
                var done = false;
                List<IMessage> messages = new List<IMessage>();
                var foundStart = false;
                //First get the first message inside the range.
                var currentProcessingMessages = await (Context.Channel.GetMessagesAsync(blockSize, CacheMode.AllowDownload).Flatten()).OrderByDescending(m => m.Timestamp).ToArrayAsync();
                do
                {
                    var lastMessage = currentProcessingMessages.LastOrDefault();
                    if (lastMessage != null) //We have at least some messages.
                    {
                        //Case 1, we have some messages in range.
                        var rangeCount = currentProcessingMessages.Count(m => m.Timestamp > startDate && m.Timestamp < endDate);
                        if (rangeCount > 0)
                        {
                            foundStart = true;
                            currentProcessingMessages = currentProcessingMessages.Where(m => m.Timestamp > startDate && m.Timestamp < endDate).OrderByDescending(m => m.Timestamp).ToArray();
                        }
                        //Case 2, we haven't reached the end.
                        if (currentProcessingMessages.All(m => m.Timestamp >= endDate))
                        {
                            currentProcessingMessages = await (Context.Channel.GetMessagesAsync(lastMessage.Id, Direction.Before, blockSize, CacheMode.AllowDownload).Flatten()).OrderByDescending(m => m.Timestamp).ToArrayAsync();
                        }
                        else if (!foundStart) //Case 3, no messages in between and all the messages not being in front means that there are messages before and after the range, but not inside.
                        {
                            await Context.Channel.SendMessageAsync("Absolutely no messages in time range.");
                            return;
                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Absolutely no valid messages in channel.");
                        return;
                    }
                } while (!foundStart);
                var optedOutUsers = OptOutUtility.GetValues().
                    Where(oo => (oo.Option & Model.OptOutOptions.Logging) == Model.OptOutOptions.Logging).
                    Select(oo => oo.UserId).
                    ToArray();
                foreach (var message in currentProcessingMessages.Where(m => !(optedOutUsers.Contains(m.Author.Id))))
                {
                    messages.Add(message);
                }
                do
                {
                    var lastMessage = messages.LastOrDefault();
                    if (lastMessage == null) break;
                    var count = 0;
                    await foreach (var message in (Context.Channel.GetMessagesAsync(lastMessage.Id, Direction.Before, blockSize, CacheMode.AllowDownload).Flatten()).Where(m => m.Timestamp > startDate && m.Timestamp < endDate).OrderByDescending(m => m.Timestamp))
                    {
                        messages.Add(message);
                        count++;
                    }
                    done = count != blockSize; //Not enough messages to get a full blocksize worth, meaning we have hit the end. In the case where the last block is exactly 200, this will get 0 on the next loop and kill itself.
                } while (!done);

                var users = new Dictionary<ulong, IUser>();
                var channels = new Dictionary<ulong, IChannel>();
                var roles = new Dictionary<ulong, IRole>();
                messages.Reverse();
                foreach (var author in messages.Select(m => m.Author).Where(a => a != null).Distinct())
                {
                    if (!users.ContainsKey(author.Id))
                    {
                        users.Add(author.Id, author);
                    }
                }
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(
                    messages.
                    GroupBy(m => m.Timestamp.Date).
                    SelectMany
                    (g =>
                        new[] { "-=" + g.Key.ToString("yyyy-MM-dd") + "=-", Environment.NewLine }.
                        Concat
                        (
                            g.Select(m => CreateLogLine(m, users, channels, roles))
                        ).
                        Append(Environment.NewLine)
                    ).
                    Aggregate((a, b) => a + Environment.NewLine + b).
                    Replace("\n", "\r\n").
                    Replace("\r\r", "\r"))))
                {
                    await Context.Channel.SendFileAsync(stream, $"{Context.Channel.Name}-{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")}-log.txt", $"Here's the log, {Context.User.Mention}.");
                }
            }
        }

        private string CreateLogLine(IMessage message, Dictionary<ulong, IUser> users, Dictionary<ulong, IChannel> channels, Dictionary<ulong, IRole> roles)
        {
            var authorName = message.Author is SocketGuildUser && !string.IsNullOrEmpty(((SocketGuildUser)message.Author).Nickname) ? ((SocketGuildUser)message.Author).Nickname : message.Author.Username;
            var returnValue = $"{message.Timestamp.DateTime:HH':'mm':'ss'Z'} <{authorName}> - {message.Content}".Replace("\n", "\r\n");
            foreach (var user in GetMentionedUsers(users, message.MentionedUserIds).Where(u => u != null))
            {
                if (user is SocketGuildUser socketGuildUser)
                    returnValue = returnValue.Replace(user.Id.ToString(), !string.IsNullOrEmpty(socketGuildUser.Nickname) ? socketGuildUser.Nickname : user.Username);
                else
                    returnValue = returnValue.Replace(user.Id.ToString(), user.Username);
            }
            foreach(var channel in GetMentionedChannels(channels, message.MentionedChannelIds).Where(u => u != null))
            {
                returnValue = returnValue.Replace(channel.Id.ToString(), channel.Name);
            }
            foreach(var role in GetMentionedRoles(roles, message.MentionedRoleIds).Where(u => u != null))
            {
                returnValue = returnValue.Replace(role.Id.ToString(), role.Name);
            }
            foreach(var attachement in message.Attachments)
            {
                returnValue += Environment.NewLine + "\t" + $"Attachment: {attachement.Filename} - {attachement.Url}";
            }
            bool exclusionFunc(IEmbed e) => e.Type != EmbedType.Image && e.Type != EmbedType.Article && e.Type != EmbedType.Link;
            if (message.Embeds.Any(exclusionFunc))
            {
                returnValue += Environment.NewLine + "Embeds: " +Environment.NewLine;
                foreach (var embed in message.Embeds.Where(exclusionFunc))
                {
                    var sb = EmbedToString(embed);
                    
                    returnValue += sb;
                }
            }
            return returnValue;
        }

        private string EmbedToString(IEmbed embed)
        {
            var sb = new StringBuilder();
            if(!string.IsNullOrEmpty(embed.Title))
                sb.AppendLine($"{embed.Title}");
            if (!string.IsNullOrEmpty(embed.Description))
                sb.AppendLine($"\tDescription - {embed.Description.Replace(Environment.NewLine, Environment.NewLine + '\t')}");
            foreach(var field in embed.Fields)
            {
                sb.AppendLine($"\t{field.Name} - {field.Value.Replace(Environment.NewLine, Environment.NewLine + '\t')}");
            }

            return sb.ToString();
        }

        private IEnumerable<IRole> GetMentionedRoles(Dictionary<ulong, IRole> roles, IReadOnlyCollection<ulong> mentionedRoleIds)
        {
            foreach(var id in mentionedRoleIds)
            {
                if(!roles.ContainsKey(id))
                {
                    var role = Context.Guild.GetRole(id);
                    roles.Add(id, role);
                }
                yield return roles[id];
            }
        }

        private IEnumerable<IChannel> GetMentionedChannels(Dictionary<ulong, IChannel> channels, IReadOnlyCollection<ulong> mentionedChannelIds)
        {
            foreach (var id in mentionedChannelIds)
            {
                if (!channels.ContainsKey(id))
                {
                    var channel = Context.Client.GetChannel(id);
                    channels.Add(id, channel);
                }
                yield return channels[id];

            }
        }

        private IEnumerable<IUser> GetMentionedUsers(Dictionary<ulong, IUser> users, IReadOnlyCollection<ulong> mentionedUserIds)
        {
            foreach(var id in mentionedUserIds)
            {
                if(!users.ContainsKey(id))
                {
                    var user = Context.Guild.GetUser(id);

                    users.Add(id, user);

                }
                yield return users[id];

            }
        }

        const long FeedbackChannelId = 391696913080254474;
        [Command("Feedback", RunMode= RunMode.Async)]
        [Summary(@"This function allows you to ~~tell Pseudonym she has no idea what she is doing~~give feedback to the creator of the bot. Submitted feedback is anonymous unless you include personally identifying information.
Examples:
.Feedback Thanks for the bot, little buddy.")]
        public async Task FeedbackCommand([Summary("Feedback to post"), Remainder]string feedbackText = "")
        {

            var feedbackChannel = Context.Client.GetChannel(FeedbackChannelId) as IMessageChannel;
            
            if (feedbackChannel == null) return;
            //var messageSending = feedbackChannel.SendMessageAsync(feedbackText);
            //var acknowledge = Context.Message.AddReactionAsync(Emote.Parse(":thumbsup:"));

            await Task.WhenAll(feedbackChannel.SendMessageAsync(feedbackText), Context.Message.AddReactionAsync(new Emoji("👍")));
                
            
        }

        [Command("ChangePrefix", RunMode= RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary(@"Admin only function. Changes the bot prefix in case you have another bot using the period as a prefix. Leaving it blank defaults to the period prefix.
Examples:
.ChangePrefix &
&roll
&ChangePrefix
.roll")]
        public async Task ChangePrefix([Summary("New desired prefix"), Remainder]string newPrefix = ".")
        {
            var channel = Context.Channel as SocketGuildChannel;
            if(channel == null)
            {
                await Context.Message.AddReactionAsync(new Emoji("👎"));
                return;
            }
            PrefixUtility.SaveCustomPrefix(channel.Guild.Id, newPrefix);
            await Context.Message.AddReactionAsync(new Emoji("👍"));

        }

        private static readonly Regex ChannelIdFormat = new Regex(@"^<#(\d+)>$");

        [Command("ManageAnnouncements", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary(@"Allows an administrator to choose what happens to announcements. This command can either enable/disable announcements on your guild or choose the channel which announcements should appear inside.
Examples:
To see the current server configuration.
`.ManageAnnouncements`
To disable announcements.
`.ManageAnnouncements disable`
To enable announcements.
`.ManageAnnouncements enable`
To force announcements to go to a specific channel.
`.ManageAnnouncements #botSpam`
")]
        public async Task ManageAnnouncements([Summary("Channel Name or Enable or Disable"), Remainder]string newChannel = "")
        {
            var channel = Context.Channel as SocketGuildChannel;
            if (channel == null)
            {
                await Context.Message.AddReactionAsync(new Emoji("👎"));
                return;
            }
            var currentSettings = AnnouncementUtility.GetAnnouncementSettingsFromGuild(channel.Guild, Context.Client.CurrentUser.Id);
            //<#544611430402621442>
            
            if(ChannelIdFormat.IsMatch(newChannel))
            {
                currentSettings.ChannelId = ulong.Parse(ChannelIdFormat.Match(newChannel).Groups[1].Value);
                AnnouncementUtility.SaveCustomPreference(currentSettings);
            }
            else if (newChannel.ToUpperInvariant() == "ENABLE" || newChannel.ToUpperInvariant() == "DISABLE")
            {
                currentSettings.Enabled = newChannel.ToUpperInvariant() == "ENABLE";
                AnnouncementUtility.SaveCustomPreference(currentSettings);
            }
            else if (newChannel == string.Empty)
            {
                var announcementChannel = channel.Guild.TextChannels.FirstOrDefault(c => c.Id == currentSettings.ChannelId);
                await ReplyAsync($"Announcements are currently {(currentSettings.Enabled ? "enabled" : "disabled")} and would be recieved in {(announcementChannel != null ? $"{announcementChannel.Mention}." : "... nowhere because a valid channel isn't set.")}");
            }
            
            
            await Context.Message.AddReactionAsync(new Emoji("👍"));

        }







    }
}
