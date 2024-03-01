using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselElectricSimCreator : SimCreator
    {
        public DieselElectricSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DE2", "DE6" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtPowr = CreateSimComponent<ThrottleGammaPowerConversionDefinitionProxy>("throttlePower");
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            ExternalControlDefinitionProxy dynBrake = null!;

            if (basisIndex == 1)
            {
                dynBrake = CreateOverridableControl(OverridableControlType.DynamicBrake);
            }

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var engine = CreateSimComponent<DieselEngineDirectDefinitionProxy>("de");
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
            tmExplosion.explosion = ExplosionPrefab.ExplosionTMOverspeed;

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("tmPassiveCooler");
            cooler.transform.parent = tm.transform;
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("tmHeat");
            heat.transform.parent = tm.transform;
            heat.inputs = new[]
            {
                new PortReferenceDefinition(DVPortValueType.HEAT_RATE, "HEAT_IN_0"),
                new PortReferenceDefinition(DVPortValueType.HEAT_RATE, "HEAT_IN_1")
            };

            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            tmRpm.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            tmRpm.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO");
            tmRpm.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TM_RPM");

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateSibling<TractionPortFeedersProxy>(traction);
            tractionFeeders.forwardSpeedPortId = FullPortId(traction, "FORWARD_SPEED_EXT_IN");
            tractionFeeders.wheelRpmPortId = FullPortId(traction, "WHEEL_RPM_EXT_IN");
            tractionFeeders.wheelSpeedKmhPortId = FullPortId(traction, "WHEEL_SPEED_KMH_EXT_IN");

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false),
                new FuseDefinition("ENGINE_STARTER", false),
                new FuseDefinition("TM_POWER", false),
            };

            sander.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");
            engine.engineStarterFuseId = FullPortId(fusebox, "ENGINE_STARTER");
            tractionGen.powerFuseId = FullPortId(fusebox, "TM_POWER");
            tm.powerFuseId = FullPortId(fusebox, "TM_POWER");
            deadTMs.tmFuseId = FullPortId(fusebox, "TM_POWER");

            _damageController.mechanicalPTDamagerPortIds = new[] { FullPortId(engine, "GENERATED_ENGINE_DAMAGE") };
            _damageController.mechanicalPTHealthStateExternalInPortIds = new[] { FullPortId(engine, "ENGINE_HEALTH_STATE_EXT_IN") };
            _damageController.mechanicalPTOffExternalInPortIds = new[] { FullPortId(engine, "COLLISION_ENGINE_OFF_EXT_IN") };

            _damageController.electricalPTDamagerPortIds = new[] { FullPortId(tm, "GENERATED_DAMAGE") };
            _damageController.electricalPTHealthStateExternalInPortIds = new[] { FullPortId(tm, "HEALTH_STATE_EXT_IN") };

            ConnectPorts(tm, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            ConnectPortRef(throttle, "EXT_IN", thrtPowr, "THROTTLE");
            ConnectPortRef(engine, "IDLE_RPM_NORMALIZED", thrtPowr, "IDLE_RPM_NORMALIZED");
            ConnectPortRef(engine, "MAX_POWER_RPM_NORMALIZED", thrtPowr, "MAX_POWER_RPM_NORMALIZED");
            ConnectPortRef(engine, "MAX_POWER", thrtPowr, "MAX_POWER");

            ConnectPortRef(sand, "AMOUNT", sander, "SAND");
            ConnectPortRef(sand, "CONSUME_EXT_IN", sander, "SAND_CONSUMPTION");

            ConnectPortRef(tractionGen, "THROTTLE", engine, "THROTTLE");

            ConnectPortRef(fuel, "AMOUNT", engine, "FUEL");
            ConnectPortRef(fuel, "CONSUME_EXT_IN", engine, "FUEL_CONSUMPTION");
            ConnectPortRef(oil, "AMOUNT", engine, "OIL");
            ConnectPortRef(oil, "CONSUME_EXT_IN", engine, "OIL_CONSUMPTION");

            ConnectPortRef(loadTorque, "LOAD_TORQUE_TOTAL", engine, "LOAD_TORQUE");
            ConnectPortRef(compressor, "LOAD_TORQUE", loadTorque, "LOAD_TORQUE_0");
            ConnectPortRef(tractionGen, "LOAD_TORQUE", loadTorque, "LOAD_TORQUE_1");

            ConnectPortRef(engine, "RPM_NORMALIZED", compressor, "ENGINE_RPM_NORMALIZED");

            ConnectPortRef(tm, "EFFECTIVE_RESISTANCE", slugPowerCalc, "INTERNAL_EFFECTIVE_RESISTANCE");
            ConnectPortRef(tm, "TOTAL_AMPS", slugPowerCalc, "INTERNAL_AMPS");

            ConnectPortRef(thrtPowr, "GOAL_POWER", tractionGen, "GOAL_POWER");
            ConnectPortRef(thrtPowr, "GOAL_RPM_NORMALIZED", tractionGen, "GOAL_RPM_NORMALIZED");

            if (basisIndex == 1)
            {
                ConnectPortRef(dynBrake, "EXT_IN", tractionGen, "DYNAMIC_BRAKE");
            }

            ConnectPortRef(engine, "RPM", tractionGen, "RPM");
            ConnectPortRef(engine, "RPM_NORMALIZED", tractionGen, "RPM_NORMALIZED");

            ConnectPortRef(tm, "CURRENT_DROP_REQUEST", tractionGen, "CURRENT_DROP_REQUEST");

            ConnectPortRef(slugPowerCalc, "TOTAL_AMPS", tractionGen, "TOTAL_AMPS");
            ConnectPortRef(slugPowerCalc, "EFFECTIVE_RESISTANCE", tractionGen, "EFFECTIVE_RESISTANCE");

            ConnectPortRef(throttle, "EXT_IN", tm, "THROTTLE");
            ConnectPortRef(reverser, "REVERSER", tm, "REVERSER");

            if (basisIndex == 1)
            {
                ConnectPortRef(dynBrake, "EXT_IN", tm, "DYNAMIC_BRAKE");
            }

            ConnectPortRef(tmRpm, "TM_RPM", tm, "MOTOR_RPM");

            ConnectPortRef(tractionGen, "VOLTAGE", tm, "APPLIED_VOLTAGE");

            ConnectPortRef(heat, "TEMPERATURE", tm, "TM_TEMPERATURE");
            ConnectPortRef(heat, "TEMPERATURE", cooler, "TEMPERATURE");

            ConnectPortRef(tm, "HEAT_OUT", heat, "HEAT_IN_0");
            ConnectPortRef(cooler, "HEAT_OUT", heat, "HEAT_IN_1");

            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", tmRpm, "WHEEL_RPM");
            ConnectPortRef(transmission, "GEAR_RATIO", tmRpm, "GEAR_RATIO");

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
        }
    }
}
