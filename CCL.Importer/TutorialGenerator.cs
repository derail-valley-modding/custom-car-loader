using CCL.Importer.Tutorial;
using CCL.Types.Tutorial;
using CCL.Types.Tutorial.Steps;
using DV.CabControls;
using DV.HUD;
using DV.Localization;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using DV.Tutorial.QT;
using LocoSim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static DV.Tutorial.QT.QuickTutorialFactory;

namespace CCL.Importer
{
    public static class TutorialGenerator
    {
        public static Dictionary<string, Func<TrainTutorialConstructor, TrainCar, QuickTutorial>> CustomTutorialImplementations = new();

        public static QuickTutorial Generate(TrainTutorialConstructor c, TrainCar loco, TutorialSetup settings)
        {
            var overrider = c.Overrider;
            var controls = c.Controls;
            var indicators = c.Indicators;
            var lamps = c.Lamps;
            var lighter = new[] { "lighter" };
            var shovels = new[] { "shovel", "ExpertShovel", "GoldenShovel" };
            var oiler = new[] { "Oiler" };

            CCLPlugin.LogVerbose("Tutorial creation: conditions");

            // Basic conditions.
            c.Tutorial.AddStartingCheck(new PlayerInLocoCondition("tutorial/cond/in_locomotive"));
            c.Tutorial.AddStartingCheck(new CarOnRailsCondition("tutorial/cond/loco_railed_start"));
            c.Tutorial.AddStartingCheck(new CarDamageCondition(0f, 0.5f, "tutorial/cond/loco_damaged"));
            c.Tutorial.AddStartingCheck(new CarSpeedCondition(0f, 0.1f, absolute: true, "tutorial/cond/loco_stationary"));
            c.Tutorial.AddStartingCheck(new CarGradeCondition(0f, 1f, "tutorial/cond/loco_grade"));

            c.Tutorial.AddStartingCheck(new TrainsetCompleteCondition(loco, Localization.Tutorials.TrainsetRequiredKey));

            // Optional conditions.
            if (settings.RequireSteamerItems)
            {
                c.Tutorial.AddStartingCheck(new AnyItemPresentCondition(lighter, "tutorial/cond/requires_lighter"));
                c.Tutorial.AddStartingCheck(new AnyItemPresentCondition(shovels, "tutorial/cond/requires_shovel"));
                c.Tutorial.AddStartingCheck(new AnyItemPresentCondition(oiler, "tutorial/cond/requires_oiler"));
            }

            // Cancel early.
            if (loco == null)
            {
                c.Tutorial.Add(new QuickTutorialPhase());
                return c.Tutorial;
            }

            CCLPlugin.LogVerbose("Tutorial creation: trainset preparation");

            // Prepare the array of the trainset always to make it easy.
            switch (CarManager.TryGetInstancedTrainset(loco, out var trainset))
            {
                case CarManager.TrainSetCompleteness.NotCCL:
                case CarManager.TrainSetCompleteness.NotComplete:
                    CCLPlugin.Log("Tutorial failed due to missing trainset");
                    c.Tutorial.Add(new QuickTutorialPhase());
                    return c.Tutorial;
                case CarManager.TrainSetCompleteness.NotPartOfTrainset:
                    trainset = new[] { loco };
                    break;
                default:
                    break;
            }

            // Get the override for the handbrake if set and is available.
            TrainCar? handbrake = settings.Controls.HandbrakeTrainsetOverride >= 0 && trainset[settings.Controls.HandbrakeTrainsetOverride].brakeSystem.hasHandbrake ?
                trainset[settings.Controls.HandbrakeTrainsetOverride] : null;

            // Keep every part of the trainset loaded.
            c.Tutorial.Add(new CarRangeWarningService(settings.MaximumDistance - 15.0f));
            c.Tutorial.AddGlobalCheck(new CarDistanceCondition(settings.MaximumDistance));
            foreach (var item in trainset)
            {
                if (!item.IsInteriorLoaded)
                {
                    item.LoadInterior();
                }
                if (!item.AreExternalInteractablesLoaded)
                {
                    item.LoadExternalInteractables();
                }

                SetupICMForCar(item);

                c.Tutorial.Add(new KeepTrainLODService(item));
                c.Tutorial.AddGlobalCheck(new CarOnRailsCondition("tutorial/fail/derailed", item));
                c.Tutorial.AddGlobalCheck(new CarEnabledCondition(item));

                if (handbrake == null && item.brakeSystem.hasHandbrake)
                {
                    handbrake = item;
                }
            }

            CCLPlugin.LogVerbose("Tutorial creation: resources");

            // This should in theory work for all arrangements CCL does with resources.
            if (settings.RequiredResources.Length > 0)
            {
                var resources = settings.RequiredResources.Select(x => ((ResourceContainerType)x, 0.1f)).ToArray();
                c.Tutorial.AddGlobalCheck(new TrainsetResourceAvailableCondition(trainset, resources));
            }

            c.Tutorial.AddGlobalCheck(new CarDamageCondition(0f, 0.5f, "tutorial/cond/loco_damaged"));

            var interiorIndicators = loco.loadedInterior?.GetComponent<LocoIndicatorReader>();
            var externalIndicators = loco.loadedExternalInteractables?.GetComponent<LocoIndicatorReader>();
            var fire = (interiorIndicators?.locoCoalLevel ?? externalIndicators?.locoCoalLevel)?.GetComponentInParent<Fire>()?.fireObj?.transform;
            var firebox = loco.SimController?.firebox;
            var waterLevel = interiorIndicators?.locoWaterLevel ?? externalIndicators?.locoWaterLevel;
            var fireTemp = interiorIndicators?.fireTemperature ?? externalIndicators?.fireTemperature;
            var steam = interiorIndicators?.steam ?? externalIndicators?.steam;

            // Prepare custom tutorial components.
            Dictionary<string, TutorialObjectID> customComps = new();
            foreach (var item in trainset)
            {
                foreach (var comp in item.GetComponentsInChildren<TutorialObjectID>(true))
                {
                    customComps.Add(comp.Id, comp);
                }

                if (item.loadedInterior != null)
                {
                    foreach (var comp in item.loadedInterior.GetComponentsInChildren<TutorialObjectID>(true))
                    {
                        customComps.Add(comp.Id, comp);
                    }
                }

                if (item.loadedExternalInteractables != null)
                {
                    foreach (var comp in item.loadedExternalInteractables.GetComponentsInChildren<TutorialObjectID>(true))
                    {
                        customComps.Add(comp.Id, comp);
                    }
                }
            }

            Dictionary<int, TutorialPhase> customPhases = settings.CustomPhases.ToDictionary(k => k.Number, v => v);

            // Reset controls on the whole trainset.
            BeginNewPhase("reset controls");
            foreach (var item in trainset)
            {
                c.Phase.Add(new SwappedLocoResetStep(item));
            }

            // Get steam going.
            if (settings.PrepareSteam)
            {
                // Basic information about steam locos.
                BeginNewPhase("steam startup cost");
                c.AddPrompt("tutorial/loco/steam_startup_costs", pause: false);
                BeginNewPhase("water level, injector, blowdown");
                c.AddLookAndAcknowledge(waterLevel,
                    LocalizationAPI.L("car/tut/water"), LocalizationAPI.L("tutorial/loco/water_meter"));
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Injector);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Blowdown);

