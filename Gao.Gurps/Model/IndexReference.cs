using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Represents a reference to an index entry in the back of a book.
    /// </summary>
    [XmlType(AnonymousType = true, TypeName ="IndexReference", Namespace = "http://gao.gurps.discord/")]
    public class IndexReference
    {
        /// <summary>
        /// The textual value from the index.
        /// </summary>
        [XmlAttribute]
        public string ReferenceText { get; set; }
        /// <summary>
        /// The pages where this can be found
        /// </summary>
        [XmlElement(ElementName ="PageReference")]
        public List<string> PageReferences { get; set; }
        /// <summary>
        /// A cross reference to another interesting topic.
        /// </summary>
        [XmlElement(ElementName = "SeeAlso")]
        public List<string> CrossReferences { get; set; }
        /// <summary>
        /// More detailed children elements of a parent index reference.
        /// </summary>
        [XmlElement(ElementName = "Child")]
        public List<IndexReference> Children { get; set; }


        public IndexReference Merge(IndexReference mergee)
        {
            
            return new IndexReference
            {
                ReferenceText = ReferenceText,
                PageReferences = 
                    (PageReferences ?? new List<string>()).
                    Union(mergee.PageReferences ?? new List<string>()).
                    OrderBy(s => SeparatePageNumbersAndPrefixes.Match(s).Groups[1].Value). //In DF12:33, this sorts by DF
                    ThenBy(s => !string.IsNullOrEmpty(SeparatePageNumbersAndPrefixes.Match(s).Groups[2].Value) ? int.Parse(SeparatePageNumbersAndPrefixes.Match(s).Groups[2].Value) : 0). //Then by 12, numerically,
                    ThenBy(s => int.Parse(SeparatePageNumbersAndPrefixes.Match(s).Groups[3].Value)). //Then 33 numerically.
                    ToList(),
                CrossReferences = 
                    (CrossReferences ?? new List<string>()).
                    Union(mergee.CrossReferences ?? new List<string>()).
                    OrderBy(s => s).
                    ToList(),
                Children = 
                    (Children ?? new List<IndexReference>()).
                    Union(mergee.Children ?? new List<IndexReference>()).
                    GroupBy(c => c.ReferenceText).
                    Select(g => g.Aggregate((a, b) => a.Merge(b))).
                    OrderBy(c => c.ReferenceText).
                    ToList()
            };
        }

        [XmlIgnore]
        private static readonly Regex SeparatePageNumbersAndPrefixes = new Regex(@"\b([A-Z]+)(?:(\d+)(?:\:)|)(\d+)\b", RegexOptions.IgnoreCase);



    }
}
