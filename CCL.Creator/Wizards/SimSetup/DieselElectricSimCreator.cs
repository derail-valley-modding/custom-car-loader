using CCL.Types.Proxies;
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
    internal class DieselElectricSimCreator : SimCreator
    {
        public DieselElectricSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DE2", "DE6" };

        public override IEnumerable<string> GetSimFeatures(int basisIndex)
        {
            yield return "Diesel Engine";

            switch (basisIndex)
            {
                case 0:
                    yield return "2 Traction Motors";
                    break;
                case 1:
                    yield return "6 Traction Motors";
                    break;
                default:
                    break;
            }

            yield return "Traction Motors";
            yield return "Mechanical Compressor";

            if (HasDynamicBrake(basisIndex)) yield return "Dynamic Brake";
            if (HasBell(basisIndex)) yield return "Bell";
        }

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtPowr = CreateSimComponent<ThrottleGammaPowerConversionDefinitionProxy>("throttlePower");
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var brakeCut = CreateOverridableControl(OverridableControlType.TrainBrakeCutout);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            var dynBrake = HasDynamicBrake(basisIndex) ? CreateOverridableControl(OverridableControlType.DynamicBrake) : null;

            var cabLight = CreateOverridableControl(OverridableControlType.IndCabLight);
            var dashLight = CreateSimComponent<ExternalControlDefinitionProxy>("dashLight");
            dashLight.saveState = true;

            // Headlights.
            var lightsR = CreateOverridableControl(OverridableControlType.HeadlightsRear, defaultValue: 0.4f);
            var lightsF = CreateOverridableControl(OverridableControlType.HeadlightsFront, defaultValue: 0.4f);

            var genericHornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            genericHornControl.defaultValue = 0.5f;
            genericHornControl.smoothTime = 0.2f;
            var hornControl = CreateSibling<HornControlProxy>(genericHornControl);
            hornControl.portId = FullPortId(genericHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;
            ReparentComponents(genericHornControl, horn);

            ExternalControlDefinitionProxy? bellControl = null;
            ElectricBellDefinitionProxy? bell = null;

            if (HasBell(basisIndex))
            {
                bellControl = CreateExternalControl("bellControl", true);
                bell = CreateSimComponent<ElectricBellDefinitionProxy>("bell");
                bell.smoothDownTime = 0.5f;
                ReparentComponents(bellControl, bell);
            }

            var waterDetector = CreateWaterDetector();

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var engine = CreateDieselEngine(false);

            var loadTorque = CreateSimComponent<ConfigurableAddDefinitionProxy>("loadTorqueCalculator");
            loadTorque.aReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_0");
            loadTorque.bReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_1");
            loadTorque.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE_TOTAL");
            ReparentComponents(engine, loadTorque);

            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var tractionGen = CreateSimComponent<TractionGeneratorDefinitionProxy>("tractionGenerator");
            var slugPowerCalc = CreateSimComponent<SlugsPowerCalculatorDefinitionProxy>("slugsPowerCalculator");
            var slugPowerProv = CreateSlugsPowerProviderModule(tractionGen, slugPowerCalc);
            ReparentComponents(tractionGen, slugPowerCalc);

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");
            CreateTMsExtras(tm, out var deadTMs, out var tmExplosion);

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("tmPassiveCooler");
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("tmHeat");
            heat.inputCount = 2;
            heat.OnValidate();
            ReparentComponents(tm, cooler, heat);

            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            tmRpm.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            tmRpm.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO");
            tmRpm.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TM_RPM");

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(tm, "WORKING_TRACTION_MOTORS");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");
            wheelslip.engineBrakingActivePortId = FullPortId(tm, "DYNAMIC_BRAKE_ACTIVE");

            // Fusebox and fuse connections.
            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false),
                new FuseDefinition("ENGINE_STARTER", false),
                new FuseDefinition("TM_POWER", false),
            };

            var dashLamp = CreateLampBasicControl("dashLamp", 0.4f);
            var gaugesLamp = CreateLampBasicControl("gaugesLamp", 0.4f);
            var cabLamp = CreateLampBasicControl("cabLightLamp", 0.4f);
            var bellLamp = bell != null ? CreateLampBasicControl("bellLamp") : null;
            var fuelLamp = CreateLampDecreasingWarning("fuelLamp", DVPortValueType.FUEL, 1f, 0.25f, 0.125f, 0f);
            var oilLamp = CreateLampDecreasingWarning("oilLamp", DVPortValueType.OIL, 1f, 0.4f, 0.2f, 0f);
            var sandLamp = CreateLampDecreasingWarning("sandLamp", DVPortValueType.SAND, 1f, 0.1f, 0.05f, 0f);
            var lightsRLamp = CreateLampHeadlightControl("headlightsRLamp");
            var lightsFLamp = CreateLampHeadlightControl("headlightsFLamp");
            var sanderLamp = CreateLampBasicControl("sanderLamp");
            var tmOffLamp = CreateLamp("tmOfflineLamp", DVPortValueType.STATE, 0.1f, 1f, -1f, -0.1f, -0.1f, 0.1f);
            var rpmLamp = CreateLampOnOnly("engineRPMLamp", DVPortValueType.RPM, 0, 1, 1, float.PositiveInfinity, false, true);
            var ampLamp = CreateAmpLamp(basisIndex);

            horn.powerFuseId = FullFuseId(fusebox, 0);
            if (bell != null) bell.powerFuseId = FullFuseId(fusebox, 0);
            sander.powerFuseId = FullFuseId(fusebox, 0);
            engine.engineStarterFuseId = FullFuseId(fusebox, 1);
            tractionGen.powerFuseId = FullFuseId(fusebox, 2);
            tm.powerFuseId = FullFuseId(fusebox, 2);
            deadTMs.tmFuseId = FullFuseId(fusebox, 2);
            slugPowerProv.powerFuseId = FullFuseId(fusebox, 2);
            dashLamp.powerFuseId = FullFuseId(fusebox, 0);
            gaugesLamp.powerFuseId = FullFuseId(fusebox, 0);
            cabLamp.powerFuseId = FullFuseId(fusebox, 0);
            if (bellLamp != null) bellLamp.powerFuseId = FullFuseId(fusebox, 0);
            fuelLamp.powerFuseId = FullFuseId(fusebox, 0);
            oilLamp.powerFuseId = FullFuseId(fusebox, 0);
            sandLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsRLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsFLamp.powerFuseId = FullFuseId(fusebox, 0);
            sanderLamp.powerFuseId = FullFuseId(fusebox, 0);
            tmOffLamp.powerFuseId = FullFuseId(fusebox, 0);
            rpmLamp.powerFuseId = FullFuseId(fusebox, 0);
            ampLamp.powerFuseId = FullFuseId(fusebox, 0);

            // Damage.
            _damageController.mechanicalPTDamagerPortIds = new[] { FullPortId(engine, "GENERATED_ENGINE_DAMAGE") };
            _damageController.mechanicalPTPercentualDamagerPortIds = new[] { FullPortId(engine, "GENERATED_ENGINE_PERCENTUAL_DAMAGE") };
            _damageController.mechanicalPTHealthStateExternalInPortIds = new[] { FullPortId(engine, "ENGINE_HEALTH_STATE_EXT_IN") };
            _damageController.mechanicalPTOffExternalInPortIds = new[] { FullPortId(engine, "COLLISION_ENGINE_OFF_EXT_IN") };

            _damageController.electricalPTDamagerPortIds = new[] { FullPortId(tm, "GENERATED_DAMAGE") };
            _damageController.electricalPTHealthStateExternalInPortIds = new[] { FullPortId(tm, "HEALTH_STATE_EXT_IN") };

            // Port connections.
            ConnectPorts(tm, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(thrtPowr, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(thrtPowr, "IDLE_RPM_NORMALIZED", engine, "IDLE_RPM_NORMALIZED");
            ConnectPortRef(thrtPowr, "MAX_POWER_RPM_NORMALIZED", engine, "MAX_POWER_RPM_NORMALIZED");
            ConnectPortRef(thrtPowr, "MAX_POWER", engine, "MAX_POWER");

            ConnectPortRef(horn, "HORN_CONTROL", genericHornControl, "CONTROL");
            if (bell != null && bellControl != null) ConnectPortRef(bell, "CONTROL", bellControl, "EXT_IN");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(engine, "THROTTLE", tractionGen, "THROTTLE");
            ConnectEmptyPortRef(engine, "RETARDER");
            ConnectEmptyPortRef(engine, "DRIVEN_RPM");
            ConnectPortRef(engine, "INTAKE_WATER_CONTENT", waterDetector, "STATE_EXT_IN");
            ConnectPortRef(engine, "FUEL", fuel, "AMOUNT");
            ConnectPortRef(engine, "FUEL_CONSUMPTION", fuel, "CONSUME_EXT_IN");
            ConnectPortRef(engine, "OIL", oil, "AMOUNT");
            ConnectPortRef(engine, "OIL_CONSUMPTION", oil, "CONSUME_EXT_IN");
            ConnectPortRef(engine, "LOAD_TORQUE", loadTorque, "LOAD_TORQUE_TOTAL");
            ConnectEmptyPortRef(engine, "TEMPERATURE");

            ConnectPortRef(loadTorque, "LOAD_TORQUE_0", compressor, "LOAD_TORQUE");
            ConnectPortRef(loadTorque, "LOAD_TORQUE_1", tractionGen, "LOAD_TORQUE");

            ConnectPortRef(compressor, "ENGINE_RPM_NORMALIZED", engine, "RPM_NORMALIZED");

            ConnectPortRef(slugPowerCalc, "INTERNAL_EFFECTIVE_RESISTANCE", tm, "EFFECTIVE_RESISTANCE");
            ConnectPortRef(slugPowerCalc, "INTERNAL_AMPS", tm, "TOTAL_AMPS");

            ConnectPortRef(tractionGen, "GOAL_POWER", thrtPowr, "GOAL_POWER");
            ConnectPortRef(tractionGen, "GOAL_RPM_NORMALIZED", thrtPowr, "GOAL_RPM_NORMALIZED");
            if (dynBrake != null)
            {
                ConnectPortRef(tractionGen, "DYNAMIC_BRAKE", dynBrake, "EXT_IN");
            }
            else
            {
                ConnectEmptyPortRef(tractionGen, "DYNAMIC_BRAKE");
            }
            ConnectPortRef(tractionGen, "RPM", engine, "RPM");
            ConnectPortRef(tractionGen, "RPM_NORMALIZED", engine, "RPM_NORMALIZED");
            ConnectPortRef(tractionGen, "TOTAL_AMPS", slugPowerCalc, "TOTAL_AMPS");
            ConnectPortRef(tractionGen, "EFFECTIVE_RESISTANCE", slugPowerCalc, "EFFECTIVE_RESISTANCE");
            ConnectPortRef(tractionGen, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE", tm, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE");
            ConnectPortRef(tractionGen, "TRANSITION_CURRENT_LIMIT", tm, "CURRENT_LIMIT_REQUEST");

            ConnectPortRef(tm, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(tm, "REVERSER", reverser, "REVERSER");
            if (dynBrake != null)
            {
                ConnectPortRef(tm, "DYNAMIC_BRAKE", dynBrake, "EXT_IN");
            }
            else
            {
                ConnectEmptyPortRef(tm, "DYNAMIC_BRAKE");
            }
            ConnectEmptyPortRef(tm, "CONFIGURATION_OVERRIDE");
            ConnectPortRef(tm, "MOTOR_RPM", tmRpm, "TM_RPM");
            ConnectPortRef(tm, "APPLIED_VOLTAGE", tractionGen, "VOLTAGE");
            ConnectPortRef(tm, "TM_TEMPERATURE", heat, "TEMPERATURE");
            ConnectEmptyPortRef(tm, "ENVIRONMENT_WATER_STATE");

            ConnectPortRef(cooler, "TEMPERATURE", heat, "TEMPERATURE");
            ConnectEmptyPortRef(cooler, "TARGET_TEMPERATURE");

            ConnectHeatRef(heat, 0, tm, "HEAT_OUT");
            ConnectHeatRef(heat, 1, cooler, "HEAT_OUT");

            ConnectPortRef(tmRpm, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(tmRpm, "GEAR_RATIO", transmission, "GEAR_RATIO");

            ConnectLampRef(dashLamp, dashLight, "EXT_IN");
            ConnectLampRef(gaugesLamp, dashLight, "EXT_IN");
            ConnectLampRef(cabLamp, cabLight, "EXT_IN");
            if (bellLamp != null && bellControl != null) ConnectLampRef(bellLamp, bellControl, "EXT_IN");
            ConnectLampRef(fuelLamp, fuel, "NORMALIZED");
            ConnectLampRef(oilLamp, oil, "NORMALIZED");
            ConnectLampRef(sandLamp, sand, "NORMALIZED");
            ConnectLampRef(lightsFLamp, lightsF, "EXT_IN");
            ConnectLampRef(lightsRLamp, lightsR, "EXT_IN");
            ConnectLampRef(sanderLamp, sander, "CONTROL_EXT_IN");
            ConnectLampRef(tmOffLamp, tm, "TMS_STATE");
            ConnectLampRef(rpmLamp, engine, "RPM_NORMALIZED");
            ConnectLampRef(ampLamp, tm, "AMPS_PER_TM");

            // Apply defaults.
            switch (basisIndex)
            {
                case 0:
                    ApplyMethodToAll<IDE2Defaults>(s => s.ApplyDE2Defaults());
                    break;
                case 1:
                    ApplyMethodToAll<IDE6Defaults>(s => s.ApplyDE6Defaults());
                    break;
                default:
                    break;
            }

            // Control blockers.
            AddControlBlocker(reverser, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");

            if (dynBrake != null)
            {
                AddControlBlocker(reverser, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);

                AddControlBlocker(throttle, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD, true)
                    .blockedControlPortId = FullPortId(throttle, "EXT_IN");

                AddControlBlocker(dynBrake, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD, true);
                AddControlBlocker(dynBrake, reverser, "REVERSER", 0, BlockType.BLOCK_ON_EQUAL_TO_THRESHOLD, true)
                    .blockedControlPortId = FullPortId(dynBrake, "EXT_IN");
            }

            AddEmptyControlBlocker(trnBrake);
            AddEmptyControlBlocker(indBrake);
            AddEmptyControlBlocker(lightsR);
            AddEmptyControlBlocker(lightsF);
            AddEmptyControlBlocker(sander);
        }

        private LampLogicDefinitionProxy CreateAmpLamp(int index)
        {
            switch (index)
            {
                case 1:
                    return CreateLampIncreasingWarning("ampLamp", DVPortValueType.AMPS, 0, 600, 940);
                default:
                    return CreateLampIncreasingWarning("ampLamp", DVPortValueType.AMPS, 0, 600, 1200);
            }
        }

        private static bool HasDynamicBrake(int index)
        {
            switch (index)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        private static bool HasBell(int index)
        {
            switch (index)
            {
                case 1:
                    return true;
                default:
                    return false;
            }
        }
    }
}
