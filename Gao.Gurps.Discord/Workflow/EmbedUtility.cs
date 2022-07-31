using Discord;
using Gao.Gurps.Mechanic;
using Gao.Gurps.Model;
using Gao.Gurps.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Workflow
{
    public static class EmbedUtility
    {
        public static Embed GenerateHeroicBackgroundEmbed(HeroicBackground background)
        {
            var builder = new EmbedBuilder()
    .WithTitle("Heroic Background Generator")
    .WithUrl("http://www.warehouse23.com/products/SJG37-2704")
    .WithColor(new Color(0x8254AB))
    .WithThumbnailUrl("https://i.imgur.com/HKHBgff.png")
    .WithAuthor(author =>
    {
        author
            .WithName("Written by David L. Pulver; Art By 鼓膜住職")
            .WithUrl("http://www.pixiv.net/member.php?id=1907158")
            .WithIconUrl("https://i.imgur.com/Od23Arm.jpg");
    })
    .AddField("Origin", GenerateHeroicOriginString(background))
    .AddField("Parents and Mentor", GenerateHeroicParentString(background));
            if (background.Siblings.Any())
            {
                builder.AddField("Siblings", GenerateHeroicSiblings(background.Siblings) );
            }
            builder
            .AddField("Hardships", GetHeroicHardshipsText(background))
            .AddField("Birthright", GetHeroicBirthrightText(background.Inheritance));
            if(background.ExperienceLevel != Experience.None)
            {
                builder.AddField("The Adventure", GetHeroicExperienceText(background.ExperienceLevel));
            }
            
            var embed = builder.Build();
            return embed;
        }

        private static string GetHeroicExperienceText(Experience experienceLevel)
        {
            return $"You have been adventuring for some time, and you have experience due to your {experienceLevel.ToPreferredString()}, which will certainly help in the coming trials.";
        }

        private static object GetHeroicBirthrightText(BackgroundInheritance inheritance)
        {
            var sb = new StringBuilder();
            sb.Append("To help you on your journey, ");
            switch(inheritance.Type)
            {
                case InheritanceType.Tools:
                case InheritanceType.Deed:
                case InheritanceType.Transportation:
                    var article = inheritance.Type == InheritanceType.Deed ? "a" : "some";
                    sb.Append($"you have inherited {article} {inheritance.Type.ToPreferredString()}.");
                    break;
                case InheritanceType.Pet:
                    sb.Append($"you had a pet left in your care: a(n) {GetBackgroundCharacterDescription(inheritance.SupportingActor)}");
                    break;
                case InheritanceType.Companion:
                    sb.Append($"you have a sworn friend that has decided to go along with you: a(n) {GetBackgroundCharacterDescription(inheritance.SupportingActor)}");
                    break;
            }
            return sb.ToString();
            
        }

        private static string GetHeroicHardshipsText(HeroicBackground background)
        {
            var sb = new StringBuilder();
            if(background.Darkness.Type != DarknessType.DarknessFree)
            {
                sb.Append($"In your youth you encountered {background.Darkness.Type.ToPreferredString()}. ");// {and supporting details if necessary.}");
                switch(background.Darkness.Type)
                {
                    case DarknessType.Evildoer:
                        sb.Append($"It was a despicable {GetEvildoerString(background.Darkness.SupportingActor)} ");
                        sb.Append(GetDarkDeedsText(background.Darkness.DarkDeed) + " ");
                        break;
                    case DarknessType.TragicDeath:
                        sb.Append($"You had a cherished {GetLovedOneString(background.Darkness.SupportingActor)}. ");
                        sb.Append($"Unfortunately, one day they {background.Darkness.Disaster.ToPreferredString()} .");
                        break;
                    case DarknessType.DarkProphecy:
                        sb.Append($"The day you were born, a great oracle declared, unfortunately, `{background.Darkness.Prophecy.ToPreferredString()}.` ");
                        break;
                }
                
            }
            else
            {
                sb.Append("You have had a semi-charmed life, and have not known the sorrow of personal tragedy. ");
            }
            sb.AppendLine();
            sb.Append($"Nevertheless you continue to soldier on. You must, because now you are met with the responsibility of {background.Burden.Type.ToPreferredString()}. ");
            switch(background.Burden.Type)
            {
                case BurdenType.Betrothed:
                    sb.Append($"You are promised to marry {GetHeroicLoverText(background.Burden.SupportingActor)} ");
                    sb.Append($"This is unfortunately a problem because {GetHeroicMarriageObstacleText(background.Burden.MarriageObstacle)}");
                    break;
                case BurdenType.LostHeir:
                    sb.Append($"You must take your rightful place, but unfortunately you are waylaid by a wicked individual who wants to take you out of the picture: a(n) {GetEvildoerString(background.Burden.SupportingActor)}");
                    break;
                case BurdenType.Outlaw:
                    sb.Append($"This is due to {background.Burden.OutlawReason.ToPreferredString()}.");
                    break;
                case BurdenType.Dependant:
                    sb.Append($"You must care for a {GetLovedOneString(background.Burden.SupportingActor)}. ");
                    if (background.Burden.SupportingActor.LovedOneInformation.ActualAge != 0)
                        sb.Append($"They are currently {background.Burden.SupportingActor.LovedOneInformation.ActualAge} years old.");
                    else
                        sb.Append("They are in the twilight of their life, but you will personally see that they can live their last days in dignity and comfort.");
                    break;
            }
            return sb.ToString();
        }

        private static string GetHeroicMarriageObstacleText(MarriageObstacleInformation marriageObstacle)
        {
            var sb = new StringBuilder();
            sb.Append(marriageObstacle.Type.ToPreferredString() +". ");
            if(marriageObstacle.Type == MarriageObstacle.Captive)
            {
                sb.Append($"They are being held by an evildoer: {GetEvildoerString(marriageObstacle.SupportingActor)}");
            }
            return sb.ToString();
        }

        private static string GetHeroicLoverText(BackgroundCharacter supportingActor)
        {
            var sb = new StringBuilder();
            if(supportingActor.IsEvildoer)
            {
                sb.Append($"the child of an evildoer. Their parent? a(n) {supportingActor.Type.ToPreferredString()}");
            }
            else
            {
                sb.Append($"a(n) {GetBackgroundCharacterDescription(supportingActor, true)}");
            }
            return sb.ToString();
        }

        private static string GetLovedOneString(BackgroundCharacter supportingActor)
        {
            var sb = new StringBuilder();
            var loi = supportingActor.LovedOneInformation; //it's getting long to type this over and over.
            var typeText = GetLovedOneDescription(loi);

            if(supportingActor.Type != BackgroundCharacterType.LovedOne)
            {
                typeText += $" (also a {supportingActor.Type.ToPreferredString()})";
            }
            return typeText;
        }

        private static string GetDarkDeedsText(BackgroundDarkDeed darkDeed)
        {
            var sb = new StringBuilder();
            switch(darkDeed.Type)
            {
                case DarkDeedType.Relative:
                    sb.Append($"Your {GetLovedOneString(darkDeed.SupportingActor)} suffered {darkDeed.GrimFate.ToPreferredString()} at the evildoer's hands.");
                    break;
                case DarkDeedType.Ravage:
                    sb.Append($"Due to the ravaging and pillaging of your home, your {GetLovedOneString(darkDeed.SupportingActor)} suffered {darkDeed.GrimFate.ToPreferredString()}.");
                    break;
                case DarkDeedType.Duped:
                    sb.Append($"Your {GetLovedOneString(darkDeed.SupportingActor)} has fallen in with the evildoer, and you want to bring them back to your side.");
                    break;
                case DarkDeedType.Attacked:
                    sb.Append($"You personally suffered {darkDeed.GrimFate.ToPreferredString()} at the evildoer's hands.");
                    break;
                case DarkDeedType.Revenge:
                    sb.Append("Because you thwarted a plot, the evildoer has personally decided to become your nemesis.");
                    break;
            }
            return sb.ToString();
        }

        private static object GetEvildoerString(BackgroundCharacter supportingActor)
        {
            return GetBackgroundCharacterDescription(supportingActor);
        }

        private static string GenerateHeroicSiblings(List<BackgroundCharacter> siblings)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"You have or had {siblings.Count} sibling(s).");
            foreach(var sibling in siblings)
            {
                sb.AppendLine(GetSiblingString(sibling));
            }
            return sb.ToString();
        }

        

        private static string GenerateHeroicParentString(HeroicBackground background)
        {
            var sb = new StringBuilder();
            if(background.Parentage.OrphanStatus != BackgroundOrphanState.NotOrphan)
            {
                sb.AppendLine($@"You never knew your real parents because you were {background.Parentage.OrphanStatus.ToPreferredString()}. Nonetheless you had some who could stand in as parents.");
            }
            if(background.Parentage.ParentFigures.Any())
            {
                foreach(var parent in background.Parentage.ParentFigures)
                {
                    sb.AppendLine(GetParentString("parent", parent));
                }
                sb.AppendLine(GetParentString("mentor", background.Mentor));
            }
            return sb.ToString();
        }

        private static string GetParentString(string description, BackgroundCharacter parentFigure)
        {
            var livingText = parentFigure.IsAlive ? "surviving" : "(now deceased)";
            var livingArticle = parentFigure.IsAlive ? "is" : "was";
            var livingVerb = parentFigure.IsAlive ? "have" : "had";
            return $@"You {livingVerb} a {livingText} {description} who {livingArticle} a {GetBackgroundCharacterDescription(parentFigure)} Their feelings towards you when last you spoke could be described as {parentFigure.Attitude}.";
        }

        private static string GetSiblingString(BackgroundCharacter sibling)
        {
            string description = GetLovedOneDescription(sibling.LovedOneInformation);
            var livingText = sibling.IsAlive ? "surviving" : "(now deceased)";
            var livingArticle = sibling.IsAlive ? "is" : "was";
            var livingVerb = sibling.IsAlive ? "have" : "had";
            var returnValue = $@"You {livingVerb} a {livingText} {description}. Their feelings towards you when last you spoke could be described as {sibling.Attitude}.";
            return returnValue;
        }

        private static string GetLovedOneDescription(LovedOne character)
        {
            string returnValue = string.Empty;
            switch(character.Type)
            {
                case LovedOneType.Sibling:
                    returnValue = character.RelativeAge != RelativeAge.SameAge ?
(
(
 character.RelativeAge == RelativeAge.Older ? "Older " :
 character.RelativeAge == RelativeAge.Younger ? "Younger " : ""
) +
(
 character.Gender ? "brother" : "sister"
)
) :
character.Twin.ToPreferredString();
                    break;
                case LovedOneType.Aunt: returnValue = character.Gender ? "Uncle" : "Aunt"; break;
                case LovedOneType.Parent: returnValue = character.Gender ? "Father" : "Mother"; break;
                case LovedOneType.Grandparent: returnValue = character.Gender ? "Grandfather" : "Grandmother"; break;
                default: returnValue = character.Type.ToPreferredString(); break;

            }
            if (character.StepRelationship && (character.Type == LovedOneType.Parent || character.Type == LovedOneType.Sibling)) returnValue = "Step-" + returnValue;

            return returnValue;
        }

        private static string GetBackgroundCharacterDescription(BackgroundCharacter character, bool ignoreLover = false)
        {
            var sb = new StringBuilder();
            if (character.SpecialFeature != HumbleFolkSpecialFeature.None)
                sb.Append($"({character.SpecialFeature.ToPreferredString()}) ");
            if (character.IsGhost)
                sb.Append("ghost ");
            if (character.IsShapeShifter)
                sb.Append("shapeshifter that can turn into a ");
            if (character.Type != BackgroundCharacterType.LovedOne && character.LovedOneInformation == null)
                sb.Append(character.Type.ToPreferredString());
            else
                sb.Append(GetLovedOneDescription(character.LovedOneInformation));
            if (character.IsMentor)
                sb.Append(", who is or was a mentor");
            if (character.IsLover && !ignoreLover)
                sb.Append(", whom you deeply love(d)");
            return sb.ToString()+".";

        }

        private static string GenerateHeroicOriginString(HeroicBackground background)
        {
            var sb = new StringBuilder();
            sb.AppendLine($@"You were born in a {background.Birthplace.ToPreferredString()} in the {background.Region.ToPreferredString()} region.");
            sb.Append($@"At the time of your birth {background.Omen.Type.ToPreferredString()} occurred. ");
            
            switch(background.Omen.Type)
            {
                case OmenType.Animals: sb.AppendLine($@"A {background.Omen.SupportingActor.Type.ToPreferredString()} swarm appeared out of nowhere."); break;
                case OmenType.Monsters: sb.AppendLine($@"An infamous {background.Omen.SupportingActor.Type.ToPreferredString()} horde suddenly attacked."); break;
                case OmenType.MysteriousStranger: sb.AppendLine($@"Your home was visited by a(n) {GetBackgroundCharacterDescription(background.Omen.SupportingActor)}"); break;
            }

            switch(background.DistinguishingFeature)
            {
                case Feature.None: break;
                default: sb.AppendLine($@"The first thing people notice about you is your {background.DistinguishingFeature.ToPreferredString()}."); break;
            }
            return sb.ToString();
        }

        internal static Embed GenerateRandomCharacterEmbed(List<SelectionGroup> character)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Random Character Generator")
                .WithUrl("http://www.warehouse23.com/products/pyramid-number-3-slash-72-alternate-dungeons")
                .WithColor(new Color(0x66AA00))
                .WithThumbnailUrl("https://i.imgur.com/jRYFkxz.png")
                .WithAuthor(author =>
                {
                    author
                        .WithName("Pointless Slaying and Looting written by Sean Punch");
                });
            foreach(var selectionGroup in character)
            {
                if (!selectionGroup.AllowDuplicates)
                    builder.AddField(selectionGroup.Name, selectionGroup.Options.Select(o => o.ToString()).OrderBy(o => o).Aggregate((accum, current) => accum + Environment.NewLine + current).Trim());
                else
                    //Aggregate the fields by count.
                    builder.AddField(selectionGroup.Name, selectionGroup.Options.Select(o => o.ToString()).OrderBy(o => o).Aggregate((accum, current) => accum + Environment.NewLine + current).Trim()); 

            }


            var embed = builder.Build();
            return embed;
        }

        internal static Embed GenerateQuickContestABunchEmbed(IEnumerable<QuickContestResult> results)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Quick Contest a Bunch")
                .WithColor(new Color(0xAA6600))
                .WithImageUrl("https://i.imgur.com/ijhHTCD.jpg");

            var resultsText = results.Select(r => (r.MarginOfVictory * (r.SecondContestantIsWinner ? -1 : 1)).ToString("+0;-#")).Aggregate((a, b) => a + "; " + b);
            if (resultsText.Length < 1000)
                builder.AddField("Results", resultsText);
            builder.AddField("Wins", results.Count(r => r.FirstContestantIsWinner));
            builder.AddField("Losses", results.Count(r => r.SecondContestantIsWinner));
            if(results.Any(r => r.IsTie))
            {
                builder.AddField("Ties", results.Count(r => r.IsTie));
            }

            var embed = builder.Build();
            return embed;
        }

        internal static Embed GenerateBrokenBlade(BrokenBladeStatistics results)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Broken Blade Stats")
                .WithColor(new Color(0x6600AA)).
                //.WithImageUrl("https://i.imgur.com/haBPaql.png").
                WithUrl("http://www.warehouse23.com/products/pyramid-number-3-slash-87-low-tech-iii").
                WithAuthor(author =>
                {
                    author
                        .WithName("Original System by Douglas Cole");
                });

            
            builder.AddField("Weapon Strength", results.WeaponStrength);
            builder.AddField("Weapon Quality", results.Quality.ToPreferredString());
            if (results.Cost != 0) builder.AddField("Original Price", results.Cost.ToString("C"));
            var otherQualities = results.IsHafted ? "Hafted" : "";
            otherQualities += results.IsSymmetrical ? results.IsHafted ? ", Symmetrical" : "Symmetrical" : "";
            if (!string.IsNullOrWhiteSpace(otherQualities))
                builder.AddField("Other Traits", otherQualities);
            builder.AddField("Attack", $@"```
Breakage Threshold - {results.AttackBreakageThreshold}
      Safety Limit - {results.AttackSafetyLimit}
```");
            builder.AddField("Defense", $@"```
Breakage Threshold - {results.DefenseBreakageThreshold}
      Safety Limit - {results.DefenseSafetyLimit}
```");
            builder.AddField("Failure Increment", results.FailureIncrement);
            builder.AddField("HT", results.Health);
            builder.AddField("DR", results.DamageResistance);
            builder.AddField("Edge HT", results.EdgeHealth);

            if(results.RepairCosts.Any())
            {
                builder.AddField("Repair Skill Modifier", results.ArmoryModifier);
                builder.AddField("Repair Costs", GenerateRepairCostString(results.RepairCosts));
            }

            var embed = builder.Build();
            return embed;
        }

        private static object GenerateRepairCostString(RepairCost[] repairCosts)
        {
            const int htWidth = 3;
            const int costWidth = 15;
            const int durationWidth = 33;
            return $@"```
{"HT",htWidth}{"Cost",costWidth}{"Repair Time",durationWidth}" + 
                Environment.NewLine +
                repairCosts.Select(rc => $"{rc.NewHealthLevel,htWidth}{rc.Cost.ToString("C"),costWidth}{NiceTimeFormat(rc.Time),durationWidth}").
                Aggregate((a,s) => a + Environment.NewLine + s) + 
                "```";
        }

        private static string NiceTimeFormat(TimeSpan time)
        {
            var days = time.Days;
            var hours = time.Hours == 18 ? 4 : time.Hours;
            var weeks = 0;
            if(days > 7)
            {
                weeks = days / 7;
                days = days % 7;
            }
            var sb = new StringBuilder();
            if(weeks > 0)
            {
                sb.Append($"{weeks} week" + (weeks > 1 ? "s" : ""));
                if (days + hours > 0) sb.Append(", ");
            }
            if(days > 0)
            {
                sb.Append($"{days} day" + (days > 1 ? "s" : ""));
                if (hours > 0) sb.Append(", ");
            }
            if(hours > 0)
            {
                
                sb.Append($"{hours} hour" + (hours > 1 ? "s" : ""));
            }
            return sb.ToString();
        }

        public static string NiceTimeFormatLastGasp(TimeSpan time)
        {
            var days = time.Days;
            var hours = time.Hours;
            var weeks = 0;
            var seconds = time.Seconds;
            if (days > 7)
            {
                weeks = days / 7;
                days = days % 7;
            }
            var minutes = time.Minutes;
            var sb = new StringBuilder();
            if (weeks > 0)
            {
                sb.Append($"{weeks} week" + (weeks > 1 ? "s" : ""));
                if (days + hours + minutes + seconds > 0) sb.Append(", ");
            }
            if (days > 0)
            {
                sb.Append($"{days} day" + (days > 1 ? "s" : ""));
                if (hours + minutes + seconds > 0) sb.Append(", ");
            }
            if (hours > 0)
            {

                sb.Append($"{hours} hour" + (hours > 1 ? "s" : ""));
                if (minutes + seconds > 0) sb.Append(", ");
            }
            if(minutes > 0)
            {
                sb.Append($"{minutes} minute" + (minutes > 1 ? "s" : ""));
                if (seconds > 0) sb.Append(", ");
            }
            if(seconds > 0 || sb.ToString() == "")
            {
                sb.Append($"{seconds} second" + (seconds != 1 ? "s" : ""));
            }
            return sb.ToString();
        }

        internal static Embed LastGaspEmbed(LongTermFatigueResults result)
        {
            EmbedBuilder builder = new EmbedBuilder()
        .WithTitle($"{(result.RecoveryRate >= FatigueRecoveryRate.Severe ? "*Heavy Breathing* " : string.Empty)}You have **{result.CurrentFatiguePoints}/{result.TotalFatiguePoints} FP**!")
        .WithDescription("*Icon by [Grouse01](https://www.pixiv.net/member.php?id=2700884).*")
        .WithColor(new Color(0xB71E20))
        .WithFooter(footer =>
        {
            footer
                .WithText("Last Gasp" + (result.HighDefinition ? " High-Resolution ST Loss" : string.Empty));
        })
        .WithThumbnailUrl("https://i.imgur.com/iq3Yj93.png");
            if (result.Penalty)
            {
                builder
        .AddField("ST Penalty", result.StrengthPenalty.ToString("P2"))
        .AddField("DX/IQ/HT Penalty", result.RegularPenalty);
            }
            else
            {
                builder
        .AddField("ST", $"{result.NewStrength}/{result.OriginalStrength}")
        .AddField("DX", $"{result.NewDexterity}/{result.OriginalDexterity}")
        .AddField("IQ", $"{result.NewIntelligence}/{result.OriginalIntelligence}")
        .AddField("HT", $"{result.NewHealth}/{result.OriginalHealth}");
            }

            builder.AddField("Fatigue Level", result.RecoveryRate);
            builder.AddField("Time to Recover 1 Mild FP", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.HoursPerFatiguePointMildPresentation)));
            if (result.CurrentFatiguePoints != result.TotalFatiguePoints && result.HoursPerFatiguePointMildPresentation != result.TotalRecoveryTimeInHoursMild)
            {
                builder.AddField($"Total Mild Recovery Time ({result.MildFatigueToRecover} FP)", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.TotalRecoveryTimeInHoursMild)));
            }
            if (result.HoursPerFatiguePointSeverePresentation != result.TotalRecoveryTimeInHoursSevere)
                builder.AddField("Time to Recover 1 Severe FP", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.HoursPerFatiguePointSeverePresentation)));
            if (result.TotalRecoveryTimeInHoursSevere > 0)
            {
                builder.AddField($"Total Severe Recovery Time ({result.SevereFatigueToRecover} FP)", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.TotalRecoveryTimeInHoursSevere)));
            }
            if (result.HoursPerFatiguePointDeepPresentation != result.TotalRecoveryTimeInHoursDeep)
                builder.AddField("Time to Recover 1 Deep FP", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.HoursPerFatiguePointDeepPresentation)));
            if (result.TotalRecoveryTimeInHoursDeep > 0)
            {
                builder.AddField($"Total Deep Recovery Time ({result.DeepFatigueToRecover} FP)", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.TotalRecoveryTimeInHoursDeep)));
            }
            if (result.TotalRecoveryTimeInHoursSevere > 0)
            {
                builder.AddField($"Total Recovery Time ({result.TotalFatiguePoints - result.CurrentFatiguePoints} FP)", NiceTimeFormatLastGasp(TimeSpan.FromHours((double)result.TotalRecoveryTimeInHours)));
            }
            var embed = builder.Build();
            return embed;
        }

        internal static Embed TreasureGeneratorEmbed(Treasure treasure)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Treasure Generator")
                .WithColor(new Color(0x9A2D4E)).
                WithThumbnailUrl("https://i.imgur.com/8IyUJ3v.png").
                WithFooter("Art from https://www.pixiv.net/artworks/11507819");

            builder.AddField("Name", treasure.Name);
            //if (treasure.Curses.Any()) 
            //    builder.AddField("Curses", treasure.Curses.Aggregate((a, b) => a + ", " + b));
            //if(treasure.Enchantments.Any())
            //    builder.AddField("Enchantments", treasure.Enchantments.Select(e => $"{e.Name}-{e.SkillLevel}").Aggregate((a, b) => a + ", " + b));
            //if(!string.IsNullOrEmpty(treasure.Origin))
            //    builder.AddField("Origin", treasure.Origin);
            //if (treasure.Quantity > 1)
            //    builder.AddField("Quantity", treasure.Quantity);
            //if (!string.IsNullOrEmpty(treasure.Size))
            //    builder.AddField("Size", treasure.Size);
            //if (treasure.SizeModifier != 0)
            //    builder.AddField("Size Modifier", treasure.SizeModifier);
            //if(treasure.)
            builder.AddField("Description", treasure.Description);

            return builder.Build();
        }

        internal static Embed InfiniteWorldEmbed(InfiniteWorld universe)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Random Alternate World")
                .WithColor(new Color(0x9A2D4E)).
                //WithImageUrl("http://i.imgur.com/JoJIDLH.jpg").
                WithThumbnailUrl("http://i.imgur.com/JoJIDLH.jpg").
                WithUrl("http://www.sjgames.com/gurps/books/infiniteworlds/").
                WithFooter("Infinite Worlds written by Kenneth Hite, Steve Jackson, and John M. Ford." +Environment.NewLine + "Art from https://chan.sankakucomplex.com/post/show/163590");


            builder.AddField("Quantum", $"Q{universe.Quantum}");
            var worldType = universe.WorldType.ToPreferredString();
            switch(universe.WorldType)
            {
                case RandomWorldType.Empty: worldType += $"- {universe.EmptyWorldType.ToPreferredString()}"; break;
                case RandomWorldType.Parallel:
                    worldType = universe.ParallelWorldType.ToPreferredString() + " Parallel";
                    if (universe.ParallelWorldType == ParallelWorldType.Close)
                        worldType += $" ({universe.CloseParallelWorldType.ToPreferredString()})";
                    break;
            }

            builder.AddField("World Type", worldType);
            if(universe.WorldType == RandomWorldType.Echo)
            {
                builder.AddField("Current Year", Math.Abs(universe.EchoWorldPresent) + " " + (universe.EchoWorldPresent < 0 ? "B.C." : "A.D."));
            }
            if(universe.WorldType == RandomWorldType.Parallel && universe.ParallelWorldType == ParallelWorldType.Farther)
            {
                var techLevel = universe.TechLevel.ToString();
                if (universe.TechnologyVariant == TechnologyVariant.DivergentTechnology)
                    techLevel = universe.TechLevelDivergencePoint + "+" + (universe.TechLevel - universe.TechLevelDivergencePoint);
                techLevel += universe.TechLevel == 9 ? "+" : "";
                if(universe.TechnologyVariant == TechnologyVariant.SplitTechnology)
                {
                    foreach(var tech in universe.SplitTechLevels)
                    {
                        techLevel += Environment.NewLine + "**"+tech.Key.ToPreferredString() + "**-TL" + tech.Value;
                    }
                    techLevel += universe.HasAirships ? Environment.NewLine + "(Airship Technology)" : "";
                }
                builder.AddField("Technology Level", "TL" + techLevel);

                var variantType = universe.TechnologyVariant.ToPreferredString();
                if (new[] { TechnologyVariant.Magic, TechnologyVariant.Psionics, TechnologyVariant.Supers}.Contains(universe.TechnologyVariant))
                {
                    variantType += $" ({universe.TechnologyVariantOpenness.ToPreferredString()})";
                }
                if (!new[] { TechnologyVariant.DivergentTechnology, TechnologyVariant.SplitTechnology }.Contains(universe.TechnologyVariant)) //Redundant.
                    builder.AddField("Technology Variant Type", variantType);
                if(universe.TechnologyVariant == TechnologyVariant.Superscience)
                {
                    var superscienceField = universe.SuperscienceTechnology.ToPreferredString();
                    superscienceField += universe.TechnologyFromAncientAstronauts ? " (From Aliens or Ancient Astronauts)" : "";
                    builder.AddField("Superscience Tech", superscienceField);
                }

                foreach(var civilization in universe.MajorCivilizations)
                {
                    var title = $"Major Civilization - {civilization.Civilization.ToPreferredString()}";
                    var content = $"**Unity** - {civilization.Unity.ToPreferredString()}";
                    if (civilization.Fragmenting) content += " (Fragmenting)";
                    if (civilization.NumberOfPowers != 1)
                        content += Environment.NewLine + $"**Number of Powers** - {civilization.NumberOfPowers}";
                    foreach(var government in civilization.PowerGovernments)
                    {
                        content += Environment.NewLine + $"*Government Type* - {government.ToPreferredString()}";
                    }
                    builder.AddField(title, content);
                }
            }
            if(universe.WorldType == RandomWorldType.Empty && universe.EmptyWorldType == EmptyWorldType.DisasterWorld)
            {
                builder.AddField("Disaster", universe.DisasterType.ToPreferredString());
            }

            var embed = builder.Build();
            return embed;
        }

        internal static Embed GenerateRolledStatsEmbed(Character result)
        {
            var builder = new EmbedBuilder()
                .WithTitle($"Random Stats Roller - {result.SpentPoints} Points")
                .WithColor(new Color(0xB04E5D))
                .WithThumbnailUrl("https://i.imgur.com/jRYFkxz.png");
            builder.AddField("ST", $"{result.BasicStrength}[{(result.BasicStrength-10)*10}]");
            builder.AddField("DX", $"{result.BasicDexterity}[{(result.BasicDexterity - 10) * 20}]");
            builder.AddField("IQ", $"{result.BasicIntelligence}[{(result.BasicIntelligence - 10) * 20}]");
            builder.AddField("HT", $"{result.BasicHealth}[{(result.BasicHealth - 10) * 10}]");
            builder.AddField("Will", $"{result.Will}[{result.WillModification * 5}]");
            builder.AddField("Perception", $"{result.Perception}[{result.PerceptionModification * 5}]");
            builder.AddField("HP", $"{result.MaximumHitPoints}[{result.HitPointModification*2}]");
            builder.AddField("FP", $"{result.MaximumFatiguePoints}[{result.FatigueModification * 3}]");
            builder.AddField("Basic Speed", $"{result.BasicSpeed:0.00}[{result.BasicSpeedModification * 5}]");
            builder.AddField("Basic Move", $"{result.BasicMove}[{result.BasicMoveModification * 5}]");
            if (result.UnspentPoints > 0)
                builder.AddField("Unspent Points", result.UnspentPoints);

            var embed = builder.Build();
            return embed;
        }

        internal static Embed GenerateJumpEmbed(Jump results, bool isImperial)
        {
            var builder = new EmbedBuilder()
                .WithTitle($"Jump Stats")
                .WithColor(new Color(0xAD6F84))
                .WithThumbnailUrl("https://i.imgur.com/Za1CnrD.png").
                WithFooter("Art from https://www.pixiv.net/member_illust.php?mode=medium&illust_id=29801075");
            builder.AddField("Basic Move", results.BasicMove);
            if (results.SuperJumpLevel > 0)
                builder.AddField("Super Jump Level", results.SuperJumpLevel);
            if (results.EnhancedMoveLevel > 0)
                builder.AddField("Enhanced Move Level", results.EnhancedMoveLevel);
            builder.AddField("Standing Jump Height", OutputLength(results.StandingHighJumpHeight, 'i', isImperial));
            builder.AddField("Running Jump Height", OutputLength(results.RunningHighJumpHeight, 'i', isImperial));
            builder.AddField("Standing Jump Distance", OutputLength(results.StandingLongJumpDistance, 'f', isImperial));
            builder.AddField("Running Jump Distance", OutputLength(results.RunningLongJumpDistance, 'f', isImperial));

            var embed = builder.Build();
            return embed;
        }

        private static string OutputLength(int scalar, char unit, bool isImperial)
        {

            return isImperial ? OutputImperialLength(ref scalar, ref unit) :
                OutputMetricLength(ref scalar, ref unit);
        }

        private static string OutputMetricLength(ref int scalar, ref char unit)
        {
            const decimal inchesInGurpsMeter = 36m; //1 meter = 1 yard
            const decimal feetInGurpsMeter = 3m;
            var result =
                unit == 'i' ? scalar / inchesInGurpsMeter : scalar / feetInGurpsMeter;

            return result < 1m ?
                (result * 100m).ToString("## centimeters") :
                (result).ToString("#.## meters");
        }

        private static string OutputImperialLength(ref int scalar, ref char unit)
        {
            const int inchesInFoot = 12;
            const int feetInYards = 3;
            var inches = 0;
            var feet = 0;
            var yards = 0;
            if (unit == 'i')
            {
                inches = scalar;
                if (inches >= inchesInFoot)
                {
                    unit = 'f';
                    scalar = inches / inchesInFoot;
                    inches %= inchesInFoot;
                }

            }
            if (unit == 'f')
            {
                feet = scalar;
                if (feet >= feetInYards)
                {
                    yards = feet / feetInYards;
                    feet %= feetInYards;
                }
            }
            var inchUnit = inches > 1 ? "inches" : "inch";
            var feetUnit = feet > 1 ? "feet" : "foot";
            var yardUnit = yards > 1 ? "yards" : "yard";
            var sb = new StringBuilder();
            if (yards > 0) sb.Append($"{yards} {yardUnit}, ");
            if (feet > 0) sb.Append($"{feet} {feetUnit}, ");
            if (inches > 0) sb.Append($"{inches} {inchUnit}");
            return sb.ToString().Trim(' ', ',');
        }
    }
}
