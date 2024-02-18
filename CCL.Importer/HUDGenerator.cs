using CCL.Types.Components.HUD;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using DV.UI.LocoHUD;
using UnityEngine;

using static CCL.Types.Components.HUD.CustomHUDLayout;

namespace CCL.Importer
{
    internal static class HUDGenerator
    {
        private static Transform? s_holder;
        private static HUDLocoControls? s_hudDE2;
        private static HUDLocoControls? s_hudDM3;
        private static HUDLocoControls? s_hudDH4;
        private static HUDLocoControls? s_hudDE6;
        private static HUDLocoControls? s_hudS060;
        private static HUDLocoControls? s_hudS282;
        private static HUDLocoControls? s_hudBE2;
        private static HUDLocoControls? s_hudHandcar;

        private static Transform Holder => Extensions.GetCached(ref s_holder, CreateHolder);
        private static HUDLocoControls HudDE2 => Extensions.GetCached(ref s_hudDE2,
            () => TrainCarType.LocoShunter.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudDM3 => Extensions.GetCached(ref s_hudDM3,
            () => TrainCarType.LocoDM3.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudDH4 => Extensions.GetCached(ref s_hudDH4,
            () => TrainCarType.LocoDH4.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudDE6 => Extensions.GetCached(ref s_hudDE6,
            () => TrainCarType.LocoDiesel.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudS060 => Extensions.GetCached(ref s_hudS060,
            () => TrainCarType.LocoS060.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudS282 => Extensions.GetCached(ref s_hudS282,
            () => TrainCarType.LocoSteamHeavy.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudBE2 => Extensions.GetCached(ref s_hudBE2,
            () => TrainCarType.LocoMicroshunter.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());
        private static HUDLocoControls HudHandcar => Extensions.GetCached(ref s_hudHandcar,
            () => TrainCarType.HandCar.ToV2().parentType.hudPrefab.GetComponent<HUDLocoControls>());

        private static Transform CreateHolder()
        {
            // Make a dummy to hold the HUD in inactive state.
            var hudHolder = new GameObject("[HUD holder]");
            hudHolder.SetActive(false);
            Object.DontDestroyOnLoad(hudHolder);

            return hudHolder.transform;
        }

        public static GameObject CreateHUD(GameObject original)
        {
            var layout = original.GetComponent<VanillaHUDLayout>();

            if (layout == null)
            {
                CCLPlugin.Warning("No VanillaHUDLayout component found at the root, destroying HUD.");
                return null!;
            }

            layout.AfterImport();

            switch (layout.HUDType)
            {
                case VanillaHUDLayout.BaseHUD.DE2:
                    return HudDE2.gameObject;
                case VanillaHUDLayout.BaseHUD.DE6:
                    return HudDE6.gameObject;
                case VanillaHUDLayout.BaseHUD.DM3:
                    return HudDM3.gameObject;
                case VanillaHUDLayout.BaseHUD.DH4:
                    return HudDH4.gameObject;
                case VanillaHUDLayout.BaseHUD.S060:
                    return HudS060.gameObject;
                case VanillaHUDLayout.BaseHUD.S282:
                    return HudS282.gameObject;
                case VanillaHUDLayout.BaseHUD.BE2:
                    return HudBE2.gameObject;
                case VanillaHUDLayout.BaseHUD.Handcar:
                    return HudHandcar.gameObject;
                default:
                    return CreateCustomHUD(layout);
            }
        }

        private static GameObject CreateCustomHUD(VanillaHUDLayout layout)
        {
            // Steal the DE2's HUD as a base.
            var newHUD = Object.Instantiate(HudDE2, Holder);

            newHUD.text.powertrainTypeText.SetTextValue(layout.CustomHUDSettings.Powertrain);
            SetupBasicControls(newHUD.basicControls, layout.CustomHUDSettings.BasicControls);

            return newHUD.gameObject;
        }

        private static void SetupBasicControls(HUDLocoControls.BasicControlsReferences newHUD, BasicControls layout)
        {
            if (layout.AmpMeter == ShouldDisplay.None)
            {
                newHUD.ampMeter.gameObject.SetActive(false);
            }

            switch (layout.Throttle)
            {
                case BasicControls.Slot0B.None:
                    newHUD.throttle.gameObject.SetActive(false);
                    break;
                case BasicControls.Slot0B.Regulator:
                    var regulator = Object.Instantiate(HudS060.basicControls.throttle, newHUD.throttle.transform.parent);
                    Object.Destroy(newHUD.throttle.gameObject);
                    newHUD.throttle = regulator;
                    break;
                default:
                    // Keep throttle as is.
                    break;
            }
        }
    }
}
