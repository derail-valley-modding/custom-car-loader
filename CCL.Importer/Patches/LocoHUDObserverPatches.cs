using CCL.Importer.Components.HUD;
using DV.HUD;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(LocoHUDObserver))]
    internal class LocoHUDObserverPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(LocoHUDObserver.Start))]
        public static void StartPostfix(LocoHUDObserver __instance)
        {
            var extra = __instance.gameObject.AddComponent<ExtraLocoHUDObserver>();
            extra.Observer = __instance;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(LocoHUDObserver.HUDChanged))]
        private static void HUDChangedPrefix(LocoHUDObserver __instance, HUDInterfacer.HUDChangeEvent obj)
        {
            var controls = __instance.locoControls;

            if (controls == null || !__instance.TryGetComponent(out ExtraLocoHUDObserver observer)) return;

            if (controls.mechanical.alerter)
            {
                controls.mechanical.alerter.controlModule.ValueChanged -= observer.AlerterValueChanged;
            }
            if (controls.mechanical.cabOrient)
            {
                controls.mechanical.cabOrient.controlModule.ValueChanged -= observer.CabOrientValueChanged;
            }
            if (controls.mechanical.pantograph)
            {
                controls.mechanical.pantograph.controlModule.ValueChanged -= observer.PantographValueChanged;
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(LocoHUDObserver.HUDChanged))]
        private static void HUDChangedPostfix(LocoHUDObserver __instance, HUDInterfacer.HUDChangeEvent obj)
        {
            var controls = __instance.locoControls;

            if (controls == null || !__instance.TryGetComponent(out ExtraLocoHUDObserver observer)) return;

            if (controls.mechanical.alerter)
            {
                controls.mechanical.alerter.controlModule.ValueChanged += observer.AlerterValueChanged;
            }
            if (controls.mechanical.cabOrient)
            {
                controls.mechanical.cabOrient.controlModule.ValueChanged += observer.CabOrientValueChanged;
            }
            if (controls.mechanical.pantograph)
            {
                controls.mechanical.pantograph.controlModule.ValueChanged += observer.PantographValueChanged;
            }
        }
    }
}
