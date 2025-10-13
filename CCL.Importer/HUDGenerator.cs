using CCL.Types.HUD;
using DV.ThingTypes;
using DV.UI.LocoHUD;
using UnityEngine;

using static CCL.Types.HUD.CustomHUDLayout;

namespace CCL.Importer
{
    internal static class HUDGenerator
    {
        // Unused HUD slots, to place overlapping controls.
        private const string Slot6 = "bg/Controls/HUDSlot (6)";
        private const string Slot11 = "bg/Controls/HUDSlot (11)";
        private const string Slot20 = "bg/Controls/HUDSlot (20)";

        private static Transform? s_holder;
        private static HUDLocoControls? s_hudDE2;
        private static HUDLocoControls? s_hudDE6;
        private static HUDLocoControls? s_hudDH4;
        private static HUDLocoControls? s_hudDM3;
        private static HUDLocoControls? s_hudS060;
        private static HUDLocoControls? s_hudS282;
        private static HUDLocoControls? s_hudBE2;
        private static HUDLocoControls? s_hudDM1U;
        private static HUDLocoControls? s_hudHandcar;

        private static Transform Holder => Extensions.GetCached(ref s_holder, CreateHolder);
        private static HUDLocoControls HudDE2 => Extensions.GetCached(ref s_hudDE2,
            () => GetHUD(QuickAccess.Locomotives.DE2));
        private static HUDLocoControls HudDE6 => Extensions.GetCached(ref s_hudDE6,
            () => GetHUD(QuickAccess.Locomotives.DE6));
        private static HUDLocoControls HudDH4 => Extensions.GetCached(ref s_hudDH4,
            () => GetHUD(QuickAccess.Locomotives.DH4));
        private static HUDLocoControls HudDM3 => Extensions.GetCached(ref s_hudDM3,
            () => GetHUD(QuickAccess.Locomotives.DM3));
        private static HUDLocoControls HudS060 => Extensions.GetCached(ref s_hudS060,
            () => GetHUD(QuickAccess.Locomotives.S060));
        private static HUDLocoControls HudS282 => Extensions.GetCached(ref s_hudS282,
            () => GetHUD(QuickAccess.Locomotives.S282A));
        private static HUDLocoControls HudBE2 => Extensions.GetCached(ref s_hudBE2,
            () => GetHUD(QuickAccess.Locomotives.Microshunter));
        private static HUDLocoControls HudDM1U => Extensions.GetCached(ref s_hudDM1U,
            () => GetHUD(QuickAccess.Locomotives.DM1U));
        private static HUDLocoControls HudHandcar => Extensions.GetCached(ref s_hudHandcar,
            () => GetHUD(QuickAccess.Locomotives.Handcar));

        private static Transform CreateHolder()
        {
            // Make a dummy to hold the HUD in inactive state.
            var hudHolder = new GameObject("[HUD holder]");
            hudHolder.SetActive(false);
            Object.DontDestroyOnLoad(hudHolder);

            return hudHolder.transform;
        }

        private static HUDLocoControls GetHUD(TrainCarLivery livery) => livery.parentType.hudPrefab.GetComponent<HUDLocoControls>();

        public static GameObject CreateHUD(VanillaHUDLayout layout)
        {
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
                case VanillaHUDLayout.BaseHUD.DM1U:
                    return HudDM1U.gameObject;
                case VanillaHUDLayout.BaseHUD.Handcar:
                    return HudHandcar.gameObject;
                case VanillaHUDLayout.BaseHUD.Custom:
                    return CreateCustomHUD(layout);
                default:
                    CCLPlugin.Error($"HUD configuration not supported ({layout.HUDType})");
                    return null!;
            }
        }

        private static GameObject CreateCustomHUD(VanillaHUDLayout layout)
        {
            // Steal the DE6's HUD as a base.
            var newHUD = Object.Instantiate(HudDE6, Holder);
            newHUD.text.powertrainTypeText.SetTextValue(layout.CustomHUDSettings.Powertrain);

            SetupBasicControls(newHUD, layout.CustomHUDSettings.BasicControls);
            SetupBrakeControls(newHUD.braking, layout.CustomHUDSettings.Braking);
            SetupSteamControls(newHUD.steam, layout.CustomHUDSettings.Steam);
            SetupCabControls(newHUD, layout.CustomHUDSettings);
            SetupMechanicalControls(newHUD.mechanical, layout.CustomHUDSettings.Mechanical);

            return newHUD.gameObject;
        }

