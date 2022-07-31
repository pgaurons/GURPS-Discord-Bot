using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    /// <summary>
    /// Tells the penalty due to missing FP for attributes.
    /// </summary>
    public class LongTermFatigueResults
    {
        /// <summary>
        /// Determines if time frames are rounded and if ST penalties are in 10% chunks.
        /// </summary>
        public bool HighDefinition { get; set; } = false;
        /// <summary>
        /// Current FP
        /// </summary>
        public int CurrentFatiguePoints { get; internal set; }
        /// <summary>
        /// Whether this instance reflects penalties to stats or calculated attributes.
        /// </summary>
        public bool Penalty { get; internal set; }
        /// <summary>
        /// The speed of recovery.
        /// </summary>
        public FatigueRecoveryRate RecoveryRate { get; internal set; }
        /// <summary>
        /// Hours of rest for mild fatigue, rounded nicely
        /// </summary>
        public decimal HoursPerFatiguePointMildPresentation
        {
            get
            {
                return RoundIfNecessary(HoursPerFatiguePointMild);
            }
        }

        /// <summary>
        /// Hours of rest for severe fatigue, rounded nicely
        /// </summary>
        public decimal HoursPerFatiguePointSeverePresentation
        {
            get
            {
                return RoundIfNecessary(HoursPerFatiguePointSevere);
            }
        }

        /// <summary>
        /// Hours of rest for deep fatigue, rounded nicely
        /// </summary>
        public decimal HoursPerFatiguePointDeepPresentation
        {
            get
            {
                return RoundIfNecessary(HoursPerFatiguePointDeep);
            }
        }

        private decimal RoundIfNecessary(decimal d)
        {
            return HighDefinition ? d : Math.Round(d * 4, MidpointRounding.ToEven) / 4;
        }
        /// <summary>
        /// How many hours of rest are needed for someone without fit to recover 1 FP.
        /// </summary>
        public decimal HoursPerFatiguePointMild
        {
            get
            {
                return 20m / TotalFatiguePoints;
            }
        }
        /// <summary>
        /// How many hours of rest are needed for someone without fit to recover 1 FP.
        /// </summary>
        public decimal HoursPerFatiguePointSevere
        {
            get
            {
                return 80m / TotalFatiguePoints;
            }
        }
        /// <summary>
        /// How many hours of rest are needed for someone without fit to recover 1 FP.
        /// </summary>
        public decimal HoursPerFatiguePointDeep
        {
            get
            {
                return  240m / TotalFatiguePoints;
            }
        }
        /// <summary>
        /// Cutoff for mild fatigue
        /// </summary>
        public int MildMin { get { return (int)Math.Ceiling(TotalFatiguePoints / 2m); } }
        /// <summary>
        /// The max for Mild fatigue
        /// </summary>
        public int MildMax { get { return TotalFatiguePoints; } }
        /// <summary>
        /// The cutoff for severe fatigue.
        /// </summary>
        public int SevereMin { get; } = 0;
        /// <summary>
        /// The maximum for sever fatigue
        /// </summary>
        public int SevereMax { get { return Math.Max(0, MildMin - 1); } }
        /// <summary>
        /// The max for deep fatigue.
        /// </summary>
        public int DeepMax { get; } = -1;
        /// <summary>
        /// How long to recover mild fatigue.
        /// </summary>
        public decimal TotalRecoveryTimeInHoursMild
        {
            get
            {
                var result = 0m;
                if (CurrentFatiguePoints <= MildMin)
                {
                    result = 10;
                }
                else
                {
                    var fatigueRatio = (decimal)CurrentFatiguePoints / TotalFatiguePoints;
                    var cutoff = MildMax;
                    var currentFatigue = CurrentFatiguePoints;
                    var hoursPerFatiguePoint = HoursPerFatiguePointMild;
                    var partialRecoveryTime = 0m;
                    while (currentFatigue < cutoff)
                    {
                        partialRecoveryTime += hoursPerFatiguePoint;
                        currentFatigue += 1;
                        fatigueRatio = (decimal)currentFatigue / TotalFatiguePoints;
                    }

                    result += RoundIfNecessary(partialRecoveryTime);
                }
              
                

                return result;
            }
        }
        /// <summary>
        /// The sum of FP considered Mild
        /// </summary>
        public int MildFatigueToRecover { get { return TotalFatiguePoints - Math.Max(MildMin, CurrentFatiguePoints); } }
        /// <summary>
        /// The sum of FP considered Severe
        /// </summary>
        public int SevereFatigueToRecover { get { return Math.Max(0, MildMin - Math.Max(SevereMin, CurrentFatiguePoints)); } }

        /// <summary>
        /// The sum of FP considered Deep
        /// </summary>
        public int DeepFatigueToRecover { get { return Math.Max(0, SevereMin - CurrentFatiguePoints); } }

        /// <summary>
        /// How long to recover severe fatigue.
        /// </summary>
        public decimal TotalRecoveryTimeInHoursSevere
        {
            get
            {
                var result = 0m;
                if (CurrentFatiguePoints <= SevereMin)
                {
                    result = 40;
                }
                else if(RecoveryRate == FatigueRecoveryRate.Mild)
                {
                    result = 0;
                }
                else
                {
                    var fatigueRatio = (decimal)CurrentFatiguePoints / TotalFatiguePoints;
                    var cutoff = SevereMax;
                    var currentFatigue = CurrentFatiguePoints;
                    var hoursPerFatiguePoint = HoursPerFatiguePointSevere;
                    var partialRecoveryTime = 0m;
                    while (currentFatigue <= cutoff)
                    {
                        partialRecoveryTime += hoursPerFatiguePoint;
                        currentFatigue += 1;
                        fatigueRatio = (decimal)currentFatigue / TotalFatiguePoints;
                    }

                    result += RoundIfNecessary(partialRecoveryTime);
                }



                return result;
            }
        }

        /// <summary>
        /// How long to recover deep fatigue.
        /// </summary>
        public decimal TotalRecoveryTimeInHoursDeep
        {
            get
            {
                var result = 0m;
                if (RecoveryRate != FatigueRecoveryRate.Deep)
                {
                    result = 0;
                }
                else
                {
                    var fatigueRatio = (decimal)CurrentFatiguePoints / TotalFatiguePoints;
                    var cutoff = DeepMax;
                    var currentFatigue = CurrentFatiguePoints;
                    var hoursPerFatiguePoint = HoursPerFatiguePointDeep;
                    var partialRecoveryTime = 0m;
                    while (currentFatigue <= cutoff)
                    {
                        partialRecoveryTime += hoursPerFatiguePoint;
                        currentFatigue += 1;
                        fatigueRatio = (decimal)currentFatigue / TotalFatiguePoints;
                    }

                    result += RoundIfNecessary(partialRecoveryTime);
                }



                return result;
            }
        }

        /// <summary>
        /// How long to recover all FP.
        /// </summary>
        public decimal TotalRecoveryTimeInHours
        {
            get
            {
                return TotalRecoveryTimeInHoursMild + TotalRecoveryTimeInHoursSevere + TotalRecoveryTimeInHoursDeep;
            }
        }
        /// <summary>
        /// Penalty for attributes besides ST
        /// </summary>
        public int RegularPenalty { get; internal set; }
        /// <summary>
        /// Penalty for ST.
        /// </summary>
        public decimal StrengthPenalty { get; internal set; }
        /// <summary>
        /// Total FP.
        /// </summary>
        public int TotalFatiguePoints { get; internal set; }
        /// <summary>
        /// ST after penalties
        /// </summary>
        public int NewStrength { get { return (int)Math.Floor(OriginalStrength * (1 + StrengthPenalty)); } }
        /// <summary>
        /// DX after penalties
        /// </summary>
        public int NewDexterity { get { return OriginalDexterity + RegularPenalty; } }
        /// <summary>
        /// IQ after penalties.
        /// </summary>
        public int NewIntelligence { get { return OriginalIntelligence + RegularPenalty; } }
        /// <summary>
        /// HT after penalties.
        /// </summary>
        public int NewHealth { get { return OriginalHealth + RegularPenalty; } }
        /// <summary>
        /// ST before penalties
        /// </summary>
        public int OriginalStrength { get; internal set; }
        /// <summary>
        /// DX before penalties
        /// </summary>
        public int OriginalDexterity { get; internal set; }
        /// <summary>
        /// IQ before penalties
        /// </summary>
        public int OriginalIntelligence { get; internal set; }
        /// <summary>
        /// HT before penalties.
        /// </summary>
        public int OriginalHealth { get; internal set; }
    }
}
