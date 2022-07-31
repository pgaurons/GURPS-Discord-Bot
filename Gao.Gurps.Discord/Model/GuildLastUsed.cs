using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Discord.Model
{
    /// <summary>
    /// Represents The last time a guild used this bot.
    /// </summary>
    public class GuildLastUsed
    {
        /// <summary>
        /// The id of the guild in question.
        /// </summary>
        public ulong GuildId { get; set; }
        /// <summary>
        /// A timestamp of the last time the guild used this bot.
        /// </summary>
        public DateTime LastUsed { get; set; }
        /// <summary>
        /// The id of the owner of the guild. 
        /// </summary>
        /// <remarks>
        /// This is important, because for verification purposes, I need to try to lower the ratio of guild owners to total guilds.
        /// </remarks>
        public ulong OwnerId { get; set; }
    }
}
