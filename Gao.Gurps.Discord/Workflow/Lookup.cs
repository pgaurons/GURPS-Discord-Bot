using Gao.Gurps.Model;
using Gao.Gurps.Utility;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Gao.Gurps.Discord.Workflow
{
    /// <summary>
    /// Finds data using GCS
    /// </summary>
    public static class Lookup
    {
        /// <summary>
        /// The path holding all the skill files.
        /// </summary>
        private readonly static string SkillsPath = ConfigurationManager.Configuration["Gao.Gurps.SkillsDirectory"];
        private readonly static string EquipmentPath = ConfigurationManager.Configuration["Gao.Gurps.EquipmentDirectory"];
        private readonly static string AdvantagesPath = ConfigurationManager.Configuration["Gao.Gurps.AdvantagesDirectory"];
        private readonly static string SpellsPath = ConfigurationManager.Configuration["Gao.Gurps.SpellsDirectory"];
        public readonly static string IndexPath = ConfigurationManager.Configuration["Gao.Gurps.IndexDirectory"];

        public static Lazy<JToken[]> LazySkillsLibrary;

        public static Lazy<JToken[]> LazyEquipmentLibrary;
        //public static IEnumerable<JToken> LazyEquipmentLibrary => Directory.GetFiles(EquipmentPath, "*.eqp", DefaultSearchOption).
        //    Select(f => JObject.Parse(File.ReadAllText(f))).
        //    RecursiveSelectMany(x => (IEnumerable<JToken>)x["rows"] ?? (IEnumerable<JToken>)x["children"] ?? new JToken[0]);
        public static Lazy<JToken[]> LazyAdvantagesLibrary;
        public static Lazy<JToken[]> LazySpellsLibrary;
        public static Lazy<JToken[]> LazyTechniquesLibrary;
        public static Lazy<List<Issue>> LazyPyramidLibrary;
        public static Lazy<Dictionary<string, string>> LazyFrequentlyAskedQuestionLibrary;
        /// <summary>
        /// A list of GURPS books.
        /// </summary>
        public static Lazy<List<Book>> LazyBookLibrary;


        static Lookup()
        {
            InitializeLazyCollections();

        }


        private static IEnumerable<JToken> RecursiveSelectMany(this IEnumerable<JToken> self, Func<JToken, IEnumerable<JToken>> function)
        {
            var results = self.SelectMany(function).ToList();
            var parentResults = results.ToArray();
            bool loopAgain;
            do
            {
                loopAgain = false;
                var childResults = parentResults.SelectMany(function).ToArray();
                results.AddRange(childResults);
                if (childResults.Any())
                {
                    parentResults = childResults;
                    loopAgain = true;
                }
            } while (loopAgain);

            return results;
        }

        const SearchOption DefaultSearchOption = SearchOption.AllDirectories;
        /// <summary>
        /// Resets the lazy collections so that they will be reinitialized the next time they are used.
        /// </summary>
        public static void InitializeLazyCollections()
        {
            var searchOption = DefaultSearchOption;
            LazySkillsLibrary = new Lazy<JToken[]>(() => LoadSkills(searchOption));

            LazyTechniquesLibrary = new Lazy<JToken[]>(() => 
            Directory.GetFiles(SkillsPath, "*.skl", searchOption).
            Select(f => JObject.Parse(File.ReadAllText(f))).
            RecursiveSelectMany(x => x["rows"]?.ToArray() ?? x["children"]?.ToArray() ?? new JToken[0]).
            Where(r => r["type"].ToString() == "technique").
            ToArray());



            LazyEquipmentLibrary = new Lazy<JToken[]>(() =>
            Directory.GetFiles(EquipmentPath, "*.eqp", searchOption).
            Select(f => JObject.Parse(File.ReadAllText(f))).
            RecursiveSelectMany(x => x["rows"]?.ToArray() ?? x["children"]?.ToArray() ?? new JToken[0]).
            ToArray());


            LazyAdvantagesLibrary = new Lazy<JToken[]>(() => 
            Directory.GetFiles(AdvantagesPath, "*.adq", searchOption).
            Select(f => JObject.Parse(File.ReadAllText(f))).
            RecursiveSelectMany(x => x["rows"]?.ToArray() ?? x["children"]?.ToArray() ?? new JToken[0]).
            Where(j => j["type"]?.ToString() == "advantage").
            ToArray()
            );

            LazySpellsLibrary = new Lazy<JToken[]>(() => 
            Directory.GetFiles(SpellsPath, "*.spl", searchOption).
            Select(f => JObject.Parse(File.ReadAllText(f))).
            RecursiveSelectMany(x => x["rows"]?.ToArray() ?? x["children"]?.ToArray() ?? new JToken[0]).
            Where(j => j["type"]?.ToString() == "spell").
            ToArray()
            );



            LazyPyramidLibrary = new Lazy<List<Issue>>(() => Directory.GetFiles(IndexPath).
                Where(f => f.Contains("pyramid.xml")).
                SelectMany(f =>
                {
                    IssueCollection result;
                    var serializer = new XmlSerializer(typeof(IssueCollection));
                    using (var fileStream = new FileStream(f, FileMode.Open))
                    {
                        result = (IssueCollection)serializer.Deserialize(fileStream);
                    }
                    return result;
                })
                .ToList()
            );


            LazyFrequentlyAskedQuestionLibrary = new Lazy<Dictionary<string, string>>(() =>
            {
                const string faqLocation = @"http://www.sjgames.com/gurps/faq/";
                using var client = new HttpClient();
                var response = client.GetAsync(faqLocation).Result;
                var pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(response.Content.ReadAsStringAsync().Result);
                return pageDocument.
                    DocumentNode.
                    SelectNodes(@"//ul/li/a").
                    ToDictionary(n => n.InnerText, n => faqLocation + n.Attributes["href"].Value);
            });

            LazyBookLibrary = new Lazy<List<Book>>(() =>
            {
                return File.ReadAllLines(Path.Join(IndexPath, "allGurps.txt")).
                    Skip(1). //Header row.
                    Select(l => l.Split('\t')).
                    Select(la => new Book { Title = la[0], ProductLine = la[1], Edition = int.Parse(la[2]) }).
                    ToList();
            });
            Gurps.Lookup.Index.InitializeIndex();
        }

        private static JToken[] LoadSkills(SearchOption searchOption)
        {
            var files = Directory.GetFiles(SkillsPath, "*.skl", searchOption);
            var json = files.Select(f => JObject.Parse(File.ReadAllText(f))).ToArray();
            var rows = json.RecursiveSelectMany(x => x["rows"]?.ToArray() ?? x["children"]?.ToArray() ?? new JToken[0]).
                Where(r => r["type"].ToString() == "skill").
                ToArray();
            return rows;
        }

        public static IEnumerable<Book> ChooseRandomBooks(int numberOfBooks = 3, int minimumAllowedVersion = 3)
        {
            return LazyBookLibrary.Value.
                Where(b => b.Edition >= minimumAllowedVersion).
                OrderBy(b => Guid.NewGuid()).
                Take(numberOfBooks).
                ToArray();
        }

        public static LookupResult FindSpell(string query, int searchThreshold)
        {
            var fullResult = LazySpellsLibrary.Value.
            Where(e => FindInIndexWhereClauseFunction(query, e["name"].ToString())).
            GroupBy(e => new { name = e["name"].ToString(), difficulty = e["difficulty"].ToString() }).
            Select(AggregateReferencesInGrouping);

            var count = fullResult.Count();

            var result = fullResult.
            Take(searchThreshold).
            Select(SpellToString);

            return new LookupResult { OriginalCount = count, Results = result.OrderBy(s => s).ToArray() };
        }

        /// <summary>
        /// Query, item
        /// </summary>
        private static Func<string, string, bool> FindInIndexWhereClauseFunction = new Func<string, string, bool>((query, item) => Regex.IsMatch((item ?? ""), query, RegexOptions.IgnoreCase));

        public static LookupResult FindInIndex(string query, int searchThreshold)
        {

            var fullResult = Gurps.Lookup.Index.FindInIndex(query).ToArray();

            var count = fullResult.Count();

            var result = fullResult.
            Take(searchThreshold).
            Select(ir => IndexReferenceToString(ir, query));


            return new LookupResult { OriginalCount = count, Results = result.OrderBy(s => s).ToArray() };
        }

        public static LookupResult FindInPyramid(string query, int searchThreshold)
        {

            var fullResult = LazyPyramidLibrary.Value.
            Where(e => FindInIndexWhereClauseFunction(query, e.Title) || (e.Articles != null && e.Articles.Any(c => FindInIndexWhereClauseFunction(query, c.Name)))).
            ToArray();

            var count = fullResult.Count();

            var result = fullResult.
            Take(searchThreshold).
            Select(ir => PyramidArticleToString(ir, query));


            return new LookupResult { OriginalCount = count, Results = result.OrderBy(s => s).ToArray() };
        }

        private static string PyramidArticleToString(Issue issue, string query)
        {
            var returnValue = $"#{issue.Volume}/{issue.Number}: {issue.Title}";
            var matchesParent = FindInIndexWhereClauseFunction(query, issue.Title);
            foreach(var article in issue.Articles.Where(a => matchesParent || FindInIndexWhereClauseFunction(query, a.Name)).OrderBy(a => a.Name))
            {
                returnValue += Environment.NewLine + "\t" + article.Name;
                var authorText = "by ";
                foreach(var author in article.Authors)
                {
                    authorText += author;
                    authorText += ", ";
                }
                returnValue +=  " " + (authorText.Trim().Trim(','));
            }
            return returnValue;
        }

        private static string IndexReferenceToString(IndexReference reference, string query)
        {
            var returnValue = $"{reference.ReferenceText}";
            var matchesParent = FindInIndexWhereClauseFunction(query, reference.ReferenceText);
            foreach(var pageNumber in reference.PageReferences.Where(pr => matchesParent))
            {
                returnValue += ", " + pageNumber;
            }
            foreach(var child in reference.Children.Where(c => matchesParent || FindInIndexWhereClauseFunction(query, c.ReferenceText)))
            {
                returnValue += Environment.NewLine + "\t" + child.ReferenceText;
                foreach(var childReference in child.PageReferences)
                {
                    returnValue += ", " + childReference;
                }
                
            }
            if(reference.CrossReferences.Any() && matchesParent)
            {
                var seeAlsoPre = reference.Children.Any() || reference.PageReferences.Any() ? ". See Also " : ". See ";
                returnValue += seeAlsoPre;
                foreach(var seeAlso in reference.CrossReferences)
                {
                    returnValue += seeAlso + ", ";
                }
                returnValue = returnValue.Trim().Trim(',');
            }
            return returnValue;
        }

        public static string SpellToString(JToken result)
        {
            var foundName = result["name"].ToString();
            var foundClass = result["spell_class"].ToString();
            var foundCastingTime = result["casting_time"].ToString();
            var foundCost = result["casting_cost"].ToString();
            var foundDuration = result["duration"].ToString();
            var foundReference = result["reference"].ToString();

            var foundDifficulty = result["difficulty"]?.ToString().ToUpperInvariant() ?? string.Empty;
                       

            var returnValue = $"{foundName} - {foundClass} ({foundDifficulty}). Duration: {foundDuration}. Cost: {foundCost}. Time to Cast {foundCastingTime}.";
            returnValue = HandleRangedWeapons(result["weapons"]?.Where(jt => jt["type"].ToString() == "ranged_weapon").FirstOrDefault(), returnValue);
            returnValue = HandleMeleeWeapons(result["weapons"]?.Where(jt => jt["type"].ToString() == "melee_weapon").ToArray() ?? new JToken[0], returnValue);

            if (!string.IsNullOrEmpty(foundReference))
            {
                returnValue += $" ({foundReference})";
            }


            return returnValue;
        }

        internal static LookupResult FindTechnique(string lookupValue, int rowsToReturn)
        {
            var name = lookupValue.ToUpperInvariant();

            var allTechniques = LazyTechniquesLibrary.Value.
            Where(e => FindInIndexWhereClauseFunction(lookupValue, e["name"].ToString())).
            GroupBy(s => new { name = s["name"].ToString(), specialization = s["default"]?["name"]?.ToString() ?? s["default"]?["type"]?.ToString() ?? "NO DEFAULT" }).
            Select(AggregateReferencesInGrouping).ToArray();

            var count = allTechniques.Length;

            var results = allTechniques.
            Take(rowsToReturn).
            Select(TechniqueToString).
            Distinct();

            return new LookupResult { OriginalCount = count, Results = results.OrderBy(s => s).ToArray() };
        }

        internal static async Task<LookupResult> FindInFrequentlyAskedQuestions(string lookupValue, int rowsToReturn)
        {
            LookupResult result = null;

            result = await Task.Run(() =>
            {
                var regex = new Regex(lookupValue, RegexOptions.IgnoreCase);

                var values = new List<string>();
                foreach (var node in LazyFrequentlyAskedQuestionLibrary.Value.Where(kvp => regex.IsMatch(kvp.Key)).ToArray())
                {
                    var link = node.Value;
                    values.Add(node.Key + Environment.NewLine + $"``` {link} ```");
                }

                return new LookupResult { OriginalCount = values.Count, Results = values.Take(rowsToReturn).ToArray() };
            });

            return result;
            
        }

        private static string TechniqueToString(JToken technique)
        {
            var foundName = technique["name"].ToString();
            var foundDifficulty = technique["difficulty"].ToString();
            var foundReference = technique["reference"].ToString();
            var foundDefaultName = technique["default"]?["name"]?.ToString() ?? technique["default"]?["type"]?.ToString() ?? "NO DEFAULT";
            var foundModifier = technique["default"]?["modifier"]?.ToString() ?? "+0";

            var modifier = int.Parse(foundModifier).ToString("+0;-#");
            if (modifier == "+0") modifier = string.Empty;

            var difficulty = foundDifficulty == "H" ? "Hard" : "Average";

            foundName += "(" + foundDefaultName + ")";
            return $"{foundName}-{difficulty}. Default: {foundDefaultName}{modifier}. ({foundReference})";
        }

        public static LookupResult FindSkill(string skillQuery, int searchThreshold)
        {
            var name = skillQuery.ToUpperInvariant();
            var lib = LazySkillsLibrary.Value;
            var allSkills = lib.
            Where(s => SkillWhereClause(s, name)).
            GroupBy(s => new { name = s["name"].ToString(), specialization = s["specialization"]?.ToString() ?? "", difficulty = s["difficulty"].ToString() }).
            Select(AggregateReferencesInGrouping).
            OrderBy(s => s["name"].ToString()).
            ThenBy(s => s["specialization"]?.ToString() ?? "").
            ToArray();

            var count = allSkills.Length;

            var results =
            allSkills.
                Take(searchThreshold).
                Select(SkillToString).
                Distinct();

            return new LookupResult { OriginalCount = count, Results = results.OrderBy(s => s).ToArray() };

        }
        private static JToken AggregateReferencesInGrouping<T>(IGrouping<T, JToken> grouping)
        {
            var first = grouping.FirstOrDefault(g => g["reference"]?.ToString() != null);
            if (first != null)
            {

                first["reference"] = grouping.
                    Select(e => e["reference"]?.ToString() ?? string.Empty).
                    SelectMany(s => s.Split(',', ';').Select(s => s.Trim())).
                    Distinct().
                    Where(s => s != string.Empty).
                    Aggregate((accum, next) => accum + ", " + next);

            }
            else
            {
                first = grouping.First();
            }

            return first;
        }

        public static LookupResult FindEquipment(string query, int searchThreshold)
        {

            var allEquipment = new List<JToken>();

            allEquipment = LazyEquipmentLibrary.Value.
            Where(e => FindInIndexWhereClauseFunction(query, e["description"].ToString())).
            GroupBy(e => e["description"].ToString()).
            Select(AggregateReferencesInGrouping).
            ToList();
            var count = allEquipment.Count;
             
            var equipment = allEquipment.
            Take(searchThreshold).
            Select(EquipmentToString).
            Distinct();

            return new LookupResult { OriginalCount = count, Results = equipment.OrderBy(s => s).ToArray() };
        }


        public static LookupResult FindAdvantage(string query, int searchThreshold)
        {


            var allAdvantages = LazyAdvantagesLibrary.Value.
            Where(e => FindInIndexWhereClauseFunction(query, e["name"].ToString())).
            GroupBy(e => e["name"].ToString()).
            Select(AggregateReferencesInGrouping).
            ToArray();

            var count = allAdvantages.Count();
            var advantages = allAdvantages.
            Take(searchThreshold).
            Select(AdvantageToString).
            Distinct();
            return new LookupResult { OriginalCount = count, Results = advantages.OrderBy(s => s).ToArray() };
        }

        private static string AdvantageToString(JToken advantageNode)
        {
            var name = advantageNode["name"].ToString();
            var cost = advantageNode["base_points"]?.ToString();
            if (cost == null)
                cost = advantageNode["points_per_level"]?.ToString() ?? "0";
            var reference = advantageNode["reference"]?.ToString() ?? "Error: Missing Page Reference. Try fixing it and submit to GCS.".ToUpperInvariant();

            var parsedCost = int.Parse(cost);
            var category =
                parsedCost == 1 ? "Perk" :
                parsedCost == -1 ? "Quirk" :
                parsedCost < -1 ? "Disadvantage" :
                "Advantage";

            return $"{category}: {name}[{parsedCost}]. ({reference})";
        }

        public static bool SkillWhereClause(JToken skill, string searchQuery)
        {
            var name = skill["name"].ToString();
            var valid = FindInIndexWhereClauseFunction(searchQuery, name);
            if (!valid)
            {
                var specialization = skill["specialization"]?.ToString();
                valid = specialization != null && FindInIndexWhereClauseFunction(searchQuery, specialization);
            }
            return valid;
        }

        public static string EquipmentToString(JToken equipment)
        {
            var foundName = equipment["description"].ToString();
            var foundWeight = equipment["weight"]?.ToString() ?? "- lbs";
            var foundValue = equipment["value"].ToString();
            if(decimal.TryParse(foundValue, out decimal parsedValue))
            {
                foundValue = parsedValue.ToString("C");
            }

            var foundReference = equipment["reference"]?.ToString() ?? "None";


            var returnValue = $"{foundName}, {foundWeight}, {foundValue}.";

            returnValue = HandleRangedWeapons(equipment["weapons"]?.Where(jt => jt["type"].ToString() == "ranged_weapon").FirstOrDefault(), returnValue);
            returnValue = HandleMeleeWeapons(equipment["weapons"]?.Where(jt => jt["type"].ToString() == "melee_weapon").ToArray() ?? new JToken[0], returnValue);


            if (!string.IsNullOrEmpty(foundReference))
            {
                returnValue += $" ({foundReference})";
            }
            return returnValue;
        }

        private static string HandleMeleeWeapons(JToken[] meleeWeaponParameters, string returnValue)
        {
            var appendResult = string.Empty;
            foreach (var meleeWeaponParameter in meleeWeaponParameters)
            {
                var damage = meleeWeaponParameter["calc"]["damage"] ?? "-";
                var reach = meleeWeaponParameter["reach"]?.ToString() ?? "-";
                var bestDefault = meleeWeaponParameter["defaults"]?.
                    OrderByDescending(n => int.Parse(n["modifier"]?.ToString() ?? "0")).
                    FirstOrDefault();
                if (bestDefault != null)
                {
                    var skillName = bestDefault["name"]?.ToString(); //<type>DX</type><modifier>0</modifier>
                    if (skillName == null)
                    {
                        skillName = bestDefault["type"].ToString();
                    }
                    var specialization = bestDefault["specialization"]?.ToString();
                    var foundSpecialty = specialization != null ? specialization : string.Empty;
                    if (!string.IsNullOrEmpty(foundSpecialty.Trim()))
                    {
                        skillName += " (" + foundSpecialty + ")";
                    }

                    var modifier = int.Parse(bestDefault["modifier"]?.ToString() ?? "0").ToString("+0;-#");
                    if (modifier == "+0") modifier = string.Empty;

                    appendResult += $" Reach {reach}, {damage}, {skillName}{modifier}.";
                }

            }
            if (appendResult != string.Empty)
                appendResult = " Melee Stats:" + appendResult;
            return returnValue + appendResult;
        }

        private static string GetDamage(JToken damageNode)
        {
            if (damageNode == null) return null;
            var returnValue = string.Empty;
            //<damage type="pi++" base="5dx2" armor_divisor="0.5"/>
            //fragmentation="2d" fragmentation_type="cut"
            var baseElement = damageNode["base"]?.ToString() ?? "-";
            var typeElement = damageNode["type"]?.ToString();
            var armorDivisor = damageNode["armor_divisor"]?.ToString();
            var strength = damageNode["st"]?.ToString();
            var fragmentation = damageNode["fragmentation"]?.ToString();
            var sb = new StringBuilder();
            if (strength != null) sb.Append(strength);
            if (baseElement != "-") sb.Append(baseElement);
            if (armorDivisor != null) sb.Append('(').Append(armorDivisor).Append(')');
            if (fragmentation != null)
            {
                var fragmentationType = damageNode["fragmentation_type"]?.ToString() ?? "";
                sb.Append("[").Append(fragmentation).Append(' ').Append(fragmentationType).Append("]");
            }
            if (typeElement != null) sb.Append(' ').Append(typeElement);


            returnValue = sb.ToString();
            return returnValue;
        }



        private static string HandleRangedWeapons(JToken rangedWeaponParameters, string returnValue)
        {
            if (rangedWeaponParameters != null)
            {
                var damage = GetDamage(rangedWeaponParameters["damage"]) ?? "-";
                var range = rangedWeaponParameters["range"]?.ToString() ?? "-";
                var bestDefault = rangedWeaponParameters["defaults"].
                    OrderByDescending(n => int.Parse(n["modifier"]?.ToString() ?? "0")).
                    FirstOrDefault();
                if (bestDefault != null)
                {
                    var skillName = bestDefault["name"]?.ToString(); //<type>DX</type><modifier>0</modifier>
                    if (skillName == null)
                    {
                        skillName = bestDefault["type"].ToString();
                    }
                    var specialization = bestDefault["specialization"]?.ToString();
                    var foundSpecialty = specialization != null ? specialization.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(foundSpecialty.Trim()))
                    {
                        skillName += " (" + foundSpecialty + ")";
                    }

                    var modifier = int.Parse(bestDefault["modifier"]?.ToString() ?? "0").ToString("+0;-#");
                    if (modifier == "+0") modifier = string.Empty;

                    returnValue += $" Ranged Stats: {range}, {damage}, {skillName}{modifier}.";
                }

            }

            return returnValue;
        }
 


        public static string SkillToString(JToken skill)
        {
            var foundName = skill["name"].ToString();
            var foundDifficulty = skill["difficulty"].ToString().ToUpperInvariant();
            var foundReference = skill["reference"]?.ToString();

            var foundSpecialty = skill["specialization"]?.ToString() ?? "";
            if (!string.IsNullOrEmpty(foundSpecialty.Trim()))
            {
                foundName += " (" + foundSpecialty + ")";
            }
            return $"{foundName} {foundDifficulty}. ({foundReference})";
        }

        public static Dictionary<string, string> AbbreviationToBook = new Dictionary<string, string>
        {
            {"ACT1", "Action 1 - Heroes"},
            {"ACT2", "Action 2 - Exploits"},
            {"ACT3", "Action 3 - Furious Fists"},
            {"ACT4", "Action 4 - Specialists"},
            {"ACT5", "Action 5 - Dictionary of Danger"},
            {"ADP", "Adaptations"},
            {"ATE1", "After the End 1 - Wastelanders"},
            {"ATE2", "After the End 2 - The New Worlds"},
            {"B", "Basic Set" },
            {"BC", "Boardroom and Curia"},
            {"BS", "Banestorm"},
            {"BSA", "Banestorm - Abydos"},
            {"BT", "Bio-Tech"},
            {"CS", "City Stats"},
            {"DF1", "Dungeon Fantasy 1 - Adventurers"},
            {"DF10", "Dungeon Fantasy 10 - Taverns"},
            {"DF11", "Dungeon Fantasy 11 - Power-Ups"},
            {"DF12", "Dungeon Fantasy 12 - Ninja"},
            {"DF13", "Dungeon Fantasy 13 - Loadouts"},
            {"DF14", "Dungeon Fantasy 14 - Psi"},
            {"DF15", "Dungeon Fantasy 15 - Henchmen"},
            {"DF16", "Dungeon Fantasy 16 - Wilderness Adventures"},
            {"DF17", "Dungeon Fantasy 17 - Guilds"},
            {"DF18", "Dungeon Fantasy 18 - Power Items"},
            {"DF19", "Dungeon Fantasy 19 - Incantation Magic"},
            {"DF2", "Dungeon Fantasy 2 - Dungeons"},
            {"DF20", "Dungeon Fantasy 20 - Slayers"},
            {"DF3", "Dungeon Fantasy 3 - The Next Level"},
            {"DF4", "Dungeon Fantasy 4 - Sages"},
            {"DF5", "Dungeon Fantasy 5 - Allies"},
            {"DF6", "Dungeon Fantasy 6 - 40 Artifacts"},
            {"DF7", "Dungeon Fantasy 7 - Clerics"},
            {"DF8", "Dungeon Fantasy 8 - Treasure Tables"},
            {"DF9", "Dungeon Fantasy 9 - Summoners"},
            {"DFA", "Dungeon Fantasy RPG - Adventurers"},
            {"DFE", "Dungeon Fantasy RPG - Exploits"},
            {"DFM", "Dungeon Fantasy RPG - Monsters"},
            {"DFM1", "Dungeon Fantasy Monsters 1"},
            {"DFM2", "Dungeon Fantasy Monsters 2 - Icky Goo"},
            {"DFM3", "Dungeon Fantasy Monsters 3 - Born of Myth & Magic"},
            {"DFS", "Dungeon Fantasy RPG - Spells"},
            {"DFSC", "Dungeon Fantasy Setting - Caverntown"},
            {"DFT1", "Dungeon Fantasy Treasures 1 - Glittering Prizes"},
            {"DFT2", "Dungeon Fantasy Treasures 2 - Epic Treasures"},
            {"DW", "Discworld Roleplaying Game"},
            {"F", "Fantasy"},
            {"FT1", "Fantasy-Tech 1 - The Edge of Reality"},
            {"FT2", "Fantasy-Tech 2 - Weapons of Fantasy"},
            {"GF", "Gun-Fu"},
            {"H", "Horror"},
            {"HBGG", "How to be a GURPS GM"},
            {"HT", "High-Tech"},
            {"ISW", "Traveller - Intersteller Wars"},
            {"IW", "Infinite Worlds"},
            {"LT", "Low-Tech"},
            {"LTC1", "Low-Tech Companion 1 - Philosophers and Kings"},
            {"LTC2", "Low-Tech Companion 2 - Weapons and Warriors"},
            {"LTC3", "Low-Tech Companion 3 - Daily Life and Economics"},
            {"M", "Magic"},
            {"MA", "Martial Arts"},
            {"MC", "Mass Combat"},
            {"MH1", "Monster Hunters 1 - Champions"},
            {"MH2", "Monster Hunters 2 - The Mission"},
            {"MH3", "Monster Hunters 3 - The Enemy"},
            {"MH4", "Monster Hunters 4 - Sidekicks"},
            {"MH5", "Monster Hunters 5 - Applied Xenology"},
            {"MH6", "Monster Hunters 6 - Holy Hunters"},
            {"MHPU1", "Monster Hunters Power-Ups 1"},
            {"MrsA", "Mars Attacks"},
            {"MYST", "Mystery"},
            {"P", "Powers"},   
            {"PDF", "Powers - Divine Favor"},
            {"PES", "Powers - Enhanced Senses"},
            {"PSI", "Psionic Powers"},
            {"PTW", "Powers - The Weird"},
            {"PU1", "Power-Ups 1 - Imbuements"},
            {"PU2", "Power-Ups 2 - Perks"},
            {"PU3", "Power-Ups 3 - Talents"},
            {"PU4", "Power-Ups 4 - Enhancements"},
            {"PU5", "Power-Ups 5 - Impulse Buys"},
            {"PU6", "Power-Ups 6 - Quirks"},
            {"PU7", "Power-Ups 7 - Wildcard Skills"},
            {"PU8", "Power-Ups 8 - Limitations"},
            {"RPM", "Thaumatology - Ritual Path Magic"},
            {"S", "Space"},
            {"SE", "Social Engineering"},
            {"SEBS", "Social Engineering - Back to School"},
            {"SEPR", "Social Engineering - Pulling Rank"},
            {"SOR", "Thaumatology - Sorcery"},
            {"SS", "Spaceships"},
            {"SU", "Supers"},
            {"TG", "Martial Arts - Technical Grappling"},
            {"THM", "Thaumatology"},
            {"TMS", "Thaumatology - Magical Styles"},
            {"TS", "Tactical Shooting"},
            {"TT1", "Template Toolkit 1 - Characters"},
            {"TT2", "Template Toolkit 2 - Races"},
            {"UT", "Ultra-Tech"}
        };

        internal static LookupResult FindBook(string lookupValue, int rowsToReturn)
        {
            var results = AbbreviationToBook.Where(e => FindInIndexWhereClauseFunction(lookupValue, e.Key)).Select(kvp => $"{kvp.Key} - {kvp.Value}").ToArray();
            return new LookupResult { OriginalCount = results.Length, Results = results };
        }
    }
}
