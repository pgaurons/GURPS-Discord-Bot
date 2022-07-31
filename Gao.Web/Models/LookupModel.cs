using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Gao.Web.Models
{
    /// <summary>
    /// The web model for doing lookups.
    /// </summary>
    public class LookupModel
    {
        public string LookupType { get; set; } = "Index";
        public string Query { get; set; } = string.Empty;
        public ObservableCollection<LookupResult> Results { get; set; } = new ObservableCollection<LookupResult>();
        
    }

}
