using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    public class Lift
    {
        public decimal BasicLift { get; set; }
        public decimal Light { get { return BasicLift * 2 ; } }
        public decimal Medium { get { return BasicLift * 3; } }
        public decimal Heavy { get { return BasicLift * 6; } }
        public decimal ExtraHeavy { get { return BasicLift * 10; } }
        public decimal OneHandedLift { get { return BasicLift * 2; } }
        public decimal TwoHandedLift { get { return BasicLift * 8; } }
        public decimal ShoveAndKnockOver { get { return BasicLift * 12; } }
        public decimal RunningShoveAndKnockOver { get { return BasicLift * 24; } }
        public decimal CarryOnBack { get { return BasicLift * 15; } }
        public decimal ShiftSlightly { get { return BasicLift * 24; } }
    }
}
