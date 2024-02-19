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
            // Steal the DE6's HUD as a base.
            var newHUD = Object.Instantiate(HudDE6, Holder);

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
            // Slot 0.
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

            // Slot 1.
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

            // Slot 2.
            if (layout.GearboxA == ShouldDisplay.Display)
            {
                newHUD.gearboxA.gameObject.SetActive(true);
            }

            // Slot 3.
            if (layout.Speedometer == ShouldDisplay.None)
            {
                newHUD.speedMeter.gameObject.SetActive(false);
            }

            if (layout.GearboxB == ShouldDisplay.Display)
            {
                newHUD.gearboxB.gameObject.SetActive(true);
            }

            // Slot 4.
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

            // Slot 5.
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
            // Slot 7.
            if (layout.BrakePipe == ShouldDisplay.None)
            {
                newHUD.brakePipeMeter.gameObject.SetActive(false);
            }

            switch (layout.BrakeType)
            {
                case Braking.Slot7B.None:
                    newHUD.trainBrake.gameObject.SetActive(false);
                    break;
                case Braking.Slot7B.NonSelfLapping:
                    var brake = Object.Instantiate(HudDM3.braking.trainBrake, newHUD.trainBrake.transform.parent);
                    brake.gameObject.SetActive(true);
                    Object.Destroy(newHUD.trainBrake.gameObject);
                    newHUD.trainBrake = brake;
                    break;
                default:
                    // Keep the self-lapping brakes.
                    break;
            }

            // Slot 8.
            if (layout.MainReservoir == ShouldDisplay.None)
            {
                newHUD.mainResMeter.gameObject.SetActive(false);
            }

            if (layout.IndependentBrake == ShouldDisplay.None)
            {
                newHUD.indBrake.gameObject.SetActive(false);
            }

            // Slot 9.
            if (layout.BrakeCylinder == ShouldDisplay.None)
            {
                newHUD.brakeCylMeter.gameObject.SetActive(false);
            }

            if (layout.DynamicBrake == ShouldDisplay.None)
            {
                newHUD.dynBrake.gameObject.SetActive(false);
            }

            // Slot 10.
            if (layout.ReleaseCylinder == ShouldDisplay.None)
            {
                newHUD.releaseCyl.gameObject.SetActive(false);
            }

            if (layout.Handbrake == ShouldDisplay.None)
            {
                newHUD.handbrake.gameObject.SetActive(false);
            }
        }

        private static void SetupSteamControls(HUDLocoControls.SteamReferences newHUD, Steam layout)
        {
            // Slot 12.
            if (layout.SteamMeter == ShouldDisplay.Display)
            {
                newHUD.steamMeter.gameObject.SetActive(true);
            }

            if (layout.CylinderCocks == ShouldDisplay.Display)
            {
                newHUD.cylCock.gameObject.SetActive(true);
            }

            // Slot 13.
            if (layout.BoilerWater == ShouldDisplay.Display)
            {
                newHUD.locoWaterMeter.gameObject.SetActive(true);
            }

            if (layout.Injector == ShouldDisplay.Display)
            {
                newHUD.injector.gameObject.SetActive(true);
            }

            // Slot 14.
            if (layout.FireboxCoal == ShouldDisplay.Display)
            {
                newHUD.locoCoalMeter.gameObject.SetActive(true);
                newHUD.locoCoalMeter.transform.localPosition = HudS060.steam.locoCoalMeter.transform.localPosition;
            }

            if (layout.Damper == ShouldDisplay.Display)
            {
                newHUD.damper.gameObject.SetActive(true);
            }

            // Slot 15.
            if (layout.FireTemperature == ShouldDisplay.Display)
            {
                newHUD.fireTemp.gameObject.SetActive(true);
            }

            if (layout.Blower == ShouldDisplay.Display)
            {
                newHUD.blower.gameObject.SetActive(true);
            }

            // Slot 16.
            if (layout.Shovel == ShouldDisplay.Display)
            {
                newHUD.shovel.gameObject.SetActive(true);
            }

            if (layout.Firedoor == ShouldDisplay.Display)
            {
                newHUD.firedoor.gameObject.SetActive(true);
            }

            // Slot 17.
            if (layout.LightFirebox == ShouldDisplay.Display)
            {
                newHUD.lightFirebox.gameObject.SetActive(true);
                newHUD.lightFirebox.transform.localPosition = HudS060.steam.lightFirebox.transform.localPosition;
            }

            if (layout.Blowdown == ShouldDisplay.Display)
            {
                newHUD.blowdown.gameObject.SetActive(true);
            }

            // Slot 18.
            if (layout.FuelDump == ShouldDisplay.Display)
            {
                var dump = Object.Instantiate(HudS060.steam.coalDump, newHUD.coalDump.transform.parent);
                dump.gameObject.SetActive(true);
                Object.Destroy(newHUD.coalDump.gameObject);
                newHUD.coalDump = dump;
            }

            // Slot 19.
            if (layout.Dynamo == ShouldDisplay.Display)
            {
                newHUD.dynamo.gameObject.SetActive(true);
                newHUD.dynamo.transform.localPosition = HudS060.steam.dynamo.transform.localPosition;
            }

            if (layout.AirPump == ShouldDisplay.Display)
            {
                newHUD.airPump.gameObject.SetActive(true);
                newHUD.airPump.transform.localPosition = HudS060.steam.airPump.transform.localPosition;
            }

            if (layout.Lubricator == ShouldDisplay.Display)
            {
                newHUD.lubricator.gameObject.SetActive(true);
                newHUD.lubricator.transform.localPosition = HudS060.steam.lubricator.transform.localPosition;
            }
        }

        private static void SetupCabControls(HUDLocoControls.CabReferences newHUD, Cab layout)
        {

        }

        private static void SetupMechanicalControls(HUDLocoControls.MechanicalReferences newHUD, Mechanical layout)
        {
            // Slot 27.
            if (layout.Pantograph == ShouldDisplay.Display)
            {
                newHUD.pantograph.gameObject.SetActive(true);
            }

            if (layout.CabOrientation == ShouldDisplay.Display)
            {
                newHUD.cabOrient.gameObject.SetActive(true);
            }

            // Slot 28.
            if (layout.TMOfflineIndicator == ShouldDisplay.None)
            {
                newHUD.tmOfflineIndicator.gameObject.SetActive(false);
            }

            if (layout.StarterFuse == ShouldDisplay.None)
            {
                newHUD.starterFuse.gameObject.SetActive(false);
            }

            if (layout.ElectricsFuse == ShouldDisplay.None)
            {
                newHUD.electricsFuse.gameObject.SetActive(false);
            }

            if (layout.TractionMotorFuse == ShouldDisplay.None)
            {
                newHUD.tractionMotorFuse.gameObject.SetActive(false);
            }

            // Slot 29.
            if (layout.Alerter == ShouldDisplay.Display)
            {
                newHUD.alerter.gameObject.SetActive(true);
            }

            if (layout.Starter == ShouldDisplay.None)
            {
                newHUD.starterControl.gameObject.SetActive(false);
            }

            if (layout.FuelCutoff == ShouldDisplay.None)
            {
                newHUD.fuelCutoff.gameObject.SetActive(false);
            }
        }
    }
}
