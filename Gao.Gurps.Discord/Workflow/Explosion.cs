using Gao.Gurps.Dice;
using Gao.Gurps.Discord.Model;
using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gao.Gurps.Discord.Workflow
{
    internal class Explosion
    {
        internal static IEnumerable<ExplosionResult> Calculate(int explosionLevel, string explosionDamageText, string shrapnelDamageText, IEnumerable<int> targets, bool useGrandUnifiedHitLocationTable)
        {
            var explosionRange = GetRangeFromDamageText(explosionDamageText, 2);
            var shrapnelRange = GetRangeFromDamageText(shrapnelDamageText, 5);
            var parsedCalculatedExplosionDamageExpression = DiceAddsParser.StartParseDiceAdds(explosionDamageText, true);
            IExpression<int> parsedCalculatedShrapnelDamageExpression = null;
            if (!string.IsNullOrWhiteSpace(shrapnelDamageText))
            {
                parsedCalculatedShrapnelDamageExpression = DiceAddsParser.StartParseDiceAdds(shrapnelDamageText, true);
            }
            foreach (var target in targets) //Calculate the damage for each possible target.
            {
                //All the common properties.
                var returnValue = new ExplosionResult();
                returnValue.Distance = target;
                returnValue.ExplosionLevel = explosionLevel;
                returnValue.ExplosionDamageParsed = explosionDamageText;
                returnValue.ShrapnelDamageParsed = shrapnelDamageText;

                //Explosion Damage
                returnValue.InExplosionRange = returnValue.Distance <= explosionRange;
                if (returnValue.InExplosionRange)
                {
                    var parsedCalculatedExplosionDamage = parsedCalculatedExplosionDamageExpression.Evaluate();
                    //Ex 1 has a 3x range divisor, ex 2 has a  2xrange divisor, and ex 3 has a 1xrange divisor.
                    returnValue.ExplosionDamageEvaluated = parsedCalculatedExplosionDamage;
                }

                //Shrapnel damage.
                returnValue.InShrapnelRange = returnValue.Distance <= shrapnelRange;
                if(parsedCalculatedShrapnelDamageExpression != null && returnValue.InShrapnelRange) //Need to calculate shrapnel damage too.
                {
                    returnValue.ShrapnelHits = CalculateShrapnelDamage(returnValue, parsedCalculatedShrapnelDamageExpression, useGrandUnifiedHitLocationTable).ToArray();
                }

                yield return returnValue;
            }
        }

        private static IEnumerable<HitResult> CalculateShrapnelDamage(ExplosionResult parentExplosion, IExpression<int> parsedCalculatedShrapnelDamageExpression, bool useGrandUnifiedHitLocationTable = false)
        {
            var rangePenalty = LookupTables.GetDistancePenalty(parentExplosion.Distance.ToString());
            
            var skillLevel = 15 + rangePenalty;
            const int recoil = 3;
            var rollResult = Roller.RollAgainst(skillLevel);
            parentExplosion.ShrapnelRoll = rollResult.RollResult.Sum();
            if (rollResult.Success || parentExplosion.Distance == 0)
            {
                var hits = Math.Max(1, (rollResult.Margin / recoil) + 1);
                foreach (var hit in Enumerable.Range(1, hits))
                {
                    var hitResult = new HitResult();
                    hitResult.LocationRoll = Roller.Roll().Sum();
                    hitResult.Location = useGrandUnifiedHitLocationTable ?
                        LookupTables.GrandUnifiedHitLocationTable().ToString() :
                        LookupTables.GetHitLocation(Gurps.Model.HitLocationTable.Humanoid, hitResult.LocationRoll).ToString();
                    
                    var parsedCalculatedShrapnelDamage = parsedCalculatedShrapnelDamageExpression.Evaluate();
                    hitResult.Damage = parsedCalculatedShrapnelDamage;
                    hitResult.IsAutomatic = parentExplosion.Distance == 0;
                    yield return hitResult;
                }
            }
        }

        private static Regex FirstDigitExpression = new Regex(@"^\d+");
        private static Regex MultiplierExpression = new Regex(@"x(\d+)");
        private static int GetRangeFromDamageText(string damageText, int rangeMultiplier)
        {
            var returnValue = 0;
            if (!string.IsNullOrWhiteSpace(damageText))
            {
                //Range is calculated from number of dice times a multiplier.
                //Number of dice, in the example adxb is a * b.
                var nominalDiceCount = int.Parse(FirstDigitExpression.Match(damageText).Value);
                //The multiplier value is not always there.
                var diceMultiplier = MultiplierExpression.IsMatch(damageText) ? int.Parse(MultiplierExpression.Match(damageText).Groups[1].Value) : 1;
                returnValue = nominalDiceCount * diceMultiplier * rangeMultiplier;
            }

            return returnValue;
        }
    }
}