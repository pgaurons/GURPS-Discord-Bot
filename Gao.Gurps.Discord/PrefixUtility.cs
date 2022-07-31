using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Discord.WebSocket;
using Gao.Gurps.Model;
using Gao.Gurps.Utility;

namespace Gao.Gurps.Discord
{
    internal class PrefixUtility
    {
        internal static string DefaultPrefix {get;}= ".";
        
        private static object _lock = new object();
        private static string _fileName = ConfigurationManager.Configuration["Gao.Gurps.Discord.GuildPrefixes"];
        internal static string GetPrefixFromChannelId(SocketGuildChannel socketGuildChannel)
        {
            var returnValue = DefaultPrefix;
            if(socketGuildChannel != null)
            {
                lock(_lock)
                {
                    List<GuildPrefix> result = GetValues();
                    var storedPrefix = result.FirstOrDefault(gp => gp.GuildId == socketGuildChannel.Guild.Id);
                    if (storedPrefix != null)
                        returnValue = storedPrefix.Prefix;
                }
            }

            return returnValue;
        }

        private static List<GuildPrefix> GetValues()
        {
            List<GuildPrefix> result;
            var deserializer = new XmlSerializer(typeof(List<GuildPrefix>));
            using (var fileStream = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                result = (List<GuildPrefix>)deserializer.Deserialize(fileStream);
            }
            return result;
        }

        internal static void SaveCustomPrefix(ulong guildId, string prefix)
        {
            //Console.WriteLine($"Changing the prefix - {guildId} : {prefix}");
            lock (_lock)
            {
                var values = GetValues();
                var existingValue = values.FirstOrDefault(gp => gp.GuildId == guildId);
                if (existingValue != null)
                {
                    //Console.WriteLine($"Found existing data - {existingValue.GuildId} : {existingValue.Prefix}");
                    values.Remove(existingValue);
                }
                values.Add(new GuildPrefix { GuildId = guildId, Prefix = prefix });
                var serializer = new XmlSerializer(typeof(List<GuildPrefix>));
                using (var fileStream = new FileStream(_fileName, FileMode.Create))
                {
                    //Console.WriteLine($"Saving Data to file {_fileName} - {guildId} : {prefix}");
                    serializer.Serialize(fileStream, values);
                }
            }

        }
    }
}