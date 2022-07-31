using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Mechanic
{
    /// <summary>
    /// Calculations for jumping
    /// </summary>
    public static class Jumping
    {
        public static Jump CalculateJumpMetrics(int basicMove, int superJumpLevel = 0, decimal enhancedMoveLevel = 0)
        {
            var returnValue = new Jump { BasicMove = basicMove, SuperJumpLevel = superJumpLevel, EnhancedMoveLevel = enhancedMoveLevel };

            returnValue.StandingHighJumpHeight = HighJumpHeight(basicMove, superJumpLevel);
            var doubleHeight = returnValue.StandingHighJumpHeight * 2;
            returnValue.RunningHighJumpHeight = Math.Max(doubleHeight, HighJumpHeight(CalculateEnhancedMove(basicMove, enhancedMoveLevel), superJumpLevel));

            returnValue.StandingLongJumpDistance = BroadJumpDistance(basicMove, superJumpLevel);
            var doubleDistance = returnValue.StandingLongJumpDistance * 2;
            returnValue.RunningLongJumpDistance = Math.Max(BroadJumpDistance(CalculateEnhancedMove(basicMove, enhancedMoveLevel), superJumpLevel), doubleDistance);

            return returnValue;
        }

        private static int BroadJumpDistance(int basicMove, int superJumpLevel)
        {
            return ((2*basicMove) -3) * (int)Math.Pow(2, superJumpLevel);
        }

        private static int HighJumpHeight(int basicMove, int superJumpLevel)
        {
            return ((6 * basicMove) - 10) * (int)Math.Pow(2, superJumpLevel);
        }
        private static int CalculateEnhancedMove(int basicMove, decimal enhancedMoveLevel)
        {
            var hasHalfLevel = enhancedMoveLevel % 1.0m == 0.5m;
            var fullLevels = (int)Math.Floor(enhancedMoveLevel);
            return (int)Math.Floor(basicMove * ((int)Math.Pow(2, fullLevels)) * (hasHalfLevel ? 1.5m : 1m));
        }
    }
}