                // Fill firebox.
                BeginNewPhase("fill firebox");
                SteamerDrivingBasicPrereq(true, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0.8f, 1f, QTSemantic.Open, false);
                c.Phase.Add(new EquipAnyItemStep(shovels, LocalizationAPI.L("tutorial/loco/take_out_shovel")));

                var shovelPileSource = loco;
                // Find a ShovelCoalPile somewhere on the trainset if the loco doesn't have one.
                if (!HasShovelPile(loco))
                {
                    foreach (var item in trainset)
                    {
                        if (item == loco) continue;

                        if (HasShovelPile(item))
                        {
                            shovelPileSource = item;
                            break;
                        }
                    }
                }

                c.AddTakeCoalStep(shovelPileSource, LocalizationAPI.L("car/tut/coal"), shouldRecheck: false);
                c.AddPutCoalIntoFireboxStep(firebox, 0.95f, true,
                    $"{LocalizationAPI.L("car/tut/firebox")}\n\n{LocalizationAPI.L("tutorial/loco/shovel_coal")}", fire);

                // Check water level.
                BeginNewPhase("check water level");
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0.8f, 1f, QTSemantic.Open, false);
                if (waterLevel != null &&
                    c.Controls.TryGetControl(InteriorControlsManager.ControlType.Injector, out var injector) &&
                    c.Controls.TryGetControl(InteriorControlsManager.ControlType.Blowdown, out var blowdown))
                {
                    c.Phase.Add(new BoilerWaterTweakStep(waterLevel, injector.controlImplBase, blowdown.controlImplBase, shouldRecheck: false));
                }

                // Light fire.
                BeginNewPhase("light fire");
                SteamerDrivingBasicPrereq(true, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0.8f, 1f, QTSemantic.Open, shouldRecheck: false);
                c.Phase.Add(new EquipAnyItemStep(lighter, LocalizationAPI.L("tutorial/loco/take_out_lighter")));
                c.Phase.Add(new LightFireStep(loco, c.Overrider, true,
                    $"{LocalizationAPI.L("car/tut/firebox")}\n\n{LocalizationAPI.L("tutorial/loco/light_fire")}", fire));

