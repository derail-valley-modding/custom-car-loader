using CCL.Types;
using System;

namespace CCL.Creator.Utility
{
    internal class CargoHelper
    {
        public static readonly BaseCargoType[] ContainerCargoEnums = new[]
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

        public static BaseCargoType[] GetCargosForType(CarCargoSet car)
        {
            switch (car)
            {
                case CarCargoSet.Flatbed:
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
                case CarCargoSet.FlatbedStakes:
                    return new[]
                    {
                        BaseCargoType.Logs,
                        BaseCargoType.Pipes
                    };
                case CarCargoSet.FlatbedMilitary:
                    return new[]
                    {
                        BaseCargoType.Biohazard,
                        BaseCargoType.MilitarySupplies,
                        BaseCargoType.MilitaryTrucks,
                        BaseCargoType.Tanks
                    };
                case CarCargoSet.Autorack:
                    return new[]
                    {
                        BaseCargoType.ImportedNewCars,
                        BaseCargoType.NewCars
                    };
                case CarCargoSet.TankOil:
                    return new[]
                    {
                        BaseCargoType.CrudeOil,
                        BaseCargoType.Diesel,
                        BaseCargoType.Gasoline
                    };
                case CarCargoSet.TankGas:
                    return new[]
                    {
                        BaseCargoType.Alcohol,
                        BaseCargoType.Methane
                    };
                case CarCargoSet.TankChem:
                    return new[]
                    {
                        BaseCargoType.Ammonia,
                        BaseCargoType.SodiumHydroxide
                    };
                case CarCargoSet.TankFood:
                    return new[]
                    {
                        BaseCargoType.Milk
                    };
                case CarCargoSet.Stock:
                    return new[]
                    {
                        BaseCargoType.Cows,
                        BaseCargoType.Goats,
                        BaseCargoType.Pigs,
                        BaseCargoType.Poultry,
                        BaseCargoType.Sheep
                    };
                case CarCargoSet.Boxcar:
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
                case CarCargoSet.BoxcarMilitary:
                    return new[]
                    {
                        BaseCargoType.Ammunition
                    };
                case CarCargoSet.Refrigerator:
                    return new[]
                    {
                        BaseCargoType.DairyProducts,
                        BaseCargoType.MeatProducts,
                        BaseCargoType.Medicine
                    };
                case CarCargoSet.Hopper:
                    return new[]
                    {
                        BaseCargoType.Coal,
                        BaseCargoType.Corn,
                        BaseCargoType.IronOre,
                        BaseCargoType.SunflowerSeeds,
                        BaseCargoType.Wheat
                    };
                case CarCargoSet.Gondola:
                    return new[]
                    {
                        BaseCargoType.ScrapMetal
                    };
                case CarCargoSet.Passenger:
                    return new[]
                    {
                        BaseCargoType.Passengers
                    };
                case CarCargoSet.NuclearFlask:
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

        public static readonly string[] ContainerCargo = new[]
        {
            "CannedFood",
            "CatFood",

            "ChemicalsIskar",
            "ChemicalsSperex",

            "ClothingNeoGamma",
            "ClothingNovae",
            "ClothingObco",
            "ClothingTraeg",

            "DairyProducts",

            "ElectronicsAAG",
            "ElectronicsIskar",
            "ElectronicsKrugmann",
            "ElectronicsNovae",
            "ElectronicsTraeg",

            "EmptyAAG",
            "EmptyBrohm",
            "EmptyChemlek",
            "EmptyGoorsk",
            "EmptyIskar",
            "EmptyKrugmann",
            "EmptyNeoGamma",
            "EmptyNovae",
            "EmptyObco",
            "EmptySperex",
            "EmptySunOmni",
            "EmptyTraeg",

            "Furniture",

            "MeatProducts",

            "Medicine",

            "ScrapContainers",

            "ToolsAAG",
            "ToolsBrohm",
            "ToolsIskar",
            "ToolsNovae",
            "ToolsTraeg",

            "TropicalFruits"
        };

        public static string[] CarToCargo(CarCargoSet car) => car switch
        {
            CarCargoSet.Flatbed => new[]
                {
                    "Acetylene",
                    "AmmoniumNitrate",
                    "Argon",
                    "Boards",
                    "CannedFood",
                    "CatFood",
                    "ChemicalsIskar",
                    "ChemicalsSperex",
                    "CityBuses",
                    "ClothingNeoGamma",
                    "ClothingNovae",
                    "ClothingObco",
                    "ClothingTraeg",
                    "CraneParts",
                    "CryoHydrogen",
                    "CryoOxygen",
                    "DairyProducts",
                    "ElectronicsAAG",
                    "ElectronicsIskar",
                    "ElectronicsKrugmann",
                    "ElectronicsNovae",
                    "ElectronicsTraeg",
                    "EmptyAAG",
                    "EmptyBrohm",
                    "EmptyChemlek",
                    "EmptyGoorsk",
                    "EmptyIskar",
                    "EmptyKrugmann",
                    "EmptyNeoGamma",
                    "EmptyNovae",
                    "EmptyObco",
                    "EmptySperex",
                    "EmptySunOmni",
                    "EmptyTraeg",
                    "Excavators",
                    "ForestryTrailers",
                    "Furniture",
                    "MeatProducts",
                    "Medicine",
                    "MiningTrucks",
                    "Nitrogen",
                    "Plywood",
                    "ScrapContainers",
                    "SemiTrailers",
                    "SteelBentPlates",
                    "SteelBillets",
                    "SteelRails",
                    "SteelRolls",
                    "SteelSlabs",
                    "ToolsAAG",
                    "ToolsBrohm",
                    "ToolsIskar",
                    "ToolsNovae",
                    "ToolsTraeg",
                    "Tractors",
                    "Trams",
                    "TropicalFruits"
                },
            CarCargoSet.FlatbedStakes => new[]
                {
                    "Logs",
                    "Pipes"
                },
            CarCargoSet.FlatbedMilitary => new[]
                {
                    "AttackHelicopsters",
                    "Biohazard",
                    "MilitaryCars",
                    "MilitarySupplies",
                    "MilitaryTrucks",
                    "Missiles",
                    "Tanks"
                },
            CarCargoSet.Autorack => new[]
                {
                    "ImportedNewCars",
                    "NewCars"
                },
            CarCargoSet.TankOil => new[]
                {
                    "CrudeOil",
                    "Diesel",
                    "Gasoline"
                },
            CarCargoSet.TankGas => new[]
                {
                    "Alcohol",
                    "Methane"
                },
            CarCargoSet.TankChem => new[]
                {
                    "Ammonia",
                    "SodiumHydroxide"
                },
            CarCargoSet.TankFood => new[]
                {
                    "Milk"
                },
            CarCargoSet.Stock => new[]
                {
                    "Cows",
                    "Goats",
                    "Pigs",
                    "Poultry",
                    "Sheep"
                },
            CarCargoSet.Boxcar => new[]
                {
                    "Bread",
                    "CatFood",
                    "ChemicalsIskar",
                    "ChemicalsSperex",
                    "Cotton",
                    "CraneParts",
                    "Furniture",
                    "ToolsIskar",
                    "Vegetables",
                    "Wool"
                },
            CarCargoSet.BoxcarMilitary => new[]
                {
                    "Ammunition"
                },
            CarCargoSet.Refrigerator => new[]
                {
                    "DairyProducts",
                    "Eggs",
                    "Fish",
                    "MeatProducts",
                    "Medicine",
                    "TemperateFruits",
                    "TropicalFruits"
                },
            CarCargoSet.Hopper => new[]
                {
                    "Coal",
                    "IronOre",
                    "WoodChips"
                },
            CarCargoSet.HopperCovered => new[]
                {
                    "Corn",
                    "Flour",
                    "SunflowerSeeds",
                    "Wheat"
                },
            CarCargoSet.Gondola => new[]
                {
                    "RailwaySleepers",
                    "ScrapMetal",
                    "WoodScrap"
                },
            CarCargoSet.Passenger => new[]
                {
                    "Passengers"
                },
            CarCargoSet.NuclearFlask => new[]
                {
                    "SpentNuclearFuel"
                },
            _ => new string[0],
        };

        public static string CleanName(string cargoId)
        {
            return cargoId.Replace("Chemicals", "")
                .Replace("Clothing", "")
                .Replace("Electronics", "")
                .Replace("Empty", "")
                .Replace("Tools", "");
        }
    }
}
