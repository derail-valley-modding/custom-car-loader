using HarmonyLib;
using LocoSim.Definitions;
using LocoSim.Implementations;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(Port))]
    internal class PortPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(Port.ExternalValueUpdate))]
        private static void Asdasd(Port __instance)
        {
            if (__instance.type != PortType.EXTERNAL_IN)
            {
                // Critical information missing from the original Error message.
                CCLPlugin.Error($"Port ID is {__instance.id}");
            }
        }
    }
}
