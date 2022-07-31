using Gao.Gurps.Model;
using Gao.Gurps.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Gao.Gurps.Lookup
{
    public class Index
    {
        public static Lazy<List<IndexReference>> LazyLibrary;
        public readonly static string Path = ConfigurationManager.Configuration["Gao.Gurps.IndexDirectory"];

        /// <summary>
        /// Query, item
        /// </summary>
        private static Func<string, string, bool> FindInIndexWhereClauseFunction = new Func<string, string, bool>((query, item) => Regex.IsMatch((item ?? ""), query, RegexOptions.IgnoreCase));

        static Index()
        {
            InitializeIndex();
        }

        /// <summary>
        /// Sets the index ready to reload.
        /// </summary>
        public static void InitializeIndex()
        {
            LazyLibrary = new Lazy<List<IndexReference>>(() => Directory.GetFiles(Path).Where(f => f.EndsWith(".xml") && !f.Contains("pyramid.xml")).SelectMany(f =>
            {
                IndexReferenceCollection result;
                var serializer = new XmlSerializer(typeof(IndexReferenceCollection));
                using (var fileStream = new FileStream(f, FileMode.Open))
                {
                    result = (IndexReferenceCollection)serializer.Deserialize(fileStream);
                }
                return result;
            })
            .ToList());
        }

        public static IEnumerable<IndexReference> FindInIndex(string query)
        {

            var fullResult = LazyLibrary.Value.
            Where(e => FindInIndexWhereClauseFunction(query, e.ReferenceText) || (e.Children != null && e.Children.Any(c => FindInIndexWhereClauseFunction(query, c.ReferenceText)))).
            GroupBy(ir => ir.ReferenceText).
            Select(g => g.Aggregate((a, b) => a.Merge(b)));

            
            return fullResult;
        }
    }
}
