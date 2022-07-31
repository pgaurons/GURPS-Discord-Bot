using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IndexNormalizer
{
    [XmlType(AnonymousType = true, TypeName = "IndexReference", Namespace = "http://gao.gurps.discord/")]
    public class IndexReference
    {
        [XmlAttribute]
        public string ReferenceText { get; set; }
        [XmlElement(ElementName = "PageReference")]
        public List<string> PageReferences { get; set; }
        [XmlElement(ElementName = "SeeAlso")]
        public List<string> CrossReferences { get; set; }
        [XmlElement(ElementName = "Child")]
        public List<IndexReference> Children { get; set; }

    }
}
