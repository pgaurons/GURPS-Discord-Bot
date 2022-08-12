using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Slash
{
    /// <summary>
    /// Base Module that includes functionality used by all interaction modules.
    /// </summary>
    public abstract class GurpsInteractionModuleBase : InteractionModuleBase
    {
        /// <summary>
        /// Splits up text into several chunks to prevent triggering discord spam throttler.
        /// </summary>
        /// <param name="linesToDisplayAtOnce">Number of lines to display</param>
        /// <param name="results">Text to disply as a collection of strings</param>
        /// <param name="privateMessageOverflow">Whether overflow goes to DMs.</param>
        /// <param name="lineDivider">Defaults to newline, if something different might be appropriate, like a tab or whatever.</param>
        /// <returns>Async task that displays text in appropriate channel.</returns>
        protected async Task SendTextWithTimeBuffers(int linesToDisplayAtOnce, IEnumerable<string> results, bool privateMessageOverflow, string lineDivider = "")
        {
            if (lineDivider == string.Empty) lineDivider = Environment.NewLine;
            var dmChannel = Context.User.CreateDMChannelAsync();
            const int maxMessageSize = 2000 - 30; //Let's give some buffer room.
            results = results.SelectMany(r => r.Length > maxMessageSize ? r.Split(lineDivider) : new[] { r }).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            IMessageChannel overflowChannel;
            IMessageChannel thisChannel = !Context.Interaction.IsDMInteraction ? 
                (await Context.Guild.GetTextChannelAsync(Context.Interaction.ChannelId.Value)) as IMessageChannel :
                await dmChannel;

            if (privateMessageOverflow)
            {
                overflowChannel = await dmChannel;
            }
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

        /// <summary>
        /// Displays an error in a channel.
        /// </summary>
        /// <param name="channel">Channel to write to.</param>
        /// <param name="message">Message to send.</param>
        /// <returns>A task that sends a message to a channel when complete.</returns>
        protected static async Task DisplayErrorMessage(IMessageChannel channel, string message)
        {
            await channel.SendMessageAsync($"{message}");
        }
    }
}
