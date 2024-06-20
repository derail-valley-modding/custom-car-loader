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
            { SpawnTrack.Sawmill, "LocoSpawnerSW" },
            { SpawnTrack.SteelMillTurntable1, "LocoSpawnerSM_T1-1" },
            { SpawnTrack.SteelMillTurntable2, "LocoSpawnerSM_T1-2" },
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
            { "LocoSpawnerSW" , "SW" },
            { "LocoSpawnerSM_T1-1" , "SM" },
            { "LocoSpawnerSM_T1-2" , "SM" },
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
        [Tooltip("Length: 155m")]
        CitySouthWest,
        [Tooltip("Length: 50m")]
        CoalMine,
        [Tooltip("Length: 300m")]
        Farm,
        [Tooltip("Length: 50m")]
        FoodFactory1,
        [Tooltip("Length: 55m")]
        FoodFactory2,
        [Tooltip("Length: 60m")]
        ForestCentral,
        [Tooltip("Length: 62m")]
        ForestSouth,
        [Tooltip("Length: 60m")]
        GoodsFactory1,
        [Tooltip("Length: 60m")]
        GoodsFactory2,
        [Tooltip("Length: 65m")]
        HarbourA1,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse1,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse2,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse5,
        [Tooltip("Length: 45m (has turntable)")]
        HarbourRoundhouse7,
        [Tooltip("Length: 45m")]
        IronMineEast,
        [Tooltip("Length: 25m (has turntable)")]
        IronMineWest,
        [Tooltip("Length: 140m")]
        MachineFactoryA1,
        [Tooltip("Length: 45m (has turntable)")]
        MachineFactoryRoundhouse2,
        [Tooltip("Length: 45m (has turntable)")]
        MachineFactoryRoundhouse3,
        [Tooltip("Length: 45m (has turntable)")]
        MachineFactoryRoundhouse4,
        [Tooltip("Length: 45m (has turntable)")]
        MachineFactoryRoundhouse7,
        [Tooltip("Length: 32m (has turntable)")]
        OilWellCentral,
        [Tooltip("Length: 40m")]
        OilWellNorth,
        [Tooltip("Length: 95m")]
        Sawmill,
        [Tooltip("Length: 32m (has turntable)")]
        SteelMillTurntable1,
        [Tooltip("Length: 32m (has turntable)")]
        SteelMillTurntable2
    }
}