        private static void SetupBasicControls(HUDLocoControls newHUDFull, BasicControls layout)
        {
            var newHUD = newHUDFull.basicControls;

            // Slot 0.
            SetDisplay(newHUD.ampMeter, layout.AmpMeter);

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
                case BasicControls.Slot1A.BothAlt:
                    newHUD.oilTempMeter = Object.Instantiate(HudDM3.basicControls.oilTempMeter, newHUD.gearboxA.transform.parent);
                    newHUD.oilTempMeter.gameObject.SetActive(true);
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
            SetDisplay(newHUD.gearboxA, layout.GearboxA);

            // Slot 3.
            SetDisplay(newHUD.speedMeter, layout.Speedometer);
            SetDisplay(newHUD.gearboxB, layout.GearboxB);

            // Slot 4.
            SetDisplay(newHUD.rpmMeter, layout.RPM);

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
                case BasicControls.Slot4B.BothAlt:
                    if (layout.RPM == ShouldDisplay.Display)
                    {
                        if (layout.Power == ShouldDisplay.Display)
                        {
                            // If both RPM and power are displayed, voltage gets moved out to slot 6.
                            turbine = Object.Instantiate(HudDH4.basicControls.turbineRpmMeter, newHUD.turbineRpmMeter.transform.parent);
                            voltage = Object.Instantiate(HudBE2.basicControls.voltageMeter, newHUDFull.transform.Find(Slot6));
                            voltage.transform.localPosition = HudDE6.basicControls.rpmMeter.transform.localPosition;
                        }
                        else
                        {
                            // If power is not displayed, move voltage down to its position.
                            turbine = Object.Instantiate(HudDH4.basicControls.turbineRpmMeter, newHUD.turbineRpmMeter.transform.parent);
                            voltage = Object.Instantiate(HudBE2.basicControls.voltageMeter, newHUD.voltageMeter.transform.parent);
                            voltage.transform.localPosition = HudDE6.basicControls.powerMeter.transform.localPosition;
                        }
                    }
                    else
                    {
                        // If the RPM isn't displayed, move the turbine RPM up.
                        turbine = Object.Instantiate(HudDH4.basicControls.turbineRpmMeter, newHUD.turbineRpmMeter.transform.parent);
                        turbine.transform.localPosition = HudDE6.basicControls.rpmMeter.transform.localPosition;
                        voltage = Object.Instantiate(HudBE2.basicControls.voltageMeter, newHUD.voltageMeter.transform.parent);
                    }
                    turbine.gameObject.SetActive(true);
                    Object.Destroy(newHUD.turbineRpmMeter.gameObject);
                    newHUD.turbineRpmMeter = turbine;
                    voltage.gameObject.SetActive(true);
                    Object.Destroy(newHUD.voltageMeter.gameObject);
                    newHUD.voltageMeter = voltage;
                    break;
                default:
                    // Keep not displaying any.
                    break;
            }

            SetDisplay(newHUD.powerMeter, layout.Power);

