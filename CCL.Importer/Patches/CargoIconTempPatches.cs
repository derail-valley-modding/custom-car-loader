using DV.RenderTextureSystem.BookletRender;
using DV.UI.PresetEditors;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    internal static class CargoIconTempPatches
    {
        [HarmonyPatch(typeof(TaskTemplatePaper))]
        internal static class TaskTemplatePaperPatches
        {
            [HarmonyPostfix, HarmonyPatch(nameof(TaskTemplatePaper.FillInData))]
            private static void FillInDataPostfix()
            {
                CargoType_v2Patches.ResetAll();
            }
        }

        [HarmonyPatch(typeof(FrontPageTemplatePaper))]
        internal static class FrontPageTemplatePaperPatches
        {
            [HarmonyPostfix, HarmonyPatch(nameof(FrontPageTemplatePaper.FillInData))]
            private static void FillInDataPostfix()
            {
                CargoType_v2Patches.ResetAll();
            }
        }

        [HarmonyPatch(typeof(TrainEditorViewElement))]
        internal static class TrainEditorViewElementPatches
        {
            [HarmonyPostfix, HarmonyPatch("UpdateView")]
            private static void UpdateViewPostfix()
            {
                CargoType_v2Patches.ResetAll();
            }
        }
    }
}
