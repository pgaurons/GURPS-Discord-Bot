using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Discord.Model
{
    public enum LookupType
    {
        Book,
        Skill,
        Equipment,
        Item,
        Spell,
        Technique,
        Advantage,
        Disadvantage,
        Quirk,
        Perk,
        Pyramid,
        Index,
        [ChoiceDisplay("Hit Location")]
        HitLocation,
        [ChoiceDisplay("FAQ")]
        Faq
    }
}
