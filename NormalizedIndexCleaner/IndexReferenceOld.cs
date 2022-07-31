using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NormalizedIndexCleaner
{
    [System.Xml.Serialization.XmlTypeAttribute(TypeName ="IndexReference")]
    public class IndexReferenceOld
    {
        public string ReferenceText { get; set; }
        public List<string> PageNumbers { get; set; } = new List<string>();
        public List<string> SeeAlsos { get; set; } = new List<string>();

        public List<IndexReferenceOld> Children { get; set; }

    }
}
