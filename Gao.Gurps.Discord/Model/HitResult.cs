namespace Gao.Gurps.Discord.Model
{
    internal class HitResult
    {
        public int Damage { get; internal set; }
        public bool IsAutomatic { get; internal set; }
        internal string Location { get; set; }
        internal int LocationRoll { get; set; }
    }
}