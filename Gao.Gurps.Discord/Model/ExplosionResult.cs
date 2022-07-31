using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Discord.Model
{
    public class ExplosionResult
    {
        public int Distance { get; internal set; }
        public int ExplosionDamageEvaluated { get; internal set; }
        public string ExplosionDamageParsed { get; internal set; }
        public int ExplosionLevel { get; internal set; }
        public bool InExplosionRange { get; internal set; }
        public bool InShrapnelRange { get; internal set; }
        public string ShrapnelDamageParsed { get; internal set; }
        public int ShrapnelRoll { get; internal set; }
        internal IEnumerable<HitResult> ShrapnelHits { get; set; } = new HitResult[0];
    }
}
