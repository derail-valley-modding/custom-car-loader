namespace CCL.Creator.Utility
{
    internal class CargoHelper
    {
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