            // Slot 5.
            SetDisplay(newHUD.wheelSlipIndicator, layout.WheelslipIndicator);
            SetDisplay(newHUD.sand, layout.Sander);
        }

        private static void SetupBrakeControls(HUDLocoControls.BrakingReferences newHUD, Braking layout)
        {
            // Slot 7.
            SetDisplay(newHUD.brakePipeMeter, layout.BrakePipe);

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
            SetDisplay(newHUD.mainResMeter, layout.MainReservoir);
            SetDisplay(newHUD.indBrake, layout.IndependentBrake);

            // Slot 9.
            SetDisplay(newHUD.brakeCylMeter, layout.BrakeCylinder);
            SetDisplay(newHUD.dynBrake, layout.DynamicBrake);

            // Slot 10.
            SetDisplay(newHUD.releaseCyl, layout.ReleaseCylinder);
            SetDisplay(newHUD.handbrake, layout.Handbrake);

            // Slot 29.
            SetDisplay(newHUD.brakeCutout, layout.BrakeCutout);
        }

        private static void SetupSteamControls(HUDLocoControls.SteamReferences newHUD, Steam layout)
        {
            // Slot 12.
            SetDisplay(newHUD.steamMeter, layout.SteamMeter);
            SetDisplay(newHUD.cylCock, layout.CylinderCocks);

            // Slot 13.
            SetDisplay(newHUD.locoWaterMeter, layout.BoilerWater);
            SetDisplay(newHUD.injector, layout.Injector);

            // Slot 14.
            SetDisplayAndPosition(newHUD.locoCoalMeter, layout.FireboxCoal, HudS060.steam.locoCoalMeter);
            SetDisplay(newHUD.damper, layout.Damper);

            // Slot 15.
            SetDisplay(newHUD.fireTemp, layout.FireTemperature);
            SetDisplay(newHUD.blower, layout.Blower);

            // Slot 16.
            SetDisplay(newHUD.shovel, layout.Shovel);
            SetDisplay(newHUD.firedoor, layout.Firedoor);

            // Slot 17.
            SetDisplayAndPosition(newHUD.lightFirebox, layout.LightFirebox, HudS060.steam.lightFirebox);
            SetDisplay(newHUD.blowdown, layout.Blowdown);

            // Slot 18.
            SetDisplayAndPosition(newHUD.chestPressureMeter, layout.ChestPressure, HudS060.steam.chestPressureMeter);
            SetDisplay(newHUD.coalDump, layout.FuelDump);

            // Slot 19.
            SetDisplayAndPosition(newHUD.dynamo, layout.Dynamo, HudS060.steam.dynamo);
            SetDisplayAndPosition(newHUD.airPump, layout.AirPump, HudS060.steam.airPump);
            SetDisplayAndPosition(newHUD.lubricator, layout.Lubricator, HudS060.steam.lubricator);
        }

        private static void SetupCabControls(HUDLocoControls newHUDFull, CustomHUDLayout layoutFull)
        {
            var newHUD = newHUDFull.cab;
            var layout = layoutFull.Cab;

            // Slot 21
            switch (layout.FuelDisplay)
            {
                case Cab.Slot21A.None:
                    newHUD.fuelLevelMeter.gameObject.SetActive(false);
                    break;
                case Cab.Slot21A.BatteryLevel:
                    newHUD.fuelLevelMeter.gameObject.SetActive(false);
                    newHUD.batteryLevelMeter = Object.Instantiate(HudBE2.cab.batteryLevelMeter, newHUD.fuelLevelMeter.transform.parent);
                    newHUD.batteryLevelMeter.gameObject.SetActive(true);
                    break;
                case Cab.Slot21A.BothAlt:
                    newHUD.batteryLevelMeter = Object.Instantiate(HudBE2.cab.batteryLevelMeter, newHUDFull.transform.Find(Slot20));
                    newHUD.batteryLevelMeter.gameObject.SetActive(true);
                    break;
                default:
                    // Keep fuel display.
                    break;
            }

            switch (layout.Wipers)
            {
                case Cab.Slot21B.None:
                    newHUD.wipers.gameObject.SetActive(false);
                    break;
                case Cab.Slot21B.DM3Wipers:
                    newHUD.wipers.gameObject.SetActive(false);
                    newHUD.indWipers1.gameObject.SetActive(true);
                    newHUD.indWipers1.transform.localPosition = HudDM3.cab.indWipers1.transform.localPosition;
                    newHUD.indWipers2.gameObject.SetActive(true);
                    newHUD.indWipers2.transform.localPosition = HudDM3.cab.indWipers2.transform.localPosition;
                    break;
                default:
                    // Keep default wipers.
                    break;
            }

            // Slot 22
            SetDisplay(newHUD.oilLevelMeter, layout.OilLevel);

            switch (layout.CabLightStyle)
            {
                case Cab.Slot22B.None:
                    newHUD.cabLight.gameObject.SetActive(false);
                    newHUD.indCabLight.gameObject.SetActive(false);
                    newHUD.indDashLight.gameObject.SetActive(false);
                    break;
                case Cab.Slot22B.CabLightsSlider:
                    newHUD.cabLight.gameObject.SetActive(true);
                    newHUD.indCabLight.gameObject.SetActive(false);
                    newHUD.indDashLight.gameObject.SetActive(false);
                    break;
                default:
                    // Keep dash and cab light buttons.
                    break;
            }

            if (layout.GearLight == ShouldDisplay.Display)
            {
                newHUD.indCabLight = Object.Instantiate(HudS282.cab.indCabLight, newHUDFull.steam.dynamo.transform.parent);
                newHUD.indCabLight.gameObject.SetActive(true);
            }

            // Slot 23
            SetDisplay(newHUD.sandMeter, layout.SandLevel);

            switch (layout.Headlights1)
            {
                case Cab.Slot23B.None:
                    newHUD.headlightsFront.gameObject.SetActive(false);
                    break;
                case Cab.Slot23B.DM3Headlights:
                    newHUD.headlightsFront.gameObject.SetActive(false);
                    newHUD.indHeadlightsTypeFront = Object.Instantiate(HudDM3.cab.indHeadlightsTypeFront, newHUD.headlightsFront.transform.parent);
                    newHUD.indHeadlights1Front = Object.Instantiate(HudDM3.cab.indHeadlights1Front, newHUD.headlightsFront.transform.parent);
                    newHUD.indHeadlights2Front = Object.Instantiate(HudDM3.cab.indHeadlights2Front, newHUD.headlightsFront.transform.parent);
                    newHUD.indHeadlightsTypeFront.gameObject.SetActive(true);
                    newHUD.indHeadlights1Front.gameObject.SetActive(true);
                    newHUD.indHeadlights2Front.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            // Slot 24
            switch (layout.BellOrWater)
            {
                case Cab.Slot24A.None:
                    newHUD.bell.gameObject.SetActive(false);
                    break;
                case Cab.Slot24A.TenderWater:
                    newHUD.bell.gameObject.SetActive(false);
                    newHUDFull.steam.tenderWaterLevel = Object.Instantiate(HudS282.steam.tenderWaterLevel, newHUD.bell.transform.parent);
                    newHUDFull.steam.tenderWaterLevel.gameObject.SetActive(true);
                    break;
                default:
                    // Keep the bell button.
                    break;
            }

            switch (layout.Headlights2)
            {
                case Cab.Slot24B.None:
                    newHUD.headlightsRear.gameObject.SetActive(false);
                    break;
                case Cab.Slot24B.DM3Headlights:
                    newHUD.headlightsRear.gameObject.SetActive(false);
                    newHUD.indHeadlightsTypeRear = Object.Instantiate(HudDM3.cab.indHeadlightsTypeRear, newHUD.headlightsRear.transform.parent);
                    newHUD.indHeadlights1Rear = Object.Instantiate(HudDM3.cab.indHeadlights1Rear, newHUD.headlightsRear.transform.parent);
                    newHUD.indHeadlights2Rear = Object.Instantiate(HudDM3.cab.indHeadlights2Rear, newHUD.headlightsRear.transform.parent);
                    newHUD.indHeadlightsTypeRear.gameObject.SetActive(true);
                    newHUD.indHeadlights1Rear.gameObject.SetActive(true);
                    newHUD.indHeadlights2Rear.gameObject.SetActive(true);
                    break;
                case Cab.Slot24B.BellSlider:
                    newHUD.headlightsRear.gameObject.SetActive(false);
                    newHUD.bell = Object.Instantiate(HudS282.cab.bell, newHUD.bell.transform.parent);
                    newHUD.bell.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            // Slot 25
            if (layout.TenderCoal == ShouldDisplay.Display)
            {
                newHUDFull.steam.tenderCoalLevel = Object.Instantiate(HudS282.steam.tenderCoalLevel, newHUD.horn.transform.parent);
                newHUDFull.steam.tenderCoalLevel.gameObject.SetActive(true);
            }

            switch (layout.HornStyle)
            {
                case Cab.Slot25B.None:
                    newHUD.horn.gameObject.SetActive(false);
                    break;
                case Cab.Slot25B.Whistle:
                    newHUD.horn.gameObject.SetActive(false);
                    newHUD.horn = Object.Instantiate(HudS060.cab.horn, newHUD.horn.transform.parent);
                    newHUD.horn.gameObject.SetActive(true);
                    break;
                default:
                    // Keep horn.
                    break;
            }
        }

        private static void SetupMechanicalControls(HUDLocoControls.MechanicalReferences newHUD, Mechanical layout)
        {
            // Slot 26.
            SetDisplay(newHUD.alerter, layout.Alerter);

            // Slot 27.
            SetDisplay(newHUD.pantograph, layout.Pantograph);
            SetDisplay(newHUD.cabOrient, layout.CabOrientation);

            // Slot 28.
            SetDisplay(newHUD.tmOfflineIndicator, layout.TMOfflineIndicator);
            SetDisplay(newHUD.starterFuse, layout.StarterFuse);
            SetDisplay(newHUD.electricsFuse, layout.ElectricsFuse);
            SetDisplay(newHUD.tractionMotorFuse, layout.TractionMotorFuse);

            // Slot 29.
            SetDisplay(newHUD.starterControl, layout.Starter);
            SetDisplay(newHUD.fuelCutoff, layout.FuelCutoff);
        }

        private static void SetDisplay(LocoHUDControlBase control, ShouldDisplay display)
        {
            control.gameObject.SetActive(display == ShouldDisplay.Display);
        }

        private static void SetDisplayAndPosition(LocoHUDControlBase control, ShouldDisplay display, Vector3 localPosition)
        {
            if (display == ShouldDisplay.Display)
            {
                control.gameObject.SetActive(true);
                control.transform.localPosition = localPosition;
            }
            else
            {
                control.gameObject.SetActive(false);
            }
        }

        private static void SetDisplayAndPosition(LocoHUDControlBase control, ShouldDisplay display, LocoHUDControlBase localPosition)
        {
            SetDisplayAndPosition(control, display, localPosition.transform.localPosition);
        }
    }
}
