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
            SetupBrakeControls(newHUD.braking, layout.CustomHUDSettings.Braking);
            SetupSteamControls(newHUD.steam, layout.CustomHUDSettings.Steam);
            SetupCabControls(newHUD.cab, layout.CustomHUDSettings.Cab);
            SetupMechanicalControls(newHUD.mechanical, layout.CustomHUDSettings.Mechanical);

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
                    regulator.gameObject.SetActive(true);
                    Object.Destroy(newHUD.throttle.gameObject);
                    newHUD.throttle = regulator;
                    break;
                default:
                    // Keep throttle as is.
                    break;
            }


            switch (layout.TMOrOilTemp)
            {
                case BasicControls.Slot1A.None:
                    newHUD.tmTempMeter.gameObject.SetActive(false);
                    newHUD.oilTempMeter.gameObject.SetActive(false);
                    break;
                case BasicControls.Slot1A.OilTemp:
                    newHUD.tmTempMeter.gameObject.SetActive(false);
                    var oilTemp = Object.Instantiate(HudDM3.basicControls.oilTempMeter, newHUD.oilTempMeter.transform.parent);
                    oilTemp.gameObject.SetActive(true);
                    Object.Destroy(newHUD.oilTempMeter.gameObject);
                    newHUD.oilTempMeter = oilTemp;
                    break;
                default:
                    // Keep TM temp visible.
                    break;
            }

            switch (layout.Reverser)
            {
                case BasicControls.Slot1B.None:
                    newHUD.reverser.gameObject.SetActive(false);
                    break;
                case BasicControls.Slot1B.Cutoff:
                    var cutoff = Object.Instantiate(HudS060.basicControls.reverser, newHUD.reverser.transform.parent);
                    cutoff.gameObject.SetActive(true);
                    Object.Destroy(newHUD.reverser.gameObject);
                    newHUD.reverser = cutoff;
                    break;
                default:
                    // Keep reverser as is.
                    break;
            }


            if (layout.GearboxA == ShouldDisplay.Display)
            {
                newHUD.gearboxA.gameObject.SetActive(true);
            }


            if (layout.Speedometer == ShouldDisplay.None)
            {
                newHUD.speedMeter.gameObject.SetActive(false);
            }

            if (layout.GearboxB == ShouldDisplay.Display)
            {
                newHUD.gearboxB.gameObject.SetActive(true);
            }


            if (layout.RPM == ShouldDisplay.None)
            {
                newHUD.rpmMeter.gameObject.SetActive(false);
            }

            switch (layout.TurbineOrVoltage)
            {
                case BasicControls.Slot4B.TurbineRPM:
                    var turbine = Object.Instantiate(HudDH4.basicControls.turbineRpmMeter, newHUD.turbineRpmMeter.transform.parent);
                    turbine.gameObject.SetActive(true);
                    Object.Destroy(newHUD.turbineRpmMeter.gameObject);
                    newHUD.turbineRpmMeter = turbine;
                    break;
                case BasicControls.Slot4B.Voltage:
                    var voltage = Object.Instantiate(HudBE2.basicControls.voltageMeter, newHUD.voltageMeter.transform.parent);
                    voltage.gameObject.SetActive(true);
                    Object.Destroy(newHUD.voltageMeter.gameObject);
                    newHUD.voltageMeter = voltage;
                    break;
                default:
                    // Keep not displaying any.
                    break;
            }

            if (layout.Power == ShouldDisplay.Display)
            {
                newHUD.powerMeter.gameObject.SetActive(true);
            }


            if (layout.WheelslipIndicator == ShouldDisplay.None)
            {
                newHUD.wheelSlipIndicator.gameObject.SetActive(false);
            }

            if (layout.Sander == ShouldDisplay.None)
            {
                newHUD.sand.gameObject.SetActive(false);
            }
        }

        private static void SetupBrakeControls(HUDLocoControls.BrakingReferences newHUD, Braking layout)
        {

        }

        private static void SetupSteamControls(HUDLocoControls.SteamReferences newHUD, Steam layout)
        {

        }

        private static void SetupCabControls(HUDLocoControls.CabReferences newHUD, Cab layout)
        {

        }

        private static void SetupMechanicalControls(HUDLocoControls.MechanicalReferences newHUD, Mechanical layout)
        {

        }
    }
}