                // Check fire temp.
                BeginNewPhase("blower and fire temp");
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0f, 0f, QTSemantic.Close, shouldRecheck: false);
                c.AddControl(InteriorControlsManager.ControlType.Blower, 1f, 1f, QTSemantic.FullyEngage);
                c.AddMonitorIndicator(fireTemp,
                    $"{LocalizationAPI.L("car/tut/firetemp")}\n{LocalizationAPI.L("tutorial/monitor_until", $"{settings.TargetFireTemperature} °C")}",
                    LocalizationAPI.L("tutorial/loco/ind_fire"), settings.TargetFireTemperature, float.PositiveInfinity, true, 3f);

                // Check boiler pressure.
                BeginNewPhase("monitor pressure");
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Blower, 1f, 1f, QTSemantic.FullyEngage);
                c.AddMonitorIndicator(steam,
                    $"{LocalizationAPI.L("car/tut/boilerpressure")}\n{LocalizationAPI.L("tutorial/monitor_until", $"{settings.TargetSteamPressure} bar")}",
                    LocalizationAPI.L("tutorial/loco/ind_boiler_pressure"), settings.TargetSteamPressure + 1, float.PositiveInfinity, true, 3f);

                // Close firedoor.
                BeginNewPhase("ensure closed firedoor");
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0f, 0f, QTSemantic.Close);
            }

            // Fill oiling points on all parts.
            if (settings.OilingPoints)
            {
                BeginNewPhase("lubricator");
                SteamerDrivingBasicPrereq(false, true, true, false, true);

                var oil = interiorIndicators?.transmissionOil ?? externalIndicators?.transmissionOil;
                c.AddAutomaticLubricatorStep(oil);
                c.AddLookAndAcknowledge(oil, LocalizationAPI.L("car/tut/oil_bearing"), LocalizationAPI.L("tutorial/loco/ind_oil_bearing"));

                foreach (var item in trainset)
                {
                    if (item == loco) continue;

                    var icm = item.interior?.GetComponentInChildren<InteriorControlsManager>();

                    if (icm == null) continue;

                    BeginNewPhase("lubricator (trainset)");
                    SteamerDrivingBasicPrereq(false, true, true, false, true);
                    ReplaceReferences(item, icm);
                    oil = item.loadedInterior?.GetComponent<LocoIndicatorReader>()?.transmissionOil ??
                        item.loadedExternalInteractables?.GetComponent<LocoIndicatorReader>()?.transmissionOil;
                    c.AddAutomaticLubricatorStep(oil);
                }

                ResetReferences();

                var controllers = trainset.Select(x => (x.loadedExternalInteractables?.GetComponentInChildren<OilingPointsPortController>(), x));

                List<(OilingPointPortFeederReader Reader, Vector3 Position, TrainCar Train)> points = new();
                Dictionary<OilingPointPortFeederReader, TrainCar> map = new();

                // Get all oiling points from all parts and their relative positions to the main loco.
                foreach ((var controller, var train) in controllers)
                {
                    if (controller == null || controller.entries == null || controller.entries.Length == 0) continue;

                    points.AddRange(controller.entries.Select(x => (x, loco.transform.InverseTransformPoint(x.transform.position), train)));
                }

                // If there are oiling points...
                if (points.Count > 0)
                {
                    foreach (var (Reader, Position, Train) in points)
                    {
                        map[Reader] = Train;
                    }

                    // Order all from rear right to front right, then front left to rear left.
                    var left = points.Where(x => x.Position.x < 0).OrderByDescending(x => x.Position.z).Select(x => x.Reader);
                    var right = points.Where(x => x.Position.x >= 0).OrderBy(x => x.Position.z).Select(x => x.Reader);
                    var orderedPoints = new List<OilingPointPortFeederReader>();
                    orderedPoints.AddRange(right);
                    orderedPoints.AddRange(left);

                    var first = orderedPoints.FirstOrDefault();
                    var control = first?.GetComponent<ControlImplBase>();
                    var indicator = first?.transform.parent.GetComponentInChildren<Indicator>();

                    // If there is a control and an indicator on that oiling point, we can proceed.
                    if (first != null && control != null && indicator != null)
                    {
                        ReplaceReferences(map[first]);

                        // Show how to open, fill and close the first oiling point.
                        BeginNewPhase("first oiling point and oiler");
                        SteamerDrivingBasicPrereq(false, true, true, false, true);
                        c.AddControl(control, 1f, 1f,
                            LocalizationAPI.L("car/tut/oiling_point"), LocalizationAPI.L("tutorial/control/oiling_point"), QTSemantic.Open);
                        c.Phase.Add(new EquipAnyItemStep(oiler, LocalizationAPI.L("tutorial/loco/take_out_oiler")));
                        c.AddRefillOilingPointStep(indicator, 1f);
                        BeginNewPhase("close first oiling point");
                        SteamerDrivingBasicPrereq(false, true, true, false, true);
                        c.AddControl(control, 0f, 0f,
                            LocalizationAPI.L("car/tut/oiling_point"), LocalizationAPI.L("tutorial/control/oiling_point"), QTSemantic.Close);

                        // For all others, just make sure they are filled, no need to tell about opening and closing.
                        foreach (var item in orderedPoints.Skip(1))
                        {
                            ReplaceReferences(map[item]);
                            indicator = item?.transform.parent.GetComponentInChildren<Indicator>();

                            if (indicator != null)
                            {
                                BeginNewPhase("extra oiling point");
                                SteamerDrivingBasicPrereq(false, true, true, false, true);
                                c.AddRefillOilingPointStep(indicator, 1f);
                            }
                        }
                    }
                }

                BeginNewPhase("oil storage");
                SteamerDrivingBasicPrereq(false, true, true, false, true);
                c.AddLookAndAcknowledge(interiorIndicators?.oil ?? externalIndicators?.oil,
                    LocalizationAPI.L("car/tut/oil"), LocalizationAPI.L("tutorial/loco/ind_oil_storage"));
            }

            if (settings.ShowBasicCabControls)
            {
                BeginNewPhase("cab controls");
                SteamerDrivingBasicPrereq(true, true, true, false, true);
                DieselBasicPrereq(true, false);

                if (!settings.Controls.HeadlightsBeforeCabLight)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndDashLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CabLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndCabLight, isSteamLoco: settings.CabLightIsGearLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Wipers);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers1);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers2);
                }

                // Allow users to customise target headlight positions.
                AddControlOverride(settings.Controls.FrontHeadlights, settings.Controls.FrontHeadlightsSettings,
                    InteriorControlsManager.ControlType.HeadlightsFront, false, false, settings.Controls.SingleHeadlightControl);

                if (settings.Controls.FrontIndHeadlightsType.Show)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndHeadlightsTypeFront);
                }
                AddControlOverride(settings.Controls.FrontIndHeadlightsType, settings.Controls.FrontIndHeadlightsTypeSettings,
                    InteriorControlsManager.ControlType.IndHeadlightsTypeFront, false, false, settings.Controls.SingleHeadlightControl);
                AddControlOverride(settings.Controls.FrontIndHeadlights1, settings.Controls.FrontIndHeadlights1Settings,
                    InteriorControlsManager.ControlType.IndHeadlights1Front, false, false, settings.Controls.SingleHeadlightControl);
                if (settings.Controls.FrontIndHeadlights2.Show)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndHeadlights2Front);
                }

                AddControlOverride(settings.Controls.RearHeadlights, settings.Controls.RearHeadlightsSettings,
                    InteriorControlsManager.ControlType.HeadlightsRear, false, false, settings.Controls.SingleHeadlightControl);
                AddControlOverride(settings.Controls.RearIndHeadlightsType, settings.Controls.RearIndHeadlightsTypeSettings,
                    InteriorControlsManager.ControlType.IndHeadlightsTypeRear, false, false, settings.Controls.SingleHeadlightControl);
                AddControlOverride(settings.Controls.RearIndHeadlights1, settings.Controls.RearIndHeadlights1Settings,
                    InteriorControlsManager.ControlType.IndHeadlights1Rear, false, false, settings.Controls.SingleHeadlightControl);
                if (settings.Controls.RearIndHeadlights2.Show)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndHeadlights2Rear);
                }

                if (settings.Controls.HeadlightsBeforeCabLight)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndDashLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CabLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndCabLight, isSteamLoco: settings.CabLightIsGearLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Wipers);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers1);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers2);
                }
            }

            if (settings.StartDieselEngine)
            {
                BeginNewPhase("start diesel engine");
                DieselBasicPrereq(true, true);
                c.AddOverridableControl(InteriorControlsManager.ControlType.Reverser, 0.49f, 0.51f, QTSemantic.SetToNeutral);
                c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);
                c.AddOverridableControl(InteriorControlsManager.ControlType.DynamicBrake, 0f, 0f, QTSemantic.Disengage);
            }

            if (settings.ShowBrakes)
            {
                BeginNewPhase("brakes");
                SteamerDrivingBasicPrereq(false, false, true, true, true);
                c.AddMonitorIndicator(interiorIndicators?.mainReservoir ?? externalIndicators?.mainReservoir,
                    $"{LocalizationAPI.L("car/tut/mainres")}\n{LocalizationAPI.L("tutorial/monitor_until", "2 bar")}",
                    LocalizationAPI.L("tutorial/loco/int_main_res"), 3f, float.PositiveInfinity, true, 3f);

                if (settings.EngageThrottleForBrakeCharging)
                {
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0.05f, 1f, QTSemantic.GentlyEngage, false);
                    c.AddLookAndAcknowledge(interiorIndicators?.engineRpm ?? externalIndicators?.engineRpm,
                        LocalizationAPI.L("car/tut/rpm"), LocalizationAPI.L("tutorial/loco/ind_rpm"));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);
                    BeginNewPhase("diesel check after brake RPM");
                    DieselBasicPrereq(false, true);
                }

                bool addedIndBrake = c.AddOverridableControl(InteriorControlsManager.ControlType.IndBrake, 0.3f, 1f, QTSemantic.Engage, false);
                if (!addedIndBrake)
                {
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrake, 0.3f, 1f, QTSemantic.Engage, false);
                }

                c.AddLookAndAcknowledge(interiorIndicators?.brakeCylinder ?? externalIndicators?.brakeCylinder,
                    LocalizationAPI.L("car/tut/brakecyl"), LocalizationAPI.L("tutorial/loco/brake_red_needle"));

                if (addedIndBrake)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.TrainBrake);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrake, 0f, 0f, QTSemantic.Disengage);
                }

                c.AddLookAndAcknowledge(interiorIndicators?.brakePipe ?? externalIndicators?.brakePipe,
                    LocalizationAPI.L("car/tut/brakepipe"), LocalizationAPI.L("tutorial/loco/brake_black_needle"));
            }

            BeginNewPhase("turn off blower, speedometer, sander, refill firebox");
            SteamerDrivingBasicPrereq(false, false, true, true, true);

            // Some steps continue out of order.
            if (settings.PrepareSteam)
            {
                c.AddControl(InteriorControlsManager.ControlType.Blower, 0f, 0f, QTSemantic.Disengage);
            }

            if (settings.Indicators.Speedometer.Show)
            {
                c.AddLookAndAcknowledge(interiorIndicators?.speed ?? externalIndicators?.speed,
                    LocalizationAPI.L("car/tut/speedometer"), LocalizationAPI.L("tutorial/loco/ind_speed"));
            }

            if (settings.Indicators.Sand.Show)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Sander);
                c.AddLookAndAcknowledge(OverrideOrDefault(settings.Indicators.Sand.Override, interiorIndicators?.sand ?? externalIndicators?.sand),
                    LocalizationAPI.L("car/tut/sand"), LocalizationAPI.L("tutorial/loco/ind_sand"));
            }

            if (settings.PrepareSteam)
            {
                c.AddPutCoalIntoFireboxStep(firebox, 0.95f, true,
                    $"{LocalizationAPI.L("car/tut/firebox")}\n\n{LocalizationAPI.L("tutorial/loco/shovel_coal")}", fire);
            }

            BeginNewPhase("engage gears, start movement");
            SteamerDrivingBasicPrereq(false, false, false, true, true);
            c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);

            if (settings.Controls.GearboxA.Show)
            {
                c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxA, true);
            }
            if (settings.Controls.GearboxB.Show)
            {
                c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxB, true);
            }

            // Moving the loco.
            if (settings.ShowMovement)
            {
                if (settings.TreatAsSteam)
                {
                    // Engage throttle, show steam chest.
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrake, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Reverser, 0.9f, 1f, QTSemantic.GoForward, isSteamLoco: true);
                    c.AddControl(InteriorControlsManager.ControlType.CylCock, 1f, 1f, QTSemantic.Open);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0.05f, 1f, QTSemantic.GentlyEngage, isSteamLoco: true);

                    c.AddMonitorIndicatorNoMessage(interiorIndicators?.steamChest ?? externalIndicators?.steamChest,
                        settings.TargetChargedChestPressure, float.PositiveInfinity);

                    // Disengage throttle.
                    BeginNewPhase("disengage throttle");
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage, isSteamLoco: true);

                    // Disengage brakes.
                    BeginNewPhase("disengage brakes");
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);

                    c.AddMonitorIndicator(interiorIndicators?.steamChest ?? externalIndicators?.steamChest,
                        $"{LocalizationAPI.L("car/tut/chestpressure")}\n{LocalizationAPI.L("tutorial/monitor_above", $"{settings.TargetChestPressure} bar")}",
                        LocalizationAPI.L("tutorial/loco/chest_pressure"), settings.TargetChestPressure + 1, float.PositiveInfinity, true, strictDismiss: true);

                    c.AddOverridableControl(InteriorControlsManager.ControlType.IndBrake, 0f, 0f, QTSemantic.Disengage);

                    // Stop the train again with the independent brake.
                    BeginNewPhase("stop train");
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);
                    c.Phase.Add(new CarSpeedStep(loco, 1.0f, true));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.IndBrake, 0.8f, 1f, QTSemantic.Engage);

                    // Show info about water and cutoff, and reset the reverser.
                    BeginNewPhase("water in cylinder and cutoff info");
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.IndBrake, 0.8f, 1f, QTSemantic.Engage);
                    c.AddControl(InteriorControlsManager.ControlType.CylCock, 0f, 0f, QTSemantic.Close);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CylCock, LocalizationAPI.L("car/tut/cylcock"), LocalizationAPI.L("tutorial/loco/water_in_cylinders"));
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Reverser, LocalizationAPI.L("car/tut/cutoff"), LocalizationAPI.L("tutorial/loco/cutoff_higher_speed"));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Reverser, 0.4f, 0.6f, QTSemantic.SetToNeutral, isSteamLoco: true);
                }
                else
                {
                    // Disengage brakes.
                    c.AddControl(InteriorControlsManager.ControlType.Handbrake, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(loco.brakeSystem.hasIndependentBrake ?
                        InteriorControlsManager.ControlType.IndBrake :
                        InteriorControlsManager.ControlType.TrainBrake,
                        0f, 0f, QTSemantic.Disengage);

                    // Ensure gearboxes are engaged.
                    if (settings.Controls.GearboxA.Show)
                    {
                        c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxA, true);
                    }
                    if (settings.Controls.GearboxB.Show)
                    {
                        c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxB, true);
                    }

                    // Engage throttle until movement is detected.
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0.05f, 1f, QTSemantic.GentlyEngage, false);
                    c.Phase.Add(new CarSpeedStep(loco, 1f, aboveTarget: true));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);

                    // Stop the loco.
                    BeginNewPhase("stop train");
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrake, 0.5f, 1f, QTSemantic.Engage);
                    c.Phase.Add(new CarSpeedStep(loco, 1f, aboveTarget: false));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Reverser, 0.49f, 0.51f, QTSemantic.SetToNeutral);
                }
            }

            BeginNewPhase("final steam controls, gearboxes, amps, TM and oil temps, fuel cutoff");

            if (settings.PrepareSteam)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Damper);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CoalDump);
                c.AddLookAndAcknowledge(waterLevel, LocalizationAPI.L("car/tut/water"), LocalizationAPI.L("tutorial/steam_water_warning"));
                c.AddLookAndAcknowledge(fire, LocalizationAPI.L("car/tut/firebox"), LocalizationAPI.L("tutorial/loco/steam_firebox_overfill"));
            }

            // Show gearboxes again. If it only has gearbox B, ensure it displays the gear thing on it.
            if (settings.Controls.GearboxA.Show)
            {
                AddLookAndAcknowledgeControlOverride(settings.Controls.GearboxA.Override, InteriorControlsManager.ControlType.GearboxA);
            }
            if (settings.Controls.GearboxB.Show)
            {
                AddLookAndAcknowledgeControlOverride(settings.Controls.GearboxB.Override, InteriorControlsManager.ControlType.GearboxB);
            }
            if (settings.Controls.GearboxA.Show)
            {
                AddLookAndAcknowledgeControlOverrideWithNames(settings.Controls.GearboxA.Override,
                    InteriorControlsManager.ControlType.GearboxA, "car/tut/gearboxa", "tutorial/loco/gears_higher_speed");
            }
            else if (settings.Controls.GearboxB.Show)
            {
                AddLookAndAcknowledgeControlOverrideWithNames(settings.Controls.GearboxB.Override,
                    InteriorControlsManager.ControlType.GearboxB, "car/tut/gearboxb", "tutorial/loco/gears_higher_speed");
            }

            // Show some indicators and fuel cutoff.
            if (settings.Indicators.Amps.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.Amps.Override, interiorIndicators?.amps ?? externalIndicators?.amps,
                    "car/tut/amperage", "tutorial/loco/ind_amps");
            }
            if (settings.Indicators.TMTemp.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.TMTemp.Override, interiorIndicators?.tmTemp ?? externalIndicators?.tmTemp,
                    "car/tut/tmtemp", "tutorial/loco/ind_tm_temp");
            }
            if (settings.Indicators.OilTemp.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.OilTemp.Override, interiorIndicators?.oilTemp ?? externalIndicators?.oilTemp,
                    "car/tut/oiltemp", "tutorial/loco/ind_transmission_oil_temp");
            }
            if (settings.Controls.FuelCutoff.Show)
            {
                AddLookAndAcknowledgeControlOverride(settings.Controls.FuelCutoff.Override,
                    InteriorControlsManager.ControlType.FuelCutoff);
            }

            // Show final controls.
            BeginNewPhase("dynamic brake, bell, horn/whistle");
            if (settings.Controls.DynamicBrake.Show)
            {
                AddLookAndAcknowledgeControlOverride(settings.Controls.DynamicBrake.Override,
                    InteriorControlsManager.ControlType.DynamicBrake);
            }
            if (settings.Controls.Bell.Show)
            {
                AddLookAndAcknowledgeControlOverride(settings.Controls.Bell.Override,
                    InteriorControlsManager.ControlType.Bell);
            }
            if (settings.Controls.Horn.Show)
            {
                AddLookAndAcknowledgeControlOverride(settings.Controls.Horn.Override,
                    InteriorControlsManager.ControlType.Horn, isSteamLoco: settings.Controls.MarkHornAsWhistle);
            }

            // Show the rest of the indicators.
            // Indicators might exist for the HUD so they are all optional.
            // Oiling points already show oil so it must be checked.
            BeginNewPhase("final indicators, resource storage");
            if (settings.Indicators.Wheelslip.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.Wheelslip.Override, c.Lamps?.wheelSlip,
                    "car/tut/wheelslip", "tutorial/loco/ind_wheel_slip");
            }
            if (settings.Indicators.Battery.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.Battery.Override, interiorIndicators?.battery ?? externalIndicators?.battery,
                    "car/tut/battery", "tutorial/loco/ind_battery");
            }
            if (settings.Indicators.Voltage.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.Voltage.Override, interiorIndicators?.voltage ?? externalIndicators?.voltage,
                    "car/tut/voltage", "tutorial/loco/ind_voltage");
            }
            if (settings.Indicators.Fuel.Show)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.Fuel.Override, interiorIndicators?.fuel ?? externalIndicators?.fuel,
                    "car/tut/fuel", "tutorial/loco/ind_fuel");
            }
            if (settings.Indicators.Oil.Show && !settings.OilingPoints)
            {
                AddLookAndAcknowledgeOverride(settings.Indicators.Oil.Override, interiorIndicators?.oil ?? externalIndicators?.oil,
                    "car/tut/oil", "tutorial/loco/ind_oil_engine");
            }

            // Show water and coal storages for each vehicle.
            // Also show fuel sockets.
            for (int i = 0; i < trainset.Length; i++)
            {
                var car = trainset[i];
                var interior = car.loadedInterior?.GetComponent<LocoIndicatorReader>();
                var external = car.loadedExternalInteractables?.GetComponent<LocoIndicatorReader>();

                if (settings.TrainsetIndicesWithWater.Contains(i))
                {
                    c.AddLookAndAcknowledge(interior?.tenderWaterLevel ?? external?.tenderWaterLevel,
                        LocalizationAPI.L("car/tut/water"), LocalizationAPI.L("tutorial/loco/ind_storage_water"));
                }
                if (settings.TrainsetIndicesWithCoal.Contains(i))
                {
                    c.AddLookAndAcknowledge(interior?.tenderCoalLevel ?? external?.tenderCoalLevel,
                        LocalizationAPI.L("car/tut/coal"), LocalizationAPI.L("tutorial/loco/ind_storage_coal"));
                }

                foreach (var socket in car.FuelSockets)
                {
                    switch (socket.connectionTag)
                    {
                        case "diesel-hose":
                            c.AddLookAndAcknowledge(socket, LocalizationAPI.L("car/tut/dieselhoseslot"), LocalizationAPI.L("tutorial/loco/fuel_hose_slot"));
                            break;
                        case "electric-charge-cable":
                            c.AddLookAndAcknowledge(socket, LocalizationAPI.L("car/tut/electriccableslot"), LocalizationAPI.L("tutorial/loco/electric_cable_slot"));
                            break;
                        default:
                            break;
                    }
                }
            }

            // Finally over...
            BeginNewPhase("complete tutorial");
            c.AddPrompt("tutorial/loco/completed", pause: false);

            return c.Tutorial;

            // Default requirements.
            void SteamerDrivingBasicPrereq(bool disengageWaterControls, bool openDamperControl, bool engageHandbrakeControl,
                bool openBrakeCutout, bool engageCompressorAndDynamo)
            {
                if (engageHandbrakeControl)
                {
                    c.AddVirtualHandbrakeControl(handbrake, 1f, 1f, QTSemantic.FullyEngage);
                }

                if (!settings.IncludeSteamerPrerequisites) return;

                if (disengageWaterControls)
                {
                    c.AddControl(InteriorControlsManager.ControlType.Blowdown, 0f, 0f, QTSemantic.Disengage);
                    c.AddControl(InteriorControlsManager.ControlType.Injector, 0f, 0f, QTSemantic.Disengage);
                }

                if (openDamperControl)
                {
                    c.AddControl(InteriorControlsManager.ControlType.Damper, 1f, 1f, QTSemantic.Open);
                }

                if (openBrakeCutout)
                {
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrakeCutout, 0.9f, 1f, QTSemantic.Open);
                }

                if (engageCompressorAndDynamo)
                {
                    AddControlOverride(settings.Controls.AirPump,
                        new TutorialSetup.SemanticRange(TutorialSetup.QTSemantic.Engage, 1, 1), InteriorControlsManager.ControlType.AirPump);
                    AddControlOverride(settings.Controls.Dynamo,
                        new TutorialSetup.SemanticRange(TutorialSetup.QTSemantic.Engage, 1, 1), InteriorControlsManager.ControlType.Dynamo);
                }
            }

            void DieselBasicPrereq(bool includeHandbrakeApplied, bool includeStartedEngine)
            {
                if (includeHandbrakeApplied)
                {
                    c.AddVirtualHandbrakeControl(handbrake, 1f, 1f, QTSemantic.FullyEngage);
                }

                if (!settings.IncludeDieselPrerequisites) return;

                c.AddFuse(InteriorControlsManager.ControlType.ElectricsFuse, QTSemantic.Engage);
                c.AddFuse(InteriorControlsManager.ControlType.StarterFuse, QTSemantic.Engage);
                c.AddFuse(InteriorControlsManager.ControlType.TractionMotorFuse, QTSemantic.Engage);

                if (includeStartedEngine)
                {
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrakeCutout, 0.9f, 1f, QTSemantic.Open);
                    c.AddEngineState(InteriorControlsManager.ControlType.StarterControl, true, (QTSemantic)settings.StarterSemantic);
                }
            }

            // Custom phases.
            void BeginNewPhase(string name)
            {
                while (customPhases.TryGetValue(c.Tutorial.phases.Count, out var custom))
                {
                    AddCustomPhase(custom);
                }

                c.BeginNewPhase();
                CCLPlugin.LogVerbose($"Tutorial phase {PhaseDisplay()}: {name}");
            }

            void AddCustomPhase(TutorialPhase phase)
            {
                c.BeginNewPhase();
                CCLPlugin.LogVerbose($"Tutorial phase {PhaseDisplay()}: {phase.Name} (custom)");

                SteamerDrivingBasicPrereq(phase.DisengageWaterControls, phase.OpenDamperControl,
                    phase.EngageHandbrakeControl, phase.OpenBrakeCutout, phase.EngageCompressorAndDynamo);
                DieselBasicPrereq(phase.EngageHandbrakeControl, phase.IncludeStartedEngine);

                foreach (var item in phase.Steps)
                {
                    switch (item)
                    {
                        case LookAndAcknowledgeStep step:
                            c.AddLookAndAcknowledge(customComps[step.TargetId],
                                LocalizationAPI.L(step.NameKey), LocalizationAPI.L(step.DescriptionKey));
                            break;
                        case ControlStep step:
                            c.AddControl(customComps[step.TargetId].GetComponent<ControlImplBase>(), step.TargetValueMin, step.TargetValueMax,
                                LocalizationAPI.L(step.NameKey), LocalizationAPI.L(step.DescriptionKey), (QTSemantic)step.Semantic, step.ShouldRecheck);
                            break;
                        case IndicatorStep step:
                            c.AddMonitorIndicator(customComps[step.TargetId].GetComponent<Indicator>(),
                                $"{LocalizationAPI.L(step.NameKey)}\n{LocalizationAPI.L(IndicatorStep.ModeToKey(step.Mode), step.Value)}",
                                LocalizationAPI.L(step.DescriptionKey),
                                step.MinValue, step.MaxValue, step.ManualDismiss);
                            break;
                        case CCL.Types.Tutorial.Steps.PromptStep step:
                            c.AddPrompt(step.Key, step.Pause);
                            break;
                        default:
                            break;
                    }
                }
            }

            // Reference hacking.
            void ReplaceReferences(TrainCar car, InteriorControlsManager? icm = null)
            {
                c.Loco = car;
                c.Overrider = car.GetComponentInChildren<BaseControlsOverrider>(true);
                c.Controls = icm ?? car.interior.GetComponentInChildren<InteriorControlsManager>();
                c.Indicators = car.interior.GetComponentInChildren<LocoIndicatorReader>();
                c.Lamps = car.interior.GetComponentInChildren<LocoLampReader>();
            }

            void ResetReferences()
            {
                c.Loco = loco;
                c.Overrider = overrider;
                c.Controls = controls;
                c.Indicators = indicators;
                c.Lamps = lamps;
            }

            // Replicate original methods but with override ability.
            void AddControlOverride(TutorialSetup.OverridableObject overrider, TutorialSetup.SemanticRange range,
                InteriorControlsManager.ControlType type, bool shouldRecheck = true, bool isSteamLoco = false, bool singleHeadlights = false)
            {
                // No show, so no work.
                if (!overrider.Show) return;

                // If an ID is specified...
                if (!string.IsNullOrEmpty(overrider.Override))
                {
                    // And that ID has a matching comp and control...
                    if (customComps.TryGetValue(overrider.Override, out var obj) && obj.TryGetComponent(out ControlImplBase control))
                    {
                        // Get the name and description for the control type.
                        var message = GetDescriptionFor(type, (QTSemantic)range.Semantic, control, isSteamLoco, singleHeadlights);
                        c.AddControl(control, range.Minimum, range.Maximum, message.controlName, message.controlDescription, (QTSemantic)range.Semantic, shouldRecheck);
                        return;
                    }

                    CCLPlugin.Error($"Error creating tutorial for {loco.carLivery.id}: " +
                        $"missing ID {overrider.Override} or no control in object '{obj.name}' for {type} control, using default");
                }

                // Fall back to the original method.
                c.AddControl(type, range.Minimum, range.Maximum, (QTSemantic)range.Semantic);
            }

            void AddLookAndAcknowledgeOverride(string overrideId, Behaviour? behaviour, string nameKey, string typeKey)
            {
                c.AddLookAndAcknowledge(OverrideOrDefault(overrideId, behaviour),
                    LocalizationAPI.L(nameKey), LocalizationAPI.L(typeKey));
            }

            void AddLookAndAcknowledgeControlOverride(string overrideId, InteriorControlsManager.ControlType type, bool isSteamLoco = false)
            {
                if (!string.IsNullOrEmpty(overrideId))
                {
                    if (customComps.TryGetValue(overrideId, out var obj))
                    {
                        c.AddLookAndAcknowledge(obj.transform, GetDescriptionForLook(type, isSteamLoco));
                        return;
                    }

                    CCLPlugin.Error($"Error creating tutorial for {loco.carLivery.id}: " +
                        $"missing ID {overrideId} for {type} look, using default");
                }

                c.AddLookAndAcknowledge(type, isSteamLoco: isSteamLoco);
            }

            void AddLookAndAcknowledgeControlOverrideWithNames(string overrideId, InteriorControlsManager.ControlType type,
                string nameKey, string typeKey, bool isSteamLoco = false)
            {
                if (!string.IsNullOrEmpty(overrideId))
                {
                    if (customComps.TryGetValue(overrideId, out var obj))
                    {
                        c.AddLookAndAcknowledge(obj.transform,
                            new ControlIconQuickTutorialMessage(LocalizationAPI.L(nameKey), LocalizationAPI.L(typeKey), 2));
                        return;
                    }

                    CCLPlugin.Error($"Error creating tutorial for {loco.carLivery.id}: " +
                        $"missing ID {overrideId} for {type} look, using default");
                }

                c.AddLookAndAcknowledge(type, LocalizationAPI.L(nameKey), LocalizationAPI.L(typeKey), isSteamLoco: isSteamLoco);
            }

            Behaviour? OverrideOrDefault(string overrideId, Behaviour? behaviour)
            {
                return customComps.TryGetValue(overrideId, out var id) ? id : behaviour;
            }

            int PhaseDisplay() => c.Tutorial.phases.Count - 1;
        }

        private static void SetupICMForCar(TrainCar car)
        {
            if (car.interior == null) return;
            var icm = car.interior.GetComponentInChildren<InteriorControlsManager>();
            if (icm == null || icm.Initialized) return;
            icm.SetBaseControlsReferences();
            icm.SetupControlReader(icm.GetComponent<LocoControlsReader>());
            icm.SetupControlReader(car.loadedExternalInteractables?.GetComponent<LocoControlsReader>());

            CCLPlugin.LogVerbose($"ICM controls {car.carLivery.id}: {string.Join(", ", icm.controls.Keys)}");
        }

        private static ControlIconQuickTutorialMessage GetDescriptionFor(InteriorControlsManager.ControlType type, QTSemantic semantic,
            ControlImplBase control, bool isSteamLoco = false, bool singleHeadlights = false)
        {
            (string Name, string Description) tuple;
            if (isSteamLoco)
            {
                tuple = type switch
                {
                    InteriorControlsManager.ControlType.Reverser => (LocalizationAPI.L("car/tut/cutoff"), LocalizationAPI.L("tutorial/control/cutoff")),
                    InteriorControlsManager.ControlType.Throttle => (LocalizationAPI.L("car/tut/regulator"), LocalizationAPI.L("tutorial/control/regulator")),
                    InteriorControlsManager.ControlType.Horn => (LocalizationAPI.L("car/tut/whistle"), LocalizationAPI.L("tutorial/control/horn")),
                    InteriorControlsManager.ControlType.IndCabLight => (LocalizationAPI.L("car/tut/gearlight"), LocalizationAPI.L("tutorial/control/gear_light")),
                    _ => (LocalizationAPI.L("car/tut/" + type.ToString().ToLower()), LocalizationAPI.L("tutorial/control/" + type.ToString().ToLower())),
                };
            }
            else
            {
                tuple.Name = (type == InteriorControlsManager.ControlType.HeadlightsFront && singleHeadlights)
                    ? LocalizationAPI.L("car/tut/headlights")
                    : LocalizationAPI.L("car/tut/" + type.ToString().ToLower());

                switch (type)
                {
                    case InteriorControlsManager.ControlType.IndHeadlightsTypeFront:
                    case InteriorControlsManager.ControlType.IndHeadlightsTypeRear:
                        tuple.Description = LocalizationAPI.L("tutorial/control/indheadlightstype");
                        break;
                    case InteriorControlsManager.ControlType.IndHeadlights1Front:
                    case InteriorControlsManager.ControlType.IndHeadlights1Rear:
                        tuple.Description = LocalizationAPI.L("tutorial/control/indheadlights1");
                        break;
                    case InteriorControlsManager.ControlType.IndHeadlights2Front:
                    case InteriorControlsManager.ControlType.IndHeadlights2Rear:
                        tuple.Description = LocalizationAPI.L("tutorial/control/indheadlights2");
                        break;
                    default:
                        tuple.Description = LocalizationAPI.L("tutorial/control/" + type.ToString().ToLower());
                        break;
                }
            }

            var message = new ControlIconQuickTutorialMessage(tuple.Name, tuple.Description).WithSprite(null, control, semantic);
            if (type == InteriorControlsManager.ControlType.Handbrake)
            {
                message.spriteIndex = 6;
            }

            return message;
        }

        private static ControlIconQuickTutorialMessage GetDescriptionForLook(InteriorControlsManager.ControlType type,
            bool isSteamLoco = false, bool singleHeadlights = false)
        {
            (string Name, string Description) tuple;
            if (isSteamLoco)
            {
                tuple = type switch
                {
                    InteriorControlsManager.ControlType.Reverser => (LocalizationAPI.L("car/tut/cutoff"), LocalizationAPI.L("tutorial/control/cutoff")),
                    InteriorControlsManager.ControlType.Throttle => (LocalizationAPI.L("car/tut/regulator"), LocalizationAPI.L("tutorial/control/regulator")),
                    InteriorControlsManager.ControlType.Horn => (LocalizationAPI.L("car/tut/whistle"), LocalizationAPI.L("tutorial/control/horn")),
                    InteriorControlsManager.ControlType.IndCabLight => (LocalizationAPI.L("car/tut/gearlight"), LocalizationAPI.L("tutorial/control/gear_light")),
                    _ => (LocalizationAPI.L("car/tut/" + type.ToString().ToLower()), LocalizationAPI.L("tutorial/control/" + type.ToString().ToLower())),
                };
            }
            else
            {
                tuple.Name = (type == InteriorControlsManager.ControlType.HeadlightsFront && singleHeadlights)
                    ? LocalizationAPI.L("car/tut/headlights")
                    : LocalizationAPI.L("car/tut/" + type.ToString().ToLower());

                switch (type)
                {
                    case InteriorControlsManager.ControlType.IndHeadlightsTypeFront:
                    case InteriorControlsManager.ControlType.IndHeadlightsTypeRear:
                        tuple.Description = LocalizationAPI.L("tutorial/control/indheadlightstype");
                        break;
                    case InteriorControlsManager.ControlType.IndHeadlights1Front:
                    case InteriorControlsManager.ControlType.IndHeadlights1Rear:
                        tuple.Description = LocalizationAPI.L("tutorial/control/indheadlights1");
                        break;
                    case InteriorControlsManager.ControlType.IndHeadlights2Front:
                    case InteriorControlsManager.ControlType.IndHeadlights2Rear:
                        tuple.Description = LocalizationAPI.L("tutorial/control/indheadlights2");
                        break;
                    default:
                        tuple.Description = LocalizationAPI.L("tutorial/control/" + type.ToString().ToLower());
                        break;
                }
            }

            var message = new ControlIconQuickTutorialMessage(tuple.Name, tuple.Description, 2);
            return message;
        }

        private static bool HasShovelPile(TrainCar car)
        {
            return (car.loadedInterior != null && car.loadedInterior.GetComponentInChildren<ShovelCoalPile>()) ||
                (car.loadedExternalInteractables != null && car.loadedExternalInteractables.GetComponentInChildren<ShovelCoalPile>());
        }
    }
}
