using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gao.Gurps.Discord.Workflow
{
    public static class ContestParser
    {
        public static Regex ValidContest = new Regex(@"^[A-Za-z\(\)\- ]*(\d+)(?: *(?:[vV][eE][rR][sS][uU][sS])|(?:[aA][gG][aA][iI][nN][sS][tT])|(?:[vV][sS]) *)?[A-Za-z\(\)\- ]*(\d+)$");

        public static bool Valid(string contest)
        {
            return ValidContest.IsMatch(contest);
        }

        public static IEnumerable<int> Parse(string contest)
        {
            var groups = ValidContest.Match(contest).Groups;
            yield return int.Parse(groups[1].ToString());
            yield return int.Parse(groups[2].ToString());
        }
    }
}
