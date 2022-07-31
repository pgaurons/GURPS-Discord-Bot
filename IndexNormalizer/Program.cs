using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.IO;

namespace IndexNormalizer
{
    class Program
    {
        const string TextBackupDirectory = @"textBackup\";

        static void Main(string[] args)
        {
            const string pdfInputDirectory = @"pdfInput\";
            const string textOutputDirectory = @"textOutput\";
            const string xmlOutputDirectory = @"xmlOutput\";

            Console.WriteLine("Do You want to extract text (y) or generate the finished XML (n)?");
            var extractRawPhase = !Console.ReadLine().ToUpperInvariant().StartsWith("N");
            if (extractRawPhase) Console.WriteLine("Extracting From PDF.");
            else Console.WriteLine("Creating XML.");

            if (!Directory.Exists(textOutputDirectory))
                Directory.CreateDirectory(textOutputDirectory);
            if (!Directory.Exists(xmlOutputDirectory))
                Directory.CreateDirectory(xmlOutputDirectory);
            if (!Directory.Exists(TextBackupDirectory))
                Directory.CreateDirectory(TextBackupDirectory);

            if (extractRawPhase)
            {
                ExtractIndexFromPdfFiles(pdfInputDirectory, textOutputDirectory);
            }

            else
            {
                ConvertIndexToXml(textOutputDirectory, xmlOutputDirectory);
            }

            Console.Read();
        }

        private static void ConvertIndexToXml(string textOutputDirectory, string xmlOutputDirectory)
        {
            foreach (var textFilename in Directory.GetFiles(textOutputDirectory))
            {
                var rawText = File.ReadAllLines(textFilename);
                var prefix = System.IO.Path.GetFileNameWithoutExtension(textFilename);
                var xmlFilename = $"{xmlOutputDirectory}{prefix}.xml";
                if (Regex.IsMatch(prefix, @"\d$")) prefix += ":"; //Add a colon if the prefix ends with a number.
                var processedIndices = rawText.
                    Where(s => s.Trim() != string.Empty).
                    Select(indexLine => ProcessIndexLine(indexLine, prefix)).
                    Where(ir => ir.ReferenceText != null).
                    Select(Cleanup).
                    ToArray();
                var serializedValue = new IndexReferenceCollection {IndexReferences = new List<IndexReference>(processedIndices) };
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(serializedValue.GetType());

                using (var stringWriter = new FileStream(xmlFilename, FileMode.Create))
                {
                    x.Serialize(stringWriter, serializedValue);
                }

                File.Move(textFilename, System.IO.Path.Combine(TextBackupDirectory, System.IO.Path.GetFileName(textFilename)));

                Console.WriteLine($"Created file {xmlFilename}");
            }
        }

        private static void ExtractIndexFromPdfFiles(string pdfInputDirectory, string textOutputDirectory)
        {
            foreach (var filename in Directory.GetFiles(pdfInputDirectory))
            {
                Console.Write($"What prefix would you like for {System.IO.Path.GetFileName(filename)}?");
                var prefix = Console.ReadLine();
                var textFilename = $"{textOutputDirectory}{prefix}.txt";

                int firstPage = 0;
                int lastPage = 0;
                FindFirstAndLastPageOfIndex(ref firstPage, ref lastPage, filename);

                var numberOfPages = lastPage - firstPage + 1;

                var reader = new PdfReader(filename);
                var strategy = new SimpleTextExtractionStrategy(); //Breaks text up into columns.
                var pageText = string.Empty;
                foreach (var pageNumber in Enumerable.Range(firstPage, numberOfPages))
                {

                    pageText = PdfTextExtractor.GetTextFromPage(reader, pageNumber, strategy);

                }

                var processedIndices = CleanupText(pageText).ToArray();
                File.WriteAllLines(textFilename, processedIndices);
                Console.WriteLine($"Created file {textFilename}");
            }
        }

