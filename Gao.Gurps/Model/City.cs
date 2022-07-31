using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gao.Gurps.Model
{
    public class City
    {
        public int Population { get; set; }
        public int SearchModifier
        {
            get
            {
                return LookupTables.GetSearchModifier(Population);
            }
        }
        public TerrainType Terrain { get; set; }
        public AppearanceLevel Appearance { get; set; }
        public int Hygiene { get; set; }
        public int SuggestedHygiene
        {
            get
            {
                var lowTechModifier = 
                        TechnologyLevel < 6 ?   -1 :
                        TechnologyLevel > 8 ?   1 :
                                                0;
                var environmentModifier =
                    new[] { TerrainType.Arctic, TerrainType.Desert, TerrainType.Mountain }.Contains(Terrain) ? 1 :
                    new[] { TerrainType.Jungle, TerrainType.Swampland }.Contains(Terrain) ? -1 :
                    0;
                var publicHealthModifier = ControlRating >= 4 ? 1 : 0;
                var wealthModifier = new[] { WealthLevel.DeadBroke, WealthLevel.Poor }.Contains(Wealth) ? -1 : 0;

                return lowTechModifier + environmentModifier + publicHealthModifier + wealthModifier;
            }
        }
        public ManaLevel Mana { get; set; }
        public EnchantmentLevel Enchantment { get; set; }
        public string[] Languages { get; set; }
        public LiteracyLevel Literacy { get; set; }
        public int TechnologyLevel { get; set; }
        public WealthLevel Wealth { get; set; }
        public int Status { get; set; }
        public Government[] GovernmentConditions { get; set; }
        public int ControlRating { get; set; }
        public int Corruption { get; set; }
        public int DefenseBonus { get; set; }

        public decimal MilitaryResources
        {
            get
            {
                var monthlyIncome = LookupTables.GetTypicalMonthlyPay(TechnologyLevel) * LookupTables.GetMonthlyPayMultiplier(Wealth);
                var militaryBudgetFactor = LookupTables.GetMilitaryBudgetFactor(ControlRating, false);
                return Population * monthlyIncome * militaryBudgetFactor;
            }
        }
    }
}
