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
            public const string ID = "PassengerJobs";
            public const string CARGO_ID = "Passengers";
            public const float CARGO_MASS = 3000;

            public static bool RequiresMod(CustomCarType carType)
            {
                return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId == PassengerJobs.CARGO_ID);
            }
        }

        public static class CustomCargo
        {
            public const string ID = "DVCustomCargo";

            public static bool RequiresMod(CustomCarType carType)
            {
                return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId != PassengerJobs.CARGO_ID && !Utilities.IsVanillaCargo(x.CargoId));
            }
        }

        public static class CustomLicenses
        {
            public const string ID = "DVCustomLicenses";

            public static bool RequiresMod(CustomCarType carType)
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
        }

        public static class Gauge
        {
            public const string ID = "Gauge";

            public static bool RequiresMod(CustomCarType carType)
            {
                return carType.UseCustomGauge;
            }
        }

        public static class GenericContainerCargo
        {
            public const string ID = "CC_GenericContainerCargo";
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

            private static bool IsAnyId(string id)
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

            public static bool RequiresMod(CustomCarType carType)
            {
                return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => GenericContainerCargo.IsAnyId(x.CargoId));
            }
        }

        public static class SkinManager
        {
            public const string ID = "SkinManagerMod";

            public static bool RequiresMod(CustomCarPack pack)
            {
                return pack.PaintSubstitutions.Any(x => x != null && !IdV2.Paints.Contains(x.Paint));
            }
        }

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

            CheckRequires(PassengerJobs.RequiresMod, PassengerJobs.ID);
            CheckRequires(CustomCargo.RequiresMod, CustomCargo.ID);
            CheckRequires(GenericContainerCargo.RequiresMod, GenericContainerCargo.ID);
            CheckRequires(CustomLicenses.RequiresMod, CustomLicenses.ID);
            CheckRequires(Gauge.RequiresMod, Gauge.ID);

            CheckRequiresPack(SkinManager.RequiresMod, SkinManager.ID);

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

            void CheckRequiresPack(Func<CustomCarPack, bool> check, string id)
            {
                if (check(pack))
                {
                    requirements.Add(id);
                }
            }
        }
    }
}