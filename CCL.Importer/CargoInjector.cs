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
                    }
                }

                var loadableInfo = new CargoType_v2.LoadableInfo(carType, entry.Models);
                matchCargo.loadableCarTypes = matchCargo.loadableCarTypes.AddToArray(loadableInfo);
            }
        }
    }
}
