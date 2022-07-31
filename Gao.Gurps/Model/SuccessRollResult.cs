using System.Collections.Generic;

namespace Gao.Gurps.Model
{
    public class SuccessRollResult
    {
        /// <summary>
        /// Negative means margin of failure
        /// </summary>
        public int Margin { get; set; }
        public bool Success { get; set; }
        public bool Critical { get; set; }
        public int Target { get; set; }
        public IEnumerable<int> RollResult { get; set; }
    }
}
