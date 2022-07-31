using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    public class Cover : IOverpenetrationTarget
    {
        public int HitPoints { get; set; }
        public int CoverDamageResistance
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Range { get; set; }

        public int CalculateReceivedDamage(int basicDamage, decimal armorDivisor, DamageType damageType)
        {
            throw new NotImplementedException();
        }
    }
}
