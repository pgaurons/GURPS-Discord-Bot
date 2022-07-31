using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Discord.Model
{
    public enum ProbabilityChoice
    {
        [ChoiceDisplay("Ritual Path Magic")]
        Rpm = 0,
        [ChoiceDisplay("Quick Contest")]
        QuickContest = 1,
        [ChoiceDisplay("Regular Contest")]
        RegularContest = 2,
        [ChoiceDisplay("Success Rolls")]
        Success = 3
    }
}
