using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// For infinite world empty worlds.
    /// </summary>
    public enum EmptyWorldType
    {
        /// <summary>
        /// Planet for mining
        /// </summary>
        ResourceExploitation,
        /// <summary>
        /// Planet for colonies
        /// </summary>
        HomelineColony,
        /// <summary>
        /// Inhospitable planets.
        /// </summary>
        DisasterWorld
    }
}
