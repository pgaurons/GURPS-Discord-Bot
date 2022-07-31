using Gao.Gurps.Dice;
using Gao.Gurps.Model;
using Gao.Gurps.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gao.Gurps
{
    /// <summary>
    /// For calculating reactions
    /// </summary>
    public static class LookupTables
    {
        /// <summary>
        /// I thought int.Min would make too big of a dictionary.
        /// </summary>
        const int SomeArbitrarilyLowReaction = 0;
        /// <summary>
        /// I thought int.Max would make too big of a dictionary
        /// </summary>
        const int SomeArbitrarilyHighReaction = 19;
        /// <summary>
        /// The reaction table.
        /// </summary>
        public static Dictionary<int, string> Reactions { get; } =
            new Dictionary<IEnumerable<int>, string>
            {
                { Enumerable.Range(SomeArbitrarilyLowReaction, (0 + 1) - SomeArbitrarilyLowReaction), "Disastrous" },
                { Enumerable.Range(1, 3), "Very Bad" },
                { Enumerable.Range(4, 3), "Bad" },
                { Enumerable.Range(7, 3), "Poor" },
                { Enumerable.Range(10, 3), "Neutral" },
                { Enumerable.Range(13, 3), "Good" },
                { Enumerable.Range(16, 3), "Very Good" },
                { Enumerable.Range(19, (SomeArbitrarilyHighReaction + 1) - 19), "Excellent" },
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public const string MechanicalOrElectricProblemText = @"Mechanical or Electrical Problem - Guns require an Armoury or IQ-based Guns roll to diagnose. Another roll against Armoury and 1 hour is required for repairs.
Grenades detonate 1d seconds late.";
        public const string MisfireText = @"Misfire - requires an Armoury+2 or IQ based Guns roll to diagnose. Repairing requires 3 seconds, two free hands, and a successful Armory+2 roll or IQ based Guns skill. Revolvers don't require fixing.
Grenades are duds and will never detonate.";
        public const string StoppageText = @"Stoppage - Weapon fires one normal shot and then jams. Requires 3 seconds, two free hands, and a successful Armoury or IQ based Guns skill to fix.
Beam Weapons require an Armoury or IQ-based weapon skill roll to diagnose. Another roll against Armoury and 1 hour is required for repairs.
Grenades are duds and will never detonate.";
        public const string PossibleExplosionText = @"Exception: at TL3 or 4, the weapon explodes in the user's face. This does 1d+2 cr ex [2d]. If the weapon actually has an explosive warhead, use that damage value instead.";
        public static Dictionary<int, string> Malfunctions { get; } =
    new Dictionary<IEnumerable<int>, string>
    {
                
                { Enumerable.Range(3, 2),  MechanicalOrElectricProblemText},
                { Enumerable.Range(5, 4), MisfireText },
                { Enumerable.Range(9, 3), StoppageText },
                { Enumerable.Range(12, 3), MisfireText },
                { Enumerable.Range(15, 4), MechanicalOrElectricProblemText +  Environment.NewLine + PossibleExplosionText}
                
    }.
    SelectMany
    (
        kvp => kvp.Key.Select
        (
            key => new KeyValuePair<int, string>(key, kvp.Value)
        )
    ).
    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        internal static decimal GetDamageModifier(DamageType damageType, InjuryTolerance injuryTolerance, HitLocation hitLocation)
        {
            var returnValue = 1.0m;
            switch (injuryTolerance)
            {
                case InjuryTolerance.Homogeneous:
                    switch(damageType)
                    {
                        case DamageType.SmallPiercing: returnValue = 0.1m; break;
                        case DamageType.Piercing: returnValue = 0.2m; break;
                        case DamageType.LargePiercing: returnValue = 1m / 3m; break;
                        case DamageType.HugePiercing: returnValue = 0.5m; break;
                        case DamageType.Impaling: returnValue = 0.5m; break;

                    }
                    break;
                    
            }
            return returnValue;
        }


        /// <summary>
        /// Gets a reaction roll result taking the reaction modifier into account.
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public static string GetReaction(int modifier)
        {
            var index = Math.Max(SomeArbitrarilyLowReaction, Math.Min(Roller.Roll().Sum() + modifier, SomeArbitrarilyHighReaction));
            var returnValue = Reactions[index];

            return returnValue;
        }

        /// <summary>
        /// Gets a malfunction result for a given roll.
        /// </summary>
        /// <param name="roll"></param>
        /// <returns></returns>
        public static string GetMalfunction(int roll)
        {
            return Malfunctions[Math.Min(18, Math.Max(3,roll))];
        }

        /// <summary>
        /// Calculates the penalties for long range modifiers
        /// </summary>
        /// <param name="argument">a range and an optional unit of measurement</param>
        /// <returns>penalty for long ranges</returns>
        public static int GetLongDistancePenalty(string argument)
        {
            var parsedRange = ConvertToMiles(argument);
            return parsedRange <= 0.1 ? 0 : Math.Min(LookupLongDistancePenalty(parsedRange), 0);
        }

        private static int LookupLongDistancePenalty(double miles)
        {
            if (miles <= 0)
                throw new ArgumentOutOfRangeException(nameof(miles));
            var penalty = 0;
            while (miles > 100)
            {
                miles /= 10;
                penalty -= 2;
            }

            penalty += LongDistancePenaltyTable.Where(kvp => kvp.Key >= miles).Max(kvp => kvp.Value);

            return penalty;
        }

        public static int GetDistancePenalty(string distance)
        {
            var parsedRange = ConvertToYards(distance);
            return parsedRange < 1 ? 0 : Math.Min(GetSizeModifier(parsedRange)*-1, 0);
        }


        public static Dictionary<double, int> SizeModiferTable { get; } =
            new Dictionary<double, int>
            {
                {1.0/540, -18 }, //1/15
                {1.0/360, -17 }, //1/10 inch.
                {1.0/216, -16 }, //1/6 inch
                {1.0/180, -15}, //1/5 inch
                {1.0/108, -14}, //1/3
                {1.0/72, -13}, //1/2
                {1.0/54, -12}, //2/3
                {1.0/36, -11}, //1 inch
                {1.0/24, -10},
                {1.0/18,  -9},
                {1.0/12, -8},
                {5.0/36, -7},
                {2.0/9, -6},
                {1.0/3, -5}, //1 foot
                {1.0/2, -4},
                {2.0/3, -3},
                {1, -2},
                {1.5, -1},
                {2, 0},
                {3, 1},
                {5, 2},
                {7, 3},
                {10, 4},
                {15, 5},
                {20, 6},
                {30, 7},
                {50, 8},
                {70, 9},
                {100, 10},
                {150, 11 }
           };

        /// <summary>
        /// The robustness threshold table is based on 120% of the size modifier keys.
        /// The table also uses HP, which are discrete, so we only need the integer values
        /// starting at 1.
        /// </summary>
        public static Dictionary<int, int> RobustnessThresholdTable { get; } =
            SizeModiferTable.
            Where(kvp => kvp.Key >= 1 && Math.Truncate(kvp.Key) == kvp.Key).
            ToDictionary(kvp => (int)(kvp.Key*6)/5, kvp => kvp.Value);

        /// <summary>
        /// Gets the Conditional Injury Robustness threshold given the hit points.
        /// </summary>
        /// <param name="hitPoints">HP or injury</param>
        /// <returns>Threshold or wound potential</returns>
        public static int GetRobustnessThreshold(int hitPoints)
        {
            if (hitPoints <= 0)
                throw new ArgumentOutOfRangeException(nameof(hitPoints));
            var threshold = 0;
            var rationalHitPoints = new decimal(hitPoints);
            while (rationalHitPoints > 100)
            {
                rationalHitPoints /= 10;
                threshold += 6;
            }

            threshold += RobustnessThresholdTable.Where(kvp => kvp.Key >= rationalHitPoints).Min(kvp => kvp.Value);
            return threshold;
        }
        public static Dictionary<double, int> LongDistancePenaltyTable { get; } =
    new Dictionary<double, int>
    {
                {0.1, 0 }, //200 yards
                {0.5, -1}, //1/2 mile
                {1, -2},
                {3, -3},
                {10, -4},
                {30, -5},
                {100, -6},
                {300, -7},
                {1000, -8}
   };



        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="index"></param>
        /// <param name="complete">Whether to use the complete table, or the basic set table.</param>
        /// <returns></returns>
        public static ComplexHitLocation GetHitLocation(HitLocationTable table, int index, bool complete=true)
        {
            var location = new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 };
            switch(table)
            {
                case HitLocationTable.Humanoid: location = HumanoidHitLocations[index]; break;
                case HitLocationTable.WingedHumanoid: location = WingedHumanoidHitLocations[index]; break;
                case HitLocationTable.FishTailedHumanoid: location = FishTailedHumanoidHitLocations[index]; break;
                case HitLocationTable.Quadruped: location = QuadrupedHitLocations[index]; break;
                case HitLocationTable.WingedQuadruped: location = QuadrupedHitLocations[index]; break;
                case HitLocationTable.Centaur: location = CentaurHitLocations[index]; break;
                case HitLocationTable.Avian: location = AvianHitLocations[index]; break;
                case HitLocationTable.Vermiform: location = VermiformHitLocations[index]; break;
                case HitLocationTable.WingedSerpent: location = WingedSerpentHitLocations[index]; break;
                case HitLocationTable.SnakeMan: location = SnakeManHitLocations[index]; break;
                case HitLocationTable.Octopod: location = OctopodHitLocations[index]; break;
                case HitLocationTable.Squid: location = SquidHitLocations[index]; break;
                case HitLocationTable.Cancroid: location = CancroidHitLocations[index]; break;
                case HitLocationTable.Scorpion: location = ScorpionHitLocations[index]; break;
                case HitLocationTable.Ichthyoid: location = IchthyoidHitLocations[index]; break;
                case HitLocationTable.Arachnoid: location = ArachnoidHitLocations[index]; break;
                case HitLocationTable.Hexapod: location = HexapodHitLocations[index]; break;
                case HitLocationTable.WingedHexapod: location = WingedHexapodHitLocations[index]; break;

            }
            location = location.Clone();
            if(location.Location == HitLocation.Leg && (table == HitLocationTable.Cancroid || table == HitLocationTable.Scorpion))
            {
                //Roll randomly for legs.
                var legNumber = Roller.Roll(1, 8).Sum();
                switch (legNumber)
                {
                    case 1: location.Location = HitLocation.LegOne; break;
                    case 2: location.Location = HitLocation.LegTwo; break;
                    case 3: location.Location = HitLocation.LegThree; break;
                    case 4: location.Location = HitLocation.LegFour; break;
                    case 5: location.Location = HitLocation.LegFive; break;
                    case 6: location.Location = HitLocation.LegSix; break;
                    case 7: location.Location = HitLocation.LegSeven; break;
                    case 8: location.Location = HitLocation.LegEight; break;
                }
            }
            if(location.Location == HitLocation.Extremity)
            {
                var extremityRoll = Roller.Roll(1, 6).Sum();
                switch(extremityRoll)
                {
                    case 1: location.Location = HitLocation.LeftHand; break;
                    case 2: location.Location = HitLocation.RightHand; break;
                    case 3: location.Location = HitLocation.LeftForefoot; break;
                    case 4: location.Location = HitLocation.RightForefoot; break;
                    case 5: location.Location = HitLocation.LeftHindFoot; break;
                    case 6: location.Location = HitLocation.RightHindFoot; break;
                }
            }
            var right = Roller.Roll(1, 6).Sum() < 4;
            switch (location.Location)
            {
                case HitLocation.Hand: location.Location = right ? HitLocation.RightHand : HitLocation.LeftHand; break;
                case HitLocation.Foot: location.Location = right ? HitLocation.RightFoot : HitLocation.LeftFoot; break;
                case HitLocation.Wing: location.Location = right ? HitLocation.RightWing : HitLocation.LeftWing; break;
                case HitLocation.Foreleg: location.Location = right ? HitLocation.RightForeleg : HitLocation.LeftForeleg; break;
                case HitLocation.HindLeg: location.Location = right ? HitLocation.RightHindLeg : HitLocation.LeftHindLeg; break;
                case HitLocation.MidLeg: location.Location = right ? HitLocation.RightMidLeg : HitLocation.LeftMidLeg; break;
                case HitLocation.Arm: location.Location = right ? HitLocation.RightArm : HitLocation.LeftArm; break;
                case HitLocation.Leg: location.Location = right ? HitLocation.RightLeg : HitLocation.LeftLeg; break;
                case HitLocation.ArmOneAndTwo: location.Location = right ? HitLocation.ArmOne : HitLocation.ArmTwo; break;
                case HitLocation.ArmThreeAndFour: location.Location = right ? HitLocation.ArmThree : HitLocation.ArmFour; break;
                case HitLocation.ArmFiveAndSix: location.Location = right ? HitLocation.ArmFive : HitLocation.ArmSix; break;
                case HitLocation.ArmSevenAndEight: location.Location = right ? HitLocation.ArmSeven : HitLocation.ArmEight; break;
                case HitLocation.SquidArmThreeAndFour: location.Location = right ? HitLocation.SquidArmThree : HitLocation.SquidArmFour; break;
                case HitLocation.SquidArmFiveAndSix: location.Location = right ? HitLocation.SquidArmFive : HitLocation.SquidArmSix; break;
                case HitLocation.SquidArmSevenAndEight: location.Location = right ? HitLocation.SquidArmSeven : HitLocation.SquidArmEight; break;
                case HitLocation.LegOneAndTwo: location.Location = right ? HitLocation.LegOne : HitLocation.LegTwo; break;
                case HitLocation.LegThreeAndFour: location.Location = right ? HitLocation.LegThree : HitLocation.LegFour; break;
                case HitLocation.LegFiveAndSix: location.Location = right ? HitLocation.LegFive : HitLocation.LegSix; break;
                case HitLocation.LegSevenAndEight: location.Location = right ? HitLocation.LegSeven : HitLocation.LegEight; break;

            }
            if (complete)
                CheckMartialArtsAndLowTechHitLocations(location); // go a step further.

            return location;
        }

        private static void CheckMartialArtsAndLowTechHitLocations(ComplexHitLocation location)
        {
            switch(location.Location)
            {
                case HitLocation.Groin:
                    location.Location = HitLocation.Abdomen;
                    location.Penalty = -1;
                    break;
                case HitLocation.Torso:
                case HitLocation.UpperTorso:
                case HitLocation.LowerTorso:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(location.Location.ToFormattedString().Replace("Torso", "") + "Chest");
                    break;
            }

            var extraRoll = Roller.Roll(1, 6).Sum();
            if (extraRoll != 1) return; //For now, only a 1 matters.
            switch(location.Location)
            {
                case HitLocation.LeftHand:
                case HitLocation.RightHand:
                case HitLocation.LeftFoot:
                case HitLocation.RightFoot:
                case HitLocation.RightForefoot:
                case HitLocation.LeftForefoot:
                case HitLocation.LeftHindFoot:
                case HitLocation.RightHindFoot:
                case HitLocation.LeftMidFoot:
                case HitLocation.RightMidFoot:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(location.Location.ToFormattedString() + "Joint");
                    location.Penalty = -7;
                    break;
                case HitLocation.LeftArm:
                case HitLocation.RightArm:
                case HitLocation.LeftLeg:
                case HitLocation.RightLeg:
                case HitLocation.LeftForeleg:
                case HitLocation.RightForeleg:
                case HitLocation.LeftHindLeg:
                case HitLocation.RightHindLeg:
                case HitLocation.LeftMidLeg:
                case HitLocation.RightMidLeg:
                case HitLocation.ArmOne:
                case HitLocation.ArmTwo:
                case HitLocation.ArmThree:
                case HitLocation.ArmFour:
                case HitLocation.ArmFive:
                case HitLocation.ArmSix:
                case HitLocation.ArmSeven:
                case HitLocation.ArmEight:
                case HitLocation.LegOne:
                case HitLocation.LegTwo:
                case HitLocation.LegThree:
                case HitLocation.LegFour:
                case HitLocation.LegFive:
                case HitLocation.LegSix:
                case HitLocation.LegSeven:
                case HitLocation.LegEight:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(location.Location.ToFormattedString() + "Artery Or Joint");
                    location.Penalty = -5;
                    break;
                case HitLocation.Face:
                    location.Location = HitLocation.SkullOrNose;
                    location.Penalty = -7;
                    break;
                case HitLocation.Neck:
                    location.Location = HitLocation.NeckVeinOrArteryOrSpine;
                    break;
                case HitLocation.Torso:
                    location.Location = HitLocation.VitalsOrSpine;
                    break;
                case HitLocation.Abdomen:
                    location.Location = HitLocation.AbdomenOrVitals;
                    break;

            }
        }

        public static Dictionary<int, ComplexHitLocation> HumanoidHitLocations { get; } =
            new Dictionary<IEnumerable<int>, ComplexHitLocation>
            {
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 2), new ComplexHitLocation { Location = HitLocation.RightLeg, Penalty = -2 } },
                { Enumerable.Range(8, 1), new ComplexHitLocation { Location = HitLocation.RightArm, Penalty = -2 } },
                { Enumerable.Range(9, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.LeftArm, Penalty = -2 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.LeftLeg, Penalty = -2 } },
                { Enumerable.Range(15, 1), new ComplexHitLocation { Location = HitLocation.Hand, Penalty = -4 } },
                { Enumerable.Range(16, 1), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },

            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> WingedHumanoidHitLocations { get; } =
    new Dictionary<IEnumerable<int>, ComplexHitLocation>
    {
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 2), new ComplexHitLocation { Location = HitLocation.RightLeg, Penalty = -2 } },
                { Enumerable.Range(8, 1), new ComplexHitLocation { Location = HitLocation.RightArm, Penalty = -2 } },
                { Enumerable.Range(9, 1), new ComplexHitLocation { Location = HitLocation.Wing, Penalty = -2 } },
                { Enumerable.Range(10, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.LeftArm, Penalty = -2 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.LeftLeg, Penalty = -2 } },
                { Enumerable.Range(15, 1), new ComplexHitLocation { Location = HitLocation.Hand, Penalty = -4 } },
                { Enumerable.Range(16, 1), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
    }.
    SelectMany
    (
        kvp => kvp.Key.Select
        (
            key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
        )
    ).
    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static ComplexHitLocation GrandUnifiedHitLocationTable()
        {
            var firstRoll = Roller.Roll().Sum();
            var location = HumanoidHitLocations[firstRoll].Clone();
            if(location.Location == HitLocation.Groin)
            {
                location.Location = HitLocation.Abdomen;
                location.Penalty = -1;
            }
            if (location.Location == HitLocation.Torso) location.Location = HitLocation.Chest;
            switch(location.Location)
            {
                case HitLocation.Face: location = GrandUnifiedHitLocationFace(); break;
                case HitLocation.LeftLeg:
                case HitLocation.RightLeg: location = GrandUnifiedHitLocationLeg(location.Location); break;
                case HitLocation.RightArm:
                case HitLocation.LeftArm: location = GrandUnifiedHitLocationArm(location.Location); break;
                case HitLocation.Chest: location = GrandUnifiedHitLocationChest(); break;
                case HitLocation.Abdomen: location = GrandUnifiedHitLocationAbdomen(); break;
                case HitLocation.Hand:
                case HitLocation.Foot: location = GrandUnifiedHitLocationExtremity(location.Location); break;
                case HitLocation.Neck:
                    location = GrandUnifiedHitLocationNeck();
                    break;

            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationNeck()
        {
            ComplexHitLocation location = new ComplexHitLocation();
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1: location.Location = HitLocation.NeckVeinOrArtery; location.Penalty = -8; break;
                default: location = HumanoidHitLocations.Values.First(v => v.Location == HitLocation.Neck).Clone(); break;
            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationExtremity(HitLocation specificExtremity)
        {
            ComplexHitLocation location = new ComplexHitLocation();
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificExtremity.ToFormattedString() + "Joint");
                    location.Penalty = -7;
                    break;
                default:
                    location = HumanoidHitLocations.Values.First(chl => chl.Location == specificExtremity).Clone();
                    break;
            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationAbdomen()
        {
            ComplexHitLocation location = new ComplexHitLocation();
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1: location.Location = HitLocation.Vitals; location.Penalty = -3; break;
                case 2:
                case 3:
                case 4: location.Location = HitLocation.DigestiveTract; location.Penalty = -3; break;
                case 5: location.Location = HitLocation.Pelvis; location.Penalty = -3; break;
                default: location.Location = HitLocation.Groin; location.Penalty = -3; break;
            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationChest()
        {
            var location = new ComplexHitLocation();
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1: location.Location = HitLocation.VitalsOrSpine; location.Penalty = -3; break;
                default: location.Location = HitLocation.Chest; location.Penalty = 0; break;
            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationArm(HitLocation specificLimb)
        {
            var location = new ComplexHitLocation();
            var specificLimbString = specificLimb.ToFormattedString().Split(' ')[0];
            if (specificLimbString.ToUpperInvariant() == "ARM") specificLimbString = string.Empty;
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1:
                case 2:
                case 3:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Forearm");
                    location.Penalty = -4; break;
                case 4:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Elbow");
                    location.Penalty = -7;
                    break;
                case 5:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Upper Arm");
                    location.Penalty = -4; break;
                default:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Shoulder");
                    location.Penalty = -7;
                    break;

            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationLeg(HitLocation specificLimb)
        {
            var location = new ComplexHitLocation();
            var specificLimbString = specificLimb.ToFormattedString().Split(' ')[0];
            if (specificLimbString.ToUpperInvariant() == "LEG") specificLimbString = string.Empty;
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1:
                case 2:
                case 3:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Shin");
                    location.Penalty = -4; break;
                case 4:
                    location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Knee");
                    location.Penalty = -7;
                    break;
                case 5: location.Location = EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Thigh"); location.Penalty = -4; break;
                default:
                    location.Location =
                EnumerationHelper.StringTo<HitLocation>(specificLimbString + "Thigh or Artery");
                    location.Penalty = -7;
                    break;

            }
            return location;
        }

        public static ComplexHitLocation GrandUnifiedHitLocationFace()
        {
            var location = new ComplexHitLocation();
            switch (Roller.Roll(1, 6).Sum())
            {
                case 1: location.Location = HitLocation.Jaw; location.Penalty = -6; break;
                case 2: location.Location = HitLocation.Nose; location.Penalty = -7; break;
                case 3: location.Location = HitLocation.Ear; location.Penalty = -7; break;
                case 4:
                case 5: location.Location = HitLocation.Cheek; location.Penalty = -6; break;
                default: location.Location = HitLocation.Eye; location.Penalty = -9; break;

            }

            return location;
        }

        public static Dictionary<int, ComplexHitLocation> FishTailedHumanoidHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(8, 1), new ComplexHitLocation { Location = HitLocation.RightArm, Penalty = -2 } },
                { Enumerable.Range(9, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.LeftArm, Penalty = -2 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(15, 1), new ComplexHitLocation { Location = HitLocation.Hand, Penalty = -4 } },
                { Enumerable.Range(16, 1), new ComplexHitLocation { Location = HitLocation.Tail, Penalty = -3 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Tuple<int, int> LookupGrapplingEncumbrance(decimal basicLift, uint encumbrance)
        {
            const decimal TableUpperBound = 47m;
            var multiple = Math.Min(TableUpperBound, encumbrance / basicLift);
            var result = GrapplingEncumbranceTable.
                Where(kvp => kvp.Key >= multiple).
                OrderBy(kvp => kvp.Key).
                First().Value;
            return result;

        }

        public static Dictionary<decimal, Tuple<int, int>> GrapplingEncumbranceTable { get; } =
            new Dictionary<decimal, Tuple<int, int>>
            {
                {0m, new Tuple<int, int>(+9, 0)},
                {1m, new Tuple<int, int>(+8, 0)},
                {2m, new Tuple<int, int>(+7, 0)},
                {3m, new Tuple<int, int>(+6, 0)},
                {3.5m, new Tuple<int, int>(+5, 0)},
                {4m, new Tuple<int, int>(+4, 0)},
                {4.2m, new Tuple<int, int>(+3, 0)},
                {4.5m, new Tuple<int, int>(+2, 0)},
                {5m, new Tuple<int, int>(+1, 0)},
                {6m, new Tuple<int, int>(0, 0)},
                {7m, new Tuple<int, int>(-1, 0)},
                {7.5m, new Tuple<int, int>(-2, 0)},
                {7.8m, new Tuple<int, int>(-3, 0)},
                {8m, new Tuple<int, int>(-4, 0)},
                {8.5m, new Tuple<int, int>(-5, -1)},
                {9m, new Tuple<int, int>(-6, -1)},
                {10m, new Tuple<int, int>(-7, -1)},
                {11m, new Tuple<int, int>(-8, -1)},
                {12m, new Tuple<int, int>(-9, -1)},
                {13m, new Tuple<int, int>(-10, -1)},
                {14.5m, new Tuple<int, int>(-11, -1)},
                {16m, new Tuple<int, int>(-12, -2)},
                {18m, new Tuple<int, int>(-13, -2)},
                {21m, new Tuple<int, int>(-14, -2)},
                {24m, new Tuple<int, int>(-15, -2)},
                {26m, new Tuple<int, int>(-16, -3)},
                {31m, new Tuple<int, int>(-17, -3)},
                {36m, new Tuple<int, int>(-18, -3)},
                {46m, new Tuple<int, int>(-19, -4)},
                {47m, new Tuple<int, int>(-20, -4)}
            };

        public static Dictionary<int, ComplexHitLocation> QuadrupedHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Foreleg, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.HindLeg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Tail, Penalty = -3 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// Calculates penalties for LFP system.
        /// </summary>
        /// <param name="currentFatiguePoints"></param>
        /// <param name="totalFatiguePoints"></param>
        /// <param name="strength"></param>
        /// <param name="dexterity"></param>
        /// <param name="intelligence"></param>
        /// <param name="health"></param>
        /// <param name="highDefinition">Optional rule for more gradient ST depletion.</param>
        /// <returns></returns>
        public static LongTermFatigueResults GetLongTermFatigueStats(int currentFatiguePoints, int totalFatiguePoints, int strength, int dexterity, int intelligence, int health, bool highDefinition)
        {
            var result = new LongTermFatigueResults { CurrentFatiguePoints = currentFatiguePoints, TotalFatiguePoints = totalFatiguePoints, HighDefinition = highDefinition };
            result.Penalty = strength == -1;//Assume that all the other attributes are also -1.
            var fatigueRatio = (decimal)currentFatiguePoints / totalFatiguePoints;
            var penaltyLevel = (int)Math.Ceiling(fatigueRatio*5)-5;
            result.RegularPenalty = penaltyLevel;
            result.StrengthPenalty = highDefinition ? (1-fatigueRatio)/-2 : penaltyLevel * 0.1m;
            result.RecoveryRate = 
                fatigueRatio >= 0.5m ? FatigueRecoveryRate.Mild : 
                fatigueRatio >= 0 ? FatigueRecoveryRate.Severe : 
                FatigueRecoveryRate.Deep;

            result.OriginalStrength = strength;
            result.OriginalDexterity = dexterity;
            result.OriginalIntelligence = intelligence;
            result.OriginalHealth = health;

            return result;
        }

        public static Dictionary<int, ComplexHitLocation> WingedQuadrupedHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Foreleg, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Wing, Penalty = -2 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.HindLeg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Tail, Penalty = -3 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> HexapodHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Foreleg, Penalty = -2 } },
                { Enumerable.Range(9, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.MidLeg, Penalty = -2 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.HindLeg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.MidLeg, Penalty = -2 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static LastGaspAction LastGaspNonPlayerCharacter(out int recoverySeconds)
        {
            recoverySeconds = 0;
            var result = LastGaspAction.Normal;
            var firstRoll = Roller.Roll(1, 6).First();
            if(firstRoll == 6)
            {
                var secondRoll = Roller.Roll(1, 6).First();
                switch(secondRoll)
                {
                    case 3:
                    case 4: result = LastGaspAction.Fatigue; break;
                    case 5:
                    case 6:
                        result = LastGaspAction.Recover;
                        recoverySeconds = Roller.Roll().Sum();
                        break;
                }
            }
            return result;
        }

        public enum LastGaspAction
        {
            Normal,
            Fatigue,
            Recover
        }

        public static Dictionary<int, ComplexHitLocation> WingedHexapodHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Foreleg, Penalty = -2 } },
                { Enumerable.Range(9, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.MidLeg, Penalty = -2 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Wing, Penalty = -2 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.HindLeg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.MidLeg, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> CentaurHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Foreleg, Penalty = -2 } },
                { Enumerable.Range(9, 2), new ComplexHitLocation { Location = HitLocation.LowerTorso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.UpperTorso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.HindLeg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Arm, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Extremity, Penalty = -4 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);



        public static Dictionary<int, ComplexHitLocation> AvianHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Wing, Penalty = -2 } },
                { Enumerable.Range(9, 2), new ComplexHitLocation { Location = HitLocation.LowerTorso, Penalty = 0 } },
                { Enumerable.Range(11, 1), new ComplexHitLocation { Location = HitLocation.UpperTorso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Leg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Tail, Penalty = -3 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> VermiformHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> WingedSerpentHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Wing, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Wing, Penalty = -2 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


        public static Dictionary<int, ComplexHitLocation> SnakeManHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.RightArm, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.LeftArm, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Hand, Penalty = -4 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        public static Dictionary<int, ComplexHitLocation> OctopodHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Brain, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.ArmOneAndTwo, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.ArmThreeAndFour, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.ArmFiveAndSix, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.ArmSevenAndEight, Penalty = -2 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        public static Dictionary<int, ComplexHitLocation> SquidHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Brain, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.ArmOneAndTwo, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.SquidArmThreeAndFour, Penalty = -3 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.SquidArmFiveAndSix, Penalty = -3 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.SquidArmSevenAndEight, Penalty = -3 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> CancroidHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Arm, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Leg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Leg, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> ScorpionHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Arm, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Tail, Penalty = -3 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Leg, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Leg, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Foot, Penalty = -4 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> IchthyoidHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Skull, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Fin, Penalty = -4 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.Fin, Penalty = -4 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.Fin, Penalty = -4 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.Tail, Penalty = -3 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, ComplexHitLocation> ArachnoidHitLocations { get; } =
new Dictionary<IEnumerable<int>, ComplexHitLocation>
{
                { Enumerable.Range(3, 2), new ComplexHitLocation { Location = HitLocation.Brain, Penalty = -7 } },
                { Enumerable.Range(5, 1), new ComplexHitLocation { Location = HitLocation.Neck, Penalty = -5 } },
                { Enumerable.Range(6, 1), new ComplexHitLocation { Location = HitLocation.Face, Penalty = -5 } },
                { Enumerable.Range(7, 2), new ComplexHitLocation { Location = HitLocation.LegOneAndTwo, Penalty = -2 } },
                { Enumerable.Range(9, 3), new ComplexHitLocation { Location = HitLocation.Torso, Penalty = 0 } },
                { Enumerable.Range(12, 1), new ComplexHitLocation { Location = HitLocation.Groin, Penalty = -3 } },
                { Enumerable.Range(13, 2), new ComplexHitLocation { Location = HitLocation.LegThreeAndFour, Penalty = -2 } },
                { Enumerable.Range(15, 2), new ComplexHitLocation { Location = HitLocation.LegFiveAndSix, Penalty = -2 } },
                { Enumerable.Range(17, 2), new ComplexHitLocation { Location = HitLocation.LegSevenAndEight, Penalty = -3 } },
}.
SelectMany
(
kvp => kvp.Key.Select
(
key => new KeyValuePair<int, ComplexHitLocation>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static List<Tuple<HitLocationTable, ComplexHitLocation>> OtherHitLocations = new List<Tuple<HitLocationTable, ComplexHitLocation>>
        {
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Eye, Penalty=-9 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Vitals, Penalty=-3 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Octopod, new ComplexHitLocation {Location = HitLocation.Eye, Penalty=-8 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Squid, new ComplexHitLocation {Location = HitLocation.Eye, Penalty=-8 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Ichthyoid, new ComplexHitLocation {Location = HitLocation.Eye, Penalty=-8 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftFoot, Penalty=-4 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightFoot, Penalty=-4 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftHand, Penalty=-4 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightHand, Penalty=-4 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Ear, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Jaw, Penalty=-6 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LimbJoint, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.ExtremityJoint, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Nose, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Spine, Penalty=-8 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LimbArtery, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.NeckVeinOrArtery, Penalty=-8 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Abdomen, Penalty=-1 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Chest, Penalty=0 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Cheek, Penalty=-6 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftShin, Penalty=-2 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightShin, Penalty=-2 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightThigh, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftThigh, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightThighOrArtery, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftThighOrArtery, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightKnee, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftKnee, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftForearm, Penalty=-2 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightForearm, Penalty=-2 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightElbow, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftElbow, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftUpperArm, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightUpperArm, Penalty=-5 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.RightShoulder, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.LeftShoulder, Penalty=-7 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.DigestiveTract, Penalty=-2 }),
            new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Undefined, new ComplexHitLocation {Location = HitLocation.Pelvis, Penalty=-3 }),

        };

        public static List<Tuple<HitLocationTable, ComplexHitLocation>> UnifiedHitLocationLookup =
            HumanoidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Humanoid, kvp.Value)).
            Union(WingedHumanoidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.WingedHumanoid, kvp.Value))).
            Union(FishTailedHumanoidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.FishTailedHumanoid, kvp.Value))).
            Union(QuadrupedHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Quadruped, kvp.Value))).
            Union(CentaurHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Centaur, kvp.Value))).
            Union(AvianHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Avian, kvp.Value))).
            Union(VermiformHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Vermiform, kvp.Value))).
            Union(WingedSerpentHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.WingedSerpent, kvp.Value))).
            Union(SnakeManHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.SnakeMan, kvp.Value))).
            Union(OctopodHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Octopod, kvp.Value))).
            Union(SquidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Squid, kvp.Value))).
            Union(CancroidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Cancroid, kvp.Value))).
            Union(ScorpionHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Scorpion, kvp.Value))).
            Union(IchthyoidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Ichthyoid, kvp.Value))).
            Union(ArachnoidHitLocations.Select(kvp => new Tuple<HitLocationTable, ComplexHitLocation>(HitLocationTable.Arachnoid, kvp.Value))).
            Union(OtherHitLocations).ToList();


        public static List<HitLocation> SearchableHitLocations = new List<HitLocation>
        {
            HitLocation.Eye,
            HitLocation.Face,
            HitLocation.Skull,
            HitLocation.Arm,
            HitLocation.Torso,
            HitLocation.Groin,
            HitLocation.Hand,
            HitLocation.Foot,
            HitLocation.Neck,
            HitLocation.Vitals,
            HitLocation.Wing,
            HitLocation.Tail,
            HitLocation.Brain,
            HitLocation.Fin,
            HitLocation.Jaw,
            HitLocation.Ear,
            HitLocation.ExtremityJoint,
            HitLocation.LimbJoint,
            HitLocation.Nose,
            HitLocation.Spine,
            HitLocation.LimbArtery,
            HitLocation.NeckVeinOrArtery,
            HitLocation.Chest,
            HitLocation.Abdomen,
            HitLocation.Shin,
            HitLocation.Thigh,
            HitLocation.Knee,
            HitLocation.Forearm,
            HitLocation.UpperArm,
            HitLocation.Elbow,
            HitLocation.Shoulder,
            HitLocation.HandJoint,
            HitLocation.FootJoint,
            HitLocation.Leg

            
        };
        public static IEnumerable<string> FindHitLocation(string location)
        {
            
            var matches = UnifiedHitLocationLookup.
                Where(hl => Regex.IsMatch(hl.Item2.Location.ToFormattedString(), location, RegexOptions.IgnoreCase)).
                Where(t => SearchableHitLocations.Contains( t.Item2.Location)).
                GroupBy(hl => hl.Item2.Location)
                .ToList();
            var matchTextPairs = GetMatchText(matches).ToList();
            var groupedPairs = matchTextPairs.GroupBy(mtp => mtp.Item2).ToList();
            foreach(var pair in groupedPairs)
            {
                yield return string.Join(", ", pair.Select(p => p.Item1)) + ": " + pair.Key;
            }
        }

        private static IEnumerable<Tuple<string,string>> GetMatchText(IEnumerable<IGrouping<HitLocation,Tuple<HitLocationTable, ComplexHitLocation>>> matches)
        {
            foreach (var match in matches)
            {
                var penaltyText = string.Empty;
                var distinctPenalties = match.Select(m => m.Item2.Penalty).Distinct().OrderByDescending(i => i).ToArray();
                if (distinctPenalties.Length > 1)
                {
                    var penalties = match.Select(a => new Tuple<HitLocationTable, int>(a.Item1, a.Item2.Penalty.Value)).GroupBy(a => a.Item2).ToList();
                    penaltyText = $"({string.Join("; ", penalties.Select(p => string.Join(", ", p.Select(t => EnumerationHelper.PascalToNormalSpaced(t.Item1.ToString()))) + ": " + p.Key))})".Replace("Undefined", "Other");

                }
                else if (distinctPenalties.Length < 1)
                {
                    //Something
                    penaltyText = $"(Didn't find the thing in the table)";
                }
                else
                {
                    penaltyText = $"({distinctPenalties[0]:+#;-#;-0})";
                }
                var locationFormatted = $@"{match.Key.ToFormattedString()} {penaltyText}";
                yield return new Tuple<string, string>(locationFormatted, FindHitLocation(match.First().Item2, false, false));
            }
        }

        public static string FindHitLocation(ComplexHitLocation location, bool includePenaltyText = true, bool hideRedundantWords = true)
        {
            string returnValue;
            var penaltyText = string.Empty;
            var locationFormatted = string.Empty;
            if (includePenaltyText)
            {
                penaltyText = $" ({location.Penalty:+#;-#;-0})";
                locationFormatted = $@"{EnumerationHelper.PascalToNormalSpaced(location.Location.ToString())}{penaltyText}: ";
                if (hideRedundantWords)
                {
                    locationFormatted = locationFormatted.Replace("Squid ", "");
                }
            }
            switch (location.Location)
            {
                case HitLocation.Pelvis:
                    returnValue =
      $@"{locationFormatted}Major wound means automatic knockdown. You cannot stand, and until healed have Lame (Missing Legs).";
                    break;
                case HitLocation.DigestiveTract:
                    returnValue =
      $@"{locationFormatted}Major wound means rolling HT-3 to resist infection. If not playing with infection rules, treat as an abdomen hit (no special rules.)";
                    break;
                case HitLocation.Eye:
                    returnValue =
      $@"{locationFormatted}Only impaling, piercing, and tight-beam burning attacks can target the eye – and only from the front or sides. Injury over HP/10 blinds the eye. Wounding modifier is x4. Knockdown rolls are at -10. Critical hits use the Critical Head Blow Table. Exception: These special effects do not apply to toxic damage. Missing the target by 1 hits the torso instead.";
                    break;
                case HitLocation.Cheek:
                case HitLocation.Face:
                    returnValue =
     $@"{locationFormatted} If the target has an open-faced helmet, ignore its DR. Knockdown rolls are at -5. Critical hits use the Critical Head Blow Table. Corrosion damage gets a x1.5 wounding modifier, and if it inflicts a major wound, it also blinds one eye (both eyes on damage over full HP). Random attacks from behind hit the skull instead. Missing the target by 1 hits the torso instead.
(Martial Arts)When attacking from behind, the face has an additional -2 to hit.";
                    break;
                case HitLocation.Groin:
                    returnValue =
    $@"{locationFormatted}Human males and the males of similar species suffer double shock from crushing damage, and get -5 to knockdown rolls. Otherwise, treat as a torso hit. Missing the target by 1 hits the torso instead.";
                    break;
                case HitLocation.Neck:
                    returnValue =
$@"{locationFormatted}Neck and throat. Increase the wounding multiplier of crushing and corrosion attacks to x1.5, and that of cutting damage to x2. At the GM’s option, anyone killed by a cutting blow to the neck is decapitated! Missing the target by 1 hits the torso instead.";
                    break;
                case HitLocation.LeftLeg:
                case HitLocation.RightLeg:
                case HitLocation.LeftHindLeg:
                case HitLocation.RightHindLeg:
                case HitLocation.LeftForeleg:
                case HitLocation.RightForeleg:
                case HitLocation.LeftMidLeg:
                case HitLocation.RightMidLeg:
                case HitLocation.LeftArm:
                case HitLocation.RightArm:
                case HitLocation.ArmOne:
                case HitLocation.ArmTwo:
                case HitLocation.ArmThree:
                case HitLocation.ArmFour:
                case HitLocation.ArmFive:
                case HitLocation.ArmSix:
                case HitLocation.ArmSeven:
                case HitLocation.ArmEight:
                case HitLocation.LegOne:
                case HitLocation.LegTwo:
                case HitLocation.LegThree:
                case HitLocation.LegFour:
                case HitLocation.LegFive:
                case HitLocation.LegSix:
                case HitLocation.LegSeven:
                case HitLocation.LegEight:
                case HitLocation.Arm:
                case HitLocation.ArmOneAndTwo:
                case HitLocation.ArmThreeAndFour:
                case HitLocation.ArmFiveAndSix:
                case HitLocation.ArmSevenAndEight:
                case HitLocation.Foreleg:
                case HitLocation.HindLeg:
                case HitLocation.Leg:
                case HitLocation.LegOneAndTwo:
                case HitLocation.LegThreeAndFour:
                case HitLocation.LegFiveAndSix:
                case HitLocation.LegSevenAndEight:
                case HitLocation.LeftShin:
                case HitLocation.RightShin:
                case HitLocation.RightThigh:
                case HitLocation.LeftThigh:
                case HitLocation.RightUpperArm:
                case HitLocation.RightForearm:
                case HitLocation.LeftUpperArm:
                case HitLocation.LeftForearm:
                case HitLocation.Shin:
                case HitLocation.Thigh:
                case HitLocation.UpperArm:
                case HitLocation.Forearm:
                    returnValue =
$@"{locationFormatted}Reduce the wounding multiplier of large piercing, huge piercing, and impaling damage to x1. Any major wound (loss of over 1/2 HP from one blow) cripples the limb. Damage beyond that threshold is lost. If Holding a shield, double the penalty to hit.";
                    break;
                case HitLocation.LeftFoot:
                case HitLocation.RightFoot:
                case HitLocation.LeftForefoot:
                case HitLocation.RightForefoot:
                case HitLocation.LeftHindFoot:
                case HitLocation.RightHindFoot:
                case HitLocation.SquidArmThree:
                case HitLocation.SquidArmFour:
                case HitLocation.SquidArmFive:
                case HitLocation.SquidArmSix:
                case HitLocation.SquidArmSeven:
                case HitLocation.SquidArmEight:
                case HitLocation.LeftHand:
                case HitLocation.RightHand:
                case HitLocation.SquidArmThreeAndFour:
                case HitLocation.SquidArmFiveAndSix:
                case HitLocation.SquidArmSevenAndEight:
                case HitLocation.Hand:
                case HitLocation.Foot:
                    returnValue =
$@"{locationFormatted}Reduce the wounding multiplier of large piercing, huge piercing, and impaling damage to x1. Any major wound (loss of over 1/3 HP from one blow) cripples the extremity. Damage beyond that threshold is lost. If Holding a shield, double the penalty to hit.";
                    break;
                case HitLocation.Fin:
                    returnValue =
$@"{locationFormatted}Reduce the wounding multiplier of large piercing, huge piercing, and impaling damage to x1. Any major wound (loss of over 1/3 HP from one blow) cripples the fin. If the fish has more than one fin, and the hit location is being determined randomly, roll to determine which fin is hit.";
                    break;
                case HitLocation.Skull:
                    returnValue =
$@"{locationFormatted}The skull gets an extra DR 2. Wounding modifier is x4. Knockdown rolls are at -10. Critical hits use the Critical Head Blow Table. Exception: These special effects do not apply to toxic damage. Missing the target by 1 hits the torso instead.
(Martial Arts)When attacking from behind, the skull has +2 to hit.";
                    break;
                case HitLocation.Brain:
                    returnValue =
$@"{locationFormatted}The brain gets an extra DR 1. Wounding modifier is x4. Knockdown rolls are at -10. Critical hits use the Critical Head Blow Table. Exception: These special effects do not apply to toxic damage. Missing the target by 1 hits the torso instead.";
                    break;
                case HitLocation.Torso:
                case HitLocation.UpperTorso:
                case HitLocation.LowerTorso:
                case HitLocation.Chest:
                case HitLocation.UpperChest:
                case HitLocation.LowerChest:
                case HitLocation.Abdomen:
                    returnValue =
$@"{locationFormatted}As Normal.";
                    break;
                case HitLocation.Vitals:
                    returnValue =
$@"{locationFormatted}Heart, lungs, kidneys, etc. Increase the wounding modifier for an impaling or any piercing attack to x3. Increase the wounding modifier for a tight-beam burning attack to x2. Other attacks cannot target the vitals. Missing the target by 1 hits the torso instead.
(Martial Arts)Crushing attacks may target the vitals, but the wounding modifier is only x1, but shock requires a HT roll for knockdown, at -5 if a major wound.";
                    break;
                case HitLocation.LeftWing:
                case HitLocation.RightWing:
                case HitLocation.Wing:
                    returnValue =
$@"{locationFormatted}Reduce the wounding multiplier of large piercing, huge piercing, and impaling damage to x1. Any major wound (loss of over 1/2 HP from one blow) cripples the wing. Damage beyond that threshold is lost. A crippled wing means no flight.";
                    break;
                case HitLocation.Tail:
                    returnValue =
$@"{locationFormatted}Reduce the wounding multiplier of large piercing, huge piercing, and impaling damage to x1. Any major wound (loss of over 1/2 HP from one blow for a limb or striker like tail, 1/3 otherwise) cripples the tail. If crippled, terrestial targets suffer -1 DX; Avian and aquatic targets suffer -2 DX and half move.";
                    break;
                case HitLocation.Ear:
                    returnValue =
$@"{locationFormatted}If the injury is not of the cutting type, treat as a face hit. If it is cutting, injury over HP/4 is lost but has no special effect . . . but twice this amount removes the ear. This is a major wound, but without the -5 to knockdown rolls for a face hit. A miss by 1 in either case hits the torso.";
                    break;
                case HitLocation.Jaw:
                    returnValue =
$@"{locationFormatted}If the target has an open-faced helmet, ignore its DR. Knockdown rolls are at -6. Critical hits use the Critical Head Blow Table. Corrosion damage gets a x1.5 wounding modifier, and if it inflicts a major wound, it also blinds one eye (both eyes on damage over full HP). Random attacks from behind hit the skull instead. Missing the target by 1 hits the torso instead.
(Martial Arts)When attacking from behind, the face has an additional -2 to hit.";
                    break;
                case HitLocation.ExtremityJoint:
                case HitLocation.LeftHandJoint:
                case HitLocation.RightHandJoint:
                case HitLocation.LeftFootJoint:
                case HitLocation.RightFootJoint:
                case HitLocation.HandJoint:
                case HitLocation.FootJoint:
                case HitLocation.RightForefootJoint:
                case HitLocation.LeftForefootJoint:
                case HitLocation.LeftHindFootJoint:
                case HitLocation.RightHindFootJoint:
                case HitLocation.LeftMidFootJoint:
                case HitLocation.RightMidFootJoint:
                    returnValue =
$@"{locationFormatted}May only be targetted with a crushing, cutting, piercing, or tight-beam burning attack. This allows crippling with injury over HP/4 (not HP/3) for an extremity. Excess injury is lost. Dismemberment still requires twice the injury needed to cripple the whole body part – not just the joint. HT rolls to recover from crippling joint injuries are at -2. A miss by 1 hits the extremity, but not the joint.";
                    break;
                case HitLocation.LeftArmArteryOrJoint:
                case HitLocation.RightArmArteryOrJoint:
                case HitLocation.LeftLegArteryOrJoint:
                case HitLocation.RightLegArteryOrJoint:
                case HitLocation.LeftForelegArteryOrJoint:
                case HitLocation.RightForelegArteryOrJoint:
                case HitLocation.LeftHindLegArteryOrJoint:
                case HitLocation.RightHindLegArteryOrJoint:
                case HitLocation.LeftMidLegArteryOrJoint:
                case HitLocation.RightMidLegArteryOrJoint:
                case HitLocation.ArmOneArteryOrJoint:
                case HitLocation.ArmTwoArteryOrJoint:
                case HitLocation.ArmThreeArteryOrJoint:
                case HitLocation.ArmFourArteryOrJoint:
                case HitLocation.ArmFiveArteryOrJoint:
                case HitLocation.ArmSixArteryOrJoint:
                case HitLocation.ArmSevenArteryOrJoint:
                case HitLocation.ArmEightArteryOrJoint:
                case HitLocation.LegOneArteryOrJoint:
                case HitLocation.LegTwoArteryOrJoint:
                case HitLocation.LegThreeArteryOrJoint:
                case HitLocation.LegFourArteryOrJoint:
                case HitLocation.LegFiveArteryOrJoint:
                case HitLocation.LegSixArteryOrJoint:
                case HitLocation.LegSevenArteryOrJoint:
                case HitLocation.LegEightArteryOrJoint:
                    returnValue =
$@"{locationFormatted}A cutting, impaling, piercing, or tight-beam burning attack hits a vein/artery, while a crushing attack hits a joint. If the former, the sudden blood loss increases the wounding modifier for that hit location by 0.5; e.g., a cutting attack gets x2 instead of x1.5 against a limb. Since the intent is to start bleeding – not to destroy bone and muscle – ignore crippling effects and damage limits for limbs. Realistically, such injuries can cause almost instant unconsciousness, with death coming in seconds. If the latter, This allows crippling with injury over HP/3 (not HP/2) for a limb. Excess injury is lost. Dismemberment still requires twice the injury needed to cripple the whole body part – not just the joint. HT rolls to recover from crippling joint injuries are at -2.";
                    break;
                case HitLocation.LeftKnee:
                case HitLocation.RightKnee:
                case HitLocation.Knee:
                case HitLocation.RightElbow:
                case HitLocation.LeftElbow:
                case HitLocation.Elbow:
                    returnValue =
$@"{locationFormatted}A crushing attack hits a joint. This allows crippling with injury over HP/3 (not HP/2) for a limb. Excess injury is lost. Dismemberment still requires twice the injury needed to cripple the whole body part – not just the joint. HT rolls to recover from crippling joint injuries are at -2. For all other damage types, reduce the wounding multiplier of large piercing, huge piercing, and impaling damage to x1. Any major wound (loss of over 1/2 HP from one blow) cripples the limb. Damage beyond that threshold is lost. If Holding a shield, double the penalty to hit.";
                    break;
                case HitLocation.LeftThighOrArtery:
                case HitLocation.RightThighOrArtery:
                case HitLocation.RightShoulder:
                case HitLocation.LeftShoulder:
                case HitLocation.ThighOrArtery:
                case HitLocation.Shoulder:
                    returnValue =
$@"{locationFormatted}A cutting, impaling, piercing, or tight-beam burning attack hits a vein/artery. The sudden blood loss increases the wounding modifier for that hit location by 0.5; e.g., a cutting attack gets x2 instead of x1.5 against a limb. Since the intent is to start bleeding – not to destroy bone and muscle – ignore crippling effects and damage limits for limbs. Realistically, such injuries can cause almost instant unconsciousness, with death coming in seconds. For other damage types, any major wound (loss of over 1/2 HP from one blow) cripples the limb. Damage beyond that threshold is lost. If Holding a shield, double the penalty to hit.";
                    break;
                case HitLocation.LimbJoint:
                    returnValue =
$@"{locationFormatted}Can be targetted with a crushing, cutting, piercing, or tight-beam burning attack. This allows crippling with injury over HP/3 (not HP/2) for a limb. Excess injury is lost. Dismemberment still requires twice the injury needed to cripple the whole body part – not just the joint. HT rolls to recover from crippling joint injuries are at -2. A miss by 1 hits the limb but not the joint.";
                    break;
                case HitLocation.SkullOrNose:
                    returnValue =
$@"{locationFormatted}This is the face.
If attacked from the front with impaling, piercing, or tight-beam burning, this counts as a skull hit. The skull gets an extra DR 2. Wounding modifier is x4. Knockdown rolls are at -10. Critical hits use the Critical Head Blow Table. Exception: These special effects do not apply to toxic damage. Missing the target by 1 hits the torso instead.
If attacked from the front with anything else, this counts as a nose hit. Treat a hit as a face hit, but injury over HP/4 breaks the nose. This counts as a major wound to the face and mangles the nose – the victim has No Sense of Smell/Taste until the injury heals. It’s possible to angle a cutting attack to lop off the nose, in which case crippling injury counts as an ordinary major wound (no -5 to knockdown for the face) and injury in excess of this is lost. However, twice this amount takes off the nose, which reduces Appearance by two levels permanently.
Otherwise, this counts as a hit to the face. If the target has an open-faced helmet, ignore its DR. Knockdown rolls are at -5. Critical hits use the Critical Head Blow Table. Corrosion damage gets a x1.5 wounding modifier, and if it inflicts a major wound, it also blinds one eye (both eyes on damage over full HP).";
                    break;
                case HitLocation.Nose:
                    returnValue =
$@"{locationFormatted} If the target has an open-faced helmet, ignore its DR. Knockdown rolls are at -5. Critical hits use the Critical Head Blow Table. Corrosion damage gets a x1.5 wounding modifier, and if it inflicts a major wound, it also blinds one eye (both eyes on damage over full HP). Random attacks from behind hit the skull instead.
(Martial Arts)When attacking from behind, the face has an additional -2 to hit.
Injury over HP/4 breaks the nose. This counts as a major wound to the face and mangles the nose – the victim has No Sense of Smell/Taste (p. B146) until the injury heals. It’s possible to angle a cutting attack to lop off the nose, in which case crippling injury counts as an ordinary major wound (no -5 to knockdown for the face) and injury in excess of this is lost. However, twice this amount takes off the nose, which reduces Appearance by two levels permanently. In all cases, a miss by 1 hits the torso.";
                    break;
                case HitLocation.Spine:
                    returnValue =
$@"{locationFormatted}Crushing, cutting, impaling, piercing, and tight-beam burning attacks from behind can target the spine. The vertebrae provide an additional DR 3. Use the wounding modifiers for the torso, but any hit for enough injury to inflict a shock penalty requires a knockdown roll, at -5 if a major wound. Injury in excess of HP cripples the spine. This causes automatic knockdown and stunning, plus all the effects of Bad Back (Severe) (p. B123) and Lame (Paraplegic) (p. B141). Roll twice after the fight to recover, once to avoid gaining each of these disadvantages on a lasting or permanent basis! A miss by 1 hits the torso.";
                    break;
                case HitLocation.LimbArtery:
                    returnValue =
$@"{locationFormatted}A fighter with a cutting, impaling, piercing, or tight-beam burning weapon can target a major blood vessel in the arm (brachial artery) or leg (femoral artery). The sudden blood loss increases the wounding modifier for that hit location by 0.5; e.g., a cutting attack gets x2 instead of x1.5 against a limb. Since the intent is to start bleeding – not to destroy bone and muscle – ignore crippling effects and damage limits for limbs. Realistically, such injuries can cause almost instant unconsciousness, with death coming in seconds.";
                    break;
                case HitLocation.NeckVeinOrArtery:
                    returnValue =
$@"{locationFormatted}A fighter with a cutting, impaling, piercing, or tight-beam burning weapon can target a major blood vessel in the neck: the jugular vein or carotid artery. The sudden blood loss increases the wounding modifier for that hit location by 0.5; e.g., a cutting attack gets x2.5 instead of x2.0 against the neck. Realistically, such injuries can cause almost instant unconsciousness, with death coming in seconds.";
                    break;
                case HitLocation.NeckVeinOrArteryOrSpine:
                    returnValue =
$@"{locationFormatted}If the attack is a cutting, impaling, piercing, or tight-beam burning weapon can target a major blood vessel in the neck: the jugular vein or carotid artery. The sudden blood loss increases the wounding modifier for that hit location by 0.5; e.g., a cutting attack gets x2.5 instead of x2.0 against the neck. Since the intent is to start bleeding – not to destroy bone and muscle – ignore crippling effects and damage limits for limbs. Realistically, such injuries can cause almost instant unconsciousness, with death coming in seconds.
If the attack is crushing damage from behind, it counts as a spine hit, but crippling effects result in Quadriplegic (p. B150) instead of Lame (Paraplegic). The vertebrae provide an additional DR 3. Use the wounding modifiers for the torso, but any hit for enough injury to inflict a shock penalty requires a knockdown roll, at -5 if a major wound. Injury in excess of HP cripples the spine. This causes automatic knockdown and stunning. Roll twice after the fight to recover, once to avoid gaining Bad Back (severe) (p. B213) and once to avoid gaining Quadriplegic (p. B150).
Otherwise, this counts as a normal neck hit. Increase the wounding multiplier of crushing and corrosion attacks to x1.5, and that of cutting damage to x2. At the GM’s option, anyone killed by a cutting blow to the neck is decapitated!";
                    break;
                case HitLocation.VitalsOrSpine:
                    returnValue =
$@"{locationFormatted}If the attack is a cutting attack from behind, this counts as a spine hit. The vertebrae provide an additional DR 3. Use the wounding modifiers for the torso, but any hit for enough injury to inflict a shock penalty requires a knockdown roll, at -5 if a major wound. Injury in excess of HP cripples the spine. This causes automatic knockdown and stunning, plus all the effects of Bad Back (Severe) (p. B123) and Lame (Paraplegic) (p. B141). Roll twice after the fight to recover, once to avoid gaining each of these disadvantages on a lasting or permanent basis!
Otherwise, if the attack is crushing, impaling, piercing, or tight-beam burning, this counts as a hit to the vitals. Heart, lungs, kidneys, etc. Increase the wounding modifier for an impaling or any piercing attack to x3. Increase the wounding modifier for a tight-beam burning attack to x2. Crushing attacks may target the vitals, but the wounding modifier is only x1, but shock requires a HT roll for knockdown, at -5 if a major wound. Other attacks cannot target the vitals.
If the attack is crushing damage from behind, it counts as a spine hit, but crippling effects result in Quadriplegic (p. B150) instead of Lame (Paraplegic). The vertebrae provide an additional DR 3. Use the wounding modifiers for the torso, but any hit for enough injury to inflict a shock penalty requires a knockdown roll, at -5 if a major wound. Injury in excess of HP cripples the spine. This causes automatic knockdown and stunning. Roll twice after the fight to recover, once to avoid gaining Bad Back (severe) (p. B213) and once to avoid gaining Quadriplegic (p. B150).
Otherwise, this counts as a normal torso hit.";
                    break;
                case HitLocation.AbdomenOrVitals:
                    returnValue =
                        $@"{locationFormatted} If the damage type could hit the vitals, treat it as a hit to the heart, lungs, kidneys, etc. Increase the wounding modifier for an impaling or any piercing attack to x3. Increase the wounding modifier for a tight-beam burning attack to x2. Other attacks cannot target the vitals.
(Martial Arts)Crushing attacks may target the vitals, but the wounding modifier is only x1, but shock requires a HT roll for knockdown, at -5 if a major wound.
If this damage type could not target the vitals, treat it as a hit to the abdomen instead, with no special wounding modifiers or shock penalties.";
                    break;
                default:
                    returnValue = $@"There was an error looking up {EnumerationHelper.PascalToNormalSpaced(location.Location.ToString())}. Let my master know.";
                    break;

            }
            return returnValue;
        }

        /// <summary>
        /// Given a distance in string form including a type of unit, gives the size modifier.
        /// </summary>
        /// <param name="distance">distance and optional prefix</param>
        /// <returns>size modifier</returns>
        public static int GetSizeModifier(string distance)
        {
            return GetSizeModifier(ConvertToYards(distance));
        }

        /// <summary>
        /// Given the yards, returns size modifier.
        /// </summary>
        /// <param name="yards">number of yards</param>
        /// <returns>size modifier</returns>
        public static int GetSizeModifier(double yards)
        {
            if (yards <= 0)
                throw new ArgumentOutOfRangeException("yards");
            var sizeModifier = 0;
            while (yards > 100)
            {
                yards /= 10;
                sizeModifier += 6;
            }
            while(yards < 1.0 / 360)
            {
                yards *= 10;
                sizeModifier -= 6;
            }

            sizeModifier += SizeModiferTable.Where(kvp => kvp.Key >= yards).Min(kvp => kvp.Value);
            //This gets us the size modifier that is slightly 

            return sizeModifier;
        }

        private static Regex DoubleFormat = new Regex(@"(\d+)?\.?\d+([eE][+\-]\d+)?");
        private static Regex DistanceUnitExpression = new Regex(@"^(y(?:ar)?d|f(?:ee|oo)?t|mi|in|[""']|(?<![[:alpha:]])m(?:eter)?|(?<![[:alpha:]])c(?:enti)?m(?:eter)?|(?<![[:alpha:]])k(?:ilo)?m(?:eter)?)", RegexOptions.IgnoreCase);

        private enum DistanceUnit
        {
            Inch,
            Centimeter,
            Foot,
            Meter,
            /// <summary>
            /// Default
            /// </summary>
            Yard,
            Kilometer,
            Mile
        }

        private static Dictionary<DistanceUnit, double> YardDistanceFactors = new Dictionary<DistanceUnit, double>
        {
            {DistanceUnit.Centimeter, (1.0/36)/2.5 },
            {DistanceUnit.Inch, 1.0/36 },
            {DistanceUnit.Foot, 1.0/3 },
            {DistanceUnit.Meter, 1 },
            {DistanceUnit.Yard, 1 },
            {DistanceUnit.Kilometer, 2000/1.5 },
            {DistanceUnit.Mile, 2000 } //Yes, this isn't correct in real life, but that's the way it is in GURPS.
        };

        private static Dictionary<DistanceUnit, double> MileDistanceFactors = new Dictionary<DistanceUnit, double>
        {
            {DistanceUnit.Centimeter, (1.0/(2000*36))/2.5 },
            {DistanceUnit.Inch, 1.0/(2000*36) },
            {DistanceUnit.Foot, 1.0/(2000*3) },
            {DistanceUnit.Meter, 1.0/2000 },
            {DistanceUnit.Yard, 1.0/2000 },
            {DistanceUnit.Kilometer, 2.0/3 },
            {DistanceUnit.Mile, 1 } 
        };


        private static double ConvertToMiles(string distance)
        {
            var distanceParts = distance.Split(' ');
            var numericalSection = distanceParts[0];
            var miles = double.Parse(numericalSection);
            var unit = distanceParts.Length == 1 ? "mile" : distanceParts[1];
            if (DistanceUnitExpression.IsMatch(unit))
            {
                var matchedUnit = DistanceUnitExpression.Match(unit).Value.ToUpperInvariant();
                var factor = 1.0;
                switch (matchedUnit)
                {
                    case "CM":
                    case "CENTIMETER":
                    case "CENTIM": factor = MileDistanceFactors[DistanceUnit.Centimeter]; break;
                    case "IN":
                    case "\"": factor = MileDistanceFactors[DistanceUnit.Inch]; break;
                    case "FT":
                    case "FEET":
                    case "FOOT":
                    case "'": factor = MileDistanceFactors[DistanceUnit.Foot]; break;
                    case "METER":
                    case "M": factor = MileDistanceFactors[DistanceUnit.Meter]; break;
                    case "YARD":
                    case "YD": factor = MileDistanceFactors[DistanceUnit.Yard]; break;
                    case "KM":
                    case "KILOMETER":
                    case "KILOM": factor = MileDistanceFactors[DistanceUnit.Kilometer]; break;
                    case "MI": factor = MileDistanceFactors[DistanceUnit.Mile]; break;
                }

                miles *= factor;
            }
            else
            {
                throw new ArgumentException(nameof(distance), "Didn't understand the unit, nyaa~");
            }

            return miles;
        }

        /// <summary>
        /// Converts a string that might contain a 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double ConvertToYards(string length)
        {
            var lengthParts = length.Split(' ');
            var numericalSection = lengthParts[0];
            var yards = double.Parse(numericalSection);
            var unit = lengthParts.Length == 1 ? "yard" : lengthParts[1];
            if (DistanceUnitExpression.IsMatch(unit))
            {
                var matchedUnit = DistanceUnitExpression.Match(unit).Value.ToUpperInvariant();
                var factor = 1.0;
                switch(matchedUnit)
                {
                    case "CM":
                    case "CENTIMETER":
                    case "CENTIM": factor = YardDistanceFactors[DistanceUnit.Centimeter]; break;
                    case "IN":
                    case "\"": factor = YardDistanceFactors[DistanceUnit.Inch]; break;
                    case "FT":
                    case "FEET":
                    case "FOOT":
                    case "'":  factor = YardDistanceFactors[DistanceUnit.Foot]; break;
                    case "METER":
                    case "M": factor = YardDistanceFactors[DistanceUnit.Meter]; break;
                    case "YARD":
                    case "YD": factor = YardDistanceFactors[DistanceUnit.Yard]; break;
                    case "KM":
                    case "KILOMETER":
                    case "KILOM": factor = YardDistanceFactors[DistanceUnit.Kilometer]; break;
                    case "MI": factor = YardDistanceFactors[DistanceUnit.Mile]; break;
                        
                }

                yards *= factor;
            }
            else
                throw new ArgumentException(nameof(length), "Didn't understand units, nyaa.");

            return yards;
        }
