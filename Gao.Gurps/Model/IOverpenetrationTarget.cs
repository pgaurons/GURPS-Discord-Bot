using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Something that could potentially be overpenetrated with a shot.
    /// </summary>
    public interface IOverpenetrationTarget
    {

        /// <summary>
        /// Returns how much damage is done to this target.
        /// </summary>
        /// <param name="basicDamage">damage rolled for an attack</param>
        /// <param name="armorDivisor">The armor divisor of the attack, usually 1, but different armor divisors make a difference</param>
        /// <returns>How much HP the target sustained.</returns>
        int CalculateReceivedDamage(int basicDamage, decimal armorDivisor, DamageType damageType);

        /// <summary>
        /// How far away this element is from the shooter.
        /// </summary>
        int Range { get; set; }
        /// <summary>
        /// How much damage this target can soak up.
        /// </summary>
        int CoverDamageResistance { get; }
    }
}
