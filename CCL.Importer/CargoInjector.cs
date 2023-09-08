using CCL.Importer.Types;
using DV;
using DV.ThingTypes;
using HarmonyLib;
using System;
using UnityEngine;

namespace CCL.Importer
{
    public static class CargoInjector
    {
        public static void InjectLoadableCargos(CCL_CarType carType)
        {
            CCLPlugin.LogVerbose($"Add cargos for {carType.id} - {carType.CargoTypes?.Entries?.Count}");
            if (carType.CargoTypes == null || carType.CargoTypes.IsEmpty)
            {
                return;
            }

            foreach (var loadableCargo in carType.CargoTypes.Entries)
            {
                CCLPlugin.LogVerbose($"Loadable cargo {carType.id} - {loadableCargo.AmountPerCar} {loadableCargo.CargoType}, {loadableCargo.ModelVariants?.Length} models");
                if (!Globals.G.Types.CargoType_to_v2.TryGetValue((CargoType)loadableCargo.CargoType, out var matchCargo))
                {
                    CCLPlugin.Error($"Couldn't find v2 cargo type {loadableCargo.CargoType} for car {carType.id}");
                }

                var loadableInfo = new CargoType_v2.LoadableInfo(carType, loadableCargo.ModelVariants);
                matchCargo.loadableCarTypes = matchCargo.loadableCarTypes.AddToArray(loadableInfo);
            }
        }
    }
}
