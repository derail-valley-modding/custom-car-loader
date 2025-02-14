using CCL.Types;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Wheels;
using UnityEngine;

using static CCL.Types.Proxies.Controls.ControlBlockerProxy.BlockerDefinition;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselElectricSimCreator : SimCreator
    {
        // TODO:
        // Headlights

        public DieselElectricSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DE2", "DE6" };

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

            // Headlights.

            var genericHornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            genericHornControl.defaultValue = 0.5f;
            genericHornControl.smoothTime = 0.2f;
            var hornControl = CreateSibling<HornControlProxy>(genericHornControl);
            hornControl.portId = FullPortId(genericHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;

            ExternalControlDefinitionProxy? bellControl = null;
            ElectricBellDefinitionProxy? bell = null;

            if (HasBell(basisIndex))
            {
                bellControl = CreateExternalControl("bellControl", true);
                bell = CreateSimComponent<ElectricBellDefinitionProxy>("bell");
                bell.smoothDownTime = 0.5f;
            }

            var waterDetector = CreateWaterDetector();

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var engine = CreateSimComponent<DieselEngineDirectDefinitionProxy>("de");
            var engineOff = CreateSibling<PowerOffControlProxy>(engine);
            engineOff.portId = FullPortId(engine, "EMERGENCY_ENGINE_OFF_EXT_IN");
            var engineOn = CreateSibling<EngineOnReaderProxy>(engine);
            engineOn.portId = FullPortId(engine, "ENGINE_ON");
            var environmentDamage = CreateSibling<EnvironmentDamagerProxy>(engine);
            environmentDamage.damagerPortId = FullPortId(engine, "FUEL_ENV_DAMAGE_METER");
            environmentDamage.environmentDamageResource = BaseResourceType.EnvironmentDamageFuel;

            engine = CreateDieselEngine(false);

            var loadTorque = CreateSimComponent<ConfigurableAddDefinitionProxy>("loadTorqueCalculator");
            loadTorque.aReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_0");
            loadTorque.bReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_1");
            loadTorque.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE_TOTAL");
            loadTorque.transform.parent = engine.transform;

            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var tractionGen = CreateSimComponent<TractionGeneratorDefinitionProxy>("tractionGenerator");
            var slugPowerCalc = CreateSimComponent<SlugsPowerCalculatorDefinitionProxy>("slugsPowerCalculator");
            slugPowerCalc.transform.parent = tractionGen.transform;
            var slugPowerProv = CreateSibling<SlugsPowerProviderModuleProxy>(slugPowerCalc);
            slugPowerProv.generatorVoltagePortId = FullPortId(tractionGen, "VOLTAGE");
            slugPowerProv.slugsEffectiveResistancePortId = FullPortId(slugPowerCalc, "EXTERNAL_EFFECTIVE_RESISTANCE_EXT_IN");
            slugPowerProv.slugsTotalAmpsPortId = FullPortId(slugPowerCalc, "EXTERNAL_AMPS_EXT_IN");

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");
            var deadTMs = CreateSibling<DeadTractionMotorsControllerProxy>(tm);
            deadTMs.overheatFuseOffPortId = FullPortId(tm, "OVERHEAT_POWER_FUSE_OFF");
            var tmExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(tm);
            tmExplosion.explosionSignalPortId = FullPortId(tm, "OVERSPEED_EXPLOSION_TRIGGER");
            tmExplosion.bodyDamagePercentage = 0.05f;
            tmExplosion.explosionPrefab = ExplosionPrefab.TMOverspeed;

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("tmPassiveCooler");
            cooler.transform.parent = tm.transform;
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("tmHeat");
            heat.transform.parent = tm.transform;
            heat.inputCount = 2;
            heat.OnValidate();

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

            horn.powerFuseId = FullFuseId(fusebox, 0);
            if (bell != null) bell.powerFuseId = FullFuseId(fusebox, 0);
            sander.powerFuseId = FullFuseId(fusebox, 0);
            engine.engineStarterFuseId = FullFuseId(fusebox, 1);
            tractionGen.powerFuseId = FullFuseId(fusebox, 2);
            tm.powerFuseId = FullFuseId(fusebox, 2);
            deadTMs.tmFuseId = FullFuseId(fusebox, 2);

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
            if (bell != null && bellControl != null)
            {
                ConnectPortRef(bell, "CONTROL", bellControl, "EXT_IN");
            }

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(engine, "THROTTLE", tractionGen, "THROTTLE");
            ConnectEmptyPortRef(engine, "RETARDER");
            ConnectEmptyPortRef(engine, "DRIVEN_RPM");
            ConnectPortRef(engine, "INTAKE_WATER_CONTENT", waterDetector, "STATE_EXTERNAL_IN");
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
            ConnectEmptyPortRef(compressor, "TARGET_TEMPERATURE");

            ConnectHeatRef(heat, 0, tm, "HEAT_OUT");
            ConnectHeatRef(heat, 1, cooler, "HEAT_OUT");

            ConnectPortRef(tmRpm, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(tmRpm, "GEAR_RATIO", transmission, "GEAR_RATIO");

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

                AddControlBlocker(throttle, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                    .blockedControlPortId = FullPortId(throttle, "EXT_IN");

                AddControlBlocker(dynBrake, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
                AddControlBlocker(dynBrake, reverser, "REVERSER", 0, BlockType.BLOCK_ON_EQUAL_TO_THRESHOLD)
                    .blockedControlPortId = FullPortId(dynBrake, "EXT_IN");
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
