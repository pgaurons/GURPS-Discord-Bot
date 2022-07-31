using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Party one is the slammer, party two is the slamee.
    /// </summary>
    public class SlamResult
    {
        public int Velocity { get; set; }
        public int PartyOneHitPoints { get; set; }
        public int PartyTwoHitPoints { get; set; }

        public ParsedDiceAdds PartyOneDamage { get; set; }
        public ParsedDiceAdds PartyTwoDamage { get; set; }

        public int PartyOneDamageResult { get; set; }
        public int PartyTwoDamageResult { get; set; }

        public bool PartyOneRollsForKnockdown { get { return PartyTwoDamageResult >= PartyOneDamageResult && PartyOneDamageResult != 0; } }
        public bool PartyOneAutomaticallyKnockedDown { get { return PartyTwoDamageResult >= (PartyOneDamageResult * 2) && PartyTwoDamageResult != 0; } }
        public bool PartyTwoRollsForKnockdown { get { return PartyOneDamageResult >= PartyTwoDamageResult && PartyOneDamageResult != 0; } }
        public bool PartyTwoAutomaticallyKnockedDown { get { return PartyOneDamageResult >= (PartyTwoDamageResult * 2) && PartyOneDamageResult != 0; } }

    }
}
