using DV.RenderTextureSystem.BookletRender;
using HarmonyLib;
using System.Linq;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StaticTextureRenderBase))]
    internal class StaticTextureRenderBasePatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(StaticTextureRenderBase.GenerateStaticPagesTextures))]
        private static void PreparePrefix(StaticTextureRenderBase __instance)
        {
            if (__instance is not VehicleCatalogRender catalog) return;

            CCLPlugin.Log("Starting loco catalog injection...");
            var sw = System.Diagnostics.Stopwatch.StartNew();

            CatalogGenerator.GeneratePages(catalog);

            catalog.gameObject.name = "VehicleCatalogStaticRender_CCL";
            catalog.vehiclePages = catalog.vehiclePages.Concat(CatalogGenerator.NewCatalogPages).ToArray();

            sw.Stop();
            CCLPlugin.Log($"Loco catalog injection successful ({sw.Elapsed.TotalSeconds:F4}s)!");
        }
    }
}
