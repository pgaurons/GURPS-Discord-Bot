using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// The fields that represent a character
    /// </summary>
    public class Character
    {
        public int SpentPoints { get; set; } = 100;
        public int BasicStrength { get; set; } = 10;
        public int BasicDexterity { get; set; } = 10;
        public int BasicHealth { get; set; } = 10;
        public int BasicIntelligence { get; set; } = 10;
        public int HitPointModification { get; set; } = 0;
        public int MaximumHitPoints { get { return BasicStrength + HitPointModification; } }
        public int FatigueModification { get; set; } = 0;
        public int MaximumFatiguePoints { get { return BasicHealth + FatigueModification; } }
        /// <summary>
        /// Each level represents 0.25 basic speed.
        /// </summary>
        public int BasicSpeedModification { get; set; } = 0;
        public decimal BasicSpeed { get { return (BasicHealth + BasicDexterity + BasicSpeedModification) / 4m; } }
        public int BasicMoveModification { get; set; } = 0;
        public int BasicMove { get { return (int)Math.Floor(BasicSpeed) + BasicMoveModification; } }

        public int WillModification { get; set; } = 0;
        public int Will { get { return BasicIntelligence + WillModification; } }
        public int PerceptionModification { get; set; } = 0;
        public int Perception { get { return BasicIntelligence + PerceptionModification;  } }

        public int UnspentPoints { get; set; } = 0;
    }
}
