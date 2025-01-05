using DV.Localization;
using DV.RenderTextureSystem.BookletRender;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(VehicleCatalogPageTemplatePaper))]
    internal class VehicleCatalogPageTemplatePaperPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(VehicleCatalogPageTemplatePaper.FillInData))]
        private static void FillInDataPostfix(VehicleCatalogPageTemplatePaper __instance)
        {
            // ?????????????
            foreach (var item in __instance.GetComponentsInChildren<Localize>())
            {
                item.UpdateLocalization();
            }
        }
    }
}