//1.0"
//1.1'
//1 yd
//1e+100 mile
//1.01e-100"



        public static Dictionary<int, int> RateOfFireBonus { get; } =
            new Dictionary<IEnumerable<int>, int>
            {
                                { Enumerable.Range(2, 3), 0},
                                { Enumerable.Range(5, 4), 1},
                                { Enumerable.Range(9, 4), 2},
                                { Enumerable.Range(13, 4), 3},
                                { Enumerable.Range(17, 8), 4},
                                { Enumerable.Range(25, 25), 5},
                                { Enumerable.Range(50, 50), 6}
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, int>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static int GetRateOfFireBonus(int rateOfFire)
        {
            rateOfFire = Math.Max(2, rateOfFire); 
            var bonus = rateOfFire > 99 ?
                (int)Math.Floor((Math.Log(rateOfFire / 50.0, 2.0))) + 6 :
                RateOfFireBonus[rateOfFire];

            return bonus;
        }

        public static Dictionary<int, string> FrightCheckTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {
                {Enumerable.Range(4,2), "Stunned for one second, then recover automatically." },
                {Enumerable.Range(6,2), "Stunned for one second. Every second after that, roll vs. unmodified Will to snap out of it." },
                {Enumerable.Range(8,2), "Stunned for one second. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it." },
                {Enumerable.Range(10,1), "Stunned for 1d seconds. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it." },
                {Enumerable.Range(11,1), "Stunned for 2d seconds. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it." },
                {Enumerable.Range(12,1), "Retching for (25 - HT) seconds, and then roll vs. HT each second to recover." },
                {Enumerable.Range(13,1), "Acquire a new mental quirk." },
                {Enumerable.Range(14,2), "Lose 1d FP, and stunned for 1d seconds. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it." },
                {Enumerable.Range(16,1), "Stunned for 1d seconds. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it. Also acquire a new quirk." },
                {Enumerable.Range(17,1), "Faint for 1d minutes, then roll vs. HT each minute to recover" },
                {Enumerable.Range(18,1), "Faint for 1d minutes, then roll vs. HT each minute to recover, and roll vs. HT immediately. On a failed roll, take 1 HP of injury as you collapse." },
                {Enumerable.Range(19,1), "Severe faint, lasting for 2d minutes. Roll vs. HT each minute to recover. Take 1 HP of injury." },
                {Enumerable.Range(20,1), "Faint bordering on shock, lasting for 4d minutes. Also, lose 1d FP." },
                {Enumerable.Range(21,1), "Panic. You run around screaming, sit down and cry, or do something else equally pointless for 1d minutes. At the end of that time, roll vs. unmodified Will once per minute to snap out of it." },
                {Enumerable.Range(22,1), "Acquire a -10-point Delusion." },
                {Enumerable.Range(23,1), "Acquire a -10-point Phobia or other -10-point mental disadvantage." },
                {Enumerable.Range(24,1), "Major physical effect, set by GM: hair turns white, age five years overnight, go partially deaf, etc. In game terms, acquire -15 points worth of physical disadvantages (for this purpose, each year of age counts as -3 points)." },
                {Enumerable.Range(25,1), "If you already have a Phobia or other mental disadvantage that is logically related to the frightening incident, your self-control number becomes one step worse. If not, or if your self-control number is already 6, add a new -10-point Phobia or other -10-point mental disadvantage." },
                {Enumerable.Range(26,1), "Faint for 1d minutes, then roll vs. HT each minute to recover, and roll vs. HT immediately. On a failed roll, take 1 HP of injury as you collapse. Acquire a -10 point delusion." },
                {Enumerable.Range(27,1), "Faint for 1d minutes, then roll vs. HT each minute to recover, and roll vs. HT immediately. On a failed roll, take 1 HP of injury as you collapse. Acquire a -10 point mental disadvantage." },
                {Enumerable.Range(28,1), "Light coma. You fall unconscious, rolling vs. HT every 30 minutes to recover. For 6 hours after you come to, all skill rolls and attribute checks are at -2." },
                {Enumerable.Range(29,1), "Coma. You fall unconscious for 1d hours, roll vs. HT every 1d hours to recover. For 6 hours after you come to, all skill rolls and attribute checks are at -2." },
                {Enumerable.Range(30,1), "Catatonia. Stare into space for 1d days. Then roll vs. HT. On a failed roll, remain catatonic for another 1d days, and so on. If you have no medical care, lose 1 HP the first day, 2 the second, and so on. If you survive and awaken, all skill rolls and attribute checks are at -2 for as many days as the catatonia lasted." },
                {Enumerable.Range(31,1), "Seizure. You lose control of your body and fall to the ground in a fit lasting 1d minutes and costing 1d FP. Also, roll vs. HT. On a failure, take 1d of injury. On a critical failure, you also lose 1 HT *permanently*." },
                {Enumerable.Range(32,1), "Stricken. You fall to the ground, taking 2d of injury in the form of a mild heart attack or stroke." },
                {Enumerable.Range(33,1), "Total panic. You are out of control; you might do *anything* (the GM rolls 3d: the higher the roll, the more useless your reaction). For instance, you might jump off a cliff to avoid the monster. If you survive your first reaction, roll vs. Will to come out of the panic. If you fail, the GM rolls for another panic reaction, and so on!" },
                {Enumerable.Range(34,1), "Acquire a -15-point Delusion." },
                {Enumerable.Range(35,1), "Acquire a -15-point Phobia or other mental disadvantage worth -15 points." },
                {Enumerable.Range(36,1), "Sever physical effect, set by GM: hair turns white, age five years overnight, go partially deaf, etc. In game terms, acquire -20 points worth of physical disadvantages (for this purpose, each year of age counts as -3 points)." },
                {Enumerable.Range(37,1), "Sever physical effect, set by GM: hair turns white, age five years overnight, go partially deaf, etc. In game terms, acquire -30 points worth of physical disadvantages (for this purpose, each year of age counts as -3 points)." },
                {Enumerable.Range(38,1), "Coma. You fall unconscious for 1d hours, roll vs. HT every 1d hours to recover. For 6 hours after you come to, all skill rolls and attribute checks are at -2. Acquire a -15-point Delusion." },
                {Enumerable.Range(39,1), "Coma. You fall unconscious for 1d hours, roll vs. HT every 1d hours to recover. For 6 hours after you come to, all skill rolls and attribute checks are at -2. Acquire a -15-point Phobia or other mental disadvantage worth -15 points." },
                {Enumerable.Range(40,1), "Coma. You fall unconscious for 1d hours, roll vs. HT every 1d hours to recover. For 6 hours after you come to, all skill rolls and attribute checks are at -2. Acquire a -15-point Phobia or other mental disadvantage worth -15 points. IQ reduced by 1 *permanently*." },
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> AweCheckTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {
                {Enumerable.Range(4,2), "Stunned for one second, then recover automatically." },
                {Enumerable.Range(6,2), "Stunned for one second. Every second after that, roll vs. unmodified Will to snap out of it." },
                {Enumerable.Range(8,2), "Stunned for 1d seconds. Every second after that, roll vs. Will to snap out of it." },
                {Enumerable.Range(10,2), "Stunned for 2d seconds. Every second after that, roll vs. Will to snap out of it." },
                {Enumerable.Range(12,2), "Ecstasy for (25 - Will) seconds, and then roll vs. Will each second to recover." },
                {Enumerable.Range(14,2), "Acquire a new mental quirk." },
                {Enumerable.Range(16,1), "Stunned for 1d seconds. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it. Also acquire a new quirk." },
                {Enumerable.Range(17,2), "Ecstasy for 1d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(19,1), "Ecstasy for 2d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(20,1), "Ecstasy for 4d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(21,1), "You worship at the feet of the one who awed you – you must obey his *every* command as if you had Slave Mentality! This lasts 3d minutes; then roll vs. Will once per minute to snap out of it." },
                {Enumerable.Range(22,2), "Acquire a -10-point mental disadvantage. This might impel you to adopt one of your new idol’s self-imposed mental disadvantages out of solidarity, turn you into his servant (Reprogrammable), or make you feel inferior (Low Self-Image)." },
                {Enumerable.Range(24,2), "Acquire a -10-point mental disadvantage. This might impel you to adopt one of your new idol’s self-imposed mental disadvantages out of solidarity, turn you into his servant (Reprogrammable), or make you feel inferior (Low Self-Image). In addition, if you already have a -5 to -10-point disadvantage that could result from Awe, it worsens to a -15-point trait!" },
                {Enumerable.Range(26,2), "Ecstasy for 1d minutes, then roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage. This might impel you to adopt one of your new idol’s self-imposed mental disadvantages out of solidarity, turn you into his servant (Reprogrammable), or make you feel inferior (Low Self-Image)." },
                {Enumerable.Range(28,2), "Ecstasy for 2d minutes, then roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage. This might impel you to adopt one of your new idol’s self-imposed mental disadvantages out of solidarity, turn you into his servant (Reprogrammable), or make you feel inferior (Low Self-Image)." },
                {Enumerable.Range(30,2), "Ecstasy for 2d minutes, then roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage. This might impel you to adopt one of your new idol’s self-imposed mental disadvantages out of solidarity, turn you into his servant (Reprogrammable), or make you feel inferior (Low Self-Image)." },
                {Enumerable.Range(32,2), "Awe overcomes you. You immediately collapse in a helpless, ecstatic fit that lasts 1d minutes and costs 1d FP. After that time, roll vs. Will each minute to recover. Any critical failure costs you 1 Will permanently." },
                {Enumerable.Range(34,2), "Acquire a -15-point mental disadvantage. This might impel Fanaticism." },
                {Enumerable.Range(36,1), "Acquire a -20-point mental disadvantage." },
                {Enumerable.Range(37,1), "Acquire a -30-point mental disadvantage. This might be made up of multiple traits." },
                {Enumerable.Range(38,1), "Ecstasy for 1d minutes, then roll vs. Will each minute to recover. Acquire a -15-point mental disadvantage. This might impel Fanaticism." },
                {Enumerable.Range(39,1), "Ecstasy for 2d minutes, then roll vs. Will each minute to recover. Acquire a -20-point mental disadvantage." },
                {Enumerable.Range(40,1), "Ecstasy for 2d minutes, then roll vs. Will each minute to recover. Acquire a -20-point mental disadvantage. Lose 1 point of Will *permanently.*" },
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> ConfusionCheckTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {
                {Enumerable.Range(4,2), "Stunned for one second, then recover automatically." },
                {Enumerable.Range(6,2), "Stunned for one second. Every second after that, roll vs. unmodified Will to snap out of it." },
                {Enumerable.Range(8,2), "Stunned for 1d seconds. Every second after that, roll vs. Will to snap out of it." },
                {Enumerable.Range(10,2), "Stunned for 2d seconds. Every second after that, roll vs. Will to snap out of it." },
                {Enumerable.Range(12,2), "Daze for (25 - IQ) seconds, and then roll vs. Will each second to recover." },
                {Enumerable.Range(14,2), "Acquire a new mental quirk." },
                {Enumerable.Range(16,1), "Stunned for 1d seconds. Every second after that, roll vs. Will, plus whatever bonuses or penalties you had on your original roll, to snap out of it. Also acquire a new quirk." },
                {Enumerable.Range(17,2), "Hallucination for 1d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(19,1), "Hallucination for 2d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(20,1), "Hallucination for 4d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(21,1), "You hallucinate (the GM specifies the details, which should fit the situation); you can try to act, but you’re out of touch with reality and at -5 on all success rolls. This lasts 3d minutes; then roll vs. Will once per minute to snap out of it." },
                {Enumerable.Range(22,2), "Acquire a -10-point mental disadvantage. This \"blows your mind,\" most likely resulting in one of Confused (12), Delusion (Major), Indecisive (12), or Short Attention Span (12)." },
                {Enumerable.Range(24,2), "Acquire a -10-point mental disadvantage. This \"blows your mind,\" most likely resulting in one of Confused (12), Delusion (Major), Indecisive (12), or Short Attention Span (12). In addition, if you already have a -5 to -10-point disadvantage that could result from Awe, it worsens to a -15-point trait!" },
                {Enumerable.Range(26,2), "Hallucination for 1d minutes, then roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage. This \"blows your mind,\" most likely resulting in one of Confused (12), Delusion (Major), Indecisive (12), or Short Attention Span (12)." },
                {Enumerable.Range(28,2), "Hallucination for 2d minutes, then roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage. This \"blows your mind,\" most likely resulting in one of Confused (12), Delusion (Major), Indecisive (12), or Short Attention Span (12)." },
                {Enumerable.Range(30,2), "Hallucination for 2d minutes, then roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage. This \"blows your mind,\" most likely resulting in one of Confused (12), Delusion (Major), Indecisive (12), or Short Attention Span (12)." },
                {Enumerable.Range(32,2), "Confusion drives you completely mad. You might do anything! The GM rolls 3d – the higher the roll, the more dangerous the action. For instance, you might believe you can fly and leap to your doom. Should you survive your first reaction, roll vs. Will to recover. If you fail, the GM rolls for another insane action, and so on." },
                {Enumerable.Range(34,2), "Acquire a -15-point mental disadvantage. This tends to cause Confused (9), Delusion (Severe), Indecisive (9), On the Edge (12), or Short Attention Span (9)." },
                {Enumerable.Range(36,1), "Acquire a -20-point mental disadvantage." },
                {Enumerable.Range(37,1), "Acquire a -30-point mental disadvantage. This might be made up of multiple traits." },
                {Enumerable.Range(38,1), "Hallucination for 1d minutes, then roll vs. Will each minute to recover. Acquire a -15-point mental disadvantage. This tends to cause Confused (9), Delusion (Severe), Indecisive (9), On the Edge (12), or Short Attention Span (9)." },
                {Enumerable.Range(39,1), "Hallucination for 2d minutes, then roll vs. Will each minute to recover. Acquire a -20-point mental disadvantage." },
                {Enumerable.Range(40,1), "Hallucination for 2d minutes, then roll vs. Will each minute to recover. Acquire a -20-point mental disadvantage. Lose 1 point of IQ *permanently.*" },
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> DespairCheckTable { get; } =
    new Dictionary<IEnumerable<int>, string>
    {
                {Enumerable.Range(4,2), "Stunned for one second, then recover automatically." },
                {Enumerable.Range(6,4), "Stunned for one second. Every second after that, roll vs. unmodified Will to snap out of it." },
                {Enumerable.Range(10,2), "Stunned for 1d seconds. Every second after that, roll vs. Will to snap out of it." },
                {Enumerable.Range(12,1), "Daze for (25 - Will) seconds." },
                {Enumerable.Range(13,1), "Crying for (25 - Will) seconds. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs Will to recover." },
                {Enumerable.Range(14,2), "Acquire a new mental quirk." },
                {Enumerable.Range(16,1), "Stunned for 1d seconds. Every second after that, roll vs. unmodified Will to snap out of it. Also acquire a new quirk." },
                {Enumerable.Range(17,1), "Dazed for 1d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(18,1), "Crying for 1d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover." },
                {Enumerable.Range(19,1), "Dazed for 2d minutes, then roll vs. Will each minute to recover." },
                {Enumerable.Range(20,1), "Crying for 2d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover." },
                {Enumerable.Range(21,1), "Crying for 4d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover." },
                {Enumerable.Range(22,2), "Acquire a -10-point mental disadvantage related to despair." },
                {Enumerable.Range(24,2), "Acquire a -10-point mental disadvantage related to despair, unless you already have a -5 or -10 disadvantage in the same vein, which worsens to a -15-point trait." },
                {Enumerable.Range(26,2), "Crying for 1d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage related to despair, unless you already have a -5 or -10 disadvantage in the same vein, which worsens to a -15-point trait." },
                {Enumerable.Range(28,2), "Crying for 2d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage related to despair, unless you already have a -5 or -10 disadvantage in the same vein, which worsens to a -15-point trait." },
                {Enumerable.Range(30,1), "Catatonia. Stare into space for 1d days. Then roll vs. HT. On a failed roll, remain catatonic for another 1d days, and so on. If you have no medical care, lose 1 HP the first day, 2 the second, and so on. If you survive and awaken, all skill rolls and attribute checks are at -2 for as many days as the catatonia lasted." },
                {Enumerable.Range(31,2), "Crying for 4d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover. Acquire a -10-point mental disadvantage related to despair, unless you already have a -5 or -10 disadvantage in the same vein, which worsens to a -15-point trait." },
                {Enumerable.Range(33,1), "Overwhelmed by despair for 2d minutes. You lie down and refuse to do anything because it is pointless." },
                {Enumerable.Range(34,2), "Acquire a -15-point mental disadvantage related to despair." },
                {Enumerable.Range(36,1), "Acquire a -20-point mental disadvantage related to despair." },
                {Enumerable.Range(37,1), "Acquire a -30-point mental disadvantage related to despair." },
                {Enumerable.Range(38,1), "Crying for 1d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover. Acquire a -15-point mental disadvantage related to despair." },
                {Enumerable.Range(39,1), "Crying for 2d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover. Acquire a -20-point mental disadvantage related to despair." },
                {Enumerable.Range(40,1), "Crying for 2d minutes. Like agony but ignoring high or low pain threshold and only losing 1 FP every two minutes. After this period, roll vs. Will each minute to recover. Acquire a -20-point mental disadvantage related to despair. Lose 1 point of Will *permanently.*" },
    }.
    SelectMany
    (
        kvp => kvp.Key.Select
        (
            key => new KeyValuePair<int, string>(key, kvp.Value)
        )
    ).
    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static string GetReactionNotes(string reaction, string specialLookupTable)
        {
            var returnValue = string.Empty;
            switch(specialLookupTable.ToUpperInvariant())
            {
                case "AID":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "The request is denied totally. Make a “potential combat” roll at -4; no reaction better than Neutral is possible. If combat is called for but not possible, the NPC opposes the PCs in any way possible."; break;
                        case "VERY BAD": returnValue = "The request is denied. Make a “potential combat” roll; no reaction better than Neutral is possible.If combat is called for but not possible, the NPC opposes the PCs in some other way."; break;
                        case "BAD": returnValue = "The request is denied. The NPC goes about his business, ignoring the PCs."; break;
                        case "POOR": returnValue = "The request is denied, but the PCs can try again, at -2; bribes, pleas, or threats may work."; break;
                        case "NEUTRAL": returnValue = "Simple requests for aid are granted. Complex requests are denied, but the PCs can try again, at - 2."; break;
                        case "GOOD": returnValue = "Reasonable requests for aid are granted. Even if the request is silly and must be denied, the NPCs offer helpful advice."; break;
                        case "VERY GOOD": returnValue = "Requests for aid are granted unless they are totally unreasonable. The NPCs volunteer any relevant information they have freely."; break;
                        case "EXCELLENT": returnValue = "Requests for aid are granted, and extra aid is offered; the NPCs do everything they can to help."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "ADMISSION":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "No chance of getting in; make a “potential combat” roll at -2."; break;
                        case "VERY BAD": returnValue = "No chance of getting in; after any further attempt, make a “potential combat” roll at -2."; break;
                        case "BAD": returnValue = "No chance of getting in; further attempts will be ignored."; break;
                        case "POOR": returnValue = "Request for entry denied, but bribes, pleas, or threats might work. The PCs may roll again at -2."; break;
                        case "NEUTRAL": returnValue = "Request for entry granted after a delay to get approval from someone in authority."; break;
                        case "GOOD": returnValue = "Request for entry granted with mild restrictions, such as leaving weapons at the door."; break;
                        case "VERY GOOD": returnValue = "Request for entry granted."; break;
                        case "EXCELLENT": returnValue = " Request for entry granted enthusiastically; +2 on subsequent reaction rolls during this visit."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "AUTHORITY":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "The PCs are arrested and charged with a crime. In the course of being arrested, they are physically mistreated."; break;
                        case "VERY BAD": returnValue = "The PCs are arrested and charged with a crime. If they are uncooperative, make a “potential combat” roll at -2; on a Bad or worse result, they will be slammed into a wall, beaten, hit with an electric stun weapon, or otherwise forcibly subdued."; break;
                        case "BAD": returnValue = "The PCs are detained and questioned for at least several hours. If they are uncooperative, make a “potential combat” roll at -2."; break;
                        case "POOR": returnValue = "The PCs are detained and questioned for an hour. If they are uncooperative, make a “potential combat” roll, as above."; break;
                        case "NEUTRAL": returnValue = "The PCs are questioned for a few minutes and then allowed to go about their business. The questioners will have consciously noticed them and may remember them."; break;
                        case "GOOD": returnValue = "The PCs are accepted as legitimate."; break;
                        case "VERY GOOD": returnValue = "The PCs are accepted as legitimate, and make a good impression: +2 on further reaction rolls."; break;
                        case "EXCELLENT": returnValue = "The PCs are treated deferentially and offered assistance."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "COMBAT":
                    switch(reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS" : returnValue = "The NPCs attack viciously, asking no quarter and giving none."; break;
                        case "VERY BAD": returnValue = "The NPCs attack, and flee only if they see they have no chance. A fight in progress continues."; break;
                        case "BAD": returnValue = "The NPCs attack unless outnumbered; if outnumbered they flee, possibly attempting an ambush later. A fight in progress continues."; break;
                        case "POOR": returnValue = "The NPCs shout threats or insults. They demand that the PCs leave the area. If the PCs stay, the NPC attack unless outnumbered; if outnumbered they flee. A fight in progress continues."; break;
                        case "NEUTRAL": returnValue = "The NPCs go their own way and let the PCs go theirs. If a fight is in progress, the NPCs try to back off."; break;
                        case "GOOD": returnValue = "The NPCs find the PCs likable, or too formidable to attack.The PCs may request aid or information or give information; roll again at + 1. If a fight is in progress, the NPCs flee if they can."; break;
                        case "VERY GOOD": returnValue = "The NPCs are friendly. The PCs may request aid or information or give information; roll again at + 3. Even sworn foes find an excuse to let the PCs go... for now.If a fight is in progress, the NPCs flee if they can, or surrender."; break;
                        case "EXCELLENT": returnValue = "The NPCs are extremely friendly, and may even join the party temporarily. The PCs may request aid or information or give information; roll again at + 5. If a fight is in progress, the NPCs surrender."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "COMMERCE":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "Make a \"potential combat\" roll at -2."; break;
                        case "VERY BAD": returnValue = "Sells at 3x, can be haggled to 1.5x. Buys at 1/3 and can be haggled to 2/3."; break;
                        case "BAD": returnValue = "Sells at 2x, can be haggled to 1x. Buys at 1/2 and can be haggled to 100%."; break;
                        case "POOR": returnValue = "Sells at 1.2x, can be haggled to 1x. Buys at 3/4 and can be haggled to 100%."; break;
                        case "NEUTRAL": returnValue = "Buys and sells at 1x, and in haggling cultures can be convinced to go from 0.9x to 1.1x."; break;
                        case "GOOD": returnValue = "Buys and sells at 1x, and in haggling cultures can be convinced to go from 0.9x to 1.1x. Helpful."; break;
                        case "VERY GOOD": returnValue = "Sells at 1x, can be haggled to 0.8x. Buys at 100% and can be haggled to 1.5x."; break;
                        case "EXCELLENT": returnValue = "Sells at 1x, can be haggled to 0.5x. Buys at 100% and can be haggled to 2x."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "HIRING":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "The organization is hostile to the PC and will act against him if it can: calling the police, reporting him to the authorities, blacklisting him, etc. This often leads to a “confrontation with authority” roll at -2."; break;
                        case "VERY BAD": returnValue = "The organization rejects the PC and is not open to future inquiries. If he persists or returns, make a “confrontation with authority” roll at -2."; break;
                        case "BAD": returnValue = "The organization finds the PC’s qualifications unsuitable; future applications are at a cumulative -2."; break;
                        case "POOR": returnValue = "The organization doesn’t hire the PC."; break;
                        case "NEUTRAL": returnValue = "The organization will hire the PC for unskilled labor, but not if he’s asking for a skilled job."; break;
                        case "GOOD": returnValue = "The organization will hire the PC for an entry-level job in his field."; break;
                        case "VERY GOOD": returnValue = "The organization will hire the PC at his established salary range and equivalent Rank (if applicable)."; break;
                        case "EXCELLENT": returnValue = "The organization values the PC highly and will design a new job around his qualifications, if necessary. If he takes the job, he gains higher Rank(if applicable) and salary. He can spend earned points to acquire the organization as a Patron."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "INFORMATION":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "Questions are met with anger; make a “potential combat” roll at -2."; break;
                        case "VERY BAD": returnValue = "Questions are answered with malicious lies."; break;
                        case "BAD": returnValue = "The NPCs lie maliciously or demand payment for information. If paid, the NPC gives true, but incomplete, information."; break;
                        case "POOR": returnValue = "The NPCs claim not to know, or give incomplete answers. A bribe may improve their memory; roll again if a bribe is offered."; break;
                        case "NEUTRAL": returnValue = "The NPC answers a simple question fully, or gives a sketchy answer to a complex question."; break;
                        case "GOOD": returnValue = "The question is answered accurately."; break;
                        case "VERY GOOD": returnValue = "The NPC answers in detail and volunteers any related information known to him."; break;
                        case "EXCELLENT": returnValue = "The question is answered completely. If the NPC doesn’t know the full answer, he makes further inquiries on the PCs’ behalf. He may volunteer to help; make a “request for aid” roll at + 2, if the request is denied, ignore whatever it says about rolling for potential combat and allow a reroll at -2."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "LOYALTY":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "The NPC hates the PCs or is in the pay of their enemies, and takes the first good chance to betray them."; break;
                        case "VERY BAD": returnValue = "The NPC dislikes the PCs, and will abandon them if possible(usually taking everything he can carry off) or sell them out if not, at the first good opportunity."; break;
                        case "BAD": returnValue = "The NPC has no respect for the PCs. He will leave or betray them with even moderate temptation, and doesn’t work hard for them."; break;
                        case "POOR": returnValue = "The NPC is unimpressed by the PCs or dislikes his position with them, thinking he could have done better. He’ll betray them if offered enough, and will take a better position if he’s offered one and allowed to leave."; break;
                        case "NEUTRAL": returnValue = "The NPC doesn’t think the PCs or the position is anything special. He works just hard enough to keep complaints to a minimum.He won’t betray the PCs without very strong temptation, or leave except for a clearly better position."; break;
                        case "GOOD": returnValue = "The NPC likes the PCs and the position. He is loyal, works hard, and accepts any reasonable hazard that the PCs share."; break;
                        case "VERY GOOD": returnValue = "The NPC makes the PCs’ interests his top priority; he works very hard and will risk his life if necessary."; break;
                        case "EXCELLENT": returnValue = "The NPC is devoted to the PCs or their cause, puts their interests first at all times, works extremely hard for them, and would gladly die in their service."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "RECREATION":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "The PCs are met with hostility. Make a “potential combat” roll at -2."; break;
                        case "VERY BAD": returnValue = "The PCs are unwelcome. If they don’t leave quickly, make a “potential combat” roll at -2."; break;
                        case "BAD": returnValue = "The PCs are met with insults and threats, but not with actual combat, unless they start a fight."; break;
                        case "POOR": returnValue = "The PCs’ company is flatly rejected."; break;
                        case "NEUTRAL": returnValue = "The PCs’ company is unwelcome, but they are put off with excuses or evasions, rather than decisively rejected. They can approach the same people another time and roll again."; break;
                        case "GOOD": returnValue = "The PCs’ company is accepted, without lasting commitment."; break;
                        case "VERY GOOD": returnValue = "The PCs’ company is accepted, and they make a good impression: +2 on further reaction rolls."; break;
                        case "EXCELLENT": returnValue = "The PCs are accepted as lasting friends; roll for loyalty at +2."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "SEDUCTION":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "Angry or outraged rejection. Make a “potential combat” roll at -4; failure leads to physical retribution, by the person approached or by a protector."; break;
                        case "VERY BAD": returnValue = "Openly disgusted rejection; any further reaction rolls are at -2."; break;
                        case "BAD": returnValue = "Cold rejection; any further reaction rolls are at -2, but a sincere apology can remove the penalty."; break;
                        case "POOR": returnValue = "Rejection, but with no offense taken; a further approach may be made later at no penalty, if the relationship has developed or the suitor’s position has improved. (Exception: If suitor’s approach violated cultural norms, treat as cold rejection, any further reaction rolls are at -2, but a sincere apology can remove the penalty.)"; break;
                        case "NEUTRAL": returnValue = "Indifference, but no offense is taken; a further approach may be made at no penalty."; break;
                        case "GOOD": returnValue = "Agreement to meet privately; a further approach may be made at no penalty."; break;
                        case "VERY GOOD": returnValue = "Agreement to actual physical contact."; break;
                        case "EXCELLENT": returnValue = "Strong desire for actual physical contact, giving +2 to rolls for sexual activity. Lasting feelings of attraction or attachment; roll for loyalty at +2."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                case "TESTIMONY":
                    switch (reaction.ToUpperInvariant())
                    {
                        case "DISASTROUS": returnValue = "The PC’s testimony is rejected angrily; make a “potential combat” roll at -2."; break;
                        case "VERY BAD": returnValue = "The PC’s testimony is rejected as an obvious Delusion; those who hear of it react at - 1."; break;
                        case "BAD": returnValue = "The PC’s testimony is treated as a lie, and dismissed. If he is testifying in court, make another reaction roll; on a Poor or worse reaction, he faces charges of perjury, and needs a lawyer."; break;
                        case "POOR": returnValue = "The PC’s testimony is not believed."; break;
                        case "NEUTRAL": returnValue = "The PC’s testimony is doubted, but may get a further hearing if he can produce a second witness, supporting evidence, or a clear explanation of his claims, or if he can appeal to human or religious proof of his honesty; roll again at - 2."; break;
                        case "GOOD": returnValue = "The PC’s testimony is accepted."; break;
                        case "VERY GOOD": returnValue = "The PC’s testimony is accepted, and he makes a good impression overall: +2 on further reaction rolls."; break;
                        case "EXCELLENT": returnValue = "The PC’s testimony is taken as totally convincing, even if his claims would ordinarily been treated as Delusions."; break;
                        default: returnValue = "Something weird happened looking up extra notes."; break;
                    }
                    break;
                default: throw new ArgumentException("Table name invalid", nameof(specialLookupTable));
            }
            return returnValue;
        }

        public static Dictionary<int, string> CriticalHitTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {
                {new int[] {3,18 }, "The blow does triple damage." },
                {new int[] {4,17 }, "The target’s DR protects at half value (round down) after applying any armor divisors." },
                {new int[] {5,16 }, "The blow does double damage." },
                {new int[] {6,15 }, "The blow does maximum normal damage." },
                {new int[] {7,13,14 }, "If *any* damage penetrates DR, treat it as if it were a major wound, regardless of the actual injury inflicted." },
                {new int[] {8 }, "If any damage penetrates DR, it inflicts double normal shock (to a maximum penalty of -8). If the injury is to a limb or extremity, that body part is crippled as well. This is only a “funny-bone” injury: crippling wears off in (16 - HT) seconds, minimum two seconds, unless the injury was enough to cripple the body part anyway." },
                {Enumerable.Range(9,3), "Normal damage only." },
                {new int[] {12 }, "Normal damage, and the victim drops anything he is holding, regardless of whether any damage penetrates DR." }
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> CriticalHeadBlowTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {
                
                {new int[] {3 }, "The blow does maximum normal damage and ignores the target’s DR." },
                {new int[] {4, 5 }, "The target’s DR protects at half value (round down) after applying any armor divisors. If *any* damage penetrates, treat it as if it were a major wound, regardless of the actual injury inflicted." },
                {new int[] {6, 7 }, "If the attack targeted the *face* or *skull*, treat it as an eye hit instead, even if the attack could not normally target the eye! If an eye hit is impossible (e.g., from behind), The target’s DR protects at half value (round down) after applying any armor divisors. If *any* damage penetrates, treat it as if it were a major wound, regardless of the actual injury inflicted." },
                {new int[] {8 }, "Normal head-blow damage, and the victim is knocked off balance: he must Do Nothing next turn (but may defend normally)." },
                {Enumerable.Range(9,3), "Normal head-blow damage only." },
                {Enumerable.Range(12,2), "Normal head-blow damage, and if any damage penetrates DR, a crushing attack deafens the victim (for recovery, see Duration of Crippling Injuries, B422), while any other attack causes severe scarring (the victim loses one appearance level, or two levels if a burning or corrosion attack)." },
                {new int[] {14 }, "Normal head-blow damage, and the victim drops his weapon (if he has two weapons, roll randomly to see which one he drops)." },
                {new int[] {15 }, "The blow does maximum normal damage." },
                {new int[] {16 }, "The blow does double damage." },
                { new int[] {17 }, "The target’s DR protects at half value (round down) after applying any armor divisors." },
                {new int[] {18 }, "The blow does triple damage." },
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> CriticalMissTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {

                {new int[] {3, 4, 17, 18 }, "Your weapon breaks and is useless. *Exception*: Certain weapons are resistant to breakage. These include solid *crushing* weapons (maces, flails, mauls, metal bars, etc.); magic weapons; firearms (other than wheel-locks, guided missiles, and beam weapons); and fine and very fine weapons of all kinds. If you have a weapon like that, roll again. Only if you get a “broken weapon” result a second time does the weapon really break. If you get any other result, you drop the weapon instead. See Broken Weapons (B485)." },
                {new int[] {5 }, "You manage to hit *yourself* in the arm or leg (50% chance each way). *Exception*: If making an *impaling* or *piercing* melee attack, or any kind of ranged attack, roll again. If you get a “hit yourself” result a second time, use *that* result – half or full damage, as the case may be. If you get something other than “hit yourself,” use that result." },
                {new int[] {6 }, "You manage to hit *yourself* in the arm or leg for half damage. (50% chance each way). *Exception*: If making an *impaling* or *piercing* melee attack, or any kind of ranged attack, roll again. If you get a “hit yourself” result a second time, use *that* result – half or full damage, as the case may be. If you get something other than “hit yourself,” use that result." },
                {new int[] {7, 13 }, "You lose your balance. You can do *nothing* else (not even a free action) until your next turn, and all your active defenses are at -2 until then." },
                {new int[] {8, 12 }, "The weapon turns in your hand. You must take an extra Ready maneuver before you can use it again." },
                {Enumerable.Range(9,3), "You drop the weapon. *Exception*: A *cheap* weapon breaks; Certain weapons are resistant to breakage. These include solid *crushing* weapons (maces, flails, mauls, metal bars, etc.); magic weapons; firearms (other than wheel-locks, guided missiles, and beam weapons.) If you have a weapon like that, roll again. Only if you get a “broken weapon” result a second time does the weapon really break. If you get any other result, you drop the weapon instead. See Broken Weapons (B485)." },
                {new int[] {14 }, "If making a *swinging* melee attack, your weapon flies 1d yards from your hand – 50% chance straight forward or straight back. Anyone on the target spot must make a DX roll or take half damage from the falling weapon! If making a *thrusting* melee attack or any kind of ranged attack, or parrying, you drop the weapon. *Exception*: A *cheap* weapon breaks; Certain weapons are resistant to breakage. These include solid *crushing* weapons (maces, flails, mauls, metal bars, etc.); magic weapons; firearms (other than wheel-locks, guided missiles, and beam weapons.) If you have a weapon like that, roll again. Only if you get a “broken weapon” result a second time does the weapon really break. If you get any other result, you drop the weapon instead. See Broken Weapons (B485)." },
                {new int[] {15 }, "You strain your shoulder! Your weapon arm is “crippled.” You do not have to drop your weapon, but you cannot use it, either to attack or defend, for 30 minutes." },
                {new int[] {16 }, "You fall down! If making a ranged attack, instead, you lose your balance. You can do *nothing* else (not even a free action) until your next turn, and all your active defenses are at -2 until then." }
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> CriticalSpellFailureTable { get; } =
    new Dictionary<IEnumerable<int>, string>
    {

                {new int[] {3}, "Spell fails. Caster is injured by 1d." },
                {new int[] {4}, "Harmful spells affect caster; helpful ones are cast on nearby foe." },
                {new int[] {5,6}, "Harmful spell is cast on one of caster's companions; helpful ones are cast on a nearby foe." },
                {new int[] {7}, "Spell hits a random target, or GM chooses an interesting target." },
                {new int[] {8}, "Spell fails. Caster takes 1 HP of injury." },
                {new int[] {9}, "Spell fails. Caster is stunned and needs an IQ roll to recover." },
                {new int[] {10,11}, "Spell is useless, but produces a loud, smelly, or bright signature." },
                {new int[] {12}, "Spell produces extremely weak version of intended effect." },
                {new int[] {13}, "Spell produces opposite of intended effect." },
                {new int[] {14}, "Spell seems to work but it is an illusion. Players should not know." },
                {new int[] {15,16}, "Spell has opposite effect, and it affects a random target." },
                {new int[] {17}, "Spell fails entirely and caster temporarily forgets spell. Caster may roll IQ weekly to try to remember it." },
                {new int[] {18}, "Spell fails entirely and an evil or malign being is summoned and aggressive towards the caster, whatever is appropriate for the given setting. If the original spell and caster had the absolute purest of intentions, the GM *may* waive this result." }
    }.
    SelectMany
    (
        kvp => kvp.Key.Select
        (
            key => new KeyValuePair<int, string>(key, kvp.Value)
        )
    ).
    ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM256
        /// </summary>
        public static Dictionary<int, string> CriticalCelticSpellFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails. Caster is injured by 1d." },
                {new int[] {4}, "Spells affect caster; If it is beneficial, caster is stunned and needs to roll IQ-2 to recover." },
                {new int[] {5}, "Spell is cast on one of caster's companions; if helpful, the target is stunned and needs to roll IQ-2 to recover." },
                {new int[] {6}, "Spell is cast on nearby foe; if harmful, the target automatically succeeds any HT rolls and is angry at the caster." },
                {new int[] {7}, "Nearest wooden object bursts into flowers." },
                {new int[] {8}, "Spell is cast on any random target or whatever the GM thinks is interesting." },
                {new int[] {9}, "Spell fails. Caster takes 1 HP of injury." },
                {new int[] {10}, "Spell fails and caster sees visions of a wondrous world. Roll IQ to not be mentally stunned." },
                {new int[] {11}, "Caster or area of effect is showered with golden flowers." },
                {new int[] {12}, "Spell produces extremely weak version of intended effect." },
                {new int[] {13}, "Spell produces opposite of intended effect." },
                {new int[] {14}, "Spell produces opposite of intended effect on a random target." },
                {new int[] {15}, "Spell fails entirely and caster temporarily forgets spell. Caster may roll IQ weekly to try to remember it." },
                {new int[] {16}, "Spell seems to work but it is an illusion. Players should not know." },
                {new int[] {17}, "Spell fails entirely and caster becomes a wild boar for a week." },
                {new int[] {18}, "Spell fails entirely and one of the following also happens (GM Choice): The caster, friends, and nearby bystanders are transported to a mystical world; an angry powerful faerie is summoned; caster loses 1 HT and acquires a -10 point divine curse." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM257
        /// </summary>
        public static Dictionary<int, string> CriticalClericalFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails. Caster is injured for 1 HP and 2d FP (in addition to spell cost)." },
                {new int[] {4}, "Harmful spells affect caster; helpful ones are cast on nearby foe." },
                {new int[] {5,6}, "Harmful spell is cast on one of caster's companions; helpful ones are cast on a nearby foe." },
                {new int[] {7}, "Spell hits a random target, or GM chooses an interesting target." },
                {new int[] {8}, "Spell fails. Caster takes 1 HP of injury." },
                {new int[] {9}, "Spell fails. Caster is stunned and needs a Will roll to recover." },
                {new int[] {10,11}, "Spell is useless, but produces a staggering sense of judgement in everyone within 20 yards of the caster. Caster rolls Will-3 - bystanders roll Will - to avoid mental stunning" },
                {new int[] {12}, "Spell produces extremely weak version of intended effect. Judging presence is felt." },
                {new int[] {13}, "Spell produces twisted or confused version of intended effect. Caster feels that the wrong being is being channeled." },
                {new int[] {14}, "Spell seems to work but it is subtly twisted or wears off prematurely. This is the work of a hostile supernatural being playing tricks. Players should not know." },
                {new int[] {15,16}, "Higher powers respond in entirely unpredictable way producing a different effect on a random target of similar power level." },
                {new int[] {17}, "Spell fails entirely and caster temporarily forgets spell. To remember the spell, the caster must perform an act of contrition and redemption that lasts at least one week." },
                {new int[] {18}, "Spell fails entirely and a servitor of the caster's patron spirit or deity compels him to perform some great deed of faith. This is a temporary Obsession (9) until complete." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM257
        /// </summary>
        public static Dictionary<int, string> CriticalComedySpellFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails. Caster is inconvenienced by obviously embarassing effect." },
                {new int[] {4}, "Harmful spells affect caster; helpful ones are cast on nearby foe." },
                {new int[] {5}, "Spell is cast on one of caster's companions at random, but never the intended target." },
                {new int[] {6}, "Spell fails entirely. Caster forgets name for 24 hours. After that, roll Will daily to remember name." },
                {new int[] {7}, "Lights dim, weird sounds, or temperature changes eyc. Caster is drenched in water or custard." },
                {new int[] {8}, "Spell is cast on any random target or whatever the GM thinks is amusing." },
                {new int[] {9}, "Spell fails. Caster takes 1 HP of injury from dramatic and inconvenient sparks." },
                {new int[] {10}, "Spell fails and caster is cursing loudly for 1d seconds. Stunned, roll Will to recover." },
                {new int[] {11}, "A weird noise, followed by 1d+1 billiard balls, or other weird things." },
                {new int[] {12}, "Spell produces extremely weak version of intended effect. 1d tiny cute animals swarm the caster in an annoyingly affectionate manner." },
                {new int[] {13}, "Spell produces a dramatic explosion that singes clothes and knocks caster's headgear 3d yards away. No injury." },
                {new int[] {14}, "Spell seems to work, but all resistance rolls are at +3, all objective effects are halved, and the caster has Nightmares for 1d+1 nights. The nightmares should be surreal." },
                {new int[] {15}, "Spell fails entirely and the caster's hair grows 2d yards and fingernails grow 1d inches." },
                {new int[] {16}, "Spell fails entirely and caster temporarily forgets spell. Caster may roll Will weekly to try to remember it. Until remembered, caster swears the spell does not exist at all if someone tries to remind them." },
                {new int[] {17}, "Bizzaro version of caster from another world replaces the real caster for 3d hours. This version of the caster is completely the same except for one major difference. The caster is oblivious to anyone that tries to explain to them what happened." },
                {new int[] {18}, "Spell fails entirely and a troublesome supernatural trickster appears." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM258
        /// </summary>
        public static Dictionary<int, string> CriticalDiabolicSpellFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell seems to work but it is an illusion. Players should not know." },
                {new int[] {4}, "Spell fails entirely. Elsewhere, magical backblast harms person, place or thing of value to the caster." },
                {new int[] {5}, "Caster loses 1 level of Will permanently." },
                {new int[] {6}, "Caster permanently loses 1 level of appearance in manner appropriate to attempted spell." },
                {new int[] {7}, "Harmful spell is cast on one of loved ones, friends, allies, innocent bystanders, or the caster in that order. Beneficial spells are cast on foes" },
                {new int[] {8}, "Spell fails and caster takes 2 HP of injury, they have an automatically infected wound." },
                {new int[] {9}, "Spell fails. Caster sees horrific visions and rolls a fright check at -5." },
                {new int[] {10}, "Spell does nothing, but the immediate area stinks of brimstone." },
                {new int[] {11}, "Spell produces opposite effect." },
                {new int[] {12}, "Spell produces opposite effect on a random passerby in the most inconvenient way possible." },
                {new int[] {13}, "Spell fails entirely. Caster takes 1 HP of injury and breaks out in boils." },
                {new int[] {14}, "Spell fails, and insects start pouring from the caster's mouth until the immediate area is filled." },
                {new int[] {15}, "Spell fails entirely, and vermin spontaneously appear *inside* caster. This will probably cause at least 2d injury, and can have any other effects as appropriate." },
                {new int[] {16}, "Spell fails entirely and one hand of the caster withers." },
                {new int[] {17}, "Spell fails, and caster ages 4d years." },
                {new int[] {18}, "Spell fails entirely and a demon never before known to the caster appears. It is immediately violent and will hurt the caster if they are in the way." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM258
        /// </summary>
        public static Dictionary<int, string> CriticalIllusorySpellFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails entirely, and caster is dazzled for 2d minutes. -2 to defense rolls, and -4 to anything where vision is pertinent." },
                {new int[] {4}, "Harmful spells affect caster; helpful ones are cast on nearby foe." },
                {new int[] {5}, "Harmful spells affect caster's companion; helpful ones are cast on nearby foe." },
                {new int[] {6}, "Spell fails entirely, and caster is deaf for 2d minutes." },
                {new int[] {7},  "Spell hits a random target, or GM chooses an interesting target." },
                {new int[] {8}, "Spell fails entirely, and caster is dazzled for 2d seconds. -2 to defense rolls, and -4 to anything where vision is pertinent." },
                {new int[] {9}, "Spell fails. Caster is stunned, needing an IQ roll to recover." },
                {new int[] {10, 11}, "Spell does nothing, but produces noise in the form of unintellible images, sounds, and smells." },
                {new int[] {12}, "Spell produces obviously fake illusion of intended effect." },
                {new int[] {13}, "Spell produces opposite of intended effect." },
                {new int[] {14}, "Spell seems to work, but seems to be taking initiative in irritating unhelpful ways." },
                {new int[] {15, 16}, "Spell has reverse of intended effect on random target." },
                {new int[] {17}, "Spell fails, and caster temporarily forgets the spell for a week. After one week, and once a week until successful, they may roll IQ to remember the spell" },
                {new int[] {18}, "Spell produces a real effect instead of an illusion. The effect is possessed by a spirit to take advantage of the failure and cause trouble for everyone nearby until it wanders away bored." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


        /// <summary>
        /// THM259
        /// </summary>
        public static Dictionary<int, string> OrientalCriticalFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails. Caster is injured for 1 HP and 2d FP (in addition to spell cost)." },
                {new int[] {4}, "Spell effect is reversed and hits a random target. If a foe is injured by this, they succeed all resistance rolls automatically, gain +2 Magic Resistance for 10 minutes, and are very mad at the caster." },
                {new int[] {5,6}, "Caster's yin becomes dominant. Caster has Bully (12), Lecherousness (12), clammy skin, -2 to HT rolls to resist disease, -3 to HT rolls to resist cold, and +2 to HT rolls to resist heat. If any of these disadvantages already belong to the caster, they are instead upgraded two steps. The caster may make an HT-2 roll after 3d hours, and again every hour after that until success to remove the effects." },
                {new int[] {7}, "Spell hits a random target, or GM chooses an interesting target." },
                {new int[] {8}, "Spell fails. Caster takes 1 HP of injury." },
                {new int[] {9}, "Spell fails. Caster is stunned and needs an IQ roll to recover." },
                {new int[] {10,11}, "Spell is useless, but produces a groaning sounds, flickering lights, and the smell of rotting flesh." },
                {new int[] {12}, "Spell produces extremely weak version of intended effect." },
                {new int[] {13}, "Spell produces reverse of intended effect." },
                {new int[] {14}, "Spell seems to work but it is an illusion that convinces the caster that it worked. The caster believes this until making a daily Will-2 roll to recover." },
                {new int[] {15,16}, "Caster's yang becomes dominant. Caster has Impulsiveness (12), Lecherousness (12), quirk level desire for pleasure; dry skin, -2 to HT rolls to resist disease, -3 to HT rolls to resist heat, and +2 to HT rolls to resist cold. If any of these disadvantages already belong to the caster, they are instead upgraded two steps. The caster may make an HT-2 roll after 3d hours, and again every hour after that until success to remove the effects." },
                {new int[] {17}, "Spell fails entirely and caster is inundated with incomprehensible visions for 1d+1 days. During this time, they have Absent-Mindedness, Confused (9), and Odious Personal Habit (inane rambling)[-5]" },
                {new int[] {18}, "Spell fails entirely attracting an evil spirit that harasses, troubles, tempts, and otherwise annoys the caster and their loved ones before causing serious harm." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM259
        /// </summary>
        public static Dictionary<int, string> RealityWarpingSpellCriticalFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails. Caster is injured by 1d in a strange, bizarre way." },
                {new int[] {4}, "Friendly spells affect foes; harmful ones affect caster." },
                {new int[] {5}, "One random possession of the caster vanishes forever." },
                {new int[] {6}, "Friendly spells affect foes; harmful ones affect caster's friends or allies." },
                {new int[] {7}, "A no-mana zone with a radius of 3d yards appears at the caster's position for 1d hours. Even if the caster leaves, they are unable to cast for 1d+2 minutes." },
                {new int[] {8}, "Spell fails and the caster temporarily acquires Unnatural Features 2. Every 24 hours, roll HT-2 to end the effect." },
                {new int[] {9}, "Spell fails. Caster is stunned and needs an IQ roll to rcover." },
                {new int[] {10}, "Spell fails and nearby sights, sounds, and smells are distorted." },
                {new int[] {11}, "Spell produces reality distortion around caster giving them Unnatural Features 5 for 1d hours." },
                {new int[] {12}, "Spell has no effect, but everyone who witnessed it has a different recollection of what it actually did." },
                {new int[] {13}, "Spell produces opposite of intended effect." },
                {new int[] {14}, "Spell produces déjà vu. Things occur as normal for 2d seconds and then rewind to the moment of casting. Bystanders can tell nothing actually happened, but caster still believes the spell worked, and may roll IQ-2 to realize nothing actually happened." },
                {new int[] {15}, "Spell has opposite effect on 1d+1 random targets." },
                {new int[] {16}, "Spell fails and Fortean paranormal phenomena occurs for the next week over 2d miles. Everyone in the area resents the caster for a few months giving them a temporary reputation of -1 or -2 in the affected area." },
                {new int[] {17, 18}, "The spell works unusually well, and the universe responds by manifesting a superhuman being who's sole purpose is to try to dissuade the caster from casting reality altering magic ever again. Lasts as long as the GM thinks it's interesting to harass the character." },
                
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        /// <summary>
        /// THM260
        /// </summary>
        public static Dictionary<int, string> SpiritOrientedSpellCriticalFailureTable { get; } =
new Dictionary<IEnumerable<int>, string>
{

                {new int[] {3}, "Spell fails entirely, casters takes 1d injury, and hears laughter from petty spirits." },
                {new int[] {4}, "Spell fails entirely, and a spirit with powers related to the spell harasses the caster for 1d+2 seconds then vanishes." },
                {new int[] {5}, "Friendly spells affect foes; harmful ones affect caster." },
                {new int[] {6}, "Friendly spells affect foes; harmful ones affect caster's friends or allies." },
                {new int[] {7}, "Spell affects random target. GM may invoke the persona of a mischievous spirit to make an interesting choice." },
                {new int[] {8}, "Spell fails and caster is stunned, needing an IQ roll to recover." },
                {new int[] {9}, "Spell fails, and caster has Phantom Voices[-10] for 2d minutes." },
                {new int[] {10,11}, "Spell does nothing, but myriad babbling voices are heard, flickering lights are seen, and weird smells are emitted." },
                {new int[] {12}, "Spell produces shadow of intended effect. Spirits are being lazy" },
                {new int[] {13}, "Spell produces opposite of intended effect." },
                {new int[] {14}, "Spell seems to work, but this is an illusion by a spirit. If the caster fails a quick contest of will versus 14, they are convinced the spell was cast successfully." },
                {new int[] {15}, "Spell produces opposite effect on random target." },
                {new int[] {16}, "Spell fails entirely and caster acquires Nightmares (6) and Sleepwalker (9) for 1d+2 nights." },
                {new int[] {17}, "Spell fails, and for the next 1d weeks, the caster must roll versus HT if attempting to cast again. Success allows casting as normal; failure means the spell fails, and the caster is mute for 1d minutes." },
                {new int[] {18}, "Spell fails entirely and an extremely powerful spirit with a dangerous attitude towards the caster is made manifest. It is more likely to subject the caster to bizarre whims as punishment for previous deeds against the spirit realm than outright attack." }
}.
SelectMany
(
kvp => kvp.Key.Select
(
    key => new KeyValuePair<int, string>(key, kvp.Value)
)
).
ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static Dictionary<int, string> UnarmedCriticalMissTable { get; } =
            new Dictionary<IEnumerable<int>, string>
            {

                {new int[] {3, 18 }, "You knock yourself out! Details are up to the GM – perhaps you trip and fall on your head, or walk facefirst into an opponent’s fist or shield. Roll vs. HT every 30 minutes to recover." },
                {new int[] {4 }, "If attacking or parrying with a limb, you strain it: take 1 HP of injury and the limb is “crippled.” You cannot use it, either to attack or defend, for 30 minutes. If biting, butting, etc., you pull a muscle and suffer moderate pain (see *Irritating Conditions*, B428) for the next (20 - HT) minutes, minimum one minute." },
                {new int[] {5, 16 }, "You hit a solid object (wall, floor, etc.) instead of striking your foe or parrying his attack. You take crushing damage equal to *your* thrusting damage to the body part you were using; DR protects normally. *Exception*: If attacking a foe armed with a ready impaling weapon, you fall on his weapon! You suffer the weapon’s damage, but based on *your* ST rather than his." },
                {new int[] {6 }, "You hit a solid object (wall, floor, etc.) instead of striking your foe or parrying his attack. You take crushing damage equal to half of *your* thrusting damage to the body part you were using; DR protects normally. *Exception*: If attacking a foe armed with a ready impaling weapon, you fall on his weapon! You suffer the weapon’s damage, but based on *your* ST and cut in half rather than his. *Exception*: If attacking with natural weapons, such as claws or teeth, they *break*: -1 damage on future attacks until you heal (for recovery, see *Duration of Crippling Injuries*, B422)." },
                {new int[] {7, 14 }, "You stumble. On an attack, you advance one yard past your opponent and end your turn facing away from him; he is now behind you! On a parry, you fall down." },
                {new int[] {8}, "You fall down!" },
                {Enumerable.Range(9,3), "You lose your balance. You can do *nothing* else (not even a free action) until your next turn, and all your active defenses are at -2 until then." },
                {new int[] {12}, "You trip. Make a DX roll to avoid falling down. Roll at DX-4 if kicking, or at *twice* the usual DX penalty for a technique that requires a DX roll to avoid mishap even on a normal failure (e.g., DX-8 for a Jump Kick)." },
                {new int[] {13}, "You drop your guard. All your active defenses are at -2 for the next turn, and any Evaluate bonus or Feint penalty against you until your next turn counts *double*! This *is* obvious to nearby opponents." },
                {new int[] {15 }, "You *tear* a muscle. Take 1d-3 of injury to the limb you used (to one limb, if you used two), or to your neck if biting, butting, etc. You are off balance and at -1 to all attacks and defenses for the next turn. You are at -3 to any action involving that limb (or to *any* action, if you injure your neck!) until this damage heals. Reduce this penalty to -1 if you have High Pain Threshold." },
                {new int[] {17 }, "If attacking or parrying with a limb, you strain it: take 1 HP of injury and the limb is “crippled.” You cannot use it, either to attack or defend, for 30 minutes. If biting, butting, etc., you pull a muscle and suffer moderate pain (see *Irritating Conditions*, B428) for the next (20 - HT) minutes, minimum one minute. *Exception*: An IQ 3-5 animal fails so miserably that it loses its nerve. It will turn and flee on its next turn, if possible. If backed into a corner, it will assume a surrender position (throat bared, belly exposed, etc.)." }            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, string>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static string GetCriticalHitResult(CriticalHitType criticalHitType)
        {
            var tableIndex = Roller.Roll().Sum();
            string returnValue;
            switch (criticalHitType)
            {
                case CriticalHitType.Normal: returnValue = CriticalHitTable[tableIndex]; break;
                case CriticalHitType.Head: returnValue = CriticalHeadBlowTable[tableIndex]; break;
                default:
                    throw new NotImplementedException($"{criticalHitType} not supported");
            }
            return returnValue;
        }

        /// <summary>
        /// Roll on a critical miss table.
        /// </summary>
        /// <param name="criticalMissType"></param>
        /// <returns></returns>
        public static string GetCriticalMissResult(CriticalMissType criticalMissType)
        {
            var tableIndex = Roller.Roll().Sum();
            string returnValue;
            switch (criticalMissType)
            {
                case CriticalMissType.Normal: returnValue = CriticalMissTable[tableIndex]; break;
                case CriticalMissType.Unarmed: returnValue = UnarmedCriticalMissTable[tableIndex]; break;
                case CriticalMissType.Magic: returnValue = CriticalSpellFailureTable[tableIndex]; break;
                case CriticalMissType.Celtic: returnValue = CriticalCelticSpellFailureTable[tableIndex]; break;
                case CriticalMissType.Clerical: returnValue = CriticalClericalFailureTable[tableIndex]; break;
                case CriticalMissType.Comedy: returnValue = CriticalComedySpellFailureTable[tableIndex]; break;
                case CriticalMissType.Diabolic: returnValue = CriticalDiabolicSpellFailureTable[tableIndex]; break;
                case CriticalMissType.Illusory: returnValue = CriticalIllusorySpellFailureTable[tableIndex]; break;
                case CriticalMissType.Oriental: returnValue = OrientalCriticalFailureTable[tableIndex]; break;
                case CriticalMissType.RealityWarping: returnValue = RealityWarpingSpellCriticalFailureTable[tableIndex]; break;
                case CriticalMissType.Spirit: returnValue = SpiritOrientedSpellCriticalFailureTable[tableIndex]; break;
                default:
                    throw new NotImplementedException($"{criticalMissType} not supported");
            }
            return returnValue;
        }

        public static Dictionary<int, int> ThrustDamageTable { get; } =
            new Dictionary<IEnumerable<int>, int>
            {
                {Enumerable.Range(1,1),-2},
                {Enumerable.Range(2,1),-2},
                {Enumerable.Range(3,1),-1},
                {Enumerable.Range(4,1),-1},
                {Enumerable.Range(5,1),0},
                {Enumerable.Range(6,1),0},
                {Enumerable.Range(7,1),1},
                {Enumerable.Range(8,1),1},
                {Enumerable.Range(9,1),2},
                {Enumerable.Range(10,1),2},
                {Enumerable.Range(11,1),3},
                {Enumerable.Range(12,1),3},
                {Enumerable.Range(13,1),4},
                {Enumerable.Range(14,1),4},
                {Enumerable.Range(15,1),5},
                {Enumerable.Range(16,1),5},
                {Enumerable.Range(17,1),6},
                {Enumerable.Range(18,1),6},
                {Enumerable.Range(19,1),7},
                {Enumerable.Range(20,1),7},
                {Enumerable.Range(21,1),8},
                {Enumerable.Range(22,1),8},
                {Enumerable.Range(23,1),9},
                {Enumerable.Range(24,1),9},
                {Enumerable.Range(25,1),10},
                {Enumerable.Range(26,1),10},
                {Enumerable.Range(27,1),11},
                {Enumerable.Range(28,1),11},
                {Enumerable.Range(29,1),12},
                {Enumerable.Range(30,1),12},
                {Enumerable.Range(31,1),13},
                {Enumerable.Range(32,1),13},
                {Enumerable.Range(33,1),14},
                {Enumerable.Range(34,1),14},
                {Enumerable.Range(35,1),15},
                {Enumerable.Range(36,1),15},
                {Enumerable.Range(37,1),16},
                {Enumerable.Range(38,1),16},
                {Enumerable.Range(39,1),17},
                {Enumerable.Range(40,5),17},
                {Enumerable.Range(45,5),20},
                {Enumerable.Range(50,5),22},
                {Enumerable.Range(55,5),24},
                {Enumerable.Range(60,5),27},
                {Enumerable.Range(65,5),29},
                {Enumerable.Range(70,5),32},
                {Enumerable.Range(75,5),34},
                {Enumerable.Range(80,5),36},
                {Enumerable.Range(85,5),38},
                {Enumerable.Range(90,5),40},
                {Enumerable.Range(95,5),42},
                {Enumerable.Range(100,1),44}
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, int>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


        public static Dictionary<int, int> SwingDamageTable { get; } =
            new Dictionary<IEnumerable<int>, int>
            {
               {Enumerable.Range(1,1),-1},
                {Enumerable.Range(2,1),-1},
                {Enumerable.Range(3,1),0},
                {Enumerable.Range(4,1),0},
                {Enumerable.Range(5,1),1},
                {Enumerable.Range(6,1),1},
                {Enumerable.Range(7,1),2},
                {Enumerable.Range(8,1),2},
                {Enumerable.Range(9,1),3},
                {Enumerable.Range(10,1),4},
                {Enumerable.Range(11,1),5},
                {Enumerable.Range(12,1),6},
                {Enumerable.Range(13,1),7},
                {Enumerable.Range(14,1),8},
                {Enumerable.Range(15,1),9},
                {Enumerable.Range(16,1),10},
                {Enumerable.Range(17,1),11},
                {Enumerable.Range(18,1),12},
                {Enumerable.Range(19,1),13},
                {Enumerable.Range(20,1),14},
                {Enumerable.Range(21,1),15},
                {Enumerable.Range(22,1),16},
                {Enumerable.Range(23,1),17},
                {Enumerable.Range(24,1),18},
                {Enumerable.Range(25,1),19},
                {Enumerable.Range(26,1),20},
                {Enumerable.Range(27,1),21},
                {Enumerable.Range(28,1),21},
                {Enumerable.Range(29,1),22},
                {Enumerable.Range(30,1),22},
                {Enumerable.Range(31,1),23},
                {Enumerable.Range(32,1),23},
                {Enumerable.Range(33,1),24},
                {Enumerable.Range(34,1),24},
                {Enumerable.Range(35,1),25},
                {Enumerable.Range(36,1),25},
                {Enumerable.Range(37,1),26},
                {Enumerable.Range(38,1),26},
                {Enumerable.Range(39,1),27},
                {Enumerable.Range(40,5),27},
                {Enumerable.Range(45,5),29},
                {Enumerable.Range(50,5),31},
                {Enumerable.Range(55,5),33},
                {Enumerable.Range(60,5),36},
                {Enumerable.Range(65,5),38},
                {Enumerable.Range(70,5),40},
                {Enumerable.Range(75,5),42},
                {Enumerable.Range(80,5),44},
                {Enumerable.Range(85,5),46},
                {Enumerable.Range(90,5),48},
                {Enumerable.Range(95,5),50},
                {Enumerable.Range(100,1),52}
            }.
            SelectMany
            (
                kvp => kvp.Key.Select
                (
                    key => new KeyValuePair<int, int>(key, kvp.Value)
                )
            ).
            ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static string GetFrightCheckResult(int marginOfFailure, int frightCheckRoll, FrightCheckType frightCheckType)
        {
            var tableIndex = Math.Min(Math.Max(marginOfFailure, 1) + frightCheckRoll, 40);
            string returnValue;
            switch(frightCheckType)
            {
                case FrightCheckType.Fright: returnValue = FrightCheckTable[tableIndex]; break;
                case FrightCheckType.Awe: returnValue = AweCheckTable[tableIndex]; break;
                case FrightCheckType.Confusion: returnValue = ConfusionCheckTable[tableIndex]; break;
                case FrightCheckType.Despair: returnValue = DespairCheckTable[tableIndex]; break;
                default:
                    throw new NotImplementedException($"{frightCheckType} not supported");
            }
            

            return returnValue;
        }


        public static string GetStrikingStrengthCalculations(int effectiveStrength, StrengthMode knowYourOwnStrength)
        {
            int thrustDamage, swingDamage;
            string lowKnowYourOwnStrengthThrust = string.Empty, lowKnowYourOwnStrengthSwing = string.Empty;
            switch(knowYourOwnStrength)
            {
                
                case StrengthMode.KnowYourOwnStrength:
                    KnowYourOwnStrengthStrikingStrength(effectiveStrength, out thrustDamage, out swingDamage);
                    if (effectiveStrength < 7)
                    {
                        int lowThrust, lowSwing;
                        KnowYourOwnStrengthStrikingStrength(effectiveStrength + 10, out lowThrust, out lowSwing);
                        lowKnowYourOwnStrengthThrust = " or " + RawDamageToDiceAddsFormat(lowThrust).ToString() + "/10";
                        lowKnowYourOwnStrengthSwing = " or " + RawDamageToDiceAddsFormat(lowSwing).ToString() + "/10";
                    }
                    break;
                case StrengthMode.ReducedSwing: ReducedSwingStrikingStrength(effectiveStrength, out thrustDamage, out swingDamage); break;
                case StrengthMode.Basic:
                default:
                    VanillaStrikingStrength(effectiveStrength, out thrustDamage, out swingDamage); break;

            }
            return $"```{Environment.NewLine}Thrust Damage: {RawDamageToDiceAddsFormat(thrustDamage).ToString() + lowKnowYourOwnStrengthThrust}{Environment.NewLine} Swing Damage: {RawDamageToDiceAddsFormat(swingDamage).ToString() + lowKnowYourOwnStrengthSwing}{Environment.NewLine}```";
        }

        public static Dictionary<int, decimal> KnowYourOwnStrengthBasicLiftTable { get; } =
        new Dictionary<int, decimal>
        {
            {1, 2.5m },
            {2, 3.2m },
            {3, 4.0m },
            {4, 5.0m },
            {5, 6.3m },
            {6, 8.0m },
            {7, 10m },
            {8, 13 },
            {9, 16 },
            {10, 20 },
            {11, 25 },
            {12, 32 },
            {13, 40 },
            {14, 50 },
            {15, 63 },
            {16, 80 },
            {17, 100 },
            {18, 126 },
            {19, 159 },
            {20, 200 },
        };


        public static int GetLiftingStrength(decimal basicLift, bool knowYourOwnStrength)
        {
            int liftingStrength;
            liftingStrength = !knowYourOwnStrength ? VanillaLiftingStrength(basicLift) : KnowYourOwnStrengthLiftingStrength(basicLift);
            return liftingStrength;
        }

        private static int KnowYourOwnStrengthLiftingStrength(decimal basicLift)
        {
            //This formula doesn't take into account that the table does some rounding, so answers can be off
            //probably at most, by one.
            var guessedLiftingStrength = (int)Math.Ceiling(10 * Math.Log10((double)(basicLift / 2)));
            var guessedBasicLift = KnowYourOwnStrengthBasicLift(guessedLiftingStrength);
            var foundBestResult = false;
            if(guessedBasicLift >= basicLift) //See if we can go down
            {
                while (!foundBestResult)
                {
                    var newGuessedStrength = guessedLiftingStrength - 1;
                    var newGuessedBasicLift = KnowYourOwnStrengthBasicLift(newGuessedStrength);
                    if (newGuessedBasicLift >= basicLift && newGuessedBasicLift != guessedBasicLift)
                    {
                        guessedLiftingStrength--;
                    }
                    else
                    {
                        foundBestResult = true;
                    }
                }
            }
            else //We need to go up.
            {
                while(!foundBestResult)
                {
                    var newGuessedStrength = guessedLiftingStrength + 1;
                    var newGuessedBasicLift = KnowYourOwnStrengthBasicLift(newGuessedStrength);
                    if (newGuessedBasicLift < basicLift)
                    {
                        guessedLiftingStrength++;
                    }
                    else
                    {
                        foundBestResult = true;
                    }
                }
            }
            return guessedLiftingStrength;
        }

        private static int VanillaLiftingStrength(decimal basicLift)
        {
            return (int)Math.Ceiling(Math.Sqrt((double)(basicLift * 5)));
        }

        public static Lift GetBasicLiftCalculations(int effectiveStrength, bool knowYourOwnStrength)
        {
            decimal basicLift;
            if (!knowYourOwnStrength)
                basicLift = VanillaBasicLift(effectiveStrength);
            else
                basicLift = KnowYourOwnStrengthBasicLift(effectiveStrength);

            return new Lift { BasicLift = basicLift };

        }

        private static decimal KnowYourOwnStrengthBasicLift(int effectiveStrength)
        {
            effectiveStrength = Math.Max(1, effectiveStrength);
            var exponent = 0;
            while(effectiveStrength > 20)
            {
                exponent++;
                effectiveStrength -= 10;
            }

            return KnowYourOwnStrengthBasicLiftTable[effectiveStrength] * Math.Round((decimal)Math.Pow(10, exponent));
        }

        private static decimal VanillaBasicLift(int effectiveStrength)
        {
            var returnValue = (effectiveStrength * effectiveStrength) / 5.0m;
            if (returnValue >= 10)
                returnValue = Math.Round(returnValue);
            return returnValue;
        }

        private static void ReducedSwingStrikingStrength(int effectiveStrength, out int thrustDamage, out int swingDamage)
        {
            thrustDamage = (effectiveStrength / 2) - 3 + (effectiveStrength % 2 == 1 ? 1 : 0); 
            swingDamage = (effectiveStrength / 2) - 1;
        }
        private static void KnowYourOwnStrengthStrikingStrength(int effectiveStrength, out int thrustDamage, out int swingDamage)
        {
            thrustDamage = effectiveStrength - 8;
            swingDamage = effectiveStrength - 6;
        }

        public static void VanillaStrikingStrength(int effectiveStrength, out int thrustDamage, out int swingDamage)
        {
            //Get a full dice of damage more for any value that exceeds the top of the table by 10.
            var bonusDamage = (Math.Max(0, effectiveStrength - 100) / 10) * 4;

            //Limit the value to between 1 and 100.
            var index = Math.Min(Math.Max(1, effectiveStrength), 100);
            thrustDamage = ThrustDamageTable[index] + bonusDamage;
            swingDamage = SwingDamageTable[index] + bonusDamage;
        }

        public static ParsedDiceAdds RawDamageToDiceAddsFormat(int raw)
        {
            //Dice is the integer part.
            var dice = Math.Max(1, raw / 4);
            var remainder = raw - (dice * 4);
            var bigRemainder = remainder == 3;
            int modifier = 0;
            if (bigRemainder)
            {
                dice++;
                modifier = -1;
            }
            else
            {
                modifier = remainder;
            }

            return new ParsedDiceAdds { Addend = modifier, Quantity = dice, Sides = 6 };
        }


        public static Dictionary<decimal, decimal> ThrowingDistanceTable { get; } =
            new Dictionary<decimal, decimal>
            {
                {   0.05m,  3.5m },
                {   0.1m,  2.5m },
                {   0.15m,  2m },
                {   0.2m,  1.5m },
                {   0.25m,  1.2m },
                {   0.3m,  1.1m },
                {   0.4m,  1m },
                {   0.5m,  0.8m },
                {   0.75m,  0.7m },
                {   1m,  0.6m },
                {   1.5m,  0.4m },
                {   2m,  0.3m },
                {   2.5m,  0.25m },
                {   3m,  0.2m },
                {   4m,  0.15m },
                {   5m,  0.12m },
                {   6m,  0.1m },
                {   7m,  0.09m },
                {   8m,  0.08m },
                {   9m,  0.07m },
                {   10m,  0.06m },
                {   12m,  0.05m }
            };


        public static Dictionary<decimal, decimal> DungeonFantasyThrowingDistanceTable { get; } =
            new Dictionary<decimal, decimal>
            {
                {   0.125m,  2.5m },
                {   0.25m,  1.5m },
                {   0.5m,  1m },
                {   1m,  0.5m },
                {   2m,  1m/3m },
                {   4m,  0.2m },
                {   8m,  0.1m }
            };

        public static Dictionary<decimal, decimal> ThrowingDamageTable { get; } =
            new Dictionary<decimal, decimal>
            {
                {   0.125m, -2m },
                {   0.25m,  -1m },
                {   0.5m,   0m  },
                {   1m, 1m  },
                {   2m, 0m  },
                {   4m, -0.5m    },
                {   8m, -1m }
            };

        public static Dictionary<decimal, decimal> DungeonFantasyThrowingDamageTable { get; } =
    new Dictionary<decimal, decimal>
    {
                {   0.125m, -2m },
                {   0.25m,  -1m },
                {   0.5m,   0m  },
                {   1m, 1m  },
                {   2m, 0m  },
                {   4m, 0m    },
                {   8m, -1m }
    };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liftingStrength"></param>
        /// <param name="strikingStrength"></param>
        /// <param name="weight"></param>
        /// <param name="knowYourOwnStrength"></param>
        /// <returns></returns>
        public static RangedAttack GetThrowingStatistics(int liftingStrength, int strikingStrength, double weight, bool knowYourOwnStrength)
        {
            var basicLift = knowYourOwnStrength ? KnowYourOwnStrengthBasicLift(liftingStrength) : VanillaBasicLift(liftingStrength);
            //We don't really need swing damage, but oh well.
            int thrustDamage, swingDamage;
            if (knowYourOwnStrength)
                KnowYourOwnStrengthStrikingStrength(strikingStrength, out thrustDamage, out swingDamage);
            else
                VanillaStrikingStrength(strikingStrength, out thrustDamage, out swingDamage);

            //Now that we know the basic lift, we need to get the ratio of basic lift to weight for the lookup tables.
            var weightLiftingStrengthRatio = (decimal)(weight / (double)basicLift);
            var returnValue = new RangedAttack { DamageModifier = 0, HalfRange = 0, MaxRange = 0, StrengthBasedDamage = ThrustSwingType.Neither };
            if (weightLiftingStrengthRatio <= 8)
            {
                var throwingDistanceMultiplier = ThrowingDistanceTable.Where(kvp => kvp.Key >= weightLiftingStrengthRatio).First().Value;
                var damageModifier = ThrowingDamageTable.Where(kvp => kvp.Key >= weightLiftingStrengthRatio).First().Value;
                var range = throwingDistanceMultiplier * strikingStrength;
                var damage = RawDamageToDiceAddsFormat(thrustDamage);
                var resolvedDamageModifier = (int)Math.Floor(damage.Quantity * damageModifier);
                damage.Addend += resolvedDamageModifier;

                returnValue.StrengthBasedDamage = ThrustSwingType.Thrust;
                returnValue.MaxRange = range;
                returnValue.DamageModifier = resolvedDamageModifier;
                returnValue.Damage = damage;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Weight Exceeds two handed lift");
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="liftingStrength"></param>
        /// <param name="strikingStrength"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static RangedAttack GetDungeonFantasyThrowingStatistics(int liftingStrength, int strikingStrength, double weight)
        {
            var basicLift = VanillaBasicLift(liftingStrength);
            //We don't really need swing damage, but oh well.
            VanillaStrikingStrength(strikingStrength, out int thrustDamage, out int swingDamage);

            //Now that we know the basic lift, we need to get the ratio of basic lift to weight for the lookup tables.
            var weightLiftingStrengthRatio = (decimal)(weight / (double)basicLift);
            var returnValue = new RangedAttack { DamageModifier = 0, HalfRange = 0, MaxRange = 0, StrengthBasedDamage = ThrustSwingType.Neither };
            if (weightLiftingStrengthRatio <= 8)
            {
                var throwingDistanceMultiplier = DungeonFantasyThrowingDistanceTable.Where(kvp => kvp.Key >= weightLiftingStrengthRatio).First().Value;
                var damageModifier = DungeonFantasyThrowingDamageTable.Where(kvp => kvp.Key >= weightLiftingStrengthRatio).First().Value;
                var range = throwingDistanceMultiplier * strikingStrength;
                var damage = RawDamageToDiceAddsFormat(thrustDamage);
                var resolvedDamageModifier = (int)Math.Floor(damageModifier);
                damage.Addend += resolvedDamageModifier;

                returnValue.StrengthBasedDamage = ThrustSwingType.Thrust;
                returnValue.MaxRange = range;
                returnValue.DamageModifier = resolvedDamageModifier;
                returnValue.Damage = damage;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Weight Exceeds two handed lift");
            }

            return returnValue;
        }

        public static Dictionary<int, int> PopulationToSearchModifier { get; } = new Dictionary<int, int>
        {
            {99, -3 },
            {999, -2 },
            {4999, -1 },
            {9999, 0 },
            {49999, 1 },
            {99999, 2 }
        };


        public static int GetSearchModifier(int population)
        {
            //If the value goes off the table, then the search modifier is the max of 3.
            var returnValue = 3;
            // Otherwise, it is based on the first key that is >= the population.
            if (population <= PopulationToSearchModifier.Keys.Max()) 
            {
                returnValue = PopulationToSearchModifier.Where(kvp => kvp.Key >= population).First().Value;
            }
            return returnValue;
        }

        public static Dictionary<int, decimal> ControlRatingToMilitaryBudgetFactor { get; } = new Dictionary<int, decimal>
        {
            {0,0 },
            {1, 0.005m },
            {2, 0.01m },
            {3, 0.02m },
            {4, 0.05m },
            {5, 0.1m },
            {6, 0.2m },
            {7, 0.5m }
        };

        public static decimal GetMilitaryBudgetFactor(int controlRating, bool atWar)
        {
            controlRating = Math.Min(Math.Max(0, controlRating), 6);
            controlRating = atWar ? controlRating + 1 : controlRating;
            return ControlRatingToMilitaryBudgetFactor[controlRating];
        }

        public static Dictionary<WealthLevel, decimal> WealthToMonthlyPayMultiplier { get; } = new Dictionary<WealthLevel, decimal>
        {
            {WealthLevel.DeadBroke, 0 },
            {WealthLevel.Poor, 0.2m },
            {WealthLevel.Struggling, 0.5m  },
            {WealthLevel.Average, 1 },
            {WealthLevel.Comfortable, 2 },
            {WealthLevel.Wealthy, 5 },
            {WealthLevel.VeryWealthy, 20 },
            {WealthLevel.FilthyRich, 100 },
            {WealthLevel.MultimillionaireOne, 1000 },
            {WealthLevel.MultimillionaireTwo, 10000 },
            {WealthLevel.MultimillionaireThree, 100000 },
            {WealthLevel.MultimillionaireFour, 1000000 },
        };

        public static decimal GetMonthlyPayMultiplier(WealthLevel wealth)
        {
            return WealthToMonthlyPayMultiplier[wealth];
        }

        public static Dictionary<int, int> TechLevelToMonthlyPay { get; } = new Dictionary<int, int>
        {
            {0, 625 },
            {1, 650 },
            {2, 675 },
            {3, 700 },
            {4, 800 },
            {5, 1100 },
            {6, 1600 },
            {7, 2100 },
            {8, 2600 },
            {9, 3600 },
            {10, 5600 },
            {11, 8100 },
            {12, 10600 }
        };

        public static int GetTypicalMonthlyPay(int techLevel)
        {
            return TechLevelToMonthlyPay[Math.Min(12, Math.Max(0, techLevel))];
        }

        public static HeroicBackground GenerateHeroicBackground()
        {
            var returnValue = new HeroicBackground();
            //step 1
            returnValue.Birthplace = GetHeroicBackgroundBirthplace();
            //step 2
            returnValue.Region = GetHeroicBackgroundRegion();
            //step 3
            returnValue.Parentage = GetHeroicBackgroundParentage();
            //step 4
            returnValue.Mentor = GetHeroicBackgroundWhoAreThey();
            //step 5
            GetHeroicBackgroundFamilialRelationships(returnValue.Parentage.ParentFigures.Union(new[] { returnValue.Mentor }));
            //Step 6
            returnValue.Siblings = GetHeroicBackgroundSiblings();
            //step 7
            returnValue.Omen = GetHeroicBackgroundOmen();
            //step 8
            returnValue.Darkness = GetHeroicBackgroundDarkness();
            //step 9
            returnValue.Legacy = GetHeroicBackgroundSpecialLegacy();
            //step 10
            returnValue.Inheritance = GetHeroicBackgroundMundaneInheritance();
            //step 11
            returnValue.Burden = GetHeroicBackgroundHeroBurden();
            //step 12
            returnValue.DistinguishingFeature = GetHeroicBackgroundDistinguishingFeatures();
            //step 13
            returnValue.ExperienceLevel = GetHeroicBackgroundExperience();

            return returnValue;
        }

        private static Experience GetHeroicBackgroundExperience()
        {
            Experience returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = Experience.None; break;
                case 2: returnValue = Experience.ProfessionalOrganization; break;
                case 3: returnValue = Experience.Noble; break;
                case 4: returnValue = Experience.Wanderer; break;
                case 5: returnValue = Experience.HarshLife; break;
                default: returnValue = Experience.DungeonAdventurer; break;
            }
            return returnValue;
        }

        private static Feature GetHeroicBackgroundDistinguishingFeatures()
        {
            var returnValue = Feature.None;
            var roll = Roller.Roll(1, 2).First();
            if(roll == 1)
            {
                Roller.Roll(1, 6).First();
                switch(roll)
                {
                    case 1: returnValue = Feature.Birthmark; break;
                    case 2: returnValue = Feature.Tattoo; break;
                    case 3: returnValue = Feature.Heterochromia; break;
                    case 4: returnValue = Feature.WitchesMark; break;
                    case 5: returnValue = Feature.UnusualSize; break;
                    default: returnValue = Feature.Scars; break;
                }
            }
            return returnValue;

        }

        private static BackgroundBurden GetHeroicBackgroundHeroBurden()
        {
            var returnValue = new BackgroundBurden();
            var roll = Roller.Roll(1, 6).First();
            switch(roll)
            {
                case 1: returnValue.Type = BurdenType.Child; break;
                case 2:
                    returnValue.Type = BurdenType.Betrothed;
                    returnValue.MarriageObstacle = GetHeroicBackgroundMarriageObstacle();
                    returnValue.SupportingActor = GetHeroicBackgroundLover();
                    break;
                case 3:
                    returnValue.Type = BurdenType.LostHeir;
                    returnValue.SupportingActor = GetHeroicBackgroundEvildoer();
                    break;
                case 4:
                    returnValue.Type = BurdenType.Outlaw;
                    returnValue.OutlawReason = GetHeroicBackgroundOutlawReason();
                    break;
                case 5:
                    returnValue.Type = BurdenType.Dependant;
                    returnValue.SupportingActor = GetHeroicBackgroundLovedOne();
                    switch(returnValue.SupportingActor.LovedOneInformation.Type)
                    {
                        case LovedOneType.Sibling:
                        case LovedOneType.Cousin:
                            returnValue.SupportingActor.LovedOneInformation.ActualAge = 7 + Roller.Roll(1, 6).First();
                            returnValue.SupportingActor.LovedOneInformation.RelativeAge = RelativeAge.Younger;
                            break;
                        default:
                            returnValue.SupportingActor.LovedOneInformation.RelativeAge = RelativeAge.Older;
                            break;
                    }
                    break;
                default: returnValue.Type = BurdenType.Guardian; break;
                    
            }
            return returnValue;
        }

        private static OutlawType GetHeroicBackgroundOutlawReason()
        {
            OutlawType returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = OutlawType.Accusal; break;
                case 2: returnValue = OutlawType.AccidentalDeath; break;
                case 3:
                case 4: returnValue = OutlawType.RefusedRuler; break;
                default: returnValue = OutlawType.Framed; break;
            }
            return returnValue;
        }

        private static MarriageObstacleInformation GetHeroicBackgroundMarriageObstacle()
        {
            var returnValue = new MarriageObstacleInformation();
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue.Type = MarriageObstacle.YouDontLike; break;
                case 2: returnValue.Type = MarriageObstacle.DoesNotLikeYou; break;
                case 3: returnValue.Type = MarriageObstacle.Dowry; break;
                case 4:
                    returnValue.Type = MarriageObstacle.Captive;
                    returnValue.SupportingActor = GetHeroicBackgroundEvildoer();
                    break;
                case 5: returnValue.Type = MarriageObstacle.Parents; break;
                default: returnValue.Type = MarriageObstacle.Age; break;
            }
            return returnValue;
        }

        private static BackgroundInheritance GetHeroicBackgroundMundaneInheritance()
        {
            var returnValue = new BackgroundInheritance();
            var roll = Roller.Roll(1, 6).First();
            switch(roll)
            {
                case 1:
                case 2: returnValue.Type = InheritanceType.Tools; break;
                case 3: returnValue.Type = InheritanceType.Deed; break;
                case 4:
                    returnValue.Type = InheritanceType.Pet;
                    returnValue.SupportingActor = GetHeroicBackgroundAnimal(false);
                    break;
                case 5:
                    returnValue.Type = InheritanceType.Companion;
                    returnValue.SupportingActor = GetHeroicBackgroundFaithfulCompanion();
                    break;
                default: returnValue.Type = InheritanceType.Transportation; break;

            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundFaithfulCompanion()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = GetHeroicBackgroundAnimal(); break;
                case 2: returnValue = GetHeroicBackgroundLover(); break;
                case 3: returnValue = GetHeroicBackgroundGhost(); break;
                case 4:
                    returnValue = GetHeroicBackgroundWhoAreThey();
                    returnValue.IsMentor = true;
                    break;
                case 5:
                    returnValue = GetHeroicBackgroundAdventurer();
                    returnValue.LovedOneInformation = GenerateLovedOneInformation();
                    returnValue.LovedOneInformation.RelativeAge = RelativeAge.Younger;
                    returnValue.LovedOneInformation.Twin = TwinType.NotTwins;
                    break;
                default:
                    returnValue = GetHeroicBackgroundAdventurer();
                    returnValue.IsApprentice = true;
                    break;
            }
            return returnValue;
        }

        private static BackgroundLegacy GetHeroicBackgroundSpecialLegacy()
        {
            var returnValue = new BackgroundLegacy();
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                    returnValue.Type = LegacyType.Favor;
                    returnValue.SupportingActor = GetHeroicBackgroundWhoAreThey();
                    returnValue.FateOfActorSaved = GetHeroicBackgroundGrimFate();
                    break;
                case 2: returnValue.Type = LegacyType.InheritedLand; break;
                case 3: returnValue.Type = LegacyType.Heir; break;
                case 4: returnValue.Type = LegacyType.InheritedBook; break;
                case 5: returnValue.Type = LegacyType.InheritedClue; break;
                default:
                    returnValue.Type = LegacyType.InheritedBrokenMagicItem;
                    returnValue.BrokenMagicItem = GetHeroicBackgroundBrokenMagicItem();
                    break;
            }
            return returnValue;
        }

        private static MagicItem GetHeroicBackgroundBrokenMagicItem()
        {
            MagicItem returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = MagicItem.Weapon; break;
                case 2: returnValue = MagicItem.MagicalTool; break;
                case 3: returnValue = MagicItem.Garment; break;
                case 4: returnValue = MagicItem.Jewelry; break;
                case 5: returnValue = MagicItem.Media; break;
                default: returnValue = MagicItem.UnusualItem; break;
            }
            return returnValue;
        }

        private static BackgroundDarkness GetHeroicBackgroundDarkness()
        {
            var returnValue = new BackgroundDarkness();
            var roll = Roller.Roll(1, 6).First();
            switch(roll)
            {
                case 1: returnValue.Type = DarknessType.DarknessFree; break;
                case 2:
                case 3:
                case 4:
                    returnValue.Type = DarknessType.Evildoer;
                    returnValue.SupportingActor = GetHeroicBackgroundEvildoer();
                    returnValue.DarkDeed = GetHeroicBackgroundDarkDeed();
                    break;
                case 5:
                    returnValue.Type = DarknessType.TragicDeath;
                    returnValue.SupportingActor = GetHeroicBackgroundLovedOne();
                    returnValue.Disaster = GetHeroicBackgroundDisaster();
                    break;
                default:
                    returnValue.Type = DarknessType.DarkProphecy;
                    returnValue.Prophecy = GetHeroicBackgroundDarkProphecy();
                    break;
            }
            return returnValue;
        }

        private static DarkProphecy GetHeroicBackgroundDarkProphecy()
        {
            DarkProphecy returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch(roll)
            {
                case 1: returnValue = DarkProphecy.KillLovedOnes; break;
                case 2: returnValue = DarkProphecy.KilledByLovedOnes; break;
                case 3: returnValue = DarkProphecy.Betrayer; break;
                case 4: returnValue = DarkProphecy.DieYoung; break;
                case 5: returnValue = DarkProphecy.GainWorldLoseEverything; break;
                default: returnValue = DarkProphecy.BecomeWhatYouHate; break;
            }
            return returnValue;
        }

        private static Disaster GetHeroicBackgroundDisaster()
        {
            Disaster returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = Disaster.Accident; break;
                case 2: returnValue = Disaster.Plague; break;
                case 3: returnValue = Disaster.Poverty; break;
                case 4: returnValue = Disaster.Earthquake; break;
                case 5: returnValue = Disaster.Drowned; break;
                default: returnValue = Disaster.Vanished; break;
            }
            return returnValue;
        }

        private static BackgroundDarkDeed GetHeroicBackgroundDarkDeed()
        {
            var returnValue = new BackgroundDarkDeed();
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                case 2:
                    returnValue.Type = DarkDeedType.Relative;
                    returnValue.SupportingActor = GetHeroicBackgroundLovedOne();
                    returnValue.GrimFate = GetHeroicBackgroundGrimFate();
                    break;
                case 3:
                    returnValue.Type = DarkDeedType.Ravage;
                    returnValue.SupportingActor = GetHeroicBackgroundLovedOne();
                    returnValue.GrimFate = GetHeroicBackgroundGrimFate();
                    break;
                case 4: returnValue.Type = DarkDeedType.Revenge; break;
                case 5:
                    returnValue.Type = DarkDeedType.Duped;
                    returnValue.SupportingActor = GetHeroicBackgroundLovedOne();
                    break;
                default:
                    returnValue.Type = DarkDeedType.Attacked;
                    returnValue.GrimFate = GetHeroicBackgroundGrimFate();
                    returnValue.Motive = GetHeroicBackgroundEvildoerMotive();
                    break;
            }
            return returnValue;
        }

        private static EvildoerMotive GetHeroicBackgroundEvildoerMotive()
        {
            EvildoerMotive returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = EvildoerMotive.Pride; break;
                case 2: returnValue = EvildoerMotive.AttemptedToStop; break;
                case 3: returnValue = EvildoerMotive.Lust; break;
                case 4: returnValue = EvildoerMotive.InTheWay; break;
                case 5: returnValue = EvildoerMotive.Possession; break;
                default: returnValue = EvildoerMotive.Sacrifice; break;
            }
            return returnValue;
        }

        private static BackgroundGrimFate GetHeroicBackgroundGrimFate()
        {
            BackgroundGrimFate returnValue;
            var roll = Roller.Roll(1, 6).First();
            if (roll != 3)
            {
                switch (roll)
                {
                    case 1: returnValue = BackgroundGrimFate.Enslaved; break;
                    case 2: returnValue = BackgroundGrimFate.Imprisoned; break;
                    case 4: returnValue = BackgroundGrimFate.UnspeakableTorment; break;
                    case 5: returnValue = BackgroundGrimFate.Exiled; break;
                    default: returnValue = BackgroundGrimFate.Killed; break;
                }
            }
            else
            {
                roll = Roller.Roll(1, 6).First();
                switch (roll)
                {
                    case 1: returnValue = BackgroundGrimFate.ScarredFace; break;
                    case 2: returnValue = BackgroundGrimFate.BrokenNose; break;
                    case 3: returnValue = BackgroundGrimFate.MissingEye; break;
                    case 4: returnValue = BackgroundGrimFate.MissingHand; break;
                    case 5: returnValue = BackgroundGrimFate.Limp; break;
                    default: returnValue = BackgroundGrimFate.MissingEar; break;
                }
            }
            return returnValue;
        }

        private static BackgroundOmen GetHeroicBackgroundOmen()
        {
            var returnValue = new BackgroundOmen();
            var roll = Roller.Roll(1, 6).First();
            switch(roll)
            {
                case 1: returnValue.Type = OmenType.NaturalDisaster; break;
                case 2:
                    returnValue.Type = OmenType.Animals;
                    returnValue.SupportingActor = GetHeroicBackgroundAnimal(true);
                    break;
                case 3:
                    returnValue.Type = OmenType.Monsters;
                    returnValue.SupportingActor = GetHeroicBackgroundMonster();
                    break;
                case 4:
                    returnValue.Type = OmenType.MysteriousStranger;
                    returnValue.SupportingActor = GetHeroicBackgroundWhoAreThey();
                    break;
                case 5: returnValue.Type = OmenType.GreatBattle; break;
                default: returnValue.Type = OmenType.CelestialEvent; break;
            }
            return returnValue;
        }

        private static List<BackgroundCharacter> GetHeroicBackgroundSiblings()
        {
            var returnValue = new List<BackgroundCharacter>();
            var siblingCount = Math.Max(0, Roller.Roll(1, 6).First() - 3);
            for(int i = 0; i < siblingCount; i++)
            {
                var sibling = new BackgroundCharacter(BackgroundCharacterType.LovedOne);
                sibling.LovedOneInformation = new LovedOne();
                sibling.LovedOneInformation.Type = LovedOneType.Sibling;
                var roll = Roller.Roll(1, 6).First();
                switch(roll)
                {
                    case 1:
                        sibling.LovedOneInformation.RelativeAge = RelativeAge.Older;
                        sibling.LovedOneInformation.Gender = true;
                        break;
                    case 2:
                        sibling.LovedOneInformation.RelativeAge = RelativeAge.Older;
                        sibling.LovedOneInformation.Gender = false;
                        break;
                    case 3:
                        sibling.LovedOneInformation.RelativeAge = RelativeAge.Younger;
                        sibling.LovedOneInformation.Gender = true;
                        break;
                    case 4:
                        sibling.LovedOneInformation.RelativeAge = RelativeAge.Younger;
                        sibling.LovedOneInformation.Gender = false;
                        break;
                    case 5:
                        sibling.LovedOneInformation.RelativeAge = RelativeAge.SameAge;
                        sibling.LovedOneInformation.Twin = TwinType.Identical;
                        break;
                    default:
                        sibling.LovedOneInformation.RelativeAge = RelativeAge.SameAge;
                        sibling.LovedOneInformation.Twin = TwinType.Fraternal;
                        break;

                }

                returnValue.Add(sibling);
            }
            GetHeroicBackgroundFamilialRelationships(returnValue);
            return returnValue;
        }

        private static void GetHeroicBackgroundFamilialRelationships(IEnumerable<BackgroundCharacter> importantPeople)
        {
            foreach(var person in importantPeople)
            {
                var roll = Roller.Roll(1, 6).First();
                person.IsAlive = roll > 2;
                roll = Roller.Roll(1, 6).First();
                person.Attitude = 
                    roll < 4 ? RelationshipState.Love :
                    roll < 6 ? RelationshipState.Estranged : 
                    RelationshipState.Hate;
            }
        }

        private static BackgroundParentage GetHeroicBackgroundParentage()
        {
            var returnValue = new BackgroundParentage();
            var roll = Roller.Roll(1, 6).First();
            var parentalFigureCount = roll > 1 && roll < 5 ? 2 : 1;
            if(roll == 1)//orphan
            {
                roll = Roller.Roll(1, 6).First();
                returnValue.OrphanStatus = roll <= 2 ? BackgroundOrphanState.Abandoned :
                    roll <= 4 ? BackgroundOrphanState.RaisedByAnimals :
                    roll == 5 ? BackgroundOrphanState.Adopted :
                    BackgroundOrphanState.Stolen;
                switch(returnValue.OrphanStatus)
                {
                    case BackgroundOrphanState.Abandoned:
                    case BackgroundOrphanState.Stolen:
                        returnValue.ParentFigures.Add(GetHeroicBackgroundWhoAreThey());
                        break;
                    case BackgroundOrphanState.RaisedByAnimals:
                        returnValue.ParentFigures.Add(GetHeroicBackgroundAnimal(true));
                        break;
                    case BackgroundOrphanState.Adopted:
                        returnValue.ParentFigures.Add(GetHeroicBackgroundHumbleFolk());
                        returnValue.ParentFigures.Add(GetHeroicBackgroundWhoAreThey());
                        break;
                }

            }
            else
            {
                returnValue.OrphanStatus = BackgroundOrphanState.NotOrphan;
                returnValue.ParentFigures.Add(GetHeroicBackgroundWhoAreThey());
                if(parentalFigureCount == 2)
                    returnValue.ParentFigures.Add(GetHeroicBackgroundWhoAreThey()); //Bonus parent!
            }
            return returnValue;
            
        }

        private static BackgroundCharacter GetHeroicBackgroundHumbleFolk()
        {
            var roll = Roller.Roll(1, 6).First();
            var returnValue = roll == 1 ? new BackgroundCharacter(BackgroundCharacterType.Crafter) :
                roll == 2 ? new BackgroundCharacter(BackgroundCharacterType.Entertainer) :
                roll == 3 ? new BackgroundCharacter(BackgroundCharacterType.Rural) :
                roll == 4 ? new BackgroundCharacter(BackgroundCharacterType.Merchant) :
                roll == 5 ? new BackgroundCharacter(BackgroundCharacterType.Outcast) :
                new BackgroundCharacter(BackgroundCharacterType.Scholar);
            returnValue.SpecialFeature = GetHeroicBackgroundHumbleFolkSpecialFeature();
            return returnValue;
        }

        private static HumbleFolkSpecialFeature GetHeroicBackgroundHumbleFolkSpecialFeature()
        {
            var roll = Roller.Roll(1, 3).First();
            return roll == 1 ? HumbleFolkSpecialFeature.Master :
                roll == 2 ? HumbleFolkSpecialFeature.VeryAttractive :
                HumbleFolkSpecialFeature.VeryWise;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wild">true if using wild animals only, false if using domestic animals only; null if either (default).</param>
        /// <returns></returns>
        private static BackgroundCharacter GetHeroicBackgroundAnimal(bool? wild = null)
        {
            int roll;
            var returnValue = new BackgroundCharacter(BackgroundCharacterType.Undefined);
            if(wild == null)
            {
                roll = Roller.Roll(1, 2).First();
                wild = roll == 1;
            }
            roll = Roller.Roll(1, 6).First();
            if (wild.Value)
            {
                if (roll != 6)
                    returnValue.Type = roll == 1 ? BackgroundCharacterType.Bat :
                        roll == 2 ? BackgroundCharacterType.Snake :
                        roll == 3 ? BackgroundCharacterType.Cat :
                        roll == 4 ? BackgroundCharacterType.Bird :
                        BackgroundCharacterType.Wolf;
                else returnValue.Type = GetHeroicBackgroundDireAnimal();
            }
            else
            {
                returnValue.Type = roll == 1 ? BackgroundCharacterType.Sheep :
                        roll == 2 ? BackgroundCharacterType.Goat :
                        roll == 3 ? BackgroundCharacterType.Dog :
                        roll == 4 ? BackgroundCharacterType.Chicken :
                        roll == 5 ?BackgroundCharacterType.Cow :
                        BackgroundCharacterType.Horse;
            }
            return returnValue;
        }

        private static BackgroundCharacterType GetHeroicBackgroundDireAnimal()
        {
            var returnValue = BackgroundCharacterType.Undefined;
            var roll = Roller.Roll(1, 3).First();
            var subroll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                    switch(subroll)
                    {
                        case 1: returnValue = BackgroundCharacterType.AcidSpider; break;
                        case 2: returnValue = BackgroundCharacterType.DireWolf; break;
                        case 3: returnValue = BackgroundCharacterType.ElectricJelly; break;
                        case 4: returnValue = BackgroundCharacterType.FleshEatingApe; break;
                        case 5: returnValue = BackgroundCharacterType.FoulBat; break;
                        case 6: returnValue = BackgroundCharacterType.FrostSnake; break;
                    }
                    break;
                case 2:
                    switch (subroll)
                    {
                        case 1: returnValue = BackgroundCharacterType.GladiatorApe; break;
                        case 2: returnValue = BackgroundCharacterType.IceWeasel; break;
                        case 3: returnValue = BackgroundCharacterType.IceWyrm; break;
                        case 4: returnValue = BackgroundCharacterType.Slorn; break;
                        case 5: returnValue = BackgroundCharacterType.SlugBeast; break;
                        case 6: returnValue = BackgroundCharacterType.Triger; break;
                    }
                    break;
                case 3:
                    switch (subroll)
                    {
                        case 1: returnValue = BackgroundCharacterType.GiantApe; break;
                        case 2: returnValue = BackgroundCharacterType.GiantRat; break;
                        case 3: returnValue = BackgroundCharacterType.GiantSnake; break;
                        case 4:
                        case 5: returnValue = BackgroundCharacterType.GiantSpider; break;
                        case 6: returnValue = BackgroundCharacterType.Gryphon; break;
                    }
                    break;
            }

            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundWhoAreThey()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch(roll)
            {
                case 1: returnValue = GetHeroicBackgroundHumbleFolk(); break;
                case 2: returnValue = GetHeroicBackgroundNonhuman(); break;
                case 3: returnValue = GetHeroicBackgroundSupernaturalEntity(); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Noble); break;
                default: returnValue = GetHeroicBackgroundAdventurer(); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundAdventurer()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                case 2:
                case 3: returnValue = GetHeroicBackgroundWarrior(); break;
                case 4: returnValue = GetHeroicBackgroundHolyFolk(); break;
                case 5: returnValue = GetHeroicBackgroundTraveler(); break;
                default: returnValue = GetHeroicBackgroundWizard(); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundWizard()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.ArtilleryWizard); break;
                case 3: 
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.ControllerWizard); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.ThaumatologistWizard); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundTraveler()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Bard); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.RangerScout); break;
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.BountyHunterScout); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.BurglarThief); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.MastermindThief); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.AssassinThief); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundHolyFolk()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: 
                case 2: 
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.Cleric); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Druid); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.MartialArtist); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.HolyWarrior); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundWarrior()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Barbarian); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.Knight); break;
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.HolyWarrior); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.MartialArtist); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.SharpshooterScout); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Swashbuckler); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundSupernaturalEntity()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Elemental); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.DivineServitor); break;
                case 3: returnValue = GetHeroicBackgroundConstruct(); break;
                case 4: returnValue = GetHeroicBackgroundDemon(); break;
                case 5: returnValue = GetHeroicBackgroundElderThing(); break;
                default: returnValue = GetHeroicBackgroundUndead(); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundUndead()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Draug); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.FlamingSkull); break;
                case 3: returnValue = GetHeroicBackgroundGhost(); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Vampire); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.Lich); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Skeleton); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundGhost()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = GetHeroicBackgroundWhoAreThey(); break;
                case 2: returnValue = GetHeroicBackgroundLovedOne(); break;
                case 3: returnValue = GetHeroicBackgroundEvildoer(); break;
                case 4: returnValue = GetHeroicBackgroundLover(); break;
                case 5:
                    returnValue = new BackgroundCharacter(BackgroundCharacterType.FormerMentor);
                    returnValue.IsMentor = true;
                    break;
                default: returnValue = GetHeroicBackgroundAnimal(true); break;
            }
            returnValue.IsGhost = true;
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundLover()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Noble); break;
                case 2: returnValue = GetHeroicBackgroundSupernaturalEntity(); break;
                case 3: returnValue = GetHeroicBackgroundNonhuman(); break;
                case 4: returnValue = GetHeroicBackgroundHumbleFolk(); break;
                case 5: returnValue = GetHeroicBackgroundAdventurer(); break;
                default:
                    returnValue = new BackgroundCharacter(BackgroundCharacterType.Lover);
                    returnValue.SupportingCharacterType = SupportingCharacterType.Evildoer;
                    returnValue.SupportingActor = GetHeroicBackgroundEvildoer(); break;
            }
            returnValue.IsLover = true;
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundEvildoer()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = GetHeroicBackgroundLovedOne(); break;
                case 2: returnValue = GetHeroicBackgroundAdventurer(); break;
                case 3: 
                case 4: returnValue = GetHeroicBackgroundMonster(); break;
                case 5: returnValue = GetHeroicBackgroundGoblin(); break;
                default: returnValue = GetHeroicBackgroundEvilPeople(); break;
            }
            returnValue.IsEvildoer = true;
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundEvilPeople()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Tyrant); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.Brigand); break;
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.Cultist); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Soldier); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.Pirate); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Slaver); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundGoblin()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 3).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Goblin); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.Hobgoblin); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Orc); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundMonster()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = GetHeroicBackgroundSavageHumanoid(); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.Dragon); break;
                case 3: returnValue = new BackgroundCharacter(GetHeroicBackgroundDireAnimal()); break;
                case 4:
                    returnValue = GetHeroicBackgroundAnimal(true);
                    returnValue.IsShapeShifter = true;
                    break;
                default: returnValue = GetHeroicBackgroundSupernaturalEntity(); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundSavageHumanoid()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 2).First();
            if (roll == 1)
            {
                roll = Roller.Roll(1, 6).First();
                switch (roll)
                {
                    case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Bugbear); break;
                    case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.Dinoman); break;
                    case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.Gargoyle); break;
                    case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.HordePygmy); break;
                    case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.LizardMan); break;
                    default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Minotaur); break;
                }
            }
            else
            {
                roll = Roller.Roll(1, 6).First();
                switch (roll)
                {
                    case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Ogre); break;
                    case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.RockMite); break;
                    case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.SiegeBeast); break;
                    case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Throttler); break;
                    case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.Troll); break;
                    default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Wildman); break;
                }
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundLovedOne()
        {
            var returnValue = new BackgroundCharacter(BackgroundCharacterType.LovedOne);
            returnValue.LovedOneInformation = GenerateLovedOneInformation();
            return returnValue;
        }

        private static LovedOne GenerateLovedOneInformation()
        {
            var returnValue = new LovedOne();
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue.Type = LovedOneType.Sibling; break;
                case 2: returnValue.Type = LovedOneType.Lover; break;
                case 3: returnValue.Type = LovedOneType.Cousin; break;
                case 4: returnValue.Type = LovedOneType.Grandparent; break;
                case 5: returnValue.Type = LovedOneType.Parent; break;
                default: returnValue.Type = LovedOneType.Aunt; break;
            }
            returnValue.Gender = Roller.Roll(1, 2).First() == 1;
            roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                case 2:
                case 3: returnValue.RelativeAge = RelativeAge.Younger; break;
                case 4:
                case 5: returnValue.RelativeAge = RelativeAge.Older; break;
                default: returnValue.RelativeAge = RelativeAge.SameAge; break;
            }
            roll = Roller.Roll(1, 6).First();
            returnValue.StepRelationship = roll == 6;
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundElderThing()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.Cultist); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.DemonFromBeyondTheStars); break;
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.EyeOfDeath); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Mindwarper); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.SphereOfMadness); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.WatcherAtTheEdgeOfTime); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundDemon()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.AsSharak); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.DemonOfOld); break;
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.DoomChild); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Hellhound); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.Peshkali); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Toxifier); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundConstruct()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.BronzeSpider); break;
                case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.CorpseGolem); break;
                case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.GolemArmorSwordsman); break;
                case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.ObsidianJaguar); break;
                case 5: returnValue = new BackgroundCharacter(BackgroundCharacterType.StoneGolem); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.SwordSpirit); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundNonhuman()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            switch (roll)
            {
                case 1:
                case 2:
                case 3: returnValue = GetHeroicBackgroundCivilizedNonhuman(); break;
                case 4: returnValue = GetHeroicBackgroundGoblin(); break;
                case 5: returnValue = GetHeroicBackgroundSavageHumanoid(); break;
                default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Faerie); break;
            }
            return returnValue;
        }

        private static BackgroundCharacter GetHeroicBackgroundCivilizedNonhuman()
        {
            BackgroundCharacter returnValue;
            var roll = Roller.Roll(1, 6).First();
            if (roll != 6)
            {
                switch (roll)
                {
                    case 1: returnValue = new BackgroundCharacter(BackgroundCharacterType.CatFolk); break;
                    case 2: returnValue = new BackgroundCharacter(BackgroundCharacterType.Dwarf); break;
                    case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.Elf); break;
                    case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.Halfling); break;
                    default: returnValue = new BackgroundCharacter(BackgroundCharacterType.Gnome); break;
                }
            }
            else
            {
                roll = Roller.Roll(1, 6).First();
                switch (roll)
                {
                    case 1: 
                    case 2: 
                    case 3: returnValue = new BackgroundCharacter(BackgroundCharacterType.HalfElf); break;
                    case 4: returnValue = new BackgroundCharacter(BackgroundCharacterType.HalfOgre); break;
                    default: returnValue = new BackgroundCharacter(BackgroundCharacterType.HalfOrc); break;
                }
            }
            return returnValue;
        }

        private static BackgroundRegion GetHeroicBackgroundRegion()
        {
            BackgroundRegion returnValue;
            var roll = Roller.Roll(1, 6).First();
            if(roll < 6)
            {
                returnValue = roll == 1 ? BackgroundRegion.Desert :
                    roll == 2 ? BackgroundRegion.Pastoral :
                    roll == 3 ? BackgroundRegion.Highland :
                    roll == 4 ? BackgroundRegion.Island :
                    BackgroundRegion.Forest;
            }
            else
            {
                roll = Roller.Roll(1, 6).First();
                returnValue = roll == 1 ? BackgroundRegion.Hell :
                    roll == 2 ? BackgroundRegion.SeaDepths :
                    roll == 3 ? BackgroundRegion.ElementalRealm :
                    roll == 4 ? BackgroundRegion.FaerieRealm :
                    roll == 5 ? BackgroundRegion.Moon :
                    BackgroundRegion.SpiritWorld;
            }
            return returnValue;
        }

        private static BackgroundBirthplace GetHeroicBackgroundBirthplace()
        {
            var roll = Roller.Roll(1, 6).First();
            return roll == 1 ? BackgroundBirthplace.Castle :
                roll == 2 ? BackgroundBirthplace.SmallVillage :
                roll == 3 ? BackgroundBirthplace.Town :
                roll == 4 ? BackgroundBirthplace.SacredPlace :
                roll == 5 ? BackgroundBirthplace.LonelyPlace :
                BackgroundBirthplace.Cave;
        }
    }
}
