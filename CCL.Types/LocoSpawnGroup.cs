using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types
{
    [Serializable]
    public class LocoSpawnGroup
    {
        public static Dictionary<SpawnTrack, string> TrackToSpawnerName = new Dictionary<SpawnTrack, string>()
        {
            { SpawnTrack.CoalMineEast, "LocoSpawnerCME" },
            { SpawnTrack.CoalMineSouth, "LocoSpawnerCMS-B-P1" },
            { SpawnTrack.CoalPowerPlant, "LocoSpawnerCP-P1" },
            { SpawnTrack.CitySouth, "LocoSpawnerCS" },
            { SpawnTrack.CityWest, "LocoSpawnerCW1" },
            { SpawnTrack.FoodFactory1, "LocoSpawnerFF1" },
            { SpawnTrack.FoodFactory2, "LocoSpawnerFF2" },
            { SpawnTrack.Farm, "LocoSpawnerFM" },
            { SpawnTrack.ForestCentral, "LocoSpawnerFRC" },
            { SpawnTrack.ForestSouth, "LocoSpawnerFRS" },
            { SpawnTrack.GoodsFactory1, "LocoSpawnerGF1" },
            { SpawnTrack.GoodsFactory2, "LocoSpawnerGF2" },
            { SpawnTrack.HarbourA1, "LocoSpawnerHB-A-1" },
            { SpawnTrack.HarbourRoundhouse1, "LocoSpawnerHB-T1-1" },
            { SpawnTrack.HarbourRoundhouse2, "LocoSpawnerHB-T1-2" },
            { SpawnTrack.HarbourRoundhouse5, "LocoSpawnerHB-T1-5" },
            { SpawnTrack.HarbourRoundhouse7, "LocoSpawnerHB-T1-7" },
            { SpawnTrack.IronMineEast, "LocoSpawnerIME" },
            { SpawnTrack.IronMineWest, "LocoSpawnerIMW" },
            { SpawnTrack.MachineFactoryA1, "LocoSpawnerMF-A-1" },
            { SpawnTrack.MachineFactoryRoundhouse2, "LocoSpawnerMF-T1-2" },
            { SpawnTrack.MachineFactoryRoundhouse3, "LocoSpawnerMF-T1-3" },
            { SpawnTrack.MachineFactoryRoundhouse4, "LocoSpawnerMF-T1-4" },
            { SpawnTrack.MachineFactoryRoundhouse7, "LocoSpawnerMF-T1-7" },
            { SpawnTrack.OilRefinery, "LocoSpawnerOR-B-P1" },
            { SpawnTrack.OilWellCentral, "LocoSpawnerOWC-A-3" },
            { SpawnTrack.OilWellNorth, "LocoSpawnerOWN" },
            { SpawnTrack.Sawmill, "LocoSpawnerSW" },
            { SpawnTrack.SteelMillTurntable1, "LocoSpawnerSM_T1-1" },
            { SpawnTrack.SteelMillTurntable2, "LocoSpawnerSM_T1-2" },
        };
        public static Dictionary<string, string> SpawnerNameToId = new Dictionary<string, string>
        {
            { "LocoSpawnerCME", "CME" },
            { "LocoSpawnerCMS-B-P1", "CMS" },
            { "LocoSpawnerCP-P1", "CP" },
            { "LocoSpawnerCS" , "CS" },
            { "LocoSpawnerCW1" , "CW" },
            { "LocoSpawnerFF1" , "FF" },
            { "LocoSpawnerFF2" , "FF" },
            { "LocoSpawnerFM" , "FM" },
            { "LocoSpawnerFRC" , "FRC" },
            { "LocoSpawnerFRS" , "FRS" },
            { "LocoSpawnerGF1" , "GF" },
            { "LocoSpawnerGF2" , "GF" },
            { "LocoSpawnerHB-A-1" , "HB" },
            { "LocoSpawnerHB-T1-1" , "HB" },
            { "LocoSpawnerHB-T1-2" , "HB" },
            { "LocoSpawnerHB-T1-5" , "HB" },
            { "LocoSpawnerHB-T1-7" , "HB" },
            { "LocoSpawnerIME" , "IME" },
            { "LocoSpawnerIMW" , "IMW" },
            { "LocoSpawnerMF-A-1" , "MF" },
            { "LocoSpawnerMF-T1-2" , "MF" },
            { "LocoSpawnerMF-T1-3" , "MF" },
            { "LocoSpawnerMF-T1-4" , "MF" },
            { "LocoSpawnerMF-T1-7" , "MF" },
            { "LocoSpawnerOR-B-P1" , "OR" },
            { "LocoSpawnerOWC-A-3" , "OWC" },
            { "LocoSpawnerOWN" , "OWN" },
            { "LocoSpawnerSW" , "SW" },
            { "LocoSpawnerSM_T1-1" , "SM" },
            { "LocoSpawnerSM_T1-2" , "SM" },
        };

        [Tooltip("The track where the loco(s) will spawn")]
        public SpawnTrack Track;
        [Tooltip("Extra locos/tenders to spawn together (in order)\n" +
            "Livery is automatically added at the front unless it is present in this array")]
        public string[] AdditionalLiveries;

        public LocoSpawnGroup() : this(SpawnTrack.MachineFactoryA1, new string[0]) { }

        public LocoSpawnGroup(SpawnTrack track, string[] liveries)
        {
            Track = track;
            AdditionalLiveries = liveries;
        }

        public bool IsDE2ExclusiveSpawn()
        {
            switch (Track)
            {
                case SpawnTrack.FoodFactory1:
                case SpawnTrack.GoodsFactory1:
                case SpawnTrack.HarbourA1:
                case SpawnTrack.MachineFactoryRoundhouse3:
                case SpawnTrack.SteelMillTurntable1:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsDisallowedSpawn() => Track switch
        {
            SpawnTrack.SteelMillTurntable1 => true,
            _ => false,
        };
    }

    public enum SpawnTrack
    {
        [Tooltip("Length: 110m")]
        CitySouth = 0,
        [Tooltip("Length: 155m")]
        CityWest = 100,
        [Tooltip("Length: 50m")]
        CoalMineEast = 200,
        [Tooltip("Length: 170m")]
        CoalMineSouth = 300,
        [Tooltip("Length: 270m")]
        CoalPowerPlant = 400,
        [Tooltip("Length: 300m")]
        Farm = 500,
        [Tooltip("Length: 50m\nDE2 exclusive")]
        FoodFactory1 = 600,
        [Tooltip("Length: 55m")]
        FoodFactory2 = 601,
        [Tooltip("Length: 60m")]
        ForestCentral = 700,
        [Tooltip("Length: 62m")]
        ForestSouth = 800,
        [Tooltip("Length: 60m\nDE2 exclusive")]
        GoodsFactory1 = 900,
        [Tooltip("Length: 60m")]
        GoodsFactory2 = 901,
        [Tooltip("Length: 65m\nDE2 exclusive")]
        HarbourA1 = 1000,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse1 = 1001,
        [Tooltip("Length: 45m (has turntable)\nS282 exclusive")]
        HarbourRoundhouse2 = 1002,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse5 = 1005,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse7 = 1007,
        [Tooltip("Length: 45m")]
        IronMineEast = 1100,
        [Tooltip("Length: 25m (has turntable)\nS282 exclusive")]
        IronMineWest = 1200,
        [Tooltip("Length: 140m")]
        MachineFactoryA1 = 1300,
        [Tooltip("Length: 45m (has turntable)")]
        MachineFactoryRoundhouse2 = 1302,
        [Tooltip("Length: 45m (has turntable)\nDE2 exclusive")]
        MachineFactoryRoundhouse3 = 1303,
        [Tooltip("Length: 45m (has turntable)")]
        MachineFactoryRoundhouse4 = 1304,
        [Tooltip("Length: 45m (has turntable)\nDE6 exclusive")]
        MachineFactoryRoundhouse7 = 1307,
        [Tooltip("Length: 130m")]
        OilRefinery = 1400,
        [Tooltip("Length: 32m (has turntable)")]
        OilWellCentral = 1500,
        [Tooltip("Length: 40m")]
        OilWellNorth = 1600,
        [Tooltip("Length: 95m")]
        Sawmill = 1700,
        [Tooltip("Length: 32m (has turntable)\nDE2 exclusive\n")]
        SteelMillTurntable1 = 1800,
        [Tooltip("Length: 32m (has turntable)")]
        SteelMillTurntable2 = 1801
    }
}
