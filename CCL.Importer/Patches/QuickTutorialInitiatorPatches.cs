using DV.Tutorial.QT;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(QuickTutorialInitiator))]
    internal class QuickTutorialInitiatorPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(QuickTutorialInitiator.Start))]
        private static void StartPrefix(QuickTutorialInitiator __instance)
        {
            foreach (var type in CarManager.CustomCarTypes)
            {
                if (type.TutorialType == CCL.Types.CustomCarType.TutorialTypeEnum.None) continue;

                foreach (var livery in type.liveries)
                {
                    __instance.supportedLocoIds[livery.id] = $"QT_CCL_{type.id}";
                }
            }
        }
    }
}
