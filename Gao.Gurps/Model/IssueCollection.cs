using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Gao.Gurps.Model
{
    [XmlRoot(Namespace = "http://gao.gurps.discord/", IsNullable = false)]
    [XmlType(AnonymousType = true, TypeName = "IndexReferences", Namespace = "http://gao.gurps.discord/")]
    public class IssueCollection : IEnumerable<Issue>, ICollection<Issue>
    {
        List<Issue> _internalList = new List<Issue>();
        public int Count
        {
            get
            {
                return _internalList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(Issue item)
        {
            _internalList.Add(item);
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(Issue item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(Issue[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Issue> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public bool Remove(Issue item)
        {
            return _internalList.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }
    }
}
