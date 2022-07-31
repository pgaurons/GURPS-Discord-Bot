namespace Gao.Gurps.Model
{
    public class RangedAttack
    {
        /// <summary>
        /// Range which the attack can still damage
        /// </summary>
        public decimal MaxRange { get; set; }

        /// <summary>
        /// Slightly smaller range where we can still do full damage. Leave 0 if there is no half range.
        /// </summary>
        public decimal HalfRange { get; set; }

        public ThrustSwingType StrengthBasedDamage { get; set; } = ThrustSwingType.Neither;

        /// <summary>
        /// Integer representation of damage beyond thrust or swing applied to an attack.
        /// </summary>
        public int DamageModifier { get; set; }

        public ParsedDiceAdds Damage { get; set; }

    }
}
