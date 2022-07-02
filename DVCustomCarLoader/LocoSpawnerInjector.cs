using CCL_GameScripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DVCustomCarLoader
{
    public static class LocoSpawnerInjector
    {
        private static FieldInfo nextLocoGroupSpawnIndex = AccessTools.Field(typeof(StationLocoSpawner), "nextLocoGroupSpawnIndex");

        public static void InjectCarsToSpawners()
        {
            foreach (var customCar in CustomCarManager.CustomCarTypes)
            {
                if (customCar.LocoSpawnLocations != StationYard.None)
                {
                    var typeList = new List<TrainCarType>() { customCar.CarType };
                    var typeListWrapper = new ListTrainCarTypeWrapper(typeList);

                    float consistLength = customCar.InterCouplerDistance + YardTracksOrganizer.Instance.GetSeparationLengthBetweenCars(1);

                    if (!string.IsNullOrEmpty(customCar.TenderID))
                    {
                        if (CarTypeInjector.TryGetCustomCarById(customCar.TenderID, out var tender))
                        {
                            typeList.Add(tender.CarType);
                            consistLength += tender.InterCouplerDistance + YardTracksOrganizer.Instance.GetSeparationLengthBetweenCars(1);
                        }
                        else
                        {
                            Main.Warning($"Couldn't find tender \"{customCar.TenderID}\" for loco {customCar.identifier}");
                        }
                    }

                    foreach (string yardId in customCar.LocoSpawnLocations.YardIds())
                    {
                        if (!LogicController.Instance.YardIdToStationController.TryGetValue(yardId, out var controller))
                        {
                            Main.Error($"Invalid station {yardId} for loco {customCar.identifier} spawning");
                            continue;
                        }

                        var spawners = controller.GetComponentsInChildren<StationLocoSpawner>();
                        if (spawners.Length == 0)
                        {
                            Main.Warning($"Station {yardId} has no loco spawn tracks - can't spawn {customCar.identifier} here");
                            continue;
                        }

                        foreach (var spawner in spawners)
                        {
                            double availableSpace = spawner.locoSpawnTrack.logicTrack.length;
                            if (availableSpace >= consistLength)
                            {
                                Main.LogVerbose($"Added loco {customCar.identifier} to autospawn at {yardId}, track {spawner.locoSpawnTrackName}");
                                spawner.locoTypeGroupsToSpawn.Add(typeListWrapper);
                                int nextIndex = UnityEngine.Random.Range(0, spawner.locoTypeGroupsToSpawn.Count);
                                nextLocoGroupSpawnIndex.SetValue(spawner, nextIndex);
                            }
                        }
                    }
                }
            }
        }
    }
}
