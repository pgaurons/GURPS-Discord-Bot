using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Discord.Model
{
    /// <summary>
    /// Represents a user that opted out and what they opted out of.
    /// </summary>
    [Serializable]
    public class OptOut
    {
        public ulong UserId { get; set; }
        public OptOutOptions Option { get; set; }  
    }

    [Flags]
    [Serializable]
    public enum OptOutOptions
    {
        [ChoiceDisplay("None - Enable everything.")]
        None = 0,
        [ChoiceDisplay("Commands - Opt out of text commands.")]
        Commands = 1,
        [ChoiceDisplay("Logging - Opt out of appearing in log dumps.")]
        Logging = 2,
        [ChoiceDisplay("All - Opt out of everything.")]
        All = 3
    }
}
