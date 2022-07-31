using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Attributes of a weapon from Broken Blade.
    /// </summary>
    public class BrokenBladeStatistics
    {
        public uint WeaponStrength { get; internal set; }
        public EquipmentQuality Quality { get; internal set; }
        public bool IsHafted { get; internal set; }
        public bool IsSymmetrical { get; internal set; }
        public uint DefenseBreakageThreshold { get; internal set; }
        public uint AttackBreakageThreshold { get; internal set; }
        public ParsedDiceAdds AttackSafetyLimit { get; internal set; }
        public ParsedDiceAdds DefenseSafetyLimit { get; internal set; }
        public int FailureIncrement { get; internal set; }
        public int Health { get; internal set; }
        public int DamageResistance { get; internal set; }
        public int EdgeHealth { get; internal set; }
        public decimal Cost { get; internal set; }
        public int HealthDamage { get; internal set; }
        public RepairCost[] RepairCosts { get; set; } = new RepairCost[0];
        public int ArmoryModifier { get; internal set; }
    }
}
