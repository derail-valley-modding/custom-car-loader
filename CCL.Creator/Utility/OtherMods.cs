using System.Collections.Generic;
using System.Linq;
using CCL.Types;

namespace CCL.Creator.Utility
{
    internal static class OtherMods
    {
        public const string PASSENGER_JOBS = "PassengerJobs";
        public const string CUSTOM_CARGO = "DVCustomCargo";
        public const string CUSTOM_LICENSES = "DVCustomLicenses";

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

            if (pack.Cars.Any(x => RequiresCustomLicenseMod(x)))
            {
                requirements.Add(CUSTOM_LICENSES);
            }

            foreach (var item in pack.AdditionalDependencies)
            {
                if (requirements.Contains(item)) continue;

                requirements.Add(item);
            }

            return requirements;
        }

        public static bool RequiresPassengerJobsMod(CustomCarType carType)
        {
            return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId == "Passengers");
        }

        public static bool RequiresCustomCargoMod(CustomCarType carType)
        {
            return carType.CargoSetup != null && carType.CargoSetup.Entries.Any(x => x.CargoId != "Passengers" && !Utilities.IsVanillaCargo(x.CargoId));
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
    }
}