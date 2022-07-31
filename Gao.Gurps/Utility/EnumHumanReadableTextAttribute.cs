using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumHumanReadableTextAttribute : Attribute
    {
        public EnumHumanReadableTextAttribute(string text) { Text = text; }
        public string Text { get; set; }
    }
}
