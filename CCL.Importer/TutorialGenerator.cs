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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static DV.Tutorial.QT.QuickTutorialFactory;

namespace CCL.Importer
{
    internal static class TutorialGenerator
    {
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
            //c.Tutorial.AddStartingCheck(new LocoFuelCondition(0.05f, 1f, "tutorial/cond/requires_fuel", "tutorial/cond/requires_coal_and_water", "tutorial/cond/requires_power"));
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
            TrainCar? handbrake = settings.Handbrake >= 0 && trainset[settings.Handbrake].brakeSystem.hasHandbrake ?
                trainset[settings.Handbrake] : null;

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
                c.Tutorial.AddGlobalCheck(new TrainsetResourceAvailableCondition(trainset, resources, "tutorial/cond/loco_damaged"));
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

            // Reset controls on the whole trainset.
            c.BeginNewPhase();
            foreach (var item in trainset)
            {
                c.Phase.Add(new SwappedLocoResetStep(item));
            }

            ProcessCustomPhases(settings.CustomPhases1);

            // Get steam going.
            if (settings.PrepareSteam)
            {
                CCLPlugin.LogVerbose("Tutorial creation: prepare steam");

                // Basic information about steam locos.
                c.BeginNewPhase();
                c.AddPrompt("tutorial/loco/steam_startup_costs", pause: false);
                c.BeginNewPhase();
                c.AddLookAndAcknowledge(waterLevel,
                    LocalizationAPI.L("car/tut/water"), LocalizationAPI.L("tutorial/loco/water_meter"));
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Injector);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Blowdown);

                // Fill firebox.
                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(true, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0.8f, 1f, QTSemantic.Open, false);
                c.Phase.Add(new EquipAnyItemStep(shovels, LocalizationAPI.L("tutorial/loco/take_out_shovel")));

                var shovelPileSource = loco;
                // Find a ShovelCoalPile somewhere on the trainset if the loco doesn't have one.
                if (loco.loadedExternalInteractables?.GetComponentInChildren<ShovelCoalPile>() == null)
                {
                    foreach (var item in trainset)
                    {
                        if (item == loco) continue;

                        if (item.loadedExternalInteractables?.GetComponentInChildren<ShovelCoalPile>() != null)
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
                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0.8f, 1f, QTSemantic.Open, false);
                if (waterLevel != null &&
                    c.Controls.TryGetControl(InteriorControlsManager.ControlType.Injector, out var injector) &&
                    c.Controls.TryGetControl(InteriorControlsManager.ControlType.Blowdown, out var blowdown))
                {
                    c.Phase.Add(new BoilerWaterTweakStep(waterLevel, injector.controlImplBase, blowdown.controlImplBase, shouldRecheck: false));
                }

                // Light fire.
                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(true, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0.8f, 1f, QTSemantic.Open, shouldRecheck: false);
                c.Phase.Add(new EquipAnyItemStep(lighter, LocalizationAPI.L("tutorial/loco/take_out_lighter")));
                c.Phase.Add(new LightFireStep(loco, c.Overrider, true,
                    $"{LocalizationAPI.L("car/tut/firebox")}\n\n{LocalizationAPI.L("tutorial/loco/light_fire")}", fire));

                // Check fire temp.
                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0f, 0f, QTSemantic.Close, shouldRecheck: false);
                c.AddControl(InteriorControlsManager.ControlType.Blower, 1f, 1f, QTSemantic.FullyEngage);
                c.AddMonitorIndicator(fireTemp,
                    $"{LocalizationAPI.L("car/tut/firetemp")}\n{LocalizationAPI.L("tutorial/monitor_until", $"{settings.TargetFireTemperature} °C")}",
                    LocalizationAPI.L("tutorial/loco/ind_fire"), settings.TargetFireTemperature, float.PositiveInfinity, true, 3f);

