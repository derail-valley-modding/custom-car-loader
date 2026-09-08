using System;
using System.Collections.Generic;
using System.Linq;
using CCL.Types;

namespace CCL.Creator.Utility
{
    internal static class OtherMods
    {
        public static class PassengerJobs
        {
            public const string MOD_ID = "PassengerJobs";
            public const string CARGO_ID = "Passengers";
            public const float CARGO_MASS = 3000;
        }

        public static class GenericContainerCargo
        {
            public const string MOD_ID = "CC_GenericContainerCargo";
            public const string CHEMICALS_ID = "GenericChemicals";
            public const string CLOTHING_ID = "GenericClothing";
            public const string ELECTRONICS_ID = "GenericElectronics";
            public const string TOOLING_ID = "GenericTools";
            public const string EMPTY_ID = "EmptyContainers";
            public const float CHEMICALS_MASS = 30000;
            public const float CLOTHING_MASS = 31000;
            public const float ELECTRONICS_MASS = 35000;
            public const float TOOLING_MASS = 37000;
            public const float EMPTY_MASS = 6000;

            public static bool IsAnyId(string id)
            {
                switch (id)
                {
                    case CHEMICALS_ID:
                    case CLOTHING_ID:
                    case ELECTRONICS_ID:
                    case EMPTY_ID:
                    case TOOLING_ID:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public const string CUSTOM_CARGO = "DVCustomCargo";
        public const string CUSTOM_LICENSES = "DVCustomLicenses";
        public const string GAUGE = "Gauge";

        public static readonly string[] CustomCargoIds = new[]
        {
            GenericContainerCargo.EMPTY_ID,
            GenericContainerCargo.CHEMICALS_ID,
            GenericContainerCargo.CLOTHING_ID,
            GenericContainerCargo.ELECTRONICS_ID,
            GenericContainerCargo.TOOLING_ID,
        };

        public static List<string> GetModRequirements(CustomCarPack pack)
        {
            var requirements = new List<string>() { ExporterConstants.MOD_ID };

            CheckRequires(RequiresPassengerJobsMod, PassengerJobs.MOD_ID);
            CheckRequires(RequiresCustomCargoMod, CUSTOM_CARGO);
            CheckRequires(RequiresGenericContainersMod, GenericContainerCargo.MOD_ID);
            CheckRequires(RequiresCustomLicenseMod, CUSTOM_LICENSES);
            CheckRequires(RequiresGaugeMod, GAUGE);

            foreach (var item in pack.AdditionalDependencies)
            {
                if (requirements.Contains(item)) continue;

                requirements.Add(item);
            }

            return requirements;

            void CheckRequires(Func<CustomCarType, bool> check, string id)
            {
                if (pack.Cars.Any(check))
                {
                    requirements.Add(id);
                }
            }
        }

        public static bool RequiresPassengerJobsMod(CustomCarType carType)
        {
            return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId == PassengerJobs.CARGO_ID);
        }

        public static bool RequiresCustomCargoMod(CustomCarType carType)
        {
            return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId != PassengerJobs.CARGO_ID && !Utilities.IsVanillaCargo(x.CargoId));
        }

        public static bool RequiresGenericContainersMod(CustomCarType carType)
        {
            return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => GenericContainerCargo.IsAnyId(x.CargoId));
        }

        public static bool RequiresCustomLicenseMod(CustomCarType carType)
        {
            if (!string.IsNullOrWhiteSpace(carType.GeneralLicense) && !Utilities.IsVanillaLicense(carType.GeneralLicense))
            {
                return true;
            }

            foreach (var license in carType.JobLicenses)
            {
                if (!string.IsNullOrWhiteSpace(license) && !Utilities.IsVanillaLicense(license))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool RequiresGaugeMod(CustomCarType carType)
        {
            return carType.UseCustomGauge /*|| carType.liveries.Any(x => x.prefab != null && x.prefab.TryGetComponent<RegaugeableMeshes>(out _))*/;
        }
    }
}