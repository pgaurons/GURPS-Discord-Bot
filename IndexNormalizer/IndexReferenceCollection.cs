using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IndexNormalizer
{
    [XmlRoot(Namespace = "http://gao.gurps.discord/", IsNullable = false)]
    [XmlType(AnonymousType = true, TypeName = "IndexReferences", Namespace = "http://gao.gurps.discord/")]
    public class IndexReferenceCollection : IEnumerable<IndexReference>, ICollection<IndexReference>
    {
        [XmlIgnore]
        public int Count
        {
            get
            {
                return IndexReferences.Count;
            }
        }

        [XmlElement(ElementName = "IndexReference")]
        public List<IndexReference> IndexReferences { get; set; }

        [XmlIgnore]
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(IndexReference item)
        {
            IndexReferences.Add(item);
        }

        public void Clear()
        {
            IndexReferences.Clear();
        }

        public bool Contains(IndexReference item)
        {
            return IndexReferences.Contains(item);
        }

        public void CopyTo(IndexReference[] array, int arrayIndex)
        {
            IndexReferences.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IndexReference> GetEnumerator()
        {
            return IndexReferences.GetEnumerator();
        }

        public bool Remove(IndexReference item)
        {
            return IndexReferences.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return IndexReferences.GetEnumerator();
        }
    }
}
