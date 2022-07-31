using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    public class RepairCost
    {
        public decimal Cost { get; internal set; }
        public TimeSpan Time { get; internal set; }
        public int NewHealthLevel { get; internal set; }
    }
}
