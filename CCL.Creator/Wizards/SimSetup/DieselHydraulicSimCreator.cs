﻿using CCL.Types;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Wheels;
using System.Collections.Generic;
using UnityEngine;

using static CCL.Types.Proxies.Controls.ControlBlockerProxy.BlockerDefinition;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselHydraulicSimCreator : SimCreator
    {
        public DieselHydraulicSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DH4" };

        public override IEnumerable<string> GetSimFeatures(int basisIndex)
        {
            yield return "Diesel Engine";
            yield return "3-Speed Hydraulic Transmission";
            yield return "Mechanical Compressor";
            yield return "Hydrodynamic Brake";
            yield return "Bell";
        }

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var brakeCut = CreateOverridableControl(OverridableControlType.TrainBrakeCutout);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            var dynBrake = CreateOverridableControl(OverridableControlType.DynamicBrake, "hydroDynamicBrake");

            var genericHornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            genericHornControl.defaultValue = 0;
            genericHornControl.smoothTime = 0.2f;
            var hornControl = CreateSibling<HornControlProxy>(genericHornControl);
            hornControl.portId = FullPortId(genericHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;

            var bellControl = CreateExternalControl("bellControl", true);
            var bell = CreateSimComponent<ElectricBellDefinitionProxy>("bell");
            bell.smoothDownTime = 0.05f;

            var lightsR = CreateOverridableControl(OverridableControlType.HeadlightsRear);
            var lightsF = CreateOverridableControl(OverridableControlType.HeadlightsFront);
            var cabLights = CreateOverridableControl(OverridableControlType.CabLight);

            var waterDetector = CreateWaterDetector();

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var engine = CreateDieselEngine(true);

            var loadTorque = CreateSimComponent<ConfigurableAddDefinitionProxy>("loadTorqueCalculator");
            loadTorque.aReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_0");
            loadTorque.bReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_1");
            loadTorque.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE_TOTAL");
            loadTorque.transform.parent = engine.transform;

            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var fluidCoupler = CreateSimComponent<HydraulicTransmissionDefinitionProxy>("fluidCoupler");
            var couplerExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(fluidCoupler);
            couplerExplosion.explosionPrefab = ExplosionPrefab.Hydraulic;
            couplerExplosion.bodyDamagePercentage = 0.1f;
            couplerExplosion.windowsBreakingDelay = 0.4f;
            couplerExplosion.explosionSignalPortId = FullPortId(fluidCoupler, "IS_BROKEN");

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("fcCooler");
            cooler.transform.parent = fluidCoupler.transform;
            var autoCooler = CreateSimComponent<AutomaticCoolerDefinitionProxy>("fcAutomaticCooler");
            autoCooler.transform.parent = fluidCoupler.transform;
            var coolant = CreateSimComponent<HeatReservoirDefinitionProxy>("coolant");
            coolant.transform.parent = fluidCoupler.transform;
            coolant.inputCount = 4;
            coolant.OnValidate();

            var poweredAxles = CreateSimComponent<ConfigurablePortDefinitionProxy>("poweredAxles");
            poweredAxles.value = 4;
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(poweredAxles, "NUM");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");
            wheelslip.engineBrakingActivePortId = FullPortId(fluidCoupler, "HYDRO_DYNAMIC_BRAKE_EFFECT");
            var directWheelslip = CreateSibling<DirectDriveMaxWheelslipRpmCalculatorProxy>(traction);
            directWheelslip.engineRpmMaxPortId = FullPortId(engine, "RPM");
            directWheelslip.gearRatioPortId = FullPortId(fluidCoupler, "GEAR_RATIO");

            // Fusebox and fuse connections.
            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false),
                new FuseDefinition("ENGINE_STARTER", false)
            };

            var dashLamp = CreateLampBasicControl("dashLamp", 0.2f);
            var cabLamp = CreateLampBasicControl("cabLightLamp", 0.3f);
            var fuelLamp = CreateLampDecreasingWarning("fuelLamp", DVPortValueType.FUEL, 1f, 0.25f, 0.05f, 0f);
            var oilLamp = CreateLampDecreasingWarning("oilLamp", DVPortValueType.OIL, 1f, 0.5f, 0.25f, 0f);
            var sandLamp = CreateLampDecreasingWarning("sandLamp", DVPortValueType.SAND, 1f, 0.5f, 0.25f, 0f);
            var lightsFLamp = CreateLampHeadlightControl("headlightsFLamp");
            var lightsRLamp = CreateLampHeadlightControl("headlightsRLamp");
            var sanderLamp = CreateLampBasicControl("sanderLamp");
            var rpmLamp = CreateLampOnOnly("engineRPMLamp", DVPortValueType.RPM, 0, 1, 1, float.PositiveInfinity, false, true);
            var coolerLamp = CreateLampOnOnly("oilActiveCoolderLamp", DVPortValueType.STATE, 0, 0.5f, 0.5f, float.PositiveInfinity);

            horn.powerFuseId = FullFuseId(fusebox, 0);
            bell.powerFuseId = FullFuseId(fusebox, 0);
            sander.powerFuseId = FullFuseId(fusebox, 0);
            engine.engineStarterFuseId = FullFuseId(fusebox, 1);
            autoCooler.powerFuseId = FullFuseId(fusebox, 0);
            dashLamp.powerFuseId = FullFuseId(fusebox, 0);
            cabLamp.powerFuseId = FullFuseId(fusebox, 0);
            fuelLamp.powerFuseId = FullFuseId(fusebox, 0);
            oilLamp.powerFuseId = FullFuseId(fusebox, 0);
            sandLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsFLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsRLamp.powerFuseId = FullFuseId(fusebox, 0);
            sanderLamp.powerFuseId = FullFuseId(fusebox, 0);
            rpmLamp.powerFuseId = FullFuseId(fusebox, 0);
            coolerLamp.powerFuseId = FullFuseId(fusebox, 0);

            // Damage.
            _damageController.mechanicalPTDamagerPortIds = new[]
            {
                FullPortId(engine, "GENERATED_ENGINE_DAMAGE"),
                FullPortId(fluidCoupler, "GENERATED_DAMAGE")
            };
            _damageController.mechanicalPTPercentualDamagerPortIds = new[]
            {
                FullPortId(engine, "GENERATED_ENGINE_PERCENTUAL_DAMAGE")
            };
            _damageController.mechanicalPTHealthStateExternalInPortIds = new[]
            {
                FullPortId(engine, "ENGINE_HEALTH_STATE_EXT_IN"),
                FullPortId(fluidCoupler, "MECHANICAL_PT_HEALTH_EXT_IN")
            };
            _damageController.mechanicalPTOffExternalInPortIds = new[]
            {
                FullPortId(engine, "COLLISION_ENGINE_OFF_EXT_IN")
            };

            // Port connections.
            ConnectPorts(fluidCoupler, "OUTPUT_SHAFT_TORQUE", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(horn, "HORN_CONTROL", genericHornControl, "CONTROL");
            ConnectPortRef(bell, "CONTROL", bellControl, "EXT_IN");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(engine, "THROTTLE", throttle, "EXT_IN");
            ConnectEmptyPortRef(engine, "RETARDER");
            ConnectPortRef(engine, "DRIVEN_RPM", fluidCoupler, "TURBINE_RPM");
            ConnectPortRef(engine, "INTAKE_WATER_CONTENT", waterDetector, "STATE_EXT_IN");
            ConnectPortRef(engine, "FUEL", fuel, "AMOUNT");
            ConnectPortRef(engine, "FUEL_CONSUMPTION", fuel, "CONSUME_EXT_IN");
            ConnectPortRef(engine, "OIL", oil, "AMOUNT");
            ConnectPortRef(engine, "OIL_CONSUMPTION", oil, "CONSUME_EXT_IN");
            ConnectPortRef(engine, "LOAD_TORQUE", loadTorque, "LOAD_TORQUE_TOTAL");
            ConnectPortRef(engine, "TEMPERATURE", coolant, "TEMPERATURE");

            ConnectPortRef(compressor, "ENGINE_RPM_NORMALIZED", engine, "RPM_NORMALIZED");

            ConnectPortRef(fluidCoupler, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(fluidCoupler, "HYDRODYNAMIC_BRAKE", dynBrake, "EXT_IN");
            ConnectPortRef(fluidCoupler, "REVERSER", reverser, "REVERSER");
            ConnectPortRef(fluidCoupler, "INPUT_SHAFT_RPM", engine, "RPM");
            ConnectPortRef(fluidCoupler, "MAX_RPM", engine, "MAX_RPM");
            ConnectPortRef(fluidCoupler, "OUTPUT_SHAFT_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(fluidCoupler, "TEMPERATURE", coolant, "TEMPERATURE");

            ConnectPortRef(cooler, "TEMPERATURE", coolant, "TEMPERATURE");
            ConnectEmptyPortRef(cooler, "TARGET_TEMPERATURE");
            ConnectPortRef(autoCooler, "IS_POWERED", engine, "ENGINE_ON");
            ConnectPortRef(autoCooler, "TEMPERATURE", coolant, "TEMPERATURE");
            ConnectEmptyPortRef(autoCooler, "TARGET_TEMPERATURE");

            ConnectPortRef(loadTorque, "LOAD_TORQUE_0", compressor, "LOAD_TORQUE");
            ConnectPortRef(loadTorque, "LOAD_TORQUE_1", fluidCoupler, "INPUT_SHAFT_TORQUE");

            ConnectHeatRef(coolant, 0, engine, "HEAT_OUT");
            ConnectHeatRef(coolant, 1, fluidCoupler, "HEAT_OUT");
            ConnectHeatRef(coolant, 2, cooler, "HEAT_OUT");
            ConnectHeatRef(coolant, 3, autoCooler, "HEAT_OUT");

            ConnectLampRef(dashLamp, cabLights, "EXT_IN");
            ConnectLampRef(cabLamp, cabLights, "EXT_IN");
            ConnectLampRef(fuelLamp, fuel, "NORMALIZED");
            ConnectLampRef(oilLamp, oil, "NORMALIZED");
            ConnectLampRef(sandLamp, sand, "NORMALIZED");
            ConnectLampRef(lightsFLamp, lightsF, "EXT_IN");
            ConnectLampRef(lightsRLamp, lightsR, "EXT_IN");
            ConnectLampRef(sanderLamp, sander, "CONTROL_EXT_IN");
            ConnectLampRef(rpmLamp, engine, "RPM_NORMALIZED");
            ConnectLampRef(coolerLamp, autoCooler, "COOLING_EFFECT");

            // Apply defaults.
            ApplyMethodToAll<IDH4Defaults>(s => s.ApplyDH4Defaults());

            // Control blockers.
            AddControlBlocker(throttle, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD, true)
                .blockedControlPortId = FullPortId(throttle, "EXT_IN");

            AddControlBlocker(reverser, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", 40, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", -40, BlockType.BLOCK_ON_BELOW_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");

            AddControlBlocker(dynBrake, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD, true);
            AddControlBlocker(dynBrake, reverser, "REVERSER", 0, BlockType.BLOCK_ON_EQUAL_TO_THRESHOLD, true)
                .blockedControlPortId = FullPortId(dynBrake, "EXT_IN");

            AddEmptyControlBlocker(indBrake);
            AddEmptyControlBlocker(lightsF);
            AddEmptyControlBlocker(lightsR);
        }
    }
}