                // Check boiler pressure.
                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Blower, 1f, 1f, QTSemantic.FullyEngage);
                c.AddMonitorIndicator(steam,
                    $"{LocalizationAPI.L("car/tut/boilerpressure")}\n{LocalizationAPI.L("tutorial/monitor_until", $"{settings.TargetSteamPressure} bar")}",
                    LocalizationAPI.L("tutorial/loco/ind_boiler_pressure"), settings.TargetSteamPressure + 1, float.PositiveInfinity, true, 3f);

                // Close fire door.
                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(false, true, true, false, false);
                c.AddControl(InteriorControlsManager.ControlType.Firedoor, 0f, 0f, QTSemantic.Close);
            }

            ProcessCustomPhases(settings.CustomPhases2);

            // Fill oiling points on all parts.
            if (settings.OilingPoints)
            {
                CCLPlugin.LogVerbose("Tutorial creation: oiling points");

                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(false, true, true, false, true);

                var oil = interiorIndicators?.transmissionOil ?? externalIndicators?.transmissionOil;
                c.AddAutomaticLubricatorStep(oil);
                c.AddLookAndAcknowledge(oil, LocalizationAPI.L("car/tut/oil_bearing"), LocalizationAPI.L("tutorial/loco/ind_oil_bearing"));

                if (settings.ShowTrainsetLubricators)
                {
                    foreach (var item in trainset)
                    {
                        if (item == loco) continue;

                        var icm = item.interior?.GetComponentInChildren<InteriorControlsManager>();

                        if (icm == null) continue;

                        c.BeginNewPhase();
                        SteamerDrivingBasicPrereq(false, true, true, false, true);
                        ReplaceReferences(item, icm);
                        oil = item.loadedInterior?.GetComponent<LocoIndicatorReader>()?.transmissionOil ??
                            item.loadedExternalInteractables?.GetComponent<LocoIndicatorReader>()?.transmissionOil;
                        c.AddAutomaticLubricatorStep(oil);
                    }

                    ResetReferences();
                }

                var controllers = trainset.Select(x => (x.loadedExternalInteractables?.GetComponentInChildren<OilingPointsPortController>(), x));

                List<(OilingPointPortFeederReader Reader, Vector3 Position, TrainCar Train)> points = new();
                Dictionary<OilingPointPortFeederReader, TrainCar> map = new();

                // Get all oiling points from all parts and their relative positions to the main loco.
                foreach ((var controller, var train) in controllers)
                {
                    if (controller == null || controller.entries == null || controller.entries.Length == 0) continue;

                    points.AddRange(controller.entries.Select(x => (x, loco.transform.InverseTransformPoint(x.transform.position), train)));
                }

                CCLPlugin.LogVerbose($"Oiling point total: {points.Count}");

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
                        c.BeginNewPhase();
                        SteamerDrivingBasicPrereq(false, true, true, false, true);
                        c.AddControl(control, 1f, 1f,
                            LocalizationAPI.L("car/tut/oiling_point"), LocalizationAPI.L("tutorial/control/oiling_point"), QTSemantic.Open);
                        c.Phase.Add(new EquipAnyItemStep(oiler, LocalizationAPI.L("tutorial/loco/take_out_oiler")));
                        c.AddRefillOilingPointStep(indicator, 1f);
                        c.BeginNewPhase();
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
                                c.BeginNewPhase();
                                SteamerDrivingBasicPrereq(false, true, true, false, true);
                                c.AddRefillOilingPointStep(indicator, 1f);
                            }
                        }
                    }
                }

                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(false, true, true, false, true);
                c.AddLookAndAcknowledge((interiorIndicators?.oil ?? externalIndicators?.oil),
                    LocalizationAPI.L("car/tut/oil"), LocalizationAPI.L("tutorial/loco/ind_oil_storage"));
            }

            ProcessCustomPhases(settings.CustomPhases3);

            if (settings.ShowBasicCabControls)
            {
                CCLPlugin.LogVerbose("Tutorial creation: cab controls");

                c.BeginNewPhase();
                SteamerDrivingBasicPrereq(true, true, true, false, true);

                if (!settings.HeadlightsBeforeCab)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndDashLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CabLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndCabLight, isSteamLoco: settings.MarkCabLightAsSteam);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Wipers);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers1);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers2);
                }

                // Allow users to customise target headlight positions.
                c.AddControl(InteriorControlsManager.ControlType.HeadlightsFront,
                    settings.FrontHeadlights.Minimum, settings.FrontHeadlights.Maximum, (QTSemantic)settings.FrontHeadlights.Semantic, false);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndHeadlightsTypeFront);
                c.AddControl(InteriorControlsManager.ControlType.IndHeadlightsTypeFront,
                    settings.FrontIndHeadlightsType.Minimum, settings.FrontIndHeadlightsType.Maximum, (QTSemantic)settings.FrontIndHeadlightsType.Semantic);
                c.AddControl(InteriorControlsManager.ControlType.IndHeadlights1Front, 
                    settings.FrontIndHeadlights1.Minimum, settings.FrontIndHeadlights1.Maximum, (QTSemantic)settings.FrontIndHeadlights1.Semantic, false);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndHeadlights2Front);
                c.AddControl(InteriorControlsManager.ControlType.HeadlightsRear,
                    settings.RearHeadlights.Minimum, settings.RearHeadlights.Maximum, (QTSemantic)settings.RearHeadlights.Semantic, false);
                c.AddControl(InteriorControlsManager.ControlType.IndHeadlightsTypeRear, 
                    settings.RearIndHeadlightsType.Minimum, settings.RearIndHeadlightsType.Maximum, (QTSemantic)settings.RearIndHeadlightsType.Semantic);
                c.AddControl(InteriorControlsManager.ControlType.IndHeadlights1Rear, 
                    settings.RearIndHeadlights1.Minimum, settings.RearIndHeadlights1.Maximum, (QTSemantic)settings.RearIndHeadlights1.Semantic, false);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndHeadlights2Rear);

                if (settings.HeadlightsBeforeCab)
                {
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndDashLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CabLight);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndCabLight, isSteamLoco: settings.MarkCabLightAsSteam);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Wipers);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers1);
                    c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.IndWipers2);
                }
            }

            ProcessCustomPhases(settings.CustomPhases4);

            if (settings.StartDieselEngine)
            {
                CCLPlugin.LogVerbose("Tutorial creation: start diesel engine");

                c.BeginNewPhase();
                DieselBasicPrereq(true, true);
                c.AddOverridableControl(InteriorControlsManager.ControlType.Reverser, 0.49f, 0.51f, QTSemantic.SetToNeutral);
                c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);
                c.AddOverridableControl(InteriorControlsManager.ControlType.DynamicBrake, 0f, 0f, QTSemantic.Disengage);
            }

            ProcessCustomPhases(settings.CustomPhases5);

            if (settings.ShowBrakes)
            {
                CCLPlugin.LogVerbose("Tutorial creation: brakes");

                c.BeginNewPhase();
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
                    c.BeginNewPhase();
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

            c.BeginNewPhase();
            SteamerDrivingBasicPrereq(false, false, true, true, true);

            // Some steps continue out of order.
            if (settings.PrepareSteam)
            {
                c.AddControl(InteriorControlsManager.ControlType.Blower, 0f, 0f, QTSemantic.Disengage);
            }

            if (settings.ShowSpeedometer)
            {
                c.AddLookAndAcknowledge(interiorIndicators?.speed ?? externalIndicators?.speed,
                    LocalizationAPI.L("car/tut/speedometer"), LocalizationAPI.L("tutorial/loco/ind_speed"));
            }

            if (settings.ShowSand)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Sander);
                c.AddLookAndAcknowledge(OverrideOrDefault(settings.SandOverride, interiorIndicators?.sand ?? externalIndicators?.sand),
                    LocalizationAPI.L("car/tut/sand"), LocalizationAPI.L("tutorial/loco/ind_sand"));
            }

            if (settings.PrepareSteam)
            {
                c.AddPutCoalIntoFireboxStep(firebox, 0.95f, true,
                    $"{LocalizationAPI.L("car/tut/firebox")}\n\n{LocalizationAPI.L("tutorial/loco/shovel_coal")}", fire);
            }

            c.BeginNewPhase();
            SteamerDrivingBasicPrereq(false, false, false, true, true);
            c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);

            if (settings.HasGearboxA)
            {
                c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxA, true);
            }
            if (settings.HasGearboxB)
            {
                c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxB, true);
            }

            ProcessCustomPhases(settings.CustomPhases6);

            // Moving the loco.
            if (settings.ShowMovement)
            {
                CCLPlugin.LogVerbose("Tutorial creation: movement");

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
                    c.BeginNewPhase();
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage, isSteamLoco: true);

                    // Disengage brakes.
                    c.BeginNewPhase();
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);

                    c.AddMonitorIndicator(interiorIndicators?.steamChest ?? externalIndicators?.steamChest,
                        $"{LocalizationAPI.L("car/tut/chestpressure")}\n{LocalizationAPI.L("tutorial/monitor_above", $"{settings.TargetChestPressure} bar")}",
                        LocalizationAPI.L("tutorial/loco/chest_pressure"), settings.TargetChestPressure + 1, float.PositiveInfinity, true, strictDismiss: true);

                    c.AddOverridableControl(InteriorControlsManager.ControlType.IndBrake, 0f, 0f, QTSemantic.Disengage);

                    // Stop the train again with the independent brake.
                    c.BeginNewPhase();
                    SteamerDrivingBasicPrereq(false, false, false, true, true);
                    c.AddVirtualHandbrakeControl(handbrake, 0f, 0f, QTSemantic.Disengage);
                    c.Phase.Add(new CarSpeedStep(loco, 1.0f, true));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.IndBrake, 0.8f, 1f, QTSemantic.Engage);

                    // Show info about water and cutoff, and reset the reverser.
                    c.BeginNewPhase();
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
                    if (settings.HasGearboxA)
                    {
                        c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxA, true);
                    }
                    if (settings.HasGearboxB)
                    {
                        c.AddManualGearShifting(InteriorControlsManager.ControlType.GearboxB, true);
                    }

                    // Engage throttle until movement is detected.
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0.05f, 1f, QTSemantic.GentlyEngage, false);
                    c.Phase.Add(new CarSpeedStep(loco, 1f, aboveTarget: true));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);

                    // Stop the loco.
                    c.BeginNewPhase();
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Throttle, 0f, 0f, QTSemantic.Disengage);
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrake, 0.5f, 1f, QTSemantic.Engage);
                    c.Phase.Add(new CarSpeedStep(loco, 1f, aboveTarget: false));
                    c.AddOverridableControl(InteriorControlsManager.ControlType.Reverser, 0.49f, 0.51f, QTSemantic.SetToNeutral);
                }
            }

            c.BeginNewPhase();

            if (settings.PrepareSteam)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Damper);
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.CoalDump);
                c.AddLookAndAcknowledge(waterLevel, LocalizationAPI.L("car/tut/water"), LocalizationAPI.L("tutorial/steam_water_warning"));
                c.AddLookAndAcknowledge(fire, LocalizationAPI.L("car/tut/firebox"), LocalizationAPI.L("tutorial/loco/steam_firebox_overfill"));
            }

            // Show gearboxes again. If it only has gearbox B, ensure it displays the gear thing on it.
            if (settings.HasGearboxA)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.GearboxA);
            }
            if (settings.HasGearboxB)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.GearboxB);
            }
            if (settings.HasGearboxA)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.GearboxA, LocalizationAPI.L("car/tut/gearboxa"), LocalizationAPI.L("tutorial/loco/gears_higher_speed"));
            }
            else if (settings.HasGearboxB)
            {
                c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.GearboxB, LocalizationAPI.L("car/tut/gearboxb"), LocalizationAPI.L("tutorial/loco/gears_higher_speed"));
            }

            ProcessCustomPhases(settings.CustomPhases7);

            CCLPlugin.LogVerbose("Tutorial creation: final bits");

            // Show some indicators and fuel cutoff.
            c.AddLookAndAcknowledge(c.Indicators?.amps, LocalizationAPI.L("car/tut/amperage"), LocalizationAPI.L("tutorial/loco/ind_amps"));
            c.AddLookAndAcknowledge(c.Indicators?.tmTemp, LocalizationAPI.L("car/tut/tmtemp"), LocalizationAPI.L("tutorial/loco/ind_tm_temp"));
            c.AddLookAndAcknowledge(c.Indicators?.oilTemp, LocalizationAPI.L("car/tut/oiltemp"), LocalizationAPI.L("tutorial/loco/ind_transmission_oil_temp"));
            c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.FuelCutoff);

            // Show final controls.
            c.BeginNewPhase();
            c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.DynamicBrake);
            c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Bell);
            c.AddLookAndAcknowledge(InteriorControlsManager.ControlType.Horn, null, isSteamLoco: settings.MarkHornAsSteam);

            // Show the rest of the indicators.
            // Indicators might exist for the HUD so they are all optional.
            // Oiling points already show oil so it must be checked.
            c.BeginNewPhase();
            if (settings.ShowWheelslip)
            {
                c.AddLookAndAcknowledge(OverrideOrDefault(settings.WheelslipOverride, c.Lamps?.wheelSlip),
                    LocalizationAPI.L("car/tut/wheelslip"), LocalizationAPI.L("tutorial/loco/ind_wheel_slip"));
            }
            c.AddLookAndAcknowledge(OverrideOrDefault(settings.BatteryOverride, interiorIndicators?.battery ?? externalIndicators?.battery),
                LocalizationAPI.L("car/tut/battery"), LocalizationAPI.L("tutorial/loco/ind_battery"));
            c.AddLookAndAcknowledge(OverrideOrDefault(settings.VoltageOverride, interiorIndicators?.voltage ?? externalIndicators?.voltage),
                LocalizationAPI.L("car/tut/voltage"), LocalizationAPI.L("tutorial/loco/ind_voltage"));
            c.AddLookAndAcknowledge(OverrideOrDefault(settings.FuelOverride, interiorIndicators?.fuel ?? externalIndicators?.fuel),
                LocalizationAPI.L("car/tut/fuel"), LocalizationAPI.L("tutorial/loco/ind_fuel"));
            if (!settings.OilingPoints && settings.ShowOil)
            {
                c.AddLookAndAcknowledge(OverrideOrDefault(settings.OilOverride, interiorIndicators?.oil ?? externalIndicators?.oil),
                    LocalizationAPI.L("car/tut/oil"), LocalizationAPI.L("tutorial/loco/ind_oil_engine"));
            }

            ProcessCustomPhases(settings.CustomPhases8);

            // Show water and coal storages for each vehicle.
            // Also show fuel sockets.
            for (int i = 0; i < trainset.Length; i++)
            {
                var car = trainset[i];
                var interior = car.loadedInterior?.GetComponent<LocoIndicatorReader>();
                var external = car.loadedExternalInteractables?.GetComponent<LocoIndicatorReader>();

                if (settings.CarsWithWater.Contains(i))
                {
                    c.AddLookAndAcknowledge(interior?.tenderWaterLevel ?? external?.tenderWaterLevel,
                        LocalizationAPI.L("car/tut/water"), LocalizationAPI.L("tutorial/loco/ind_storage_water"));
                }
                if (settings.CarsWithCoal.Contains(i))
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
            c.BeginNewPhase();
            c.AddPrompt("tutorial/loco/completed", pause: false);

            return c.Tutorial;

            void SteamerDrivingBasicPrereq(bool disengageWaterControls, bool openDamperControl, bool engageHandbrakeControl,
                bool openBrakeCutout, bool engageCompressorAndDynamo)
            {
                if (disengageWaterControls)
                {
                    c.AddControl(InteriorControlsManager.ControlType.Blowdown, 0f, 0f, QTSemantic.Disengage);
                    c.AddControl(InteriorControlsManager.ControlType.Injector, 0f, 0f, QTSemantic.Disengage);
                }

                if (openDamperControl)
                {
                    c.AddControl(InteriorControlsManager.ControlType.Damper, 1f, 1f, QTSemantic.Open);
                }

                if (engageHandbrakeControl)
                {
                    c.AddVirtualHandbrakeControl(handbrake, 1f, 1f, QTSemantic.FullyEngage);
                }

                if (openBrakeCutout)
                {
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrakeCutout, 0.9f, 1f, QTSemantic.Open);
                }

                if (engageCompressorAndDynamo)
                {
                    ReplaceReferencesByTrainset(settings.AirPump);
                    c.AddControl(InteriorControlsManager.ControlType.AirPump, 1f, 1f, QTSemantic.Engage);
                    ResetReferences();
                    ReplaceReferencesByTrainset(settings.Dynamo);
                    c.AddControl(InteriorControlsManager.ControlType.Dynamo, 1f, 1f, QTSemantic.Engage);
                    ResetReferences();
                }
            }

            void DieselBasicPrereq(bool includeHandbrakeApplied, bool includeStartedEngine)
            {
                if (includeHandbrakeApplied)
                {
                    c.AddVirtualHandbrakeControl(handbrake, 1f, 1f, QTSemantic.FullyEngage);
                }

                c.AddFuse(InteriorControlsManager.ControlType.ElectricsFuse, QTSemantic.Engage);
                c.AddFuse(InteriorControlsManager.ControlType.StarterFuse, QTSemantic.Engage);
                c.AddFuse(InteriorControlsManager.ControlType.TractionMotorFuse, QTSemantic.Engage);

                if (includeStartedEngine)
                {
                    c.AddOverridableControl(InteriorControlsManager.ControlType.TrainBrakeCutout, 0.9f, 1f, QTSemantic.Open);
                    c.AddEngineState(InteriorControlsManager.ControlType.StarterControl, true, (QTSemantic)settings.StarterSemantic);
                }
            }

            Behaviour? OverrideOrDefault(string overrideId, Behaviour? behaviour)
            {
                return customComps.TryGetValue(overrideId, out var id) ? id : behaviour;
            }

            void ProcessCustomPhases(List<TutorialPhase> phases)
            {
                foreach (var phase in phases)
                {
                    AddCustomPhase(phase);
                }
            }

            void AddCustomPhase(TutorialPhase phase)
            {
                c.BeginNewPhase();

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

            void ReplaceReferences(TrainCar car, InteriorControlsManager? icm = null)
            {
                c.Loco = car;
                c.Overrider = car.GetComponentInChildren<BaseControlsOverrider>(true);
                c.Controls = icm ?? car.interior.GetComponentInChildren<InteriorControlsManager>();
                c.Indicators = car.interior.GetComponentInChildren<LocoIndicatorReader>();
                c.Lamps = car.interior.GetComponentInChildren<LocoLampReader>();
            }

            void ReplaceReferencesByTrainset(int index)
            {
                if (index > -1)
                {
                    var icm = trainset[index].interior?.GetComponentInChildren<InteriorControlsManager>();

                    if (icm == null) return;

                    ReplaceReferences(trainset[index], icm);
                }
            }

            void ResetReferences()
            {
                c.Loco = loco;
                c.Overrider = overrider;
                c.Controls = controls;
                c.Indicators = indicators;
                c.Lamps = lamps;
            }
        }

        private static void SetupICMForCar(TrainCar car)
        {
            if (car.interior == null) return;
            var icm = car.interior.GetComponentInChildren<InteriorControlsManager>();
            if (icm == null || icm.Initialized) return;
            icm.SetBaseControlsReferences();
            icm.SetupControlReader(icm.GetComponent<LocoControlsReader>());
            icm.SetupControlReader(car.loadedExternalInteractables?.GetComponent<LocoControlsReader>());

            CCLPlugin.LogVerbose($"ICM controls: {string.Join(", ", icm.controls.Keys)}");
        }
    }
}
