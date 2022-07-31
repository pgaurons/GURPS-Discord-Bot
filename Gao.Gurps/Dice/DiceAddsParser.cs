using Gao.Gurps.Dice;
using Gao.Gurps.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gao.Gurps.Dice
{
    public static class DiceAddsParser
    {
        /// <summary>
        /// Matches the dice adds numbers as much as I care.
        /// </summary>
        public static Regex ValidDiceAdds = new Regex(@"([1-9][0-9]*)([dD][0-9]*)([+\-][0-9]+)?" );
        private static readonly Regex _validDiceAddsFullString = new Regex(@$"^{ValidDiceAdds.ToString()}$");
        public static bool Valid(string diceAdds)
        {
            var valid = true;
            if (diceAdds == null)
                valid = false;
            else if (diceAdds != string.Empty)
            {
                valid = _validDiceAddsFullString.IsMatch(diceAdds);
                if (valid)
                {
                    var groups = _validDiceAddsFullString.Match(diceAdds).Groups;
                    var sides = groups[2].ToString().Trim();
                    valid =
                        sides.ToUpperInvariant() == "D" ||
                        int.Parse(sides.Trim('d', 'D')) > 0;
                }

            }
            return valid;
        }

        public static ParsedDiceAdds Parse(string diceAdds)
        {
            var returnValue = new ParsedDiceAdds();
            if (int.TryParse(diceAdds, out int result))
            {
                returnValue.Sides = 6;
                returnValue.Addend = result;
                returnValue.Quantity = 0;
            }
            else if (diceAdds != string.Empty)
            {
                var groups = _validDiceAddsFullString.Match(diceAdds).Groups;
                var quantity = int.Parse(groups[1].ToString());
                var sides = groups[2].ToString().Trim('d', 'D');
                var addend = !string.IsNullOrEmpty(groups[3].ToString()) ? int.Parse(groups[3].ToString()) : 0;
                returnValue = new ParsedDiceAdds
                {
                    Quantity = int.Parse(groups[1].ToString()),
                    Sides = sides == string.Empty ? 6 : int.Parse(sides),
                    Addend = addend
                };
            }

            return returnValue;
        }

        /// <summary>
        /// Factors a dice adds number.
        /// </summary>
        /// <param name="target">Dice adds to factor.</param>
        /// <returns>a factored value</returns>
        public static ParsedDiceAdds Factor(this ParsedDiceAdds target)
        {
            var copy = new ParsedDiceAdds(target);
            var originalNumericValue = target.HighTechDenormalize();
            ParsedDiceAdds bestMatch;
            if (copy.Multiplicand != 1)
            {
                copy.Quantity *= target.Multiplicand;
                copy.Multiplicand = 1;
            }
            if (copy.Quantity >= 12)
            {
                bestMatch = Enumerable.Range(4, 12).Select(i =>
                {
                    var item = new ParsedDiceAdds(copy);
                    item.Multiplicand = item.Quantity/i;
                    item.Quantity = i;
                    item.Addend /= i;
                    return item;

                }).Select(pda => new Tuple<ParsedDiceAdds, decimal>(pda, Math.Abs(1 - (pda.HighTechDenormalize() / originalNumericValue)))).
                OrderBy(t => t.Item2).
                ThenByDescending(t => t.Item1.Multiplicand).
                First().Item1;
            }
            else
            {
                bestMatch = copy;
            }
            
            return bestMatch;
        }

        /// <summary>
        /// Turns a dice adds into an equivalent decimal value.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <remarks>High tech treats one die as 3.5, not alternately 4, and 3.</remarks>
        public static decimal HighTechDenormalize(this ParsedDiceAdds target) => 
            (target.Multiplicand * target.Quantity * 3.5m) + target.Addend;
        
        /// <summary>
        /// Normalizes a dice adds object by making 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ParsedDiceAdds Normalize(this ParsedDiceAdds target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.Sides != 6) throw new ArgumentException(nameof(target), "This method is only for six sided dice");
            if (target.Quantity == 1 && target.Addend < 0) return new ParsedDiceAdds(target);
            const int valueOfTwoDice = 7;
            const int positiveValueOfOneDie = 4;
            const int negativeValueOfOneDie = 3;

            var valueOfOneDie = target.Addend > 0 ? positiveValueOfOneDie : negativeValueOfOneDie;

            var returnValue = new ParsedDiceAdds { Quantity = target.Quantity, Sides = target.Sides };
            var addend = target.Addend;
            var twoDice = addend /valueOfTwoDice;
            addend -= (twoDice * valueOfTwoDice);
            //see if there is a remainder greater than 4.
            var oneDie = addend / valueOfOneDie;
            if (addend == -6) oneDie++; //I think this only happens on -6.
            addend -= (oneDie * valueOfOneDie);
            returnValue.Quantity += oneDie + (2 * twoDice);
            if (addend < -1)
            {
                returnValue.Quantity -= 1;
                addend += positiveValueOfOneDie;
            }
            if (addend == 3)
            {
                returnValue.Quantity += 1;
                returnValue.Addend = -1;
            }
            else
            {
                returnValue.Addend = addend;
            }
            if(returnValue.Quantity < 1)
            {
                var quantity = returnValue.Quantity -1; //0 is -1; -1 is -2, etc.
                returnValue.Quantity = 1;
                returnValue.Addend += ((quantity/2)*valueOfTwoDice);
                returnValue.Addend += ((quantity % 2) * positiveValueOfOneDie);
                
            }
            return returnValue;
        }

        public static IExpression<int> StartParseDiceAdds(string diceAdds, bool skipEvaluateDiceRolls = false)
        {
            var sanitized = diceAdds.ToUpperInvariant().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("X", "*");
            if (Regex.IsMatch(sanitized, @"D([^0-9(]|$)"))//Assume that a 6 was intended if not included.
            {
                sanitized = Regex.Replace(sanitized, @"D([^0-9(\b]|$)", m => "D6" + m.Groups[1].Value);//eg "1D+5", find "D+" and change it to "D6+"
            }
            if (Regex.IsMatch(sanitized, @"([0-9])\("))//Assume that we are multiplying the integer by the paranthetical value.
            {
                sanitized = Regex.Replace(sanitized, @"([0-9])\(", m => m.Groups[1].Value + "*(");//eg "54(1+2)", find "4(" and change it to "4*("
            }
            if (Regex.IsMatch(sanitized, @"\)([0-9])"))//Assume that we are multiplying the parenthetical value by the integer.
            {
                sanitized = Regex.Replace(sanitized, @"\)([0-9])", m => ")*" + m.Groups[1].Value);//eg "(1+2)54", find ")5" and change it to ")*5"
            }
            
            if(!skipEvaluateDiceRolls)
                sanitized = EvaluateDiceRolls(sanitized);
            
            return ParseDiceAdds(sanitized);
        }

        private static string EvaluateDiceRolls(string sanitized)
        {
            var diceRoll = new Regex(@"\b(\d)D(\d+)\b");
            return diceRoll.Replace(sanitized, m => Roller.Roll(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)).Aggregate("(", (accum, i) => accum + i + "+", s => s.Trim('+') + ")"));
        }

        private static IExpression<int> ParseDiceAdds(string diceAdds)
        {
            if (new[] { '+', '-' }.Contains(diceAdds.First()))//If someone types in a plus or minus sign as the first value, add or subtract it from 0.
            {
                diceAdds = "0" + diceAdds;
            }
            var isParanthetical = IsParanthetical(diceAdds);
            if (isParanthetical)
            {
                diceAdds = diceAdds.Remove(diceAdds.Length - 1, 1).Remove(0, 1);
                //return the identity with no parenthesis.
                return new UnaryExpression<int> { Operand = ParseDiceAdds(diceAdds), Function = o => o, PrettyPrint = $"(   {diceAdds}   )" };
            }
            if (Regex.IsMatch(diceAdds, @"^\d+$")) // We've reached a number.
            {
                return new IdentityExpression<int> { Operand = int.Parse(diceAdds) };
            }
            //Now we need to do pemdas in reverse order. The d function has the highest precedence.
            string firstHalf;
            string secondHalf;
            char operation;

            if (CheckIfSplitOnOperator(diceAdds, out firstHalf, out secondHalf, out operation, false, '+', '-'))
            {
                Func<int, int, int> function;
                if (operation == '+')
                    function = (a, b) => a + b;
                else
                    function = (a, b) => a - b;
                return new BinaryExpression<int> { FirstOperand = ParseDiceAdds(firstHalf), SecondOperand = ParseDiceAdds(secondHalf), Function = function, PrettyPrint = $"{firstHalf}   {operation}   {secondHalf}", PrettyPrintOperator = operation };
            }
            if (CheckIfSplitOnOperator(diceAdds, out firstHalf, out secondHalf, out operation, false, '*', '/'))
            {
                Func<int, int, int> function;
                if (operation == '*')
                    function = (a, b) => a * b;
                else
                    function = (a, b) => a / b;
                return new BinaryExpression<int> { FirstOperand = ParseDiceAdds(firstHalf), SecondOperand = ParseDiceAdds(secondHalf), Function = function, PrettyPrint = $"{firstHalf}   {operation}   {secondHalf}", PrettyPrintOperator = operation };
            }
            if (CheckIfSplitOnOperator(diceAdds, out firstHalf, out secondHalf, out operation, true, '^'))
            {
                return new BinaryExpression<int> { FirstOperand = ParseDiceAdds(firstHalf), SecondOperand = ParseDiceAdds(secondHalf), Function = (a, b) => (int)Math.Floor(Math.Pow(a, b)), PrettyPrint = $"{firstHalf}   {operation}   {secondHalf}", PrettyPrintOperator = operation };
            }
            if (CheckIfSplitOnOperator(diceAdds, out firstHalf, out secondHalf, out operation, false, 'D'))
            {
                var firstOperand = ParseDiceAdds(firstHalf);
                var secondOperand = ParseDiceAdds(secondHalf);
                Func<int,int,int> function = (a, b) => Roller.Roll(a, b).Sum();
                return new BinaryExpression<int> { FirstOperand = firstOperand, SecondOperand = secondOperand, Function = function, PrettyPrint = $"{firstHalf}   {operation}   {secondHalf}", PrettyPrintOperator = 'd' };
            }
            throw new InvalidOperationException("Reached end of line without finding a good operator");
        }

        private static bool CheckIfSplitOnOperator(string sanitized, out string firstHalf, out string secondHalf, out char operation, bool rightAssociative, params char[] operators)
        {
            firstHalf = string.Empty;
            secondHalf = string.Empty;
            operation = ' ';
            var found = false;
            var parenDepth = 0;
            if (rightAssociative)
            {
                for (int i = 0; i<sanitized.Length; i++)
                {
                    var character = sanitized[i];
                    if (character == '(')
                        parenDepth++;
                    if (character == ')')
                        parenDepth--;
                    if (parenDepth == 0 && operators.Contains(character))
                    {
                        operation = character;
                        firstHalf = new string(sanitized.Take(i).ToArray()); // Take everything before the operator.
                        secondHalf = new string(sanitized.Skip(i + 1).ToArray()); //Everything after the operator.
                        found = true;
                        break;
                    }

                }
            }
            else
            {
                for (int i = sanitized.Length -1; i >= 0; i--)
                {
                    var character = sanitized[i];
                    if (character == ')')
                        parenDepth++;
                    if (character == '(')
                        parenDepth--;
                    if (parenDepth == 0 && operators.Contains(character))
                    {
                        operation = character;
                        firstHalf = new string(sanitized.Take(i).ToArray()); // Take everything before the operator.
                        secondHalf = new string(sanitized.Skip(i + 1).ToArray()); //Everything after the operator.
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }

        private static bool IsParanthetical(string value)
        {
            var valid = false;

            var count = 0;
            var index = 0;
            foreach (var character in value)
            {
                if (character == '(')
                    count++;
                if (character == ')')
                    count--;

                if (count < 0)
                {
                    throw new ArgumentException("Badly matched parantheses.", "value");
                }

                if (count == 0 && (index + 1 != value.Length)) //The parens don't wrap the entire equation.
                {
                    valid = false;
                    break;
                }
                else if (count == 0 && (index + 1 == value.Length))
                {
                    valid = value.First() == '(' && value.Last() == ')';
                }
                else if (count != 0 && (index + 1 == value.Length))
                {
                    throw new ArgumentException("Badly matched parantheses.", "value");
                }
                index++;
            }


            return valid;
        }
    }
}
