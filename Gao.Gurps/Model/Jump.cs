using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    public class Jump
    {
        public int BasicMove { get; set; }
        public int SuperJumpLevel { get; set; }
        public decimal EnhancedMoveLevel { get; set; }
        /// <summary>
        /// inches
        /// </summary>
        public int StandingHighJumpHeight { get; set; }
        /// <summary>
        /// inches
        /// </summary>
        public int RunningHighJumpHeight { get; set; }

        /// <summary>
        /// feet
        /// </summary>
        public int StandingLongJumpDistance { get; set; }
        /// <summary>
        /// feet
        /// </summary>
        public int RunningLongJumpDistance { get; set; }

    }
}
