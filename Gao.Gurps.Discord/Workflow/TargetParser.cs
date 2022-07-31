using Gao.Gurps.Model;
using System.Text.RegularExpressions;

namespace Gao.Gurps.Discord.Workflow
{
    public static class TargetParser
    {
        private static readonly Regex _preAmble = new Regex(@"[A-Za-z\(\)\!\- ]*");
        public static Regex ValidTarget = new Regex(@$"({Dice.DiceAddsParser.ValidDiceAdds})");

        public static bool Valid(string target)
        {
            var value = Regex.Replace(target, '^' + _preAmble.ToString(), string.Empty);
            var valid = ValidTarget.IsMatch(value) || int.TryParse(value, out int _);

            return valid;
        }

        public static string Parse(string target)
        {
            var value = Regex.Replace(target, '^' + _preAmble.ToString(), string.Empty);
            return value;
        }
    }
}