        private static void FindFirstAndLastPageOfIndex(ref int firstPage, ref int lastPage, string filename)
        {
            Console.WriteLine("Finding first page");
            firstPage = FindPage(filename);
            Console.WriteLine("Finding second page");
            lastPage = FindPage(filename);
            Console.WriteLine("Done finding first and last page.");
        }

        private static int FindPage(string filename)
        {
            var pageNumber = 0;
            var foundPage = false;
            
            while (!foundPage)
            {
                var strategy = new SimpleTextExtractionStrategy(); //Breaks text up into columns.
                Console.Write("Guess the page: ");
                pageNumber = int.Parse(Console.ReadLine());
                var reader = new PdfReader(filename);
                var sample = PdfTextExtractor.GetTextFromPage(reader, pageNumber, strategy).Substring(0, 100);
                Console.WriteLine("Does this sample look correct? (y/n)");
                Console.WriteLine(sample);
                foundPage = Console.ReadLine().ToUpperInvariant().StartsWith("Y");
            }

            return pageNumber;
        }

        private static IndexReference ProcessIndexLine(string indexLine, string pageReferencePrefix)
        {

            var parent = new IndexReference { Children = new List<IndexReference>() };
            var sections = indexLine.Split(new[] { ';', '.' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim(' ', '.')).ToArray();

            var mainEntry = GetEntryName(indexLine, sections[0]);
            if(mainEntry.First().ToString().ToUpperInvariant() == mainEntry.First().ToString() && mainEntry.Contains(',')) //If the first letter is capitalized, and there is a comma.
            {
                mainEntry = mainEntry.Split(',')[0];
            }
            foreach(var section in sections.Where(s => !SeeAlso.IsMatch(s)))
            {
                var isParent = false;
                try
                {
                    
                    var entryName = GetEntryName(indexLine, section);
                    if (entryName.Contains(mainEntry))//Special handling on the very first guy.
                    {
                        isParent = true;
                        foreach (var seeAlso in sections.Where(s => SeeAlso.IsMatch(s)))
                        {
                            if (parent.CrossReferences == null) parent.CrossReferences = new List<string>();
                            parent.CrossReferences.AddRange(SeeAlso.Match(seeAlso).Groups[2].Value.Trim().Split(',').Select(s => s.Trim()));
                        }
                    }
                    if (isParent)
                    {
                        SetPropertiesOnReference(pageReferencePrefix, parent, section, entryName);
                    }
                    else
                    {
                        var child = new IndexReference();// { SourceValue = section };
                        child.ReferenceText = entryName;
                        SetPropertiesOnReference(pageReferencePrefix, child, section, entryName);
                        parent.Children.Add(child);
                    }
                }
                catch
                {
                    Console.Error.WriteLine($"Failed to process the following line: {indexLine}");
                    throw;
                }
            }

            return parent;

        }

        private static void SetPropertiesOnReference(string pageReferencePrefix, IndexReference indexReference, string section, string entryName)
        {
            indexReference.ReferenceText = entryName;
            foreach (var subsection in section.Split(',').Select(ss => ss.Trim()))
            {
                if (PageReference.IsMatch(subsection))
                {
                    if (indexReference.PageReferences == null) indexReference.PageReferences = new List<string>();
                    indexReference.PageReferences.Add(pageReferencePrefix + subsection);
                }
                if (SeeAlso.IsMatch(subsection))
                {
                    if (indexReference.CrossReferences == null) indexReference.CrossReferences = new List<string>();
                    indexReference.CrossReferences.Add(SeeAlso.Match(subsection).Groups[2].Value.Trim());
                }
            }
        }

        private static IndexReference Cleanup(IndexReference reference)
        {

            var returnValue = reference;
            var originalReferenceText = reference.ReferenceText;
            if (originalReferenceText.Count(c => c == ',') == 1)
            {
                try
                {
                    var splitText = reference.ReferenceText.Split(',').Select(s => s.Trim()).ToArray();
                    returnValue.ReferenceText = splitText[0];
                    var newChild = new IndexReference { ReferenceText = splitText[1], PageReferences = reference.PageReferences.ToList() };
                    returnValue.Children.Add(newChild);
                    returnValue.PageReferences = null;
                }
                catch
                {
                    returnValue.ReferenceText = originalReferenceText;
                }
            }
            return returnValue;
        }

        static Regex SeeAlso = new Regex(@"^((?:[Ss]ee also)|(?:[Ss]ee))(.*)$");

        static Regex PageReference = new Regex(@"^(\d+\-?\d*)$");

        private static string GetEntryName(string wholeLine, string entry)
        {
            var sections = entry.Split(',').Select(s => s.Trim(' ', '.')).ToArray();
            var returnValue = sections.TakeWhile(p => wholeLine.StartsWith(p) || (!PageReference.IsMatch(p) && !SeeAlso.IsMatch(p))).Aggregate((accum, p) => accum + ", "  + p);
            return returnValue.Trim(',', ' ');
        }

        static Regex BadCarriageReturnLineFeeds = new Regex(@"(?<!\.)$\n", RegexOptions.Multiline);
        static Regex HyphenAtTheEndOfALine = new Regex(@"(?<![0-9])-$\n", RegexOptions.Multiline);
        static Regex HyphenatedPageRangeAtTheEndOfALine = new Regex(@"([0-9])-$\n", RegexOptions.Multiline);

        static Regex[] BlacklistExpressions = new[]
        {
            new Regex(@"^\d* INDEX$", RegexOptions.Multiline),
            new Regex(@"^INDEX \d*$", RegexOptions.Multiline),
            new Regex(@"^INDEX$", RegexOptions.Multiline)
        };
        static string[] BlacklistConstants = new[]
        {
            "This index covers both books of the Basic Set. The pages are sequentially numbered; Book 2 starts on p. 337.",
            "With rare exceptions, traits (advantages, disadvantages, skills, spells, and so on) are not listed in this index.",
            "Instead, they have their own alphabetical listings. See the Trait Lists on pp. 297-306.",
            "● See the ratings other users have given . . . and add your own ratings.",
            "● Buy it once, have it always. Download your purchases again whenever you need to.",
            "Download ● Print ● Play e23 sells high-quality game adventures and supplements in PDF format.",
            "STEVE JACKSON GAMES e23.sjgames.com Stuck for an adventure? No problem.",
            "e23 is part of Warehouse 23, the online store at Steve Jackson Games. Warehouse 23 is also the official Internet retailer for Dork Storm Press, Atlas Games, and many other publishers. Visit us today at www.warehouse23.com for all your game STUFF!",
            string.Empty,
            "WEAPONRY INDEX"
        };
        private static IEnumerable<string> CleanupText(string pageText)
        {
            //First remove innappropriate text... most often, this is the page number, Capslocked INDEX, and maybe introductory paragraphs.
            //Get rid of weird headers:
            foreach(var expression in BlacklistExpressions)
            {
                pageText = expression.Replace(pageText, string.Empty);
            }
            //Fix hyphenated page ranges first:
            pageText = HyphenatedPageRangeAtTheEndOfALine.Replace(pageText, m => m.Groups[1].Value + "-");
            //Remove hyphens at the end of lines
            pageText = HyphenAtTheEndOfALine.Replace(pageText, string.Empty);
            //Remove superflous CRLFs
            pageText = BadCarriageReturnLineFeeds.Replace(pageText, " ");
            //Remove occassional double spaces.
            pageText = pageText.Replace("  ", " ");
            return pageText.Split('\r','\n').Except(BlacklistConstants).Where(item => !BlacklistExpressions.Any(ble => ble.IsMatch(item))).Select(s => s.Trim());
        }
    }
}
