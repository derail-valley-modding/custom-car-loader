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
            [HarmonyFinalizer, HarmonyPatch(nameof(TaskTemplatePaper.FillInData))]
            private static void FillInDataFinalizer()
            {
                CargoType_v2Patches.ResetAll();
            }
        }

        [HarmonyPatch(typeof(FrontPageTemplatePaper))]
        internal static class FrontPageTemplatePaperPatches
        {
            [HarmonyFinalizer, HarmonyPatch(nameof(FrontPageTemplatePaper.FillInData))]
            private static void FillInDataFinalizer()
            {
                CargoType_v2Patches.ResetAll();
            }
        }

        [HarmonyPatch(typeof(TrainEditorViewElement))]
        internal static class TrainEditorViewElementPatches
        {
            [HarmonyFinalizer, HarmonyPatch("UpdateView")]
            private static void UpdateViewFinalizer()
            {
                CargoType_v2Patches.ResetAll();
            }
        }
    }
}
