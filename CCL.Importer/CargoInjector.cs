using CCL.Importer.Patches;
using CCL.Importer.Processing;
using CCL.Importer.Types;
using CCL.Types;
using DV;
using DV.ThingTypes;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer
{
    public static class CargoInjector
    {
        public static void InjectLoadableCargos(CCL_CarType carType)
        {
            CCLPlugin.LogVerbose($"Adding cargos for {carType.id} ({carType.CargoSetup?.Entries.Count})");

            if (carType.CargoSetup == null || carType.CargoSetup.IsEmpty) return;

            foreach (var entry in carType.CargoSetup.Entries)
            {
                CCLPlugin.LogVerbose($"Cargo '{entry.CargoId}': {entry.AmountPerCar}, {entry.Models?.Length} model(s)");

                if (!Globals.G.Types.TryGetCargo(entry.CargoId, out var matchCargo))
                {
                    CCLPlugin.Error($"Couldn't find  cargo '{entry.CargoId}'");
                    continue;
                }

                if (entry.Models != null)
                {
                    for (int i = 0; i < entry.Models.Length; i++)
                    {
                        ModelProcessor.DoBasicProcessing(entry.Models[i]);

                        if (entry.Models[i].transform.TryFind(CarPartNames.Colliders.ROOT, out var colliderRoot))
                        {
                            AddItemColliderIfNeeded(colliderRoot);
                            ColliderProcessor.FixLayers(colliderRoot);
                        }
                    }
                }

                var loadableInfo = new CargoType_v2.LoadableInfo(carType, entry.Models);
                matchCargo.loadableCarTypes = matchCargo.loadableCarTypes.AddToArray(loadableInfo);
            }
        }

        public static Sprite GetCargoIcon(CargoType_v2 cargo, TrainCarType_v2 car)
        {
            if (car is not CCL_CarType ccl)
            {
                if (CargoType_v2Patches.OriginalSprites.TryGetValue(cargo, out var icon)) return icon;

                return cargo.icon;
            }

            // It should never be null if we get here, but it doesn't hurt to check.
            if (ccl.CargoSetup == null || !ccl.CargoSetup.Entries.TryFind(x => x.CargoId == cargo.id, out var entry)) return null!;

            return entry.OverrideLoadedIcon ? entry.LoadedIcon : cargo.icon;
        }

        private static void AddItemColliderIfNeeded(Transform root)
        {
            // If there's no walkable to copy or an [items] object already exists...
            if (root.TryFind(CarPartNames.Colliders.ITEMS, out var items) ||
                !root.TryFind(CarPartNames.Colliders.WALKABLE, out var walkable)) return;

            items = Object.Instantiate(walkable, root);
            items.name = CarPartNames.Colliders.ITEMS;
        }
    }
}
