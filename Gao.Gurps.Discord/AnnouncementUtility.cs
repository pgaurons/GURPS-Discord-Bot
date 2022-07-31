using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Discord.WebSocket;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Model;
using Gao.Gurps.Utility;

namespace Gao.Gurps.Discord
{
    internal class AnnouncementUtility
    {
        
        private static object _lock = new object();
        private static string _fileName = ConfigurationManager.Configuration["Gao.Gurps.Discord.GuildAnnouncements"];
        internal static GuildAnnouncement GetAnnouncementSettingsFromGuild(SocketGuild guild, ulong botUserId)
        {
            var returnValue = GetDefaultAnnouncementSetting(guild, botUserId);
            GuildAnnouncement storedPreference = null;
            lock (_lock)
            {
                List<GuildAnnouncement> result = GetValues();
                storedPreference = result.FirstOrDefault(gp => gp.GuildId == guild.Id);
            }
                if (storedPreference != null)
                    returnValue = storedPreference;


            return returnValue;
        }

        private static GuildAnnouncement GetDefaultAnnouncementSetting(SocketGuild guild, ulong botUserId)
        {
            var returnValue = new GuildAnnouncement { GuildId = guild.Id, Enabled = false };
            var firstChannelName = guild.TextChannels.FirstOrDefault()?.Name ?? "";
            foreach (var channel in guild.TextChannels.OrderByDescending(g => g != null && g.Name != null && g.Name.ToUpperInvariant() != null && (g.Name.ToUpperInvariant().Contains("TEST") || g.Name.ToUpperInvariant().Contains("ROLL") || g.Name.ToUpperInvariant().Contains("BOT"))))
            {
                if (channel == null || channel.Name == null) continue;
                var name = channel.Name.ToUpperInvariant();
                var nameValid = name.Contains("TEST") || name.Contains("ROLL") || name.Contains("BOT") || channel.Position == 0;

                var botInChannel = channel.Users.Any(u => u.Id == botUserId);
                var botHasPermissions = botInChannel && channel.Users.ToList().First(u => u.Id == botUserId).GetPermissions(channel).SendMessages;
                if (nameValid && botInChannel && botHasPermissions)
                {
                    returnValue.ChannelId = channel.Id;
                    break;
                }
            }
            return returnValue;
        }

        private static List<GuildAnnouncement> GetValues()
        {
            List<GuildAnnouncement> result;
            var deserializer = new XmlSerializer(typeof(List<GuildAnnouncement>));
            using (var fileStream = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                result = (List<GuildAnnouncement>)deserializer.Deserialize(fileStream);
            }
            return result;
        }

        internal static void SaveCustomPreference(GuildAnnouncement settings)
        {
            //Console.WriteLine($"Changing the prefix - {guildId} : {prefix}");
            lock (_lock)
            {
                var values = GetValues();
                var existingValue = values.FirstOrDefault(gp => gp.GuildId == settings.GuildId);
                if (existingValue != null)
                {
                    //Console.WriteLine($"Found existing data - {existingValue.GuildId} : {existingValue.Prefix}");
                    values.Remove(existingValue);
                }
                values.Add(settings);
                var serializer = new XmlSerializer(typeof(List<GuildAnnouncement>));
                using (var fileStream = new FileStream(_fileName, FileMode.Create))
                {
                    //Console.WriteLine($"Saving Data to file {_fileName} - {guildId} : {prefix}");
                    serializer.Serialize(fileStream, values);
                }
            }

        }
    }
}