using System.Collections.Generic;
using System.Linq;
using CCL.Types;

namespace CCL.Creator.Utility
{
    internal static class OtherMods
    {
        public const string PASSENGER_JOBS = "PassengerJobs";
        public const string CUSTOM_CARGO = "DVCustomCargo";

        public static List<string> GetModRequirements(CustomCarPack pack)
        {
            var requirements = new List<string>() { ExporterConstants.MOD_ID };

            if (pack.Cars.Any(x => RequiresPassengerJobsMod(x)))
            {
                requirements.Add(PASSENGER_JOBS);
            }

            if (pack.Cars.Any(x => RequiresCustomCargoMod(x)))
            {
                requirements.Add(CUSTOM_CARGO);
            }

            return requirements;
        }

        public static bool RequiresPassengerJobsMod(CustomCarType carType)
        {
            if (carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId == "Passengers"))
            {
                return true;
            }

            if (carType.CatalogPage != null && carType.CatalogPage.AllLicenses.Any(x => x == "Passengers"))
            {
                return true;
            }

            return false;
        }

        public static bool RequiresCustomCargoMod(CustomCarType carType)
        {
            return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId != "Passengers" && !Utilities.IsVanillaCargo(x.CargoId));
        }

        public static bool RequiresCustomLicenseMod(CustomCarType carType)
        {
            if (!Utilities.IsVanillaLicense(carType.LicenseID))
            {
                return true;
            }

            if (carType.CatalogPage != null && carType.CatalogPage.AllLicenses.Any(x => !Utilities.IsVanillaLicense(x)))
            {
                return true;
            }

            return false;
        }
    }
}