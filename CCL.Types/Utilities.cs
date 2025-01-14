using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    public static class Utilities
    {
        public static bool IsVanillaLicense(string id)
        {
            return IdV2.GeneralLicenses.Any(x => x == id) || IdV2.JobLicenses.Any(x => x == id);
        }

        public static bool IsVanillaOrPassengerJobsLicense(string id)
        {
            return id == "Passengers" || IsVanillaLicense(id);
        }

        public static void CopyTransform(Transform source, Transform target)
        {
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }
    }
}
