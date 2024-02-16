using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    // A copy of the above class, but with only the livery IDs.
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
            { SpawnTrack.SteelMillTurntable1, "LocoSpawnerSM_T1-1" },
            { SpawnTrack.SteelMillTurntable2, "LocoSpawnerSM_T1-2" },
            { SpawnTrack.Sawmill, "LocoSpawnerSW" }
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
        CoalMine,
        CitySouthWest,
        FoodFactory1,
        FoodFactory2,
        Farm,
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
        SteelMillTurntable1,
        SteelMillTurntable2,
        Sawmill
    }
}
