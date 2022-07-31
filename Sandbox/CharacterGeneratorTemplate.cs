using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandbox
{
    /// <summary>
    /// A template that generates a character, does so randomly.
    /// </summary>
    public class CharacterGeneratorTemplate
    {
        private static Random random = new Random();
        public int Points { get; set; }
        public List<SelectionGroup> SelectionGroups { get; set; } = new List<SelectionGroup>();

        public string ProduceCharacter()
        {
            var sb = new StringBuilder();
            var remainingBudget = Points;
            var selectionGroups = new List<SelectionGroup>();
            foreach(var group in SelectionGroups)
            {
                sb.AppendLine($"Selecting from {group.Options.Count} Options in {group.Name}. Remaining Budget = {remainingBudget}.");
                var destinationGroup = new SelectionGroup { Name = group.Name, Options = new List<Option>() };
                selectionGroups.Add(destinationGroup);
                var currentLimit = group.Limit;
                var negative = currentLimit < 0;
                while ((currentLimit < 0 && negative) || (currentLimit > 0 && !negative))
                {
                    var allCurrentlySelectedOptions = selectionGroups.SelectMany(g => g.Options).ToList();
                    var allowedOptions = group.Options.
                        //Filter out duplicates if not allowed.
                        Where(o => group.AllowDuplicates || !allCurrentlySelectedOptions.Any(dgo => dgo == o)).
                        //Get all the items with no prerequisites, or the ones for which we meet the prerequisites.
                        Where(o => o.Prerequisites == null || o.Prerequisites.Count == 0 || 
                        (
                            (o.AnyOrAllPrerequisites && o.Prerequisites.Any(p => allCurrentlySelectedOptions.Contains(p))) ||
                            (!o.AnyOrAllPrerequisites && o.Prerequisites.All(p => allCurrentlySelectedOptions.Contains(p)))
                        )).
                        //Hide the ones that we are disqualified from choosing.
                        Where(o => o.Disqualifiers == null || o.Disqualifiers.Count == 0 ||
                        (
                            (!o.Disqualifiers.Any(d => allCurrentlySelectedOptions.Contains(d)))
                        )).
                        //Hide the options we can't afford.
                        Where(o => (o.PointValue <= currentLimit && !negative) || (currentLimit <= o.PointValue && negative)).
                        ToList();
                    Option selection;
                    if(allowedOptions.Count == 0)
                    {
                        selection = new Option { Name = "Leftover Points", PointValue = currentLimit };
                    }
                    else
                    {
                        selection = allowedOptions[random.Next(0, allowedOptions.Count)];
                    }
                    
                    destinationGroup.Options.Add(selection);
                    currentLimit -= selection.PointValue;
                    remainingBudget -= selection.PointValue;
                    sb.AppendLine($"\tSelected {selection} Option from {allowedOptions.Count} possibilities. Remaining Budget = {remainingBudget}.");

                }
                
            }
            return sb.ToString();
        }
    }

    public class SelectionGroup
    {
        /// <summary>
        /// If true, Limit is the amount of points to spend in this category; If False, it is the number of options to select.
        /// </summary>
        public bool PointBased { get; set; } = true;
        public int Limit { get; set; }
        public List<Option> Options { get; set; }
        /// <summary>
        /// Most groups don't allow duplicate selections, but some do.
        /// </summary>
        public bool AllowDuplicates { get; set; } = false;
        public string Name { get; internal set; }
    }

    public class Option
    {
        public Option(string name, string specialty) 
        {
            Name = name;
            Specialty = specialty;
        }

        public Option(string name) : this(name, "Any") { }
        public Option() : this(string.Empty) { }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public int PointValue { get; set; }
        /// <summary>
        /// If true (default), than any prerequisite needs to match; if false, all must match.
        /// </summary>
        public bool AnyOrAllPrerequisites { get; set; } = true;
        public List<Option> Prerequisites { get; set; } = new List<Option>();
        public List<Option> Disqualifiers { get; set; } = new List<Option>();

        public override bool Equals(object obj)
        {
            if(obj.GetType() != typeof(Option))
                return base.Equals(obj);
            var casted = (Option)obj;
            return
                Name == casted.Name &&
                (
                    Specialty == casted.Specialty || 
                    (Specialty.ToUpperInvariant() == "ANY" || casted.Specialty.ToUpperInvariant() == "ANY")
                );
        }

        public override string ToString()
        {
            var specialtyString = Specialty.ToUpperInvariant() != "ANY" ? " (" + Specialty + ")" : string.Empty;
            return $"{Name}{specialtyString}[{PointValue}]";
        }
    }

}
