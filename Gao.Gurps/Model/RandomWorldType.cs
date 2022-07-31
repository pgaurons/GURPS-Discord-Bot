using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// For infinite world types.
    /// </summary>
    public enum RandomWorldType
    {
        /// <summary>
        /// No people.
        /// </summary>
        Empty,
        /// <summary>
        /// Exactly like our world at a different time.
        /// </summary>
        Echo,
        /// <summary>
        /// World where different historical event occurred.
        /// </summary>
        Parallel,
        /// <summary>
        /// World where adventures are weird.
        /// </summary>
        Challenge
    }
}
