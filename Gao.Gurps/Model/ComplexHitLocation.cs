using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Includes the enumeration for hit location and a custom penalty
    /// </summary>
    public class ComplexHitLocation : ICloneable
    {
        public HitLocation Location { get; set; }
        public int? Penalty { get; set; }

        public ComplexHitLocation Clone()
        {
            return new ComplexHitLocation { Location = Location, Penalty = Penalty };
        }

        public override string ToString()
        {
            return Utility.EnumerationHelper.PascalToNormalSpaced(Location.ToString());
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
