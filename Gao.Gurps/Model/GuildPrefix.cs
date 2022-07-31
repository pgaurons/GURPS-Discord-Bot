using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    [XmlType(AnonymousType = true, TypeName = "GuildPrefix", Namespace = "http://gao.gurps/")]
    public class GuildPrefix
    {
        [XmlAttribute]
        public ulong GuildId { get; set; }
        [XmlAttribute]
        public string Prefix { get; set; }

        public override string ToString()
        {
            return $"{GuildId} : {Prefix}";
        }

    }
}
