using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(DVObjectModel))]
    internal class ObjectModelPatches
    {
        private static MethodBase TargetMethod()
        {
            return typeof(DVObjectModel).GetMethod("RecalculateMapping", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TrainCarType), typeof(TrainCarLivery));
        }

        [HarmonyPrefix, HarmonyPatch]
        private static bool RecalculateCarMapping(ref Dictionary<TrainCarType, TrainCarLivery> mapping, List<TrainCarLivery> source)
        {
            mapping = new Dictionary<TrainCarType, TrainCarLivery>();
            foreach (var livery in source)
            {
                if (livery.v1 == TrainCarType.NotSet) continue;

                mapping.Add(livery.v1, livery);
            }
            return false;
        }
    }
}
