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
    internal class DieselHydraulicSimCreator : SimCreator
    {
        public DieselHydraulicSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DH4" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtPowr = CreateSimComponent<ThrottleGammaPowerConversionDefinitionProxy>("throttlePower");
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
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

            // Headlights.

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
            var engineExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(engine);
            engineExplosion.explosion = ExplosionPrefab.Mechanical;
            engineExplosion.bodyDamagePercentage = 0.1f;
            engineExplosion.explosionSignalPortId = FullPortId(engine, "IS_BROKEN");

            var loadTorque = CreateSimComponent<ConfigurableAddDefinitionProxy>("loadTorqueCalculator");
            loadTorque.aReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_0");
            loadTorque.bReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_1");
            loadTorque.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE_TOTAL");
            loadTorque.transform.parent = engine.transform;

            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var fluidCoupler = CreateSimComponent<HydraulicTransmissionDefinitionProxy>("fluidCoupler");
            var couplerExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(fluidCoupler);
            couplerExplosion.explosion = ExplosionPrefab.Hydraulic;
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

            var poweredAxles = CreateSimComponent<ConstantPortDefinitionProxy>("poweredAxles");
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

            horn.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");
            bell.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");
            sander.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");
            engine.engineStarterFuseId = FullPortId(fusebox, "ENGINE_STARTER");
            autoCooler.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");

            // Damage.
            _damageController.mechanicalPTDamagerPortIds = new[]
            {
                FullPortId(engine, "GENERATED_ENGINE_DAMAGE"),
                FullPortId(fluidCoupler, "GENERATED_DAMAGE")
            };
            _damageController.mechanicalPTHealthStateExternalInPortIds = new[]
            {
                FullPortId(engine, "ENGINE_HEALTH_STATE_EXT_IN"),
                FullPortId(fluidCoupler, "MECHANICAL_PT_HEALTH_EXT_IN")
            };
            _damageController.mechanicalPTOffExternalInPortIds = new[] { FullPortId(engine, "COLLISION_ENGINE_OFF_EXT_IN") };

            // Port connections.
            ConnectPorts(fluidCoupler, "OUTPUT_SHAFT_TORQUE", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(genericHornControl, "CONTROL", horn, "HORN_CONTROL");
            ConnectPortRef(bellControl, "EXT_IN", bell, "CONTROL");

            ConnectPortRef(sand, "AMOUNT", sander, "SAND");
            ConnectPortRef(sand, "CONSUME_EXT_IN", sander, "SAND_CONSUMPTION");

            ConnectPortRef(throttle, "EXT_IN", engine, "THROTTLE");
            ConnectPortRef(fluidCoupler, "TURBINE_RPM", engine, "DRIVEN_RPM");
            ConnectPortRef(fuel, "AMOUNT", engine, "FUEL");
            ConnectPortRef(fuel, "CONSUME_EXT_IN", engine, "FUEL_CONSUMPTION");
            ConnectPortRef(oil, "AMOUNT", engine, "OIL");
            ConnectPortRef(oil, "CONSUME_EXT_IN", engine, "OIL_CONSUMPTION");
            ConnectPortRef(loadTorque, "LOAD_TORQUE_TOTAL", engine, "LOAD_TORQUE");
            ConnectPortRef(coolant, "TEMPERATURE", engine, "TEMPERATURE");

            ConnectPortRef(engine, "RPM_NORMALIZED", compressor, "ENGINE_RPM_NORMALIZED");

            ConnectPortRef(dynBrake, "EXT_IN", fluidCoupler, "HYDRODYNAMIC_BRAKE");
            ConnectPortRef(reverser, "REVERSER", fluidCoupler, "REVERSER");
            ConnectPortRef(engine, "RPM", fluidCoupler, "INPUT_SHAFT_RPM");
            ConnectPortRef(engine, "MAX_RPM", fluidCoupler, "MAX_RPM");
            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", fluidCoupler, "OUTPUT_SHAFT_RPM");
            ConnectPortRef(coolant, "TEMPERATURE", fluidCoupler, "TEMPERATURE");

            ConnectPortRef(coolant, "TEMPERATURE", cooler, "TEMPERATURE");
            ConnectPortRef(engine, "ENGINE_ON", autoCooler, "IS_POWERED");
            ConnectPortRef(coolant, "TEMPERATURE", autoCooler, "TEMPERATURE");

            ConnectPortRef(compressor, "LOAD_TORQUE", loadTorque, "LOAD_TORQUE_0");
            ConnectPortRef(fluidCoupler, "INPUT_SHAFT_TORQUE", loadTorque, "LOAD_TORQUE_1");

            ConnectPortRef(engine, "HEAT_OUT", coolant, "HEAT_IN_0");
            ConnectPortRef(fluidCoupler, "HEAT_OUT", coolant, "HEAT_IN_1");
            ConnectPortRef(cooler, "HEAT_OUT", coolant, "HEAT_IN_2");
            ConnectPortRef(autoCooler, "HEAT_OUT", coolant, "HEAT_IN_3");

            // Apply defaults.
            ApplyMethodToAll<IDH4Defaults>(s => s.ApplyDH4Defaults());

            // Control blockers.
            AddControlBlocker(throttle, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                .blockedControlPortId = FullPortId(throttle, "EXT_IN");

            AddControlBlocker(reverser, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, dynBrake, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", 40, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", -40, BlockType.BLOCK_ON_BELOW_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");

            AddControlBlocker(dynBrake, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(dynBrake, reverser, "REVERSER", 0, BlockType.BLOCK_ON_EQUAL_TO_THRESHOLD)
                .blockedControlPortId = FullPortId(dynBrake, "EXT_IN");
        }
    }
}
