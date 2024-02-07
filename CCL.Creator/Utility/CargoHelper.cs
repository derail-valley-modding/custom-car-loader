using CCL.Types;
using System;

namespace CCL.Creator.Utility
{
    internal class CargoHelper
    {
        public static readonly BaseCargoType[] ContainerCargo = new[]
        {
            BaseCargoType.CatFood,

            BaseCargoType.ChemicalsIskar,
            BaseCargoType.ChemicalsSperex,

            BaseCargoType.CannedFood,

            BaseCargoType.ClothingNeoGamma,
            BaseCargoType.ClothingNovae,
            BaseCargoType.ClothingObco,
            BaseCargoType.ClothingTraeg,

            BaseCargoType.ElectronicsAAG,
            BaseCargoType.ElectronicsIskar,
            BaseCargoType.ElectronicsKrugmann,
            BaseCargoType.ElectronicsNovae,
            BaseCargoType.ElectronicsTraeg,

            BaseCargoType.EmptyAAG,
            BaseCargoType.EmptyBrohm,
            BaseCargoType.EmptyChemlek,
            BaseCargoType.EmptyGoorsk,
            BaseCargoType.EmptyIskar,
            BaseCargoType.EmptyKrugmann,
            BaseCargoType.EmptyNeoGamma,
            BaseCargoType.EmptyNovae,
            BaseCargoType.EmptyObco,
            BaseCargoType.EmptySperex,
            BaseCargoType.EmptySunOmni,
            BaseCargoType.EmptyTraeg,

            BaseCargoType.ToolsAAG,
            BaseCargoType.ToolsBrohm,
            BaseCargoType.ToolsIskar,
            BaseCargoType.ToolsNovae,
            BaseCargoType.ToolsTraeg,
        };

        public static BaseCargoType[] GetCargosForType(CarParentType car)
        {
            switch (car)
            {
                case CarParentType.Flatbed:
                    return new[]
                    {
                        // Shared with Boxcar
                        BaseCargoType.CatFood,
                        BaseCargoType.ChemicalsIskar,
                        BaseCargoType.ChemicalsSperex,
                        BaseCargoType.Furniture,

                        BaseCargoType.Acetylene,
                        BaseCargoType.Argon,
                        BaseCargoType.Boards,
                        BaseCargoType.CannedFood,
                        BaseCargoType.ClothingNeoGamma,
                        BaseCargoType.ClothingNovae,
                        BaseCargoType.ClothingObco,
                        BaseCargoType.ClothingTraeg,
                        BaseCargoType.CryoHydrogen,
                        BaseCargoType.CryoOxygen,
                        BaseCargoType.ElectronicsAAG,
                        BaseCargoType.ElectronicsIskar,
                        BaseCargoType.ElectronicsKrugmann,
                        BaseCargoType.ElectronicsNovae,
                        BaseCargoType.ElectronicsTraeg,
                        BaseCargoType.EmptyAAG,
                        BaseCargoType.EmptyBrohm,
                        BaseCargoType.EmptyChemlek,
                        BaseCargoType.EmptyGoorsk,
                        BaseCargoType.EmptyIskar,
                        BaseCargoType.EmptyKrugmann,
                        BaseCargoType.EmptyNeoGamma,
                        BaseCargoType.EmptyNovae,
                        BaseCargoType.EmptyObco,
                        BaseCargoType.EmptySperex,
                        BaseCargoType.EmptySunOmni,
                        BaseCargoType.EmptyTraeg,
                        BaseCargoType.Excavators,
                        BaseCargoType.Nitrogen,
                        BaseCargoType.Plywood,
                        BaseCargoType.SteelBentPlates,
                        BaseCargoType.SteelBillets,
                        BaseCargoType.SteelRails,
                        BaseCargoType.SteelRolls,
                        BaseCargoType.SteelSlabs,
                        BaseCargoType.ToolsAAG,
                        BaseCargoType.ToolsBrohm,
                        BaseCargoType.ToolsIskar,
                        BaseCargoType.ToolsNovae,
                        BaseCargoType.ToolsTraeg,
                        BaseCargoType.Tractors
                    };
                case CarParentType.FlatbedStakes:
                    return new[]
                    {
                        BaseCargoType.Logs,
                        BaseCargoType.Pipes
                    };
                case CarParentType.FlatbedMilitary:
                    return new[]
                    {
                        BaseCargoType.Biohazard,
                        BaseCargoType.MilitarySupplies,
                        BaseCargoType.MilitaryTrucks,
                        BaseCargoType.Tanks
                    };
                case CarParentType.Autorack:
                    return new[]
                    {
                        BaseCargoType.ImportedNewCars,
                        BaseCargoType.NewCars
                    };
                case CarParentType.TankOil:
                    return new[]
                    {
                        BaseCargoType.CrudeOil,
                        BaseCargoType.Diesel,
                        BaseCargoType.Gasoline
                    };
                case CarParentType.TankGas:
                    return new[]
                    {
                        BaseCargoType.Alcohol,
                        BaseCargoType.Methane
                    };
                case CarParentType.TankChem:
                    return new[]
                    {
                        BaseCargoType.Ammonia,
                        BaseCargoType.SodiumHydroxide
                    };
                case CarParentType.TankFood:
                    return new[]
                    {
                        BaseCargoType.Milk
                    };
                case CarParentType.Stock:
                    return new[]
                    {
                        BaseCargoType.Cows,
                        BaseCargoType.Goats,
                        BaseCargoType.Pigs,
                        BaseCargoType.Poultry,
                        BaseCargoType.Sheep
                    };
                case CarParentType.Boxcar:
                    return new[]
                    {
                        BaseCargoType.Bread,
                        BaseCargoType.Cotton,
                        BaseCargoType.Eggs,
                        BaseCargoType.LocalFruits,
                        BaseCargoType.Vegetables,
                        BaseCargoType.Wool,

                        // Shared with Flatbed
                        BaseCargoType.CatFood,
                        BaseCargoType.ChemicalsIskar,
                        BaseCargoType.ChemicalsSperex,
                        BaseCargoType.Furniture
                    };
                case CarParentType.BoxcarMilitary:
                    return new[]
                    {
                        BaseCargoType.Ammunition
                    };
                case CarParentType.Refrigerator:
                    return new[]
                    {
                        BaseCargoType.DairyProducts,
                        BaseCargoType.MeatProducts,
                        BaseCargoType.Medicine
                    };
                case CarParentType.Hopper:
                    return new[]
                    {
                        BaseCargoType.Coal,
                        BaseCargoType.Corn,
                        BaseCargoType.IronOre,
                        BaseCargoType.SunflowerSeeds,
                        BaseCargoType.Wheat
                    };
                case CarParentType.Gondola:
                    return new[]
                    {
                        BaseCargoType.ScrapMetal
                    };
                case CarParentType.Passenger:
                    return new[]
                    {
                        BaseCargoType.Passengers
                    };
                case CarParentType.NuclearFlask:
                    return new[]
                    {
                        BaseCargoType.SpentNuclearFuel
                    };
                default:
                    return new BaseCargoType[] { };
            }
        }

        public static string CleanName(BaseCargoType cargoType)
        {
            string name = Enum.GetName(typeof(BaseCargoType), cargoType);

            return name.Replace("Chemicals", "")
                .Replace("Clothing", "")
                .Replace("Electronics", "")
                .Replace("Empty", "")
                .Replace("Tools", "");
        }
    }
}
