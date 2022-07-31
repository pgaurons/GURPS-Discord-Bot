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
    public class RandomModule : InteractionModuleBase
    {

        [SlashCommand("probability", "Calculates the probabilities of various rolls.", runMode: RunMode.Async)]
        [CommandRemarks(@"The current options for the probability calculator are as follows:
QuickContest - Tells the odds of succeeding a quick contest. expects two integers.
RegularContest - Tells the odds of succeeding at a regular contest, given two integers representing two participants' skill levels. Automatically normalizes very high or low skill levels.
RPM - Does a Monte Carlo simulation (not 100% accurate but accurate enough) to give averages and metrics with regards to RPM.
Success - Tells the odds of Critical Success, Success, Failure, and Critical Failure given a skill level. Regular and critical results are not cumulative; overall success is the sum of the odds of regular success and critical success.
")]
        public async Task CalculateProbability(ProbabilityChoice probabilityType, string probabilityArguments)
        {

            var twoIntegers = new Regex(@"(\d+) (\d+)");
            var oneInteger = new Regex(@"(\d+)");
            EnergyGatheringParameters parsedParameters;
            switch (probabilityType)
            {
                case ProbabilityChoice.Rpm:
                    parsedParameters = EnergyGather.Parse(probabilityArguments);
                    var results = Probability.MonteCarloEnergyGather(parsedParameters);
                    var builder = new EmbedBuilder().
                        WithTitle("RPM Probabilities").
                        WithColor(new Color(0x967bb6)).
                        WithThumbnailUrl("https://i.imgur.com/0YWseJt.png").
                        WithAuthor(author =>
                        {
                            author.WithName("Art by 馬典（バテン）").
                            WithUrl("https://www.pixiv.net/member.php?id=1158215");
                        });
                    builder.AddField("Probability Of Succeeding", results.OddsOfSuccess.ToString("p2"));
                    builder.AddField("Probability Of Succeeding Without Quirks", results.OddsOfSuccessWithoutQuirks.ToString("p2"));
                    builder.AddField("Average Success Time", results.AverageSuccessTime.HasValue ? EmbedUtility.NiceTimeFormatLastGasp(results.AverageSuccessTime.Value) + $" (σ = {EmbedUtility.NiceTimeFormatLastGasp(results.SuccessTimeSigma.Value)})" : " - ");
                    builder.AddField("Average Number of Quirks", results.AverageQuirkCount.HasValue ? results.AverageQuirkCount.Value.ToString("0.##") + $" (σ = {results.QuirkSigma:0.##})" : "-");
                    builder.AddField("Average Size of Failure Botch", results.AverageBotchEnergyLevel.HasValue ? results.AverageBotchEnergyLevel.Value.ToString("0.##") + $" (σ = {results.BotchSigma:0.##})" : "-");
                    builder.AddField("Average Failure Time", results.AverageFailureTime.HasValue ? EmbedUtility.NiceTimeFormatLastGasp(results.AverageFailureTime.Value) + $" (σ = {EmbedUtility.NiceTimeFormatLastGasp(results.FailureTimeSigma.Value)})" : "-");
                    var embed = builder.Build();
                    await Context.Interaction.RespondAsync(string.Empty, embed: embed);
                    break;
                case ProbabilityChoice.QuickContest:

                    if (twoIntegers.IsMatch(probabilityArguments))
                    {
                        var match = twoIntegers.Match(probabilityArguments);
                        var mySkill = int.Parse(match.Groups[1].Value);
                        var theirSkill = int.Parse(match.Groups[2].Value);
                        var myOdds = Probability.CalculateOddsOfQuickContest(mySkill, theirSkill);
                        var theirOdds = Probability.CalculateOddsOfQuickContest(theirSkill, mySkill);
                        await Context.Interaction.RespondAsync($@"
        Probability of contestant 1 winning is {myOdds:p2}.
        Probability of contestant 2 winning is {theirOdds:p2}.
        Probability of a tie is {1 - myOdds - theirOdds:p2}.");
                    }
                    else
                    {
                        await Context.Interaction.RespondAsync($"I don't understand `{probabilityArguments}` nyaa~.");
                    }
                    break;
                case ProbabilityChoice.RegularContest:
                    if (twoIntegers.IsMatch(probabilityArguments))
                    {
                        var match = twoIntegers.Match(probabilityArguments);
                        var mySkill = int.Parse(match.Groups[1].Value);
                        var theirSkill = int.Parse(match.Groups[2].Value);
                        var odds = Probability.CalculateOddsOfRegularContest(mySkill, theirSkill);
                        await Context.Interaction.RespondAsync($@"
        ```
        Contestant 1 Used Skill: {odds.MySkill}
        Contestant 2 Used Skill: {odds.TheirSkill}
        Contestant 1 has {odds.MyOddsOfWinningAnIteration:p2} chance of winning an iteration and {odds.MyOddsOfWinningTheContest:p2} chance of winning the contest.
        Contestant 2 has {odds.TheirOddsOfWinningAnIteration:p2} chance of winning an iteration and {odds.TheirOddsOfWinningTheContest:p2} chance of winning the contest.
        The contest is {odds.OddsOfCompletingByLikelyIterations:p2} likely to be completed in {odds.LikelyIterationsToCompletion} iterations.
        ```");
                    }
                    else
                    {
                        await Context.Interaction.RespondAsync($"I don't understand `{probabilityArguments}` nyaa~.");
                    }
                    break;

                case ProbabilityChoice.Success:
                    if (oneInteger.IsMatch(probabilityArguments))
                    {
                        var match = int.Parse(oneInteger.Match(probabilityArguments).Groups[1].Value);
                        var odds = Probability.CalculateOddsForSuccessRoll(match);
                        await Context.Interaction.RespondAsync($@"```
           Skill: {match}
Critical Success: {odds.CriticalSuccessOdds:p2}
         Success: {odds.SuccessOdds:p2}
         Failure: {odds.FailureOdds:p2}
Critical Failure: {odds.CriticalFailureOdds:p2}```");

                    }
                    else
                    {
                        await Context.Interaction.RespondAsync($"I don't understand `{probabilityArguments}` nyaa~.");
                    }
                    break;
                default:
                    await Context.Interaction.RespondAsync($"{probabilityType} is not a supported type.");
                    return;
            }

        }

        [SlashCommand("criticalhit", "Rolls on the critical hit table", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls on the critical hit table. Add the word head if you want to roll on the head critical hit table.
Examples:
.CriticalHit
.ch head")]
        public async Task CriticalHit(CriticalHitType type = CriticalHitType.Normal)
        {

            var validType = true;


            if (validType)
            {
                var sb = new StringBuilder();

                var result = LookupTables.GetCriticalHitResult(type);
                sb.AppendLine($"{result}");

                await Context.Interaction.RespondAsync(sb.ToString());
            }
            else
            {
                await Context.Interaction.RespondAsync($"Can't parse '{type}'.");
            }
        }

        [SlashCommand("criticalmiss", "Rolls on critical miss table", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls on the critical miss table. Add the word unarmed if you want to roll on the unarmed critical miss table.
Examples:
.CriticalMiss
.CriticalFailure
.cm unarmed")]
        [CommandRemarks(@"The table allows the following variations.
`Celtic` - Celtic Magical Critical Failures
`Clerical` - Clerical Magical Critical Failures
`Comedy` - Comedy Magical Critical Failures
`Diabolic` - Diabolic Magical Critical Failures
`Illusory` - Illusory Magical Critical Failures
`Magic` - The Critical Failure table from GURPS Magic
`Normal` - (default) Normal critical miss table for combat with a melee weapon
`Oriental` - Oriental Magical Critical Failures
`RealityWarping` - Reality-Warping Magical Critical Failures
`Spirit` - Spirit-Oriented Magical Critical Failures
`Unarmed` - Unarmed critical miss table for melee combat
")]
        public async Task CriticalMiss(CriticalMissType type = CriticalMissType.Normal)
        {
            var validType = type != CriticalMissType.Undefined;

            if (validType)
            {
                var sb = new StringBuilder();

                var result = LookupTables.GetCriticalMissResult(type);
                sb.AppendLine($"{result}");

                await Context.Interaction.RespondAsync(sb.ToString());
            }
            else
            {
                await Context.Interaction.RespondAsync($"Can't parse '{type}'.");
            }
        }

        [SlashCommand("dungeon-fantasy-slam", "Calculate slam damage using Dungeon Fantasy rules", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates the slam damage in dice, and also immediately executes the results telling how much damage both parties might lose and whether anyone falls down.
Parameters in order are velocity (yards/second) Slammer striking strength, and target striking strength.
Examples:
For a move 6 slam executed by one character with 11 Striking ST against another with 10
.Slam 6 11 10
")]
        public async Task DungeonFantasySlam([Summary("Velocity")] int velocity, [Summary("Slammer-Striking-ST")] int slammerStrikingStrength, [Summary("Target-Striking-ST")] int targetStrikingStrength)
        {
            var result = Mechanic.Slam.ExecuteDungeonFantasy(velocity, slammerStrikingStrength, targetStrikingStrength);
            var knockdownBit = (result.PartyOneAutomaticallyKnockedDown ?
                "The Slammer is knocked down." : result.PartyTwoAutomaticallyKnockedDown ?
                "The Target is knocked down." : result.PartyTwoRollsForKnockdown ?
                "The Target must roll to remain standing." : result.PartyOneRollsForKnockdown ?
                "The Slammer must roll to remain standing." :
                "Neither party is knocked down."
                );
            await Context.Interaction.RespondAsync($@"```
Slammer did {result.PartyOneDamage} which comes out to {result.PartyOneDamageResult}.
 Target did {result.PartyTwoDamage} which comes out to {result.PartyTwoDamageResult}.
{knockdownBit}
```");

        }


        [SlashCommand("dungeon-fantasy-throw", "Calculate throw damage via DF rules", runMode: RunMode.Async)]
        [CommandSummary(@"Given the appropriate parameters, calculates the parameters of distance and damage of a thrown object according to DFRPG.
Examples:
.dfthrow 0.5 10
.dfth 1 10
.dungeonfantasythrow 1 10 11")]
        public async Task DungeonFantasyThrow(double weight, uint liftingStrength, uint strikingStrength = uint.MaxValue, [Choice("Imperial", "IMPERIAL"), Choice("Metric", "METRIC")]string imperialOrMetric = "IMPERIAL")
        {
            var isMetric = imperialOrMetric.ToUpperInvariant().StartsWith("M");
            var weightUnit = isMetric ? "kilograms" : "pounds";
            var distanceUnit = isMetric ? "meters" : "yards";
            if (strikingStrength == uint.MaxValue) strikingStrength = liftingStrength;

            try
            {
                if (weight < 0)
                    throw new ArgumentOutOfRangeException();
                var returnValue = LookupTables.GetDungeonFantasyThrowingStatistics((int)liftingStrength, (int)strikingStrength, isMetric ? weight * 2 : weight);
                var normalizedDamage = returnValue.Damage.Normalize();
                var normalizedDamageString = normalizedDamage.Equals(returnValue.Damage) ? "" : $@"
Normalized Damage: {returnValue.Damage.Normalize().ToString()} = {Math.Max(0, Roller.Roll(returnValue.Damage.Normalize()).Sum())}";
                await Context.Interaction.RespondAsync($@"```
 Lifting Strength: {liftingStrength}
Striking Strength: {strikingStrength}
           Weight: {weight:#,###.##} {weightUnit}
            Range: {returnValue.MaxRange:#,###.##} {distanceUnit}
           Damage: {returnValue.Damage.ToString()} = {Math.Max(0, Roller.Roll(returnValue.Damage).Sum())}{normalizedDamageString}
```");
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync($"Weight exceeded maximum two handed lift or was less than 0.");
            }

        }

        private static Regex ExplosionParameterExpression = new Regex(
            @"^(?<ExplosionLevel>[1-3]?\b) *(?<ExplosionDamage>[1-9][0-9]*[dD](?:[+\-x][0-9]+)?) *(?<ShrapnelDamage>(?:\[[1-9][0-9]*[dD](?:[+\-x][0-9]+)?\])?) *(?<TargetDistances>[0-9 ]+)$");

        [SlashCommand("explosion", "Simulate an explosion quickly", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates the damage done by explosions and shrapnel. The minimum parameters are the damage of the explosion, and the distance in yards of at least one potential target.
examples:
To calculate damage for a normal 5d cr ex explosion for people standing 0, 1, 2, and 3 yards away from the explosion
`.explosion 5d 0 1 2 3`
If that explosion has the effect of explosion 2, meaning that damage dissipates from the center more slowly
`.ex 2 5d 0 1 2 3`
If that explosion also has 1d of cutting shrapnel.
`.ex 2 5d[1d] 0 1 2 3`")]
        public async Task Explosion(string explosionParameters)
        {
            await ExplosionWorkflow(explosionParameters, false);
        }

        private async Task ExplosionWorkflow(string explosionParameters, bool useGUHLT)
        {
            var valid = ExplosionParameterExpression.IsMatch(explosionParameters);
            if (!valid)
            {
                await Context.Interaction.RespondAsync($"Can't parse '{explosionParameters}'.");
                return;
            }
            var groups = ExplosionParameterExpression.Match(explosionParameters).Groups;
            //Process the possible parameters.
            //Explosion level, default is 1.
            var explosionLevelText = groups["ExplosionLevel"].Value;
            int.TryParse(explosionLevelText, out int explosionLevel);
            explosionLevel = Math.Max(explosionLevel, 1);
            //Explosion damage, no default.
            var explosionDamageText = groups["ExplosionDamage"].Value;
            //Shrapnel Damage.
            var shrapnelDamageText = groups["ShrapnelDamage"].Value.Replace("[", "").Replace("]", "");
            //Targets, distance away from the explosion.
            var targets = groups["TargetDistances"].Value.Split(' ').Select(s => int.Parse(s));

            //The parameters have been converted from text to usable values, so let's process them now.
            var results = Workflow.Explosion.Calculate(explosionLevel, explosionDamageText, shrapnelDamageText, targets, useGUHLT);
            await Context.Interaction.RespondAsync($"Explosion Results for: '{explosionParameters}'.");
            await SendTextWithTimeBuffers(5, TextUtility.FormatExplosionResults(results), false);
        }

        [SlashCommand("grandunifiedhitlocationexplosion", "Uses the GUHLT to simulate an explosion", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates the damage done by explosions and shrapnel. The minimum parameters are the damage of the explosion, and the distance in yards of at least one potential target.
This command uses the GUHLT table.
examples:
To calculate damage for a normal 5d cr ex explosion for people standing 0, 1, 2, and 3 yards away from the explosion
`.guhltex 5d 0 1 2 3`
If that explosion has the effect of explosion 2, meaning that damage dissipates from the center more slowly
`.guhltex 2 5d 0 1 2 3`
If that explosion also has 1d of cutting shrapnel.
`.guhltex 2 5d[1d] 0 1 2 3`")]
        public async Task GrandUnifiedHitLocationTableExplosion(string explosionParameters)
        {
            await ExplosionWorkflow(explosionParameters, true);
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

        [SlashCommand("fnord", @"You're not cleared for that.", runMode: RunMode.Async)]
        [CommandSummary(@"You're not cleared for that.")]
        public async Task Fnord(string fnord = "")
        {
            await FnordOrYiff();
        }

        [SlashCommand("yiff", @"You're not cleared for that.", runMode: RunMode.Async)]
        [CommandSummary(@"You're not cleared for that.")]
        public async Task Yiff(string yiff = "")
        {
            await FnordOrYiff();
        }

        private async Task FnordOrYiff()
        {
            if (Roller.Roll(1, 1000).First() == 1)
            {
                Roller.IncrementYiffs();
                await Context.Interaction.RespondAsync(new Emoji("😎").Name);
            }
            else
            {
                await Context.Interaction.RespondAsync("You're not cleared for that.");
            }
        }


        [SlashCommand("fall", "Calculates damage from falling", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates damage received after falling. By Default, it is assumed you are dealing with 1 G gravity, and hitting a hard surface.
Examples:
A 10 HP person falls 20 yards
.Fall 10 20
A 10 HP person falls 20 yards and hits a soft surface.
.Fall 10 20 Soft
A 10 HP person falls 20 yards, hits a soft surface and they are on the moon.
.F 10 20 Soft 0.165
")]
        [CommandRemarks("Terminal Velocity is kinda fuzzy even according to the basic set, but keep in mind that I'm not putting a ceiling on velocity in these calculations." +
    " I also am only checking if the first character in the hard/soft parameter is an S, so type whatever fun s words you like if you want to make the ground soft instead of hard.")]
        public async Task Fall(int hitPoints, int heightInYards, [Choice("Hard", "Hard"), Choice("Soft", "Soft")] string surfaceHardness = "Hard", [Summary("Gravity-in-Gs")] decimal gravity = 1m)
        {
            var softSurface = surfaceHardness.ToUpperInvariant().StartsWith("S");
            var result = Mechanic.Slam.FallDamage(heightInYards, hitPoints * (softSurface ? 1 : 2), gravity);
            await Context.Interaction.RespondAsync($@"The results are as follows.
```
    Height: {heightInYards} yards
  Velocity: {result.Velocity}
Hit Points: {hitPoints}
 
The Fall damage against a {(softSurface ? "soft" : "hard")} surface is {result.PartyOneDamage} which comes out to {result.PartyOneDamageResult} damage.
```");

        }


        [SlashCommand("frightcheck", "Roll on the frightcheck table", runMode: RunMode.Async)]
        [CommandSummary(@"Given the margin of failure on a failed fright check, rolls the fright check automatically. Optionally, add the word Fright, Awe, Despair, or Confusion after the number for different tables.
examples:
.FrightCheck 5
.fc 1
.fc 5 Awe
.frightCheck 2 Confusion")]
        public async Task FrightCheck(uint marginOfFailure, FrightCheckType type = FrightCheckType.Fright)
        {


            var sb = new StringBuilder();

            var result = LookupTables.GetFrightCheckResult((int)marginOfFailure, Roller.Roll().Sum(), type);
            sb.AppendLine(result);

            await Context.Interaction.RespondAsync(sb.ToString());


        }


        [SlashCommand("gatherenergy", "Gather energy via the Ritual Path Magic mechanics", runMode: RunMode.Async)]
        [CommandSummary(@"Gather Energy according to the rules of RPM. 
Example command usage: 
To gather 30 energy with 15 skill.
.GatherEnergy Path of Mind-15 30
To gather 30 energy with 15 skill and gathering occurs every 5 seconds.
.ge Path of Body-15 30 00:00:05
If you are in a hurry to gather 30 energy with 15 skill.
.ge 15 30 ")]
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
                await SendTextWithTimeBuffers(linesToDisplayAtOnce, results.VerboseOutput().Split(Environment.NewLine), false);
            else
                await Context.Interaction.RespondAsync(results.ToString());

        }


        [SlashCommand("generatecharacter", "Puts a bunch of things together from pointless slaying and looting", runMode: RunMode.Async)]
        [CommandSummary(@"Generates a random character.
Example usage:
`.GenerateCharacter`
`.gc`")]
        public async Task GenerateCharacter(string templateName = "PointlessSlayingAndLooting")
        {
            var template = CharacterGeneratorTemplate.PointlessSlayingAndLooting;
            var character = template.ProduceCharacter();

            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateRandomCharacterEmbed(character));
        }

        [SlashCommand("generateworld", "Generate a world using the Infinite Worlds rules.", runMode: RunMode.Async)]
        [CommandSummary(@"Generates a random world using tables from the `Infinte Worlds` source book. You may specify if generating the world from a Homeline or Centrum perspective; defaults to homeline.
Example usage:
`.GenerateWorld Homeline`
`.gw centrum`
`.gw`")]
        public async Task GenerateWorld([Summary("Originating-Perspective"), Choice("Homeline", "HOMELINE"), Choice("Centrum", "CENTRUM") ] string perspective = "HOMELINE")
        {
            var isHomeline = perspective.ToUpperInvariant() == "HOMELINE";
            InfiniteWorld universe;
            try
            {
                universe = InfinteWorldGenerator.Generate(isHomeline);
                await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.InfiniteWorldEmbed(universe));
            }
            catch (Exception ex)
            {
                await Context.Interaction.RespondAsync($"Caught {ex.GetType()} - {ex.Message}" + Environment.NewLine + ex.StackTrace);
                return;
            }


        }

        [SlashCommand("generate-treasure", "Makes a random treasure using the DF8 treasure generator web API.", runMode: RunMode.Async)]
        [CommandSummary(@"Generates a random treasure.
Example usage:
`.GenerateTreasure`
`.gt`")]
        [CommandRemarks("The functionality for the Dungeon Fantasy treasure generation is furnished by the webservice at https://df-treasure-generator.herokuapp.com/")]
        public async Task GenerateTreasure(string treasureType = "DungeonFantasy")
        {
            var treasure = await TreasureGenerator.Generate();
            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.TreasureGeneratorEmbed(treasure));
        }

        [SlashCommand("grand-unified-hit-location-table", "Rolls on the Grand Unified Hit Location Table.", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls for a random hit location using the popular house ruled Grand Unified Hit Location Table. http://forums.sjgames.com/showthread.php?t=109239 for more information")]
        [CommandRemarks(@"If an optional hit location is included, and is valid, then one can roll on one of the sub tables. The following are valid:
`Face`
`Arm`
`Leg`
`Chest`
`Abdomen`
`Hand`
`Foot`
`Neck`
")]
        public async Task GrandUnifiedHitLocationTable([Summary("Hit-Location"), 
            Choice("Normal", ""), 
            Choice("Abdomen", "abdomen"),
            Choice("Arm", "Arm"),
            Choice("Chest", "chest"),
            Choice("Face", "face"),
            Choice("Foot", "foot"),
            Choice("Hand", "hand"),
            Choice("Leg", "leg"),
            Choice("Neck", "neck")] string argument = "")
        {
            ComplexHitLocation location;
            switch (argument.Trim().ToUpperInvariant())
            {
                case "": location = LookupTables.GrandUnifiedHitLocationTable(); break;
                case "ABDOMEN": location = LookupTables.GrandUnifiedHitLocationAbdomen(); break;
                case "ARM": location = LookupTables.GrandUnifiedHitLocationArm(Gurps.Model.HitLocation.Arm); break;
                case "CHEST": location = LookupTables.GrandUnifiedHitLocationChest(); break;
                case "FACE": location = LookupTables.GrandUnifiedHitLocationFace(); break;
                case "FOOT":
                case "HAND": location = LookupTables.GrandUnifiedHitLocationExtremity(EnumerationHelper.StringTo<Gurps.Model.HitLocation>(argument)); break;
                case "LEG": location = LookupTables.GrandUnifiedHitLocationLeg(Gurps.Model.HitLocation.Leg); break;
                case "NECK": location = LookupTables.GrandUnifiedHitLocationNeck(); break;

                default:
                    await ReplyAsync($"`{argument}` is an invalid argument, desu nya.");
                    return;
            }

            var hitLocationInformation = LookupTables.FindHitLocation(location);
            await Context.Interaction.RespondAsync($"{hitLocationInformation}");

        }

        [SlashCommand("heroic-background", "Generates a Heroic Background according to the rules in Pyramid #3/104's Heroic Background Generator", runMode: RunMode.Async)]
        [CommandSummary(@"Generates a Heroic Background according to the rules in Pyramid #3/104's Heroic Background Generator, created by David L. Pulver.
Example usage:
`.heroicbackground`
`.hb`")]
        public async Task HeroicBackground()
        {
            var character = LookupTables.GenerateHeroicBackground();
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(HeroicBackground));
            //var result = string.Empty;
            //using (var textWriter = new StringWriter())
            //{
            //    xmlSerializer.Serialize(textWriter, character);
            //    result = textWriter.ToString();
            //}


            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateHeroicBackgroundEmbed(character));

        }

        [SlashCommand("hit-location", "Roll on the hit location table", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls for a random hit location.
Examples:
Default hit location table
.HitLocation
Roll on the quadruped table.
.hl quadruped
Roll using the rules from basic set only.
.hl humanoid false")]
        [CommandRemarks(@"The following tables are implemented:
`Humanoid`: (default) Stands on two legs, has two arms, and normal vital organs.
`Winged Humanoid`: Like a humanoid... with wings.
`Fish-Tailed Humanoid`: Like a mermaid.
`Quadruped`: An animal that walks along on four legs.
`Winged Quadruped`: A flying quadruped.
`Hexapod`: Six legged entity, like an insect.
`Winged Hexapod`: Like a flying insect.
`Centaur`: A humanlike torso replace from the hips down with a quadruped like torso.
`Avian`: A bird with two legs and wings.
`Vermiform`: Something that crawls without legs, like a snake.
`Winged Serpent`: Flying Vermiform.
`Snake Man`: Like a snake with arms.
`Octopod`: An eight armed entity.
`Squid`: Like an octopod with some arms specialized.
`Cancroid`: Arthropods with legs and huge claws, like crabs.
`Scorpion`: Like a cancroid, but with a tail.
`Ichthyoid`: Morphology suited to swimming, like a fish.
`Arachnoid`: Terrestial eight-legged animals, like spiders.
")]
        public async Task HitLocation(DiscordHlc hitLocationTable = DiscordHlc.Humanoid, bool useComplexHitLocations = true)
        {

            var tableType = EnumerationHelper.StringTo(hitLocationTable.ToString(), ModelHlc.Undefined);
            if (tableType == ModelHlc.Undefined)
            {
                await Context.Interaction.RespondAsync($"I can't find a table type of {hitLocationTable}, nyaah~.");
                return;
            }

            var location = LookupTables.GetHitLocation(tableType, Roller.Roll().Sum(), useComplexHitLocations);
            var hitLocationInformation = LookupTables.FindHitLocation(location);
            await Context.Interaction.RespondAsync($"{hitLocationInformation}");

        }

        [SlashCommand("iron-gurps", "Randomly generate a list of books.", runMode:RunMode.Async)]
        [CommandSummary(@"Gives a random list of books to use for your next game.
`.IronGURPS` - Normal! Gets 3 books from all editions of GURPS.
`.IG 5` - Get 5 books from any edition of GURPS.
`.IG 5 4` - Get 5 books, but only from GURPS 4e.")]
        public async Task IronGurps(int numberOfBooks = 3, int minimumEdition = 3)
        {
            
            if (numberOfBooks > 9)
            {
                await Context.Interaction.RespondAsync(@"No spam, desu nyaa~");
                return;
            }

            await Context.Interaction.RespondAsync("Results:");
            await SendTextWithTimeBuffers(10, Workflow.Lookup.ChooseRandomBooks(numberOfBooks, minimumEdition).Select(b => $"{b.Title}{(b.Edition != 4 ? ($" ({b.Edition}e)") : "")}"), false);

        }

        [SlashCommand("last-gasp-non-player-character", "Roll on a table to see if scrubs are tired", runMode: RunMode.Async), CommandSummary(@"Roll to see how a mook behaves in combat when you don't want to track everyone's AP. Optionally, you can supply a number of mooks to roll for. (capped to 10 for now.)
Examples:
`.lastgaspnonplayercharacter`
`.lgnpc`
`.lgnpc 10`
")]
        public async Task LastGaspNonPlayerCharacter([Summary("Number-of-Mooks-to-roll-for")] uint quantity = 1)
        {
            quantity = Math.Min(Math.Max(1, quantity), 10);
            var result = Enumerable.Range(1, (int)quantity).
                Select(i =>
                {
                    var aResult = LookupTables.LastGaspNonPlayerCharacter(out int recoveryTime);
                    return new Tuple<LookupTables.LastGaspAction, int>(aResult, recoveryTime);
                }).
                Select(t =>
                {
                    var returnValue = string.Empty;
                    switch (t.Item1)
                    {
                        case LookupTables.LastGaspAction.Normal: returnValue = "Normal action."; break;
                        case LookupTables.LastGaspAction.Fatigue: returnValue = "Spend 1 FP to recover, followed by a normal action."; break;
                        default: returnValue = $"Rest for {t.Item2} seconds."; break;
                    }
                    return returnValue;
                });

            await Context.Interaction.RespondAsync(result.Aggregate((a, b) => a + Environment.NewLine + b));
        }


        [SlashCommand("malfunction", "rolls on Basic Set's malfunction table.", runMode: RunMode.Async), CommandSummary(@"Roll on the Basic Set Firearm Malfunction table.
Examples:
.Malfunction
.malf")]
        public async Task Malfunction()
        {
            var result = LookupTables.GetMalfunction(Roller.Roll().Sum());
            await Context.Interaction.RespondAsync(result);
        }

        [SlashCommand("quick-contest", "Execute a quick contest", runMode: RunMode.Async), CommandSummary(@"Roll Quick Contest.
Examples:
.qc 10 vs 12")]
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
.quickcontestabunch 16 10 12 14
.qcab 16 16 15 14")]
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
.quickcontestabunchIdentical 16 12 10
.qcabi 16 12 10")]
        public async Task QuickContestABunch(uint instigatorSkill, uint defenderSkillLevels, uint numberOfDefenders)
        {
            var results =
                Enumerable.Repeat(defenderSkillLevels, (int)numberOfDefenders).
                Select(osl => Roller.NewQuickContest((int)instigatorSkill, (int)osl)).
                ToList();

            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateQuickContestABunchEmbed(results));
        }

        [SlashCommand("reaction", "Get a reaction!", runMode: RunMode.Async), CommandSummary(@"Calculates a reaction given an optional reaction modifier and a special lookup table if desired.
The special lookup tables are:
`Aid` - Response to a request for aid.
`Admission` - Response to be let in somewhere.
`Authority` - Reaction from the police or similar.
`Combat` - The possibility of combat starting or ending.
`Commerce` - The reaction to haggling.
`Hiring` - The reaction when looking for a job.
`Information` - The reaction to questions given to an NPC.
`Loyalty` - Whether one of your hired subordinates is likely to betray you.
`Recreation` - Response to asking if you can play.
`Seduction` - Response to a request for intimacy or a romantic relationship.
`Testimony` - Response to information you have given to NPCs and whether they believe it.
Examples:
.reaction
.re
.reaction -2
.re 3
.re 4 commerce")]
        public async Task Reaction(int reactionModifier = 0, ReactionTable specialLookupTable = ReactionTable.None)
        {

            var reaction = LookupTables.GetReaction(reactionModifier);


            var response = string.Empty;
            if (specialLookupTable != ReactionTable.None)
            {
                response = LookupTables.GetReactionNotes(reaction, specialLookupTable.ToString());
            }
            else
            {
                var vowels = new[] { 'A', 'E', 'I', 'O', 'U' };
                var article = vowels.Contains(reaction.ToUpperInvariant().First()) ? "an" : "a";
                response = $"You received {article} {reaction} reaction.";
            }
            await Context.Interaction.RespondAsync($"{response}");
        }

        [SlashCommand("regular-contest", "Do a regular contest", runMode: RunMode.Async), CommandSummary(@"Rolls a regular contest where each iteration defaults to a length of 1 second. 
Optional iteration lengths may be supplied.
Examples:
.rc 10 10
.regularcontest 12 10 0:30")]
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

        [SlashCommand("roll-a-bunch", "Make the same roll multiple times", runMode: RunMode.Async)]
        [CommandSummary(@"Rolls as many times as told. It defaults to rolling 3d, but optionally, a different roll can
be provided.
Examples:
.rollABunch 5
.rab 50
.rab 5 7d+1
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
.rollABunchAgainst 10 5
.raba 10 50")]
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
.ra 13
.success Stealth-12
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


        [SlashCommand("roll-stats", "Rolls up stats randomly because no one said it could be done.", runMode: RunMode.Async)]
        [CommandSummary(@"Randomly rolls up character stats. Defaults to 100 points, but a different number can be provided.
Examples:
.RollStats
.rs 150
.rs 200 false")]
        [CommandRemarks(@"HP, FP, Basic Speed, and Basic Move have relative maxima based on their source attributes. Possibilities are weighted to favor more expensive traits first.
The second parameter determines whether attributes and secondary characteristics have a possibility of being lowered. It defaults to on (`true`) but if `false`, all attributes are guaranteed to be at least 10, and all secondary characteristics are guaranteed to have 0 or more points invested.")]
        public async Task RollStats([Summary("character-points")] int characterPoints = 0, [Summary("allow-lowered-traits")] bool allowLoweredValues = true)
        {

            if (characterPoints < 0)
            {
                await Context.Interaction.RespondAsync($"Why did you do this? What did you think was going to happen?");
                return;
            }
            var result = AttributeRandomizer.RollAttributes(characterPoints, allowLoweredValues);

            await Context.Interaction.RespondAsync(string.Empty, embed: EmbedUtility.GenerateRolledStatsEmbed(result));
        }

        [SlashCommand("slam", "Calculate slam damage", runMode: RunMode.Async)]
        [CommandSummary(@"Calculates the slam damage in dice, and also immediately executes the results telling how much damage both parties might lose and whether anyone falls down.
Parameters in order are velocity (yards/second) Slammer Hit Points, and target Hit Points.
Examples:
For a move 6 slam executed by one character with 11 HP against another with 10
.Slam 6 11 10
")]
        public async Task Slam([Summary("Velocity")] int velocity, [Summary("Slammer-Hit-Points")] int slammerHitPoints, [Summary("Target-Hit-Points")] int targetHitPoints)
        {

            var result = Mechanic.Slam.Execute(velocity, slammerHitPoints, targetHitPoints);
            var knockdownBit = (result.PartyOneAutomaticallyKnockedDown ?
                "The Slammer is knocked down." : result.PartyTwoAutomaticallyKnockedDown ?
                "The Target is knocked down." : result.PartyTwoRollsForKnockdown ?
                "The Target must roll to remain standing." :
                "Neither party is knocked down."
                );
            await Context.Interaction.RespondAsync($@"The results are as follows.
```
  Velocity: {velocity}
Slammer HP: {slammerHitPoints}
 Target HP: {targetHitPoints}

Slammer did {result.PartyOneDamage} which comes out to {result.PartyOneDamageResult}.
 Target did {result.PartyTwoDamage} which comes out to {result.PartyTwoDamageResult}.
{knockdownBit}
```");

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

        [SlashCommand("throw", "Calculate throw metrics via Basic Set", runMode: RunMode.Async)]
        [CommandSummary(@"Given the appropriate parameters, calculates the parameters of distance and damage of a thrown object.
Examples:
.throw 0.5 10
.th 1 10 kyos
.th 5 15 20 kyos metric
.throw .7 10 11")]
        public async Task Throw(double weight, uint liftingStrength, int strikingStrength = -1, [Summary("normal-or-kyos"), Choice("Normal", "NORMAL"), Choice("Knowing Your Own Strength", "KYOS")] string kyosOptionAsText = "NORMAL", [Choice("Imperial", "IMPERIAL"), Choice("Metric", "METRIC")] string metricOrImperial = "IMPERIAL")
        {
            var isMetric = metricOrImperial.ToUpperInvariant().StartsWith("M");
            var distanceUnit = isMetric ? "meters" : "yards";
            var weightUnit = isMetric ? "kilograms" : "pounds";
            if (strikingStrength < 0)
                strikingStrength = (int)liftingStrength;

            var kyosOption = !string.IsNullOrWhiteSpace(kyosOptionAsText) && kyosOptionAsText.ToUpperInvariant().StartsWith("K");

            try
            {
                if (weight < 0)
                    throw new ArgumentOutOfRangeException();
                var returnValue = LookupTables.GetThrowingStatistics((int)liftingStrength, strikingStrength, isMetric ? weight * 2 : weight, kyosOption);
                var normalizedDamage = returnValue.Damage.Normalize();
                var normalizedDamageString = normalizedDamage.Equals(returnValue.Damage) ? "" : $@"
Normalized Damage: {returnValue.Damage.Normalize().ToString()} = {Math.Max(0, Roller.Roll(returnValue.Damage.Normalize()).Sum())}";
                await Context.Interaction.RespondAsync($@"```
 Lifting Strength: {liftingStrength}
Striking Strength: {strikingStrength}
           Weight: {weight:#,###.##} {weightUnit}
            Range: {returnValue.MaxRange:#,###.##} {distanceUnit}
           Damage: {returnValue.Damage.ToString()} = {Math.Max(0, Roller.Roll(returnValue.Damage).Sum())}{normalizedDamageString}
```");
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync($"Weight exceeded maximum two handed lift or was less than 0.");
            }

        }

        private static async Task DisplayErrorMessage(IMessageChannel channel, string message)
        {
            await channel.SendMessageAsync($"{message}");
        }
    }
}
