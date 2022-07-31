using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Gao.Gurps.Discord.Model
{
    [XmlType(AnonymousType = true, TypeName = "GuildAnnouncement", Namespace = "http://gao.gurps/")]
    public class GuildAnnouncement
    {
        [XmlAttribute]
        public ulong GuildId { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; }

        [XmlAttribute]
        public ulong ChannelId { get; set; }

        public override string ToString()
        {
            return $"{GuildId} : {ChannelId} ({(Enabled? "Enabled" : "Disabled")})";
        }

    }
}
