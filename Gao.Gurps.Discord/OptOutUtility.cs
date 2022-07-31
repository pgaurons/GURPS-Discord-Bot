using Gao.Gurps.Discord.Model;
using Gao.Gurps.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Gao.Gurps.Discord
{
    internal class OptOutUtility
    {
        internal static OptOutOptions OptingOut(ulong userId) => (GetValues().FirstOrDefault(oo => oo.UserId == userId)?.Option) ?? OptOutOptions.None;


        private static readonly object _lock = new object();
        private static readonly string _fileName = ConfigurationManager.Configuration["Gao.Gurps.Discord.OptOuts"];

        internal static List<OptOut> GetValues()
        {
            List<OptOut> result;
            var deserializer = new XmlSerializer(typeof(List<OptOut>));
            using (var fileStream = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                result = (List<OptOut>)deserializer.Deserialize(fileStream);
            }
            return result;
        }

        public static void SetOptions(ulong id, OptOutOptions options)
        {
            lock(_lock)
            {
                var values = GetValues();
                var newValue = values.FirstOrDefault(oo => oo.UserId == id);// ?? new OptOut { UserId = id };
                if(newValue != null)
                {
                    newValue.Option = options;
                }
                else
                {
                    values.Add(new OptOut { UserId = id, Option = options });
                }
                
                SaveValueChanges(values);
            }
        }

        private static void SaveValueChanges(List<OptOut> values)
        {
            var serializer = new XmlSerializer(typeof(List<OptOut>));
            using var fileStream = new FileStream(_fileName, FileMode.Create);
            //Console.WriteLine($"Saving Data to file {_fileName} - {guildId} : {prefix}");
            serializer.Serialize(fileStream, values);
        }
    }
}