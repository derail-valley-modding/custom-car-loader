using CCL.Importer.Components.HUD;
using DV.CabControls;
using DV.HUD;
using DV.UI.LocoHUD;
using HarmonyLib;
using System;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(LocoHUDProvider))]
    internal class LocoHUDProviderPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(LocoHUDProvider.Start))]
        public static void StartPostfix(LocoHUDProvider __instance)
        {
            var extra = __instance.gameObject.AddComponent<ExtraLocoHUDProvider>();
            extra.Provider = __instance;
        }

        [HarmonyPostfix, HarmonyPatch(nameof(LocoHUDProvider.SubControls))]
        private static void SubControlsPostfix(LocoHUDProvider __instance, LocoControlsReader lcr)
        {
            if (lcr is not ExtraLocoControlsReaderInternal elcr || !__instance.TryGetComponent(out ExtraLocoHUDProvider provider)) return;

            var controls = __instance.locoControls;
            SubscribeControlEvent(elcr.Alerter, controls.mechanical.alerter, provider.AlerterUpdated, ref provider.Alerter);
            SubscribeControlEvent(elcr.CabOrient, controls.mechanical.cabOrient, provider.CabOrientUpdated, ref provider.CabOrient);
            SubscribeControlEvent(elcr.Pantograph, controls.mechanical.pantograph, provider.PantographUpdated, ref provider.Pantograph);

            static void SubscribeControlEvent(GameObject? control, LocoHUDControlBase? hudElement, Action<ValueChangedEventArgs> action, ref ControlImplBase? impl)
            {
                if (hudElement == null || control == null || !control.TryGetComponent<ControlImplBase>(out impl)) return;

                impl.ValueChanged += action;
                action?.Invoke(new ValueChangedEventArgs(0f, impl.Value));
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(LocoHUDProvider.UnsubControls))]
        private static void UnsubControlsPostfix(LocoHUDProvider __instance, LocoControlsReader lcr)
        {
            if (lcr is not ExtraLocoControlsReaderInternal elcr || !__instance.TryGetComponent(out ExtraLocoHUDProvider provider)) return;

            provider.RemoveReferences();
            UnsubscribeControlEvent(elcr.Alerter, provider.AlerterUpdated);
            UnsubscribeControlEvent(elcr.CabOrient, provider.CabOrientUpdated);
            UnsubscribeControlEvent(elcr.Pantograph, provider.PantographUpdated);

            static void UnsubscribeControlEvent(GameObject? control, Action<ValueChangedEventArgs> action)
            {
                if (control == null || !control.TryGetComponent(out ControlImplBase impl)) return;

                impl.ValueChanged -= action;
            }
        }
    }
}
