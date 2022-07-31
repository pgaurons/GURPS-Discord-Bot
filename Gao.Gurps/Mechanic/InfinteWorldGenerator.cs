using Gao.Gurps.Dice;
using Gao.Gurps.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gao.Gurps.Mechanic
{
    /// <summary>
    /// Generates Infinite World objects.
    /// </summary>
    public static class InfinteWorldGenerator
    {
        public static InfiniteWorld Generate(bool isHomeline)
        {
            var returnValue = new InfiniteWorld();

            switch(Roller.Roll(1, 6).First())
            {
                case 1: returnValue.Quantum = isHomeline ? 3 : 8; break;
                case 2: returnValue.Quantum = isHomeline ? 4 : 9; break;
                case 3: returnValue.Quantum = isHomeline ? 5 : 10; break;
                case 4:
                case 5: returnValue.Quantum = 6; break;
                default: returnValue.Quantum = 7; break;
            }

            switch(Roller.Roll(1,6).First())
            {
                case 1: returnValue.WorldType = RandomWorldType.Empty; break;
                case 6: returnValue.WorldType = RandomWorldType.Challenge; break;
                default:
                    switch(Roller.Roll(1,3).First())
                    {
                        case 1: returnValue.WorldType = RandomWorldType.Echo; break;
                        default: returnValue.WorldType = RandomWorldType.Parallel; break;
                    }
                    break;

            }
            if(returnValue.WorldType == RandomWorldType.Empty)
            {
                switch(Roller.Roll(1,6).First())
                {
                    case 5: returnValue.EmptyWorldType = EmptyWorldType.HomelineColony; break;
                    case 6: returnValue.EmptyWorldType = EmptyWorldType.DisasterWorld; break;
                    default: returnValue.EmptyWorldType = EmptyWorldType.ResourceExploitation; break;
                }
                if(returnValue.EmptyWorldType == EmptyWorldType.DisasterWorld)
                {
                    switch(Roller.Roll(3,6).Sum())
                    {
                        case 3:
                        case 4: returnValue.DisasterType = DisasterWorldType.Disease; break;
                        case 5:
                        case 6: returnValue.DisasterType = DisasterWorldType.CosmicCatastrophe; break;
                        case 7:
                        case 8: returnValue.DisasterType = DisasterWorldType.EcologicalDisaster; break;
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15: returnValue.DisasterType = DisasterWorldType.GlobalWar; break;
                        default: returnValue.DisasterType = DisasterWorldType.OddDisaster; break;
                    }
                }
            }
            if(returnValue.WorldType == RandomWorldType.Echo)
            {
                var startingYear = 0;
                switch(Roller.Roll(1,6).First())
                {
                    case 1: startingYear = -3400; break;
                    case 2: startingYear = -2500; break;
                    case 3: startingYear = -1600; break;
                    case 4: startingYear = -700; break;
                    case 5: startingYear = 200; break;
                    default: startingYear = 1100; break;
                }
                startingYear += ((Roller.Roll(1, 6).First() - 1) * 150);
                startingYear += ((Roller.Roll(1, 6).First() - 1) * 25);
                startingYear += ((Roller.Roll(1, 6).First() - 1) * 4);
                startingYear += ((Roller.Roll(1, 6).First() - 1) *1);
                returnValue.EchoWorldPresent = startingYear;
            }
            if(returnValue.WorldType == RandomWorldType.Parallel)
            {
                switch(Roller.Roll(2,6).Sum())
                {
                    case 2:
                    case 3:
                    case 4: returnValue.ParallelWorldType = ParallelWorldType.Close; break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10: returnValue.ParallelWorldType = ParallelWorldType.Farther; break;
                    case 11: returnValue.ParallelWorldType = ParallelWorldType.HighInertia; break;
                    default: returnValue.ParallelWorldType = ParallelWorldType.Myth; break;
                }
                if(returnValue.ParallelWorldType == ParallelWorldType.Close)
                {
                    switch(Roller.Roll(1,6).First())
                    {
                        case 1: returnValue.CloseParallelWorldType = CloseParallelWorldType.PopularCulture; break;
                        case 2: returnValue.CloseParallelWorldType = CloseParallelWorldType.HighCulture; break;
                        case 3: returnValue.CloseParallelWorldType = CloseParallelWorldType.EconomicOrCorporate; break;
                        case 4: returnValue.CloseParallelWorldType = CloseParallelWorldType.Technological; break;
                        case 5: returnValue.CloseParallelWorldType = CloseParallelWorldType.Political; break;
                        default: returnValue.CloseParallelWorldType = CloseParallelWorldType.Geopolitical; break;
                    }
                }
                if(returnValue.ParallelWorldType == ParallelWorldType.Farther)
                {
                    GenerateFartherParallelValues(returnValue);
                }
            }
            return returnValue;
        }

        private static void GenerateFartherParallelValues(InfiniteWorld returnValue)
        {
            switch (Roller.Roll().Sum())
            {
                case 3: returnValue.TechLevel = (Roller.Roll(1, 3).First()); break;
                case 4:
                case 5: returnValue.TechLevel = 4; break;
                case 6: returnValue.TechLevel = 5; break;
                case 7:
                case 8:
                case 9: returnValue.TechLevel = 6; break;
                case 10:
                case 11:
                case 12:
                case 13:
                case 14: returnValue.TechLevel = 7; break;
                case 15:
                case 16: returnValue.TechLevel = 8; break;
                default: returnValue.TechLevel = 9; break;
            }
            switch (Math.Min(Math.Max(3, Roller.Roll(2, 6).Sum() + returnValue.TechLevel - 5), 10))
            {
                case 3: returnValue.TechnologyVariant = TechnologyVariant.Magic; break;
                case 4:
                case 5: returnValue.TechnologyVariant = TechnologyVariant.DivergentTechnology; break;
                case 6:
                case 7: returnValue.TechnologyVariant = TechnologyVariant.SplitTechnology; break;
                case 8: returnValue.TechnologyVariant = TechnologyVariant.Psionics; break;
                case 9: returnValue.TechnologyVariant = TechnologyVariant.Supers; break;
                default: returnValue.TechnologyVariant = TechnologyVariant.Superscience; break;
            }
            if (new[]
            {
                        TechnologyVariant.Magic,
                        TechnologyVariant.Psionics,
                        TechnologyVariant.Supers
                    }.Contains(returnValue.TechnologyVariant))
            {
                switch (Roller.Roll(1, 6).First())
                {
                    case 1: returnValue.TechnologyVariantOpenness = TechnologyVariantOpenness.Covert; break;
                    case 2:
                    case 3: returnValue.TechnologyVariantOpenness = TechnologyVariantOpenness.OpenAndRare; break;
                    case 4: returnValue.TechnologyVariantOpenness = TechnologyVariantOpenness.OpenAndCommon; break;
                    case 5: returnValue.TechnologyVariantOpenness = TechnologyVariantOpenness.Dominant; break;
                    default: returnValue.TechnologyVariantOpenness = TechnologyVariantOpenness.Universal; break;
                }
            }
            if (returnValue.TechnologyVariant == TechnologyVariant.Magic)
            {
                switch (Roller.Roll(1, 6).First())
                {
                    case 1: returnValue.ManaLevel = ManaLevel.Low; break;
                    case 2:
                    case 3:
                    case 4: returnValue.ManaLevel = ManaLevel.Normal; break;
                    case 5: returnValue.ManaLevel = ManaLevel.High; break;
                    default: returnValue.ManaLevel = ManaLevel.VeryHigh; break;
                }
            }
            if (returnValue.TechnologyVariant == TechnologyVariant.DivergentTechnology)
            {
                var split = 0;
                var modifier = returnValue.TechLevel - 7;
                do
                {
                    split = Roller.Roll(1, 6).First() + modifier;
                } while (split <= 0);
                returnValue.TechLevelDivergencePoint = split;
            }
            if (returnValue.TechnologyVariant == TechnologyVariant.SplitTechnology)
            {
                var totalIterations = Roller.Roll(1, 3).First();
                for (var i = 0; i < totalIterations; i++)
                {
                    TechnologyCategory techCategory;
                    switch (Roller.Roll(1, 6).First())
                    {
                        case 1: techCategory = TechnologyCategory.Communications; break;
                        case 2: techCategory = TechnologyCategory.Transportation; break;
                        case 3:
                        case 4: techCategory = TechnologyCategory.Weapons; break;
                        case 5: techCategory = TechnologyCategory.Power; break;
                        default: techCategory = TechnologyCategory.Medicine; break;
                    }
                    if (!returnValue.SplitTechLevels.ContainsKey(techCategory))
                    {
                        returnValue.SplitTechLevels.Add(techCategory, Math.Min(12, Math.Max(0, returnValue.TechLevel + (Roller.Roll(1, 6).First() * Roller.ChooseOne(-1,1)))));
                    }
                    else
                    {
                        returnValue.HasAirships = true;
                    }
                }
            }
            if (returnValue.TechnologyVariant == TechnologyVariant.Superscience)
            {
                switch (Roller.Roll(1, 6).First())
                {
                    case 1:
                    case 2: returnValue.SuperscienceTechnology = SuperscienceTechnology.Antigravity; break;
                    case 3: returnValue.SuperscienceTechnology = SuperscienceTechnology.ForceFields; break;
                    case 4: returnValue.SuperscienceTechnology = SuperscienceTechnology.RayGuns; break;
                    case 5: returnValue.SuperscienceTechnology = SuperscienceTechnology.ZeroPointEnergy; break;
                    default: returnValue.SuperscienceTechnology = SuperscienceTechnology.MartianCanalsAndReactionlessDrives; break;
                }
                returnValue.TechnologyFromAncientAstronauts = Roller.Roll(1, 2).First() == 2;
            }
            var civilizationRollModifier = 0;
            switch(returnValue.TechLevel)
            {
                case 2: civilizationRollModifier = 3; break;
                case 3:
                case 4: civilizationRollModifier = 2; break;
                case 7:
                case 8:
                case 9: civilizationRollModifier = -2; break;
            }
            var numberOfCivilizations = Math.Max(1,Roller.Roll(1, 6).First() + civilizationRollModifier);
            var civilizations = new List<InfiniteWorldCivilization>();
            foreach(var i in Enumerable.Range(1, numberOfCivilizations))
            {
                InfiniteWorldCivilization newCivilization;
                do
                {
                    newCivilization = GenerateMajorCivilization(returnValue.TechLevel);
                } while (civilizations.Any(c => c.Civilization == newCivilization.Civilization));
                civilizations.Add(newCivilization);
            }
            returnValue.MajorCivilizations = civilizations.ToArray();
        }

        private static InfiniteWorldCivilization GenerateMajorCivilization(int techLevel)
        {
            var returnValue = new InfiniteWorldCivilization();
            returnValue.Civilization = SelectCivilization(techLevel);
            int unityModifier;
            switch(returnValue.Civilization)
            {
                case Civilization.Japanese:
                case Civilization.Andean:
                case Civilization.AndeanAndMesoAmerican:
                case Civilization.MississippianAndeanAndMesoAmerican:
                case Civilization.MississippianAndAndean: unityModifier = -4; break;
                case Civilization.Egyptian:
                case Civilization.Orthodox: unityModifier = -2; break;
                case Civilization.Roman:
                case Civilization.Chinese: unityModifier = -1; break;
                case Civilization.Islamic: unityModifier = 1; break;
                case Civilization.Western:
                case Civilization.Bactrian:
                case Civilization.Hellenic:
                case Civilization.Indic: unityModifier = 2; break;
                case Civilization.Celtic:
                case Civilization.Norse: unityModifier = 4; break;
                default: unityModifier = 0; break;
            }
            var unityRoll = Roller.Roll(2, 6).Sum() + unityModifier;
            switch (Math.Min(12, Math.Max(2, unityRoll)))
            {
                case 2: returnValue.Unity = CivilizationUnity.Unitary; break;
                case 3:
                case 4: returnValue.Unity = CivilizationUnity.Empire; break;
                case 5:
                case 6: returnValue.Unity = CivilizationUnity.EmpireWithSatelliteStates; break;
                case 7: returnValue.Unity = CivilizationUnity.EmpireWithRivals; break;
                case 8:
                    returnValue.Unity = CivilizationUnity.Bipolar;
                    returnValue.NumberOfPowers = 2;
                    break;
                case 9:
                case 10:
                case 11:
                    returnValue.Unity = CivilizationUnity.Multipolar;
                    returnValue.NumberOfPowers = Roller.Roll(1, 6).First() + 2;
                    break;
                default: returnValue.Unity = CivilizationUnity.Diffuse; break;
            }
            var isFragmented =
                returnValue.Unity != CivilizationUnity.Diffuse &&
                (Roller.Roll(1, 6).First() + (returnValue.Civilization == Civilization.Steppe ? 2 : 0) >= 6);
            returnValue.Fragmenting = isFragmented;
            var governments = new List<Government>();
            foreach(var i in Enumerable.Range(1, returnValue.NumberOfPowers))
            {
                switch(Roller.Roll(3,6).Sum())
                {
                    case 3: governments.Add(Government.Anarchy); break;
                    case 4: governments.Add(Government.Technocracy); break;
                    case 5: governments.Add(Government.Caste); break;
                    case 6:
                    case 7:
                    case 8: governments.Add(Government.Feudal); break;
                    case 9:
                    case 10:
                    case 11: governments.Add(Government.Dictatorship); break;
                    case 12: governments.Add(Government.Oligarchy); break;
                    case 13: governments.Add(Government.RepresentativeDemocracy); break;
                    case 14:
                    case 15: governments.Add(Government.ClanTribal); break;
                    case 16: governments.Add(Government.Theocracy); break;
                    case 17: governments.Add(Government.CorporateState); break;
                    default: governments.Add(Government.AthenianDemocracy); break;
                }
            }
            returnValue.PowerGovernments = governments.ToArray();

            return returnValue;
        }

        private static Civilization SelectCivilization(int techLevel)
        {
            var roll = Roller.Roll(3, 6).Sum();
            Civilization returnValue;
            switch (techLevel)
            {
                case 0:
                case 1:
                    switch (roll)
                    {
                        case 3: returnValue = Civilization.Andean; break;
                        case 4:
                        case 5: returnValue = Civilization.Hittite; break;
                        case 6:
                        case 7:
                        case 8: returnValue = Civilization.Chinese; break;
                        case 9:
                        case 10: returnValue = Civilization.Egyptian; break;
                        case 11:
                        case 12:
                        case 13: returnValue = Civilization.Mesopotamian; break;
                        case 14: returnValue = Civilization.IndusValley; break;
                        case 15:
                        case 16: returnValue = Civilization.Minoan; break;
                        case 17: returnValue = Civilization.MesoAmerican; break;
                        default: returnValue = SelectCivilization(2); break; //Choose from the next TL group instead.

                    }
                    break;
                case 2:
                    switch(roll)
                    {
                        case 3: returnValue = Civilization.Mississippian; break;
                        case 4: returnValue = Roller.ChooseOne(Civilization.Andean, Civilization.MesoAmerican, Civilization.AndeanAndMesoAmerican); break;
                        case 5: returnValue = Civilization.Celtic; break;
                        case 6: returnValue = Civilization.Bactrian; break;
                        case 7: returnValue = Civilization.Indic; break;
                        case 8: returnValue = Civilization.Iranic; break;
                        case 9: returnValue = Civilization.Hellenic; break;
                        case 10:
                        case 11: returnValue = Civilization.Chinese; break;
                        case 12:
                        case 13: returnValue = Civilization.Roman; break;
                        case 14: returnValue = Civilization.Steppe; break;
                        case 15: returnValue = Civilization.Mesopotamian; break;
                        case 16: returnValue = Civilization.WestAfrican; break;
                        default: returnValue = SelectCivilization(0); break; //select from previous TL instead.
                    }
                    break;
                case 3:
                case 4:
                    switch(roll)
                    {
                        case 3: returnValue = Roller.ChooseOne(Civilization.Mississippian, Civilization.Andean, Civilization.MesoAmerican, Civilization.AndeanAndMesoAmerican, Civilization.MississippianAndAndean, Civilization.MississippianAndMesoAmerican, Civilization.MississippianAndeanAndMesoAmerican); break;
                        case 4: returnValue = Civilization.Malay; break;
                        case 5: returnValue = Civilization.Japanese; break;
                        case 6: returnValue = Civilization.Norse; break;
                        case 7: returnValue = Civilization.Indic; break;
                        case 8: returnValue = Civilization.Orthodox; break;
                        case 9:
                        case 10: returnValue = Civilization.Chinese; break;
                        case 11: returnValue = Civilization.Islamic; break;
                        case 12:
                        case 13: returnValue = Civilization.Western; break;
                        case 14: returnValue = Civilization.Steppe; break;
                        case 15: returnValue = Civilization.Roman; break;
                        case 16:
                        case 17: returnValue = SelectCivilization(2); break; //select from previous TL instead.
                        default: returnValue = Civilization.WestAfrican; break;
                    }
                    break;
                case 5:
                case 6:
                    switch(roll)
                    {
                        case 3:
                        case 4: returnValue = Roller.ChooseOne(Civilization.Malay, Civilization.WestAfrican); break;
                        case 5: returnValue = Civilization.Japanese; break;
                        case 6:
                        case 7: returnValue = Civilization.Indic; break;
                        case 8: 
                        case 9: returnValue = Civilization.Chinese; break;
                        case 10: 
                        case 11: 
                        case 12: returnValue = Civilization.Western; break;
                        case 13: returnValue = Civilization.Islamic; break;
                        case 14: 
                        case 15: returnValue = Civilization.Orthodox; break;
                        default: returnValue = SelectCivilization(3); break; //Previous TL group.
                    }
                    break;
                default:
                    switch(roll)
                    {
                        case 3:
                        case 4: returnValue = Roller.ChooseOne(Civilization.Malay, Civilization.WestAfrican); break;
                        case 5:
                        case 6:
                        case 7: returnValue = Civilization.Indic; break;
                        case 8:
                        case 9: returnValue = Roller.ChooseOne(Civilization.Chinese, Civilization.Japanese); break;
                        case 10:
                        case 11:
                        case 12: returnValue = Civilization.Western; break;
                        case 13: returnValue = Civilization.Orthodox; break;
                        case 14:
                        case 15:
                        case 16:
                        case 17: returnValue = Civilization.Islamic; break;
                        default: returnValue = SelectCivilization(3); break;
                    }
                    break;
            }
            return returnValue;
        }
    }
}
