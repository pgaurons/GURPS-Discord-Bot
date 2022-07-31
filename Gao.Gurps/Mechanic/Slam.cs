using Gao.Gurps.Model;
using System;
using System.Linq;

namespace Gao.Gurps.Mechanic
{
    /// <summary>
    /// calculates slams, collisisons and falls.
    /// </summary>
    public static class Slam
    {
        public static SlamResult Execute(int velocity, int slammerHitPoints, int targetHitPoints)
        {
            //Damage by the slammer
            var slammerDamage = GetSlamDamage(velocity, slammerHitPoints);
            //Damage by the target.
            var targetDamage = GetSlamDamage(velocity, targetHitPoints);

            var resolvedSlammerDamage = Math.Max(0,Dice.Roller.Roll(slammerDamage).Sum());
            var resolvedTargetDamage = Math.Max(0,Dice.Roller.Roll(targetDamage).Sum());

            return new SlamResult
            {
                Velocity = velocity,
                PartyOneHitPoints = slammerHitPoints,
                PartyTwoHitPoints = targetHitPoints,

                PartyOneDamage = slammerDamage,
                PartyTwoDamage = targetDamage,

                PartyOneDamageResult = resolvedSlammerDamage,
                PartyTwoDamageResult = resolvedTargetDamage
            };

        }
        /// <summary>
        /// Calculates slam damage from a fall.
        /// </summary>
        /// <param name="height">height in yards</param>
        /// <param name="hitPoints">Hit Points of falling character</param>
        /// <param name="gravity">Optionally the gravitational constant. the default is 1, for 1g.</param>
        /// <returns></returns>
        public static SlamResult FallDamage(int height, int hitPoints, decimal gravity = 1m)
        {
            var fallVelocity = CalculateFallVelocity(height, gravity);
            //Damage by the slammer
            var slammerDamage = GetSlamDamage(fallVelocity, hitPoints);

            var resolvedSlammerDamage = Math.Max(0, Dice.Roller.Roll(slammerDamage).Sum());

            return new SlamResult
            {
                Velocity = fallVelocity,
                PartyOneHitPoints = hitPoints,
                PartyTwoHitPoints = 0,

                PartyOneDamage = slammerDamage,
                PartyTwoDamage = null,

                PartyOneDamageResult = resolvedSlammerDamage,
                PartyTwoDamageResult = 0
            };

        }

        /// <summary>
        /// Formula from B431
        /// </summary>
        /// <param name="height">Height fallen</param>
        /// <param name="gravity">Gravity in Gs.</param>
        /// <returns>velocity in yards per second rounded to the nearest number.</returns>
        private static int CalculateFallVelocity(int height, decimal gravity) => 
            (int)Math.Round((decimal)Math.Sqrt((double)(21.4m * gravity * height)));


        public static SlamResult ExecuteDungeonFantasy(int velocity, int slammerStrikingStrength, int targetStrikingStrength)
        {
            //Damage by the slammer
            var slammerDamage = GetSlamDamageDungeonFantasy(velocity, slammerStrikingStrength);
            //Damage by the target.
            var targetDamage = GetSlamDamageDungeonFantasy(velocity, targetStrikingStrength);

            var resolvedSlammerDamage = Math.Max(0, Dice.Roller.Roll(slammerDamage).Sum());
            var resolvedTargetDamage = Math.Max(0, Dice.Roller.Roll(targetDamage).Sum());

            return new SlamResult
            {
                Velocity = velocity,
                PartyOneHitPoints = slammerStrikingStrength,
                PartyTwoHitPoints = targetStrikingStrength,

                PartyOneDamage = slammerDamage,
                PartyTwoDamage = targetDamage,

                PartyOneDamageResult = resolvedSlammerDamage,
                PartyTwoDamageResult = resolvedTargetDamage
            };

        }

        private static ParsedDiceAdds GetSlamDamageDungeonFantasy(int velocity, int partyStrikingStrength)
        {
            var bonusDamage = LookupTables.GetSizeModifier(velocity);
            LookupTables.VanillaStrikingStrength(partyStrikingStrength, out int thrust, out int swing);
            var returnValue = LookupTables.RawDamageToDiceAddsFormat(thrust);
            returnValue.Addend -= 2; //It's thrust -2, but DFRPG needs this to happen out of order.
            returnValue.Addend += returnValue.Quantity * bonusDamage;
            return returnValue;

        }

        private static ParsedDiceAdds GetSlamDamage(int velocity, int partyHitPoints)
        {
            var result = new ParsedDiceAdds { Sides = 6 };
            var product = velocity * partyHitPoints;
            if(product >= 100) //1d and more damage.
            {
                result.Quantity = product / 100;
                var remainder = product % 100;
                if (remainder >= 50)
                    result.Quantity += 1;
            }
            else //Less than 1d damage.
            {
                result.Quantity = 1;
                result.Addend =
                    product <= 25 ? -3 :
                    product <= 50 ? -2 : -1;

            }
            return result;
        }
    }
}
