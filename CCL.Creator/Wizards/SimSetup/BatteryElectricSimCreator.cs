using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Wheels;
using UnityEngine;

using static CCL.Types.Proxies.Controls.ControlBlockerProxy.BlockerDefinition;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class BatteryElectricSimCreator : SimCreator
    {
        // TODO:
        // Headlights

        public BatteryElectricSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "BE2" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtCurv = CreateSimComponent<PowerFunctionDefinitionProxy>("throttleCurve");
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);

            // Headlights.

            var genericHornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            genericHornControl.defaultValue = 0;
            genericHornControl.smoothTime = 0.2f;
            var hornControl = CreateSibling<HornControlProxy>(genericHornControl);
            hornControl.portId = FullPortId(genericHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;

            var batteryCharge = CreateResourceContainer(ResourceContainerType.ElectricCharge, "batteryCharge");
            var batteryController = CreateSimComponent<BatteryDefinitionProxy>("batteryController");
            var pwrConsumCalc = CreateSimComponent<ConfigurableAddDefinitionProxy>("powerConsumptionCalculator");
            pwrConsumCalc.aReader = new PortReferenceDefinition(DVPortValueType.POWER, "POWER_CONSUMPTION_0");
            pwrConsumCalc.bReader = new PortReferenceDefinition(DVPortValueType.POWER, "POWER_CONSUMPTION_1");
            pwrConsumCalc.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "POWER_CONSUMPTION_TOTAL");
            pwrConsumCalc.transform.parent = batteryController.transform;

            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            tmRpm.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            tmRpm.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO");
            tmRpm.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TM_RPM");

            var tmController = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmController");
            tmController.aReader = new PortReferenceDefinition(DVPortValueType.VOLTS, "BATTERY_VOLTAGE");
            tmController.bReader = new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE");
            tmController.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "TM_VOLTAGE");

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");
            var deadTMs = CreateSibling<DeadTractionMotorsControllerProxy>(tm);
            deadTMs.overheatFuseOffPortId = FullPortId(tm, "OVERHEAT_POWER_FUSE_OFF");
            var tmExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(tm);
            tmExplosion.explosionSignalPortId = FullPortId(tm, "OVERSPEED_EXPLOSION_TRIGGER");
            tmExplosion.bodyDamagePercentage = 0.05f;
            tmExplosion.explosion = ExplosionPrefab.TMOverspeed;

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("tmPassiveCooler");
            cooler.transform.parent = tm.transform;
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("tmHeat");
            heat.transform.parent = tm.transform;
            heat.inputCount = 2;
            heat.OnValidate();

            var compressor = CreateSimComponent<ElectricCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(tm, "WORKING_TRACTION_MOTORS");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");

            // Fusebox and fuse connections.
            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRICS_MAIN", false),
                new FuseDefinition("TM_POWER", false),
            };

            sander.powerFuseId = FullPortId(fusebox, "ELECTRICS_MAIN");
            batteryController.powerFuseId = FullPortId(fusebox, "ELECTRICS_MAIN");
            tm.powerFuseId = FullPortId(fusebox, "TM_POWER");
            deadTMs.tmFuseId = FullPortId(fusebox, "TM_POWER");
            compressor.powerFuseId = FullPortId(fusebox, "ELECTRICS_MAIN");

            // Damage.
            _damageController.electricalPTDamagerPortIds = new[] { FullPortId(tm, "GENERATED_DAMAGE") };
            _damageController.electricalPTHealthStateExternalInPortIds = new[] { FullPortId(tm, "HEALTH_STATE_EXT_IN") };

            // Port connections.
            ConnectPorts(tm, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(throttle, "EXT_IN", thrtCurv, "IN");
            ConnectPortRef(genericHornControl, "CONTROL", horn, "HORN_CONTROL");

            ConnectPortRef(batteryCharge, "NORMALIZED", batteryController, "NORMALIZED_CHARGE");
            ConnectPortRef(batteryCharge, "CONSUME_EXT_IN", batteryController, "CHARGE_CONSUMPTION");
            ConnectPortRef(pwrConsumCalc, "POWER_CONSUMPTION_TOTAL", batteryController, "POWER");

            ConnectPortRef(tm, "POWER_IN", pwrConsumCalc, "POWER_CONSUMPTION_0");
            ConnectPortRef(compressor, "POWER_CONSUMPTION", pwrConsumCalc, "POWER_CONSUMPTION_1");

            ConnectPortRef(sand, "AMOUNT", sander, "SAND");
            ConnectPortRef(sand, "CONSUME_EXT_IN", sander, "SAND_CONSUMPTION");

            ConnectPortRef(batteryController, "VOLTAGE_NORMALIZED", compressor, "VOLTAGE_NORMALIZED");

            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", tmRpm, "WHEEL_RPM");
            ConnectPortRef(transmission, "GEAR_RATIO", tmRpm, "GEAR_RATIO");

            ConnectPortRef(batteryController, "VOLTAGE", tmController, "BATTERY_VOLTAGE");
            ConnectPortRef(thrtCurv, "OUT", tmController, "THROTTLE");

            ConnectPortRef(throttle, "EXT_IN", tm, "THROTTLE");
            ConnectPortRef(reverser, "REVERSER", tm, "REVERSER");

            ConnectPortRef(tmRpm, "TM_RPM", tm, "MOTOR_RPM");
            ConnectPortRef(tmController, "TM_VOLTAGE", tm, "APPLIED_VOLTAGE");

            ConnectPortRef(heat, "TEMPERATURE", tm, "TM_TEMPERATURE");
            ConnectPortRef(heat, "TEMPERATURE", cooler, "TEMPERATURE");

            ConnectPortRef(tm, "HEAT_OUT", heat, "HEAT_IN_0");
            ConnectPortRef(cooler, "HEAT_OUT", heat, "HEAT_IN_1");

            // Apply defaults.
            ApplyMethodToAll<IBE2Defaults>(s => s.ApplyBE2Defaults());

            // Control blockers.
            AddControlBlocker(reverser, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");
        }
    }
}
