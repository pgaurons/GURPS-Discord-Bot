using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gao.Gurps.Mechanic
{
    public static class BrokenBlade
    {
        public static BrokenBladeStatistics GenerateStatistics(uint weaponStrength, EquipmentQuality quality, bool isHafted, bool isSymmetrical, decimal baseValue, int healthDamage)
        {
            var returnValue = new BrokenBladeStatistics
            {
                WeaponStrength = weaponStrength,
                Quality = quality,
                IsHafted = isHafted,
                IsSymmetrical = isSymmetrical
            };
            returnValue.AttackBreakageThreshold = (weaponStrength * 3) / 2;
            returnValue.DefenseBreakageThreshold = isSymmetrical ? returnValue.AttackBreakageThreshold : weaponStrength;

            const int divisor = 6;
            var attackSafetyLimitDividend = returnValue.AttackBreakageThreshold / divisor;
            var attackSafetyLimitRemainder = returnValue.AttackBreakageThreshold % divisor;
            returnValue.AttackSafetyLimit = new ParsedDiceAdds
            {
                Addend = attackSafetyLimitDividend == 0 ? ((int)attackSafetyLimitRemainder - 6) : (int)attackSafetyLimitRemainder,
                Quantity = Math.Max(1, (int)attackSafetyLimitDividend),
                Sides = 6
            };

            var defenseSafetyLimitDividend = returnValue.DefenseBreakageThreshold / divisor;
            var defenseSafetyLimitRemainder = returnValue.DefenseBreakageThreshold % divisor;
            returnValue.DefenseSafetyLimit = new ParsedDiceAdds
            {
                Addend = defenseSafetyLimitDividend == 0 ? ((int)defenseSafetyLimitRemainder - 6) : (int)defenseSafetyLimitRemainder,
                Quantity = Math.Max(1, (int)defenseSafetyLimitDividend),
                Sides = 6
            };
            returnValue.FailureIncrement = Math.Max(1, (int)Math.Round(Convert.ToDecimal(weaponStrength) / 5m));
            returnValue.Health =
                quality == EquipmentQuality.Cheap ? 10 :
                quality == EquipmentQuality.Good ? 12 :
                quality == EquipmentQuality.Fine ? 14 :
                /*quality == EquipmentQuality.VeryFine ?*/ 16;
            if (Math.Abs(healthDamage) > returnValue.Health) throw new ArgumentOutOfRangeException(nameof(healthDamage));
            returnValue.DamageResistance = isHafted ? 4 : 6;
            returnValue.EdgeHealth = returnValue.Health - 2;
            returnValue.Cost = baseValue;
            returnValue.HealthDamage = healthDamage;

            if(baseValue != 0 && healthDamage != 0)
            {
                var repairCosts = new List<RepairCost>();
                returnValue.ArmoryModifier = 12 - returnValue.Health;
                //calculate repair costs.
                for(var i = healthDamage + 1; i <= 0; i++)
                {
                    var costPercent = CostPercentageDictionary[healthDamage] - CostPercentageDictionary[i];
                    var time = TimeDictionary[healthDamage] - TimeDictionary[i];
                    var newHealthLevel = returnValue.Health + i;
                    repairCosts.Add
                    (
                        new RepairCost
                        {
                            Cost = returnValue.Cost * costPercent,
                            Time = time,
                            NewHealthLevel = newHealthLevel
                        }
                    );
                    
                }
                returnValue.RepairCosts = repairCosts.ToArray();
                if(returnValue.Health + healthDamage <= 6)
                {
                    returnValue.DamageResistance = Math.Max(returnValue.DamageResistance / 2, 1);
                    if(!returnValue.IsSymmetrical)
                    {
                        returnValue.AttackBreakageThreshold = returnValue.DefenseBreakageThreshold;
                        returnValue.AttackSafetyLimit = returnValue.DefenseSafetyLimit;
                    }
                }
            }

            return returnValue;
        }

        private static Dictionary<int, decimal> CostPercentageDictionary = new Dictionary<int, decimal>
        {
            {0, 0m },
            {-1, 0.025m },
            {-2, 0.2m },
            {-3, 0.5m },
            {-4, 1m },
            {-5, 1.5m },
            {-6, 2.5m },
            {-7, 3.25m },
            {-8, 4.5m },
            {-9, 5.5m },
            {-10, 7m },
            {-11, 8.5m },
            {-12, 10.25m },
            {-13, 12m },
            {-14, 14m },
            {-15, 16m },
            {-16, 18.25m },
            {-17, 20.75m },
            {-18, 23.25m },
        };
        private static Dictionary<int, TimeSpan> TimeDictionary = new Dictionary<int, TimeSpan>
        {
            {0, new TimeSpan(0) },
            {-1, new TimeSpan(6,0,0) },
            {-2, new TimeSpan(24,0,0) },
            {-3, new TimeSpan(48,0,0) },
            {-4, new TimeSpan(120,0,0) },
            {-5, new TimeSpan(240,0,0) },
            {-6, new TimeSpan(336,0,0) },
            {-7, new TimeSpan(504,0,0) },
            {-8, new TimeSpan(840,0,0) },
            {-9, new TimeSpan(1176,0,0) },
            {-10, new TimeSpan(1680,0,0) },
            {-11, new TimeSpan(2352,0,0) },
            {-12, new TimeSpan(3024,0,0) },
            {-13, new TimeSpan(4032,0,0) },
            {-14, new TimeSpan(5040,0,0) },
            {-15, new TimeSpan(6216,0,0) },
            {-16, new TimeSpan(7728,0,0) },
            {-17, new TimeSpan(9408,0,0) },
            {-18, new TimeSpan(11424,0,0) }
        };

    }
}
