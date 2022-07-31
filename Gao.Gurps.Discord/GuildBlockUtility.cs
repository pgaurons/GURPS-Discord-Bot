using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Discord.WebSocket;
using Gao.Gurps.Utility;

namespace Gao.Gurps.Discord
{
    internal class GuildBlockUtility
    {
        internal static bool IsBlocked(SocketGuildChannel socketGuildChannel)
        {
            //If this isn't a guild channel, it isn't blocked by the guild block utility.
            var returnValue = socketGuildChannel != null;
            if(returnValue)
            {
                var guild = socketGuildChannel.Guild.Id;
                returnValue = GetValues().Contains(guild);

            }
            return returnValue;
        }

        private static object _lock = new object();
        private static string _fileName = ConfigurationManager.Configuration["Gao.Gurps.Discord.GuildBlocks"];

        private static List<ulong> GetValues()
        {
            List<ulong> result;
            var deserializer = new XmlSerializer(typeof(List<ulong>));
            using (var fileStream = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                result = (List<ulong>)deserializer.Deserialize(fileStream);
            }
            return result;
        }

        private static void Unblock(ulong id)
        {
            lock(_lock)
            {
                var values = GetValues();
                values.Remove(id);
                SaveValueChanges(values);
            }
        }

        private static void Block(ulong id)
        {
            lock (_lock)
            {
                var values = GetValues();
                if(!values.Contains(id))
                {
                    values.Add(id);
                }
                
                SaveValueChanges(values);
            }
        }

        /// <summary>
        /// Toggles a guild banned or unbanned
        /// </summary>
        /// <param name="id">guild to ban</param>
        /// <returns>true if block, false if unblock</returns>
        public static bool Toggle(ulong id)
        {
            bool returnValue;
            if (GetValues().Contains(id))
            {
                Unblock(id);
                returnValue = false;
            }
            else
            {
                Block(id);
                returnValue = true;
            }
            return returnValue;
        }

        private static void SaveValueChanges(List<ulong> values)
        {
            var serializer = new XmlSerializer(typeof(List<ulong>));
            using (var fileStream = new FileStream(_fileName, FileMode.Create))
            {
                //Console.WriteLine($"Saving Data to file {_fileName} - {guildId} : {prefix}");
                serializer.Serialize(fileStream, values);
            }
        }
    }
}