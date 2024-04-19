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
            { SpawnTrack.CoalMine, "LocoSpawnerCM" },
            { SpawnTrack.CitySouthWest, "LocoSpawnerCSW1" },
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
            { SpawnTrack.OilWellCentral, "LocoSpawnerOWC-A-3" },
            { SpawnTrack.OilWellNorth, "LocoSpawnerOWN" },
            { SpawnTrack.SteelMillTurntable1, "LocoSpawnerSM_T1-1" },
            { SpawnTrack.SteelMillTurntable2, "LocoSpawnerSM_T1-2" },
            { SpawnTrack.Sawmill, "LocoSpawnerSW" }
        };

        public static Dictionary<string, string> SpawnerNameToId = new Dictionary<string, string>
        {
            { "LocoSpawnerCM", "CM" },
            { "LocoSpawnerCSW1" , "CSW" },
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
            { "LocoSpawnerOWC-A-3" , "OWC" },
            { "LocoSpawnerOWN" , "OWN" },
            { "LocoSpawnerSM_T1-1" , "SM" },
            { "LocoSpawnerSM_T1-2" , "SM" },
            { "LocoSpawnerSW" , "SW" }
        };

        [Tooltip("The track where the loco(s) will spawn")]
        public SpawnTrack Track;
        [Tooltip("Extra locos/tenders to spawn together (in order)")]
        public string[] Liveries;

        public LocoSpawnGroup() : this(SpawnTrack.MachineFactoryA1, new string[0]) { }

        public LocoSpawnGroup(SpawnTrack track, string[] liveries)
        {
            Track = track;
            Liveries = liveries;
        }
    }

    public enum SpawnTrack
    {
        CitySouthWest,
        CoalMine,
        Farm,
        FoodFactory1,
        FoodFactory2,
        ForestCentral,
        ForestSouth,
        GoodsFactory1,
        GoodsFactory2,
        HarbourA1,
        HarbourRoundhouse1,
        HarbourRoundhouse2,
        HarbourRoundhouse5,
        HarbourRoundhouse7,
        IronMineEast,
        IronMineWest,
        MachineFactoryA1,
        MachineFactoryRoundhouse2,
        MachineFactoryRoundhouse3,
        MachineFactoryRoundhouse4,
        MachineFactoryRoundhouse7,
        OilWellCentral,
        OilWellNorth,
        Sawmill,
        SteelMillTurntable1,
        SteelMillTurntable2
    }
}
