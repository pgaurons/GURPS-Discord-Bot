namespace Gao.Gurps.Model
{
    public class ParsedDiceAdds
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ParsedDiceAdds() { }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="original">object to copy</param>
        public ParsedDiceAdds(ParsedDiceAdds original)
        {
            Sides = original.Sides;
            Quantity = original.Quantity;
            Addend = original.Addend;
        }
        public int Sides { get; set; } = 6;
        public int Quantity { get; set; } = 3;
        public int Addend { get; set; } = 0;
        public int Multiplicand { get; set; } = 1;

        public override string ToString()
        {
            var sidePrintable = Sides == 6 ? "" : Sides.ToString();
            var addendPrintable = Addend != 0 ? Addend.ToString("+#;-#;0") : "";


            return $"{(Multiplicand != 1 ? Multiplicand +"x": "")}{Quantity}d{sidePrintable}{addendPrintable}";
        }
        /// <summary>
        /// Hashcode function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (unchecked(Sides.GetHashCode() * 17 + Quantity.GetHashCode() * 17 + Addend.GetHashCode() * 17)).GetHashCode();
        }

        /// <summary>
        /// Tells if two values are equal.
        /// </summary>
        /// <param name="obj">object to compare.</param>
        /// <returns>whether they are equal</returns>
        public override bool Equals(object obj)
        {

            return
                obj is ParsedDiceAdds compare &&
                compare.Addend == Addend &&
                compare.Quantity == Quantity &&
                compare.Sides == Sides &&
                compare.Multiplicand == Multiplicand;

        }

    }
}
