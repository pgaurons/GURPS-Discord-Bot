using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gao.Gurps.Mechanic
{
    /// <summary>
    /// Rolls random attributes
    /// </summary>
    public static class AttributeRandomizer
    {
        /// <summary>
        /// Given x CP, add random attributes to a character.
        /// </summary>
        /// <param name="characterPoints">Number of character points to spend.</param>
        /// <param name="possibleNegatives">If true, starts all stats slightly lower, and gives back points for it.</param>
        /// <returns></returns>
        public static Character RollAttributes(int characterPoints, bool possibleNegatives = false)
        {

            var returnValue = new Character { SpentPoints = characterPoints };
            if (possibleNegatives) AdjustCharacterDown(returnValue, ref characterPoints);
            KeyValuePair<string, int>[] traitList;
            do
            {
                traitList = TraitList.
                    Where(kvp => kvp.Value <= characterPoints).
                    Where(kvp => !MeetOrExceedThreshold(returnValue, kvp.Key)).
                    ToArray();
                if (!traitList.Any()) break;
                var selectedTrait = traitList[Gao.Gurps.Dice.Roller.NumberGenerator.Next(traitList.Length)];
                characterPoints -= selectedTrait.Value;
                switch (selectedTrait.Key)
                {
                    case "Strength":
                        returnValue.BasicStrength++;
                        returnValue.HitPointModification--;
                        break;
                    case "Health":
                        returnValue.BasicHealth++;
                        returnValue.BasicSpeedModification--;
                        returnValue.FatigueModification--;
                        break;
                    case "Dexterity":
                        returnValue.BasicDexterity++;
                        returnValue.BasicSpeedModification--;
                        break;
                    case "Intelligence":
                        returnValue.BasicIntelligence++;
                        returnValue.PerceptionModification--;
                        returnValue.WillModification--;
                        break;
                    case "Hit Points": returnValue.HitPointModification++; break;
                    case "Fatigue Points": returnValue.FatigueModification++; break;
                    case "Basic Move": returnValue.BasicMoveModification++; break;
                    case "Basic Speed":
                        returnValue.BasicSpeedModification++;
                        if (returnValue.BasicSpeedModification % 4 == 0)
                        {
                            returnValue.BasicMoveModification--;
                            characterPoints += 5;
                        }
                        break;
                    case "Will": returnValue.WillModification++; break;
                    case "Perception": returnValue.PerceptionModification++; break;
                    default: throw new NotImplementedException(selectedTrait.Key);
                }

            } while (traitList.Length > 0);
            returnValue.UnspentPoints = characterPoints;
            if (returnValue.BasicMove < 1 || returnValue.MaximumHitPoints < 5 || returnValue.MaximumFatiguePoints < 5) //Possible case when traits are optionally lowered.
            {
                returnValue = RollAttributes(returnValue.SpentPoints, possibleNegatives);
            }

            return returnValue;
        }



        private static void AdjustCharacterDown(Character character, ref int characterPoints)
        {
            character.BasicStrength -= 3; //30
            character.BasicHealth -= 3;//30
            character.BasicIntelligence -= 3;//60
            character.BasicDexterity -= 3; //60
            character.WillModification -= 2; //10
            character.PerceptionModification -= 2; //10
            character.BasicSpeedModification -= 8; //40
            var speedLevels = 0;
            character.BasicSpeedModification += speedLevels;
            character.BasicMoveModification -= 0; //0
            var moveLevels = 0;
            character.BasicMoveModification += moveLevels;
            character.HitPointModification -= 2; //4
            character.FatigueModification -= 2; //6
            characterPoints += (250 - ((speedLevels + moveLevels) * 5));
        }

        private static bool MeetOrExceedThreshold(Character character, string trait)
        {
            bool returnValue;
            switch (trait)
            {
                case "Hit Points": //30% threshold
                    returnValue = (10 * character.MaximumHitPoints) / character.BasicStrength >= 13;
                    break;
                case "Strength":
                    returnValue = (10 * character.MaximumHitPoints) / character.BasicStrength <= 7;
                    break;
                case "Health":
                    returnValue = ((10 * character.MaximumFatiguePoints) / character.BasicHealth <= 7) ||
                        character.BasicSpeedModification <= -8;
                    break;
                case "Fatigue Points":
                    returnValue = (10 * character.MaximumFatiguePoints) / character.BasicHealth >= 13;
                    break;
                case "Basic Move": returnValue = character.BasicMoveModification >= 3; break;
                case "Basic Speed":
                    returnValue = character.BasicSpeedModification >= 8 ||
        (character.BasicSpeedModification % 4 == 3 && character.BasicMoveModification <= -3); break;
                case "Dexterity": returnValue = character.BasicSpeedModification <= -8; break;
                case "Intelligence":
                    returnValue =
                        (10 * character.Will) / character.BasicIntelligence <= 7 ||
                        (10 * character.Perception) / character.BasicIntelligence <= 7;
                    break;

                default: returnValue = false; break;
            }

            return returnValue;
        }

        private static int _intelligenceBaseOdds = 59;
        private static int _dexterityBaseOdds = 28;
        private static int _strengthBaseOdds = 40;
        private static int _healthBaseOdds = 34;
        private static int _perceptionBaseOdds = 39;
        private static int _willBaseOdds = 39;
        private static int _moveBaseOdds = 28;
        private static int _speedBaseOdds = 112;
        private static int _fatigueBaseOdds = 40;
        private static int _hitPointBaseOdds = 38;
        private static KeyValuePair<string, int>[] _traitList;
        private static KeyValuePair<string, int>[] TraitList
        {
            get
            {
                if (_traitList == null) _traitList = new Tuple<KeyValuePair<String, int>, int>[]
                 {
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Strength", 8), _strengthBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Health", 2), _healthBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Dexterity", 15), _dexterityBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Intelligence", 10), _intelligenceBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Hit Points", 2), _hitPointBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Fatigue Points", 3), _fatigueBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Basic Move", 5), _moveBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Basic Speed", 5), _speedBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>( "Will", 5), _willBaseOdds),
                    new Tuple<KeyValuePair<string, int>, int>(new KeyValuePair<string, int>("Perception", 5), _perceptionBaseOdds)
                 }.
                     SelectMany(tkvp => Enumerable.Range(1, tkvp.Item2).Select(i => tkvp.Item1)).
                     ToArray();

                return _traitList;
            }
        }
    }
}
