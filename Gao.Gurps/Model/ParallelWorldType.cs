using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Type of parallel world for Infinite worlds.
    /// </summary>
    public enum ParallelWorldType
    {
        /// <summary>
        /// Minor details changed
        /// </summary>
        Close,
        /// <summary>
        /// Important details changed
        /// </summary>
        Farther,
        /// <summary>
        /// Worlds with big changes but surprising similarities
        /// </summary>
        HighInertia,
        /// <summary>
        /// Seems to based on fictional world.
        /// </summary>
        Myth
    }
}
