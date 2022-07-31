using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    [XmlType(AnonymousType = true, TypeName = "PyramidArticle", Namespace = "http://gao.gurps.discord/")]
    public class Article
    {
        [XmlElement(ElementName = "Author")]
        public List<string> Authors { get; set; } = new List<string>();
        [XmlAttribute]
        public string Name { get; set; }
    }
}