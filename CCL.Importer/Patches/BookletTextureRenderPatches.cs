using DV.RenderTextureSystem.BookletRender;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StaticPagesRender))]
    internal class StaticPagesRenderPatches
    {
        private const string CatalogName = "VehicleCatalogStaticRender";

        [HarmonyPrefix, HarmonyPatch("GetStaticTemplatePaperData")]
        private static void GenerateTexturesPrefix(StaticPagesRender __instance)
        {
            if (!__instance.gameObject.name.StartsWith(CatalogName))
            {
                return;
            }

            CCLPlugin.Log("Starting loco catalog injection...");

            CatalogGenerator.GeneratePages(__instance);

            __instance.gameObject.name = "VehicleCatalogStaticRender_CCL";
            __instance.staticPages.AddRange(CatalogGenerator.NewCatalogPages);

            CCLPlugin.Log("Loco catalog injection successful!");
        }
    }
}
