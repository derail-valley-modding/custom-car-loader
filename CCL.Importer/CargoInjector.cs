using CCL.Importer.Processing;
using CCL.Importer.Types;
using DV;
using DV.ThingTypes;
using HarmonyLib;

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

                if (loadableCargo.IsCustom && !Globals.G.Types.cargos.TryFind(x => x.id == loadableCargo.CustomCargoId, out var matchCargo))
                {
                    CCLPlugin.Error($"Couldn't find custom cargo {loadableCargo.CustomCargoId} for car {carType.id}");
                    continue;
                }
                else if (!Globals.G.Types.CargoType_to_v2.TryGetValue((CargoType)loadableCargo.CargoType, out matchCargo))
                {
                    CCLPlugin.Error($"Couldn't find v2 cargo type {loadableCargo.CargoType} for car {carType.id}");
                    continue;
                }

                if (loadableCargo.ModelVariants != null)
                {
                    for (int i = 0; i < loadableCargo.ModelVariants.Length; i++)
                    {
                        ModelProcessor.DoBasicProcessing(loadableCargo.ModelVariants[i]);
                    }
                }

                var loadableInfo = new CargoType_v2.LoadableInfo(carType, loadableCargo.ModelVariants);
                matchCargo.loadableCarTypes = matchCargo.loadableCarTypes.AddToArray(loadableInfo);
            }
        }
    }
}
