using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(DVObjectModel))]
    internal class ObjectModelPatches
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(DVObjectModel), "RecalculateMapping", generics: new[] { typeof(TrainCarType), typeof(TrainCarLivery) });
        }

        [HarmonyPrefix]
        [HarmonyPatch]
        static bool RecalculateCarMapping(ref Dictionary<TrainCarType, TrainCarLivery> mapping, List<TrainCarLivery> source)
        {
            mapping = new Dictionary<TrainCarType, TrainCarLivery>();
            foreach (var livery in source.Where(l => l.v1 != TrainCarType.NotSet))
            {
                mapping.Add(livery.v1, livery);
            }
            return false;
        }
    }
}
