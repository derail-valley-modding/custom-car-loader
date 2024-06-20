namespace CCL.Types
{
    public static class Utilities
    {
        public static bool IsVanillaLicense(string id)
        {
            switch (id)
            {
                case "Basic":
                case "FreightHaul":
                case "Hazmat1":
                case "Hazmat2":
                case "Hazmat3":
                case "LogisticalHaul":
                case "Military1":
                case "Military2":
                case "Military3":
                case "Shunting":
                case "TrainLength1":
                case "TrainLength2":
                case "ConcurrentJobs1":
                case "ConcurrentJobs2":
                case "DE2":
                case "DE6":
                case "DH4":
                case "DM3":
                case "ManualService":
                case "MultipleUnit":
                case "S060":
                case "SH282":
                case "TrainDriver":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsVanillaOrPassengerJobsLicense(string id)
        {
            return id == "Passengers" || IsVanillaLicense(id);
        }
    }
}
