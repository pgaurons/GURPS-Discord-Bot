using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// A shield, protects people that hold them.
    /// </summary>
    public class Shield : IOverpenetrationTarget
    {
        public int HitPoints { get; set; }
        public int DamageResistance { get; set; }
        public int CoverDamageResistance
        {
            get
            {
                return DamageResistance + (HitPoints / 4);
            }
        }


        /// <summary>
        /// Important for overpenetration rules.
        /// </summary>
        public int Range { get; set; }
        /// <summary>
        /// Note, shield is homogeneous.
        /// </summary>
        /// <param name="basicDamage"></param>
        /// <param name="armorDivisor"></param>
        /// <returns></returns>
        public int CalculateReceivedDamage(int basicDamage, decimal armorDivisor, DamageType damageType)
        {
            var unadjustedDamage = basicDamage - (int)Math.Floor(DamageResistance / armorDivisor);
            var damageModifier = LookupTables.GetDamageModifier(damageType, InjuryTolerance.Homogeneous, HitLocation.None);

            return (int)Math.Floor(unadjustedDamage * damageModifier);
        }
    }
}
