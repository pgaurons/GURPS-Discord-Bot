using Gao.Gurps.Mechanic;
using Gao.Gurps.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sandbox
{
    public class Program
    {
        [XmlType(AnonymousType = true, TypeName = "GuildPrefix", Namespace = "http://gao.gurps/")]
        public class GuildPrefix
        {
            [XmlAttribute]
            public long GuildId { get; set; }
            [XmlAttribute]
            public string Prefix { get; set; }

            public override string ToString()
            {
                return $"{GuildId} : {Prefix}";
            }
        }



static bool Unfair { get; set; } = true;
        static void Main(string[] args)
        {

            LookForQuestion("what").Wait();
            return;
            //var characters = Enumerable.Range(1, 20000).Select(i => ConvertToText(RollAttributes(0, true))).ToArray();

            //var result = string.Join(Environment.NewLine, characters);


            //Console.WriteLine(result);



            var serializer = new XmlSerializer(typeof(List<ulong>));
            using (var fileStream = new FileStream("guildBlocks.xml", FileMode.Create))
            {
                //Console.WriteLine($"Saving Data to file {_fileName} - {guildId} : {prefix}");
                serializer.Serialize(fileStream, new List<ulong>(new ulong[] { 1, 2, 3 }));
            }

            return;
            var trials = 0;
            while (Unfair)
            {
                var characters = new ConcurrentQueue<Character>();
                trials++;
                const int iterations = 20000;
                Parallel.For(0, iterations, (i) =>
                {
                    characters.Enqueue(RollAttributes(0, true));
                }
                );

                var averages = GetAverages(characters.ToArray());
                Console.WriteLine($"Trial {trials}: (Still Unfair? {Unfair})");
                Console.WriteLine(averages);
                Console.WriteLine($@"
                {_intelligenceBaseOdds}
                {_dexterityBaseOdds}
                {_strengthBaseOdds}
                {_healthBaseOdds}
                {_perceptionBaseOdds}
                {_willBaseOdds}
                {_moveBaseOdds}
                {_speedBaseOdds}
                {_fatigueBaseOdds}
                {_hitPointBaseOdds}
                ");
                Console.WriteLine("-------------------");
                Console.WriteLine();
            }
            /*
        private static int _intelligenceBaseOdds = 12;
        private static int _dexterityBaseOdds = 12;
        private static int _strengthBaseOdds = 24;
        private static int _healthBaseOdds = 19;
        private static int _perceptionBaseOdds = 24;
        private static int _willBaseOdds = 23;
        private static int _moveBaseOdds = 23;
        private static int _speedBaseOdds = 35;
        private static int _fatigueBaseOdds = 24;
        private static int _hitPointBaseOdds = 22;
        13
13
38
21
25
25
23
38
25
24
*/
            Console.WriteLine($@"
{_intelligenceBaseOdds}
{_dexterityBaseOdds}
{_strengthBaseOdds}
{_healthBaseOdds}
{_perceptionBaseOdds}
{_willBaseOdds}
{_moveBaseOdds}
{_speedBaseOdds}
{_fatigueBaseOdds}
{_hitPointBaseOdds}
");


        }

        private static async Task LookForQuestion(string query)
        {
            var regex = new Regex(query, RegexOptions.IgnoreCase);


            using var client = new HttpClient();
            var response = await client.GetAsync("http://www.sjgames.com/gurps/faq/");
            HtmlDocument pageDocument = new HtmlDocument();
            pageDocument.LoadHtml(await response.Content.ReadAsStringAsync());
            var nodes = pageDocument.
                DocumentNode.
                SelectNodes(@"//ul/li/a").
                Where(n => regex.IsMatch(n.InnerText));
            foreach (var node in nodes)
            {
                var link = @"http://www.sjgames.com/gurps/faq/" + node.Attributes["href"].Value;
                Console.WriteLine(node.InnerText + $" ({link})");
            }
        }

        private static object GetAverages(Character[] characters)
        {
            var stillUnfair = false;
            const double over = 1.008;
            const double under = 0.992;
            var dxAverage = characters.Average(c => c.BasicDexterity) / 10.00;
            if (dxAverage >= over)
            {
                _dexterityBaseOdds--;
                stillUnfair = true;
            }
            else if(dxAverage <= under)
            {
                _dexterityBaseOdds++;
                stillUnfair = true;
            }
            var htAverage = characters.Average(c => c.BasicHealth) / 10.00;
            if (htAverage >= over)
            {
                _healthBaseOdds--;
                stillUnfair = true;
            }
            else if (htAverage <= under)
            {
                _healthBaseOdds++;
                stillUnfair = true;
            }
            var intelligenceAverage = characters.Average(c => c.BasicIntelligence) / 10.00;
            if (intelligenceAverage >= over)
            {
                _intelligenceBaseOdds--;
                stillUnfair = true;
            }
            else if (intelligenceAverage <= under)
            {
                _intelligenceBaseOdds++;
                stillUnfair = true;
            }
            var moveAverage = characters.Average(c => c.BasicMove) / 5.00;
            if (moveAverage >= over)
            {
                _moveBaseOdds--;
                stillUnfair = true;
            }
            else if (moveAverage <= under)
            {
                _moveBaseOdds++;
                stillUnfair = true;
            }
            var speedAverage = (double)characters.Average(c => c.BasicSpeed) / 5.00;
            if (speedAverage >= over)
            {
                _speedBaseOdds--;
                stillUnfair = true;
            }
            else if (speedAverage <= under)
            {
                _speedBaseOdds++;
                stillUnfair = true;
            }
            var fatigueAverage = characters.Average(c => c.MaximumFatiguePoints) / 10.00;
            if (fatigueAverage >= over)
            {
                _fatigueBaseOdds--;
                stillUnfair = true;
            }
            else if (fatigueAverage <= under)
            {
                _fatigueBaseOdds++;
                stillUnfair = true;
            }
            var hitPointsAverage = characters.Average(c => c.MaximumHitPoints) / 10.00;
            if (hitPointsAverage >= over)
            {
                _hitPointBaseOdds--;
                stillUnfair = true;
            }
            else if (hitPointsAverage <= under)
            {
                _hitPointBaseOdds++;
                stillUnfair = true;
            }
            var perceptionAverage = characters.Average(c => c.Perception) / 10.00;
            if (perceptionAverage >= over)
            {
                _perceptionBaseOdds--;
                stillUnfair = true;
            }
            else if (perceptionAverage <= under)
            {
                _perceptionBaseOdds++;
                stillUnfair = true;
            }
            var willAverage = characters.Average(c => c.Will) / 10.00;
            if (willAverage >= over)
            {
                _willBaseOdds--;
                stillUnfair = true;
            }
            else if (willAverage <= under)
            {
                _willBaseOdds++;
                stillUnfair = true;
            }
            var strengthAverage = characters.Average(c => c.BasicStrength) / 10.00;
            if (strengthAverage >= over)
            {
                _strengthBaseOdds--;
                stillUnfair = true;
            }
            else if (strengthAverage <= under)
            {
                _strengthBaseOdds++;
                stillUnfair = true;
            }
            _traitList = null;
            Unfair = stillUnfair;

            return $@"DX - {dxAverage:P3}
HT - {htAverage:P3}
IQ - {intelligenceAverage:P3}
Mv - {moveAverage:P3}
Bs - {speedAverage:P3}
ST - {strengthAverage:P3}
FP - {fatigueAverage:P3}
HP - {hitPointsAverage:P3}
Per- {perceptionAverage:P3}
Will {willAverage:P3}";
            
        }


        private static string ConvertToText(Character result)
        {
            int disadvantages =
                Math.Min(0, (result.BasicStrength - 10) * 10) +
                Math.Min(0, (result.BasicDexterity - 10) * 20) +
                Math.Min(0, (result.BasicIntelligence - 10) * 20) +
                Math.Min(0, (result.BasicHealth - 10) * 10) +
                Math.Min(0, result.WillModification * 5) +
                Math.Min(0, result.PerceptionModification * 5) +
                Math.Min(0, result.HitPointModification * 2) +
                Math.Min(0, result.FatigueModification * 3) +
                Math.Min(0, result.BasicSpeedModification * 5) +
                Math.Min(0, result.BasicMoveModification * 5);

            //return $"{result.BasicDexterity},{result.BasicHealth},{result.BasicIntelligence},{result.BasicMove},{result.BasicSpeed},{result.BasicStrength},{result.MaximumFatiguePoints},{result.MaximumHitPoints},{result.Perception},{result.UnspentPoints},{result.Will},{disadvantages}";
            return $"{result.BasicIntelligence},{result.Perception},{result.Will},{result.BasicStrength},{result.MaximumHitPoints},{result.BasicDexterity},{result.BasicHealth},{result.MaximumFatiguePoints},{result.BasicMove},{result.BasicSpeed}";
        }


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
            else BuyNormalAbsoluteTraits(returnValue, ref characterPoints);
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
                        if(returnValue.BasicSpeedModification%4 == 0)
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

        private static void BuyNormalAbsoluteTraits(Character returnValue, ref int characterPoints)
        {
            var speedLevels = Gao.Gurps.Dice.Roller.Roll(4, 3).Sum() - 4; //random number from 0 to 8
            var moveLevels = Gao.Gurps.Dice.Roller.Roll(3, 2).Sum() - 3;

            returnValue.BasicMoveModification += moveLevels;
            returnValue.BasicSpeedModification += speedLevels;

            characterPoints -= (moveLevels + speedLevels) * 5;
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
            var speedLevels = 0;// Gao.Gurps.Dice.Roller.Roll(4, 5).Sum() - 4; //random number from 0 to 16
            character.BasicSpeedModification += speedLevels;
            character.BasicMoveModification -= 0; //0
            var moveLevels = 0;// Gao.Gurps.Dice.Roller.Roll(3, 3).Sum() - 3;
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
                case "Basic Speed": returnValue = character.BasicSpeedModification >= 8 ||
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

        /*
                59
                28
                40
                34
                39
                39
                28
                112
                40
                38

                59
                28
                40
                34
                40
                39
                28
                112
                40
                38
        */
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
