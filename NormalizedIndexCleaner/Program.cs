using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NormalizedIndexCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceDirectory = @"E:\standalone\gurpsIndices";
            var outputDirectory = @"cleanup\";
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            foreach (var xmlFilename in Directory.GetFiles(sourceDirectory))
            {
                IndexReferenceOld[] result;
                var deserializer = new XmlSerializer(typeof(IndexReferenceOld[]));
                using (var fileStream = new FileStream(xmlFilename, FileMode.Open))
                {
                    result = (IndexReferenceOld[])deserializer.Deserialize(fileStream);
                }

                var newIndexFormat = result.Where(r => r.ReferenceText != null).Select(ConvertToNewFormat).Select(Cleanup);

                var serializer = new XmlSerializer(typeof(IndexReferenceCollection));
                using (var fileStream = new FileStream(Path.Combine(outputDirectory, Path.GetFileName(xmlFilename)), FileMode.Create))
                {
                    var serializedValue = new IndexReferenceCollection { IndexReferences = new List<IndexReference>(newIndexFormat.Where(i => i.ReferenceText != null)) };
                    serializer.Serialize(fileStream, serializedValue);
                }
            }
        }

        private static IndexReference Cleanup(IndexReference reference)
        {

            var returnValue = reference;
            var originalReferenceText = reference.ReferenceText;
            if(originalReferenceText.Count(c => c == ',') == 1)
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

        private static IndexReference ConvertToNewFormat(IndexReferenceOld original)
        {
            var returnValue = new IndexReference();
            returnValue.PageReferences = original.PageNumbers;
            returnValue.ReferenceText = original.ReferenceText;
            returnValue.CrossReferences = original.SeeAlsos;
            returnValue.Children = new List<IndexReference>();
            foreach(var child in original.Children)
            {
                var newChild = new IndexReference();
                newChild.CrossReferences = child.SeeAlsos;
                newChild.ReferenceText = child.ReferenceText;
                newChild.PageReferences = child.PageNumbers;
                if (!newChild.CrossReferences.Any()) newChild.CrossReferences = null;
                if (!newChild.PageReferences.Any()) newChild.PageReferences = null;

                returnValue.Children.Add(newChild);
            }

            if (!returnValue.PageReferences.Any()) returnValue.PageReferences = null;
            if (!returnValue.Children.Any()) returnValue.Children = null;
            if (!returnValue.CrossReferences.Any()) returnValue.CrossReferences = null;
            

            return returnValue;
        }
    }
}
