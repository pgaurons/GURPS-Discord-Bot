using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{

    public class LookupResult
    {
        public int OriginalCount { get; set; }
        public IEnumerable<string> Results {get; set;}
    }
}
