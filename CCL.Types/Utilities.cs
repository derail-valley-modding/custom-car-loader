using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    public static class Utilities
    {
        public static bool IsVanillaCargo(string id)
        {
            return IdV2.Cargos.Any(x => x == id);
        }

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

        public static int GetAudioPoolSize(DVTrainCarKind kind) => kind switch
        {
            DVTrainCarKind.Loco => 10,
            DVTrainCarKind.Slug => 1,
            _ => 0,
        };
    }
}
