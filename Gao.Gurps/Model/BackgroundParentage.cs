using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    [XmlType(AnonymousType = true, TypeName = "Parentage", Namespace = "http://gao.gurps.discord/")]
    public class BackgroundParentage
    {
        [XmlAttribute]
        public BackgroundOrphanState OrphanStatus { get; set; }
        [XmlElement(ElementName = "ParentFigure")]
        public List<BackgroundCharacter> ParentFigures { get; set; } = new List<BackgroundCharacter>();
    }
}