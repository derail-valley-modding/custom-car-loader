using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL_GameScripts
{
    /// <summary>
    /// The underlying type of cars.
    /// </summary>
    public enum BaseTrainCarType
    {
        NotSet = 0,
        LocoShunter = 10,
        LocoSteamHeavy = 20,
        Tender = 21,
        LocoSteamHeavyBlue = 22,
        TenderBlue = 23,
        LocoRailbus = 30,
        LocoDiesel = 40,
        FlatbedEmpty = 200,
        FlatbedStakes = 201,
        FlatbedMilitary = 202,
        AutorackRed = 250,
        AutorackBlue = 251,
        AutorackGreen = 252,
        AutorackYellow = 253,
        TankOrange = 300,
        TankWhite = 301,
        TankYellow = 302,
        TankBlue = 303,
        TankChrome = 304,
        TankBlack = 305,
        BoxcarBrown = 400,
        BoxcarGreen = 401,
        BoxcarPink = 402,
        BoxcarRed = 403,
        BoxcarMilitary = 404,
        RefrigeratorWhite = 450,
        HopperBrown = 500,
        HopperTeal = 501,
        HopperYellow = 502,
        PassengerRed = 600,
        PassengerGreen = 601,
        PassengerBlue = 602,
        HandCar = 700,
        CabooseRed = 750,
        NuclearFlask = 800
    }

    public enum BaseCargoContainerType
    {
        None,
        Hopper,
        TankerOil,
        TankerGas,
        TankerChem,
        Flatcar,
        FlatcarStakes,
        Boxcar,
        Gondola,
        Refrigerator,
        Cars,
        Passengers,
        MilitaryBoxcar,
        NuclearFlask,
        MilitaryFlatcar,

        Custom = 9999
    }

    public enum BaseCargoType
    {
        None,
        Coal = 20,
        IronOre,
        CrudeOil = 40,
        Diesel,
        Gasoline,
        Methane,
        Logs = 60,
        Boards,
        Plywood,
        Wheat = 80,
        Corn,
        Pigs = 100,
        Cows,
        Chickens,
        Sheep,
        Goats,
        Bread = 120,
        DairyProducts,
        MeatProducts,
        CannedFood,
        CatFood,
        SteelRolls = 140,
        SteelBillets,
        SteelSlabs,
        SteelBentPlates,
        SteelRails,
        ScrapMetal = 150,
        ElectronicsIskar = 160,
        ElectronicsKrugmann,
        ElectronicsAAG,
        ElectronicsNovae,
        ElectronicsTraeg,
        ToolsIskar = 180,
        ToolsBrohm,
        ToolsAAG,
        ToolsNovae,
        ToolsTraeg,
        Furniture = 200,
        Pipes,
        ClothingObco = 220,
        ClothingNeoGamma,
        ClothingNovae,
        ClothingTraeg,
        Medicine = 240,
        ChemicalsIskar,
        ChemicalsSperex,
        NewCars = 260,
        ImportedNewCars,
        Tractors,
        Excavators,
        Alcohol = 280,
        Acetylene,
        CryoOxygen,
        CryoHydrogen,
        Argon,
        Nitrogen,
        Ammonia,
        SodiumHydroxide,
        SpentNuclearFuel = 300,
        Ammunition,
        Biohazard,
        Tanks = 320,
        MilitaryTrucks,
        MilitarySupplies,
        EmptySunOmni = 900,
        EmptyIskar,
        EmptyObco,
        EmptyGoorsk,
        EmptyKrugmann,
        EmptyBrohm,
        EmptyAAG,
        EmptySperex,
        EmptyNovae,
        EmptyTraeg,
        EmptyChemlek,
        EmptyNeoGamma,
        Passengers = 1000,

        Custom = 9999
    }

    [Flags]
    public enum BaseJobLicenses
    {
        Basic = 0,
        Hazmat1 = 1,
        Hazmat2 = 2,
        Hazmat3 = 4,
        Military1 = 8,
        Military2 = 16,
        Military3 = 32,
        FreightHaul = 512,
        Shunting = 1024,
        LogisticalHaul = 2048,
        TrainLength1 = 16384,
        TrainLength2 = 32768
    }

    [Flags]
    public enum StationYard
    {
        None = 0,
        SteelMill = 0x1,
        Farm = 0x2,
        FoodFactory = 0x4,
        GoodsFactory = 0x8,
        CitySouthWest = 0x10,
        Harbor = 0x20,
        MachineFactory = 0x40,
        CoalMine = 0x80,
        IronMineEast = 0x100,
        IronMineWest = 0x200,
        ForestCentral = 0x400,
        ForestSouth = 0x800,
        Sawmill = 0x1000,
        OilWellNorth = 0x2000,
        OilWellCentral = 0x4000,
        MilitaryBase = 0x8000,
        MachineMilitary = 0x10000,
        HarborMilitary = 0x20000
    }

    public static class CCLEnumExtensions
    {
        private static readonly (StationYard, string)[] yardNames = new[]
        {
            (StationYard.SteelMill,         "SM"),
            (StationYard.Farm,              "FM"),
            (StationYard.FoodFactory,       "FF"),
            (StationYard.GoodsFactory,      "GF"),
            (StationYard.CitySouthWest,     "CSW"),
            (StationYard.Harbor,            "HB"),
            (StationYard.MachineFactory,    "MF"),
            (StationYard.CoalMine,          "CM"),
            (StationYard.IronMineEast,      "IME"),
            (StationYard.IronMineWest,      "IMW"),
            (StationYard.ForestCentral,     "FRC"),
            (StationYard.ForestSouth,       "FRS"),
            (StationYard.Sawmill,           "SW"),
            (StationYard.OilWellNorth,      "OWN"),
            (StationYard.OilWellCentral,    "OWC"),
            (StationYard.MilitaryBase,      "MB"),
            (StationYard.MachineMilitary,   "MFMB"),
            (StationYard.HarborMilitary,    "HMB"),
        };

        public static IEnumerable<string> YardIds(this StationYard yards)
        {
            if (yards == StationYard.None) yield break;

            foreach ((StationYard yard, string id) in yardNames)
            {
                if (yards.HasFlag(yard))
                {
                    yield return id;
                }
            }

            yield break;
        }
    }
}
