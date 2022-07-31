using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    [XmlType(AnonymousType = true, TypeName = "PyramidIssue", Namespace = "http://gao.gurps.discord/")]
    public class Issue
    {
        public Issue()
        {
        }

        [XmlElement(ElementName = "Article")]
        public List<Article> Articles { get; set; } = new List<Article>();
        [XmlAttribute]
        public DateTime ReleaseDate { get; set; }
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public string Url { get; set; }

        [XmlAttribute]
        public int Volume { get; set; } = 3;
        [XmlAttribute]
        public int Number { get; set; }
    }
}