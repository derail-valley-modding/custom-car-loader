using CCL.Types.Proxies;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Wheels;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class SlugSimCreator : SimCreator
    {
        public SlugSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DE6 Slug" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var reverser = CreateReverserControl();
            var dynBrake = CreateOverridableControl(OverridableControlType.DynamicBrake);

            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");
            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            tmRpm.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            tmRpm.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO");
            tmRpm.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TM_RPM");
            tmRpm.transform.parent = tm.transform;

            var voltFeed = CreateSibling<ConstantPortDefinitionProxy>(tm, "slugVoltageFeed");
            voltFeed.port = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, "APPLIED_VOLTAGE_EXT_IN");
            var slug = CreateSibling<SlugModuleProxy>(tm);
            slug.appliedVoltagePortId = FullPortId(voltFeed, voltFeed.port.ID);
            slug.effectiveResistancePortId = FullPortId(tm, "EFFECTIVE_RESISTANCE");
            slug.totalAmpsPortId = FullPortId(tm, "TOTAL_AMPS");

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(tm, "WORKING_TRACTION_MOTORS");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");
            wheelslip.engineBrakingActivePortId = FullPortId(tm, "DYNAMIC_BRAKE_ACTIVE");

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("PROVIDER_POWER", true)
            };

            sander.powerFuseId = FullFuseId(fusebox, 0);
            tm.powerFuseId = FullFuseId(fusebox, 0);
            slug.powerFuseId = FullFuseId(fusebox, 0);

            _damageController.electricalPTDamagerPortIds = new[] { FullPortId(tm, "GENERATED_DAMAGE") };
            _damageController.electricalPTHealthStateExternalInPortIds = new[] { FullPortId(tm, "HEALTH_STATE_EXT_IN") };

            ConnectPorts(tm, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            ConnectPortRef(sand, "AMOUNT", sander, "SAND");
            ConnectPortRef(sand, "CONSUME_EXT_IN", sander, "SAND_CONSUMPTION");

            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", tmRpm, "WHEEL_RPM");
            ConnectPortRef(transmission, "GEAR_RATIO", tmRpm, "GEAR_RATIO");

            ConnectPortRef(throttle, "EXT_IN", tm, "THROTTLE");
            ConnectPortRef(reverser, "REVERSER", tm, "REVERSER");
            ConnectPortRef(dynBrake, "EXT_IN", tm, "DYNAMIC_BRAKE");

            // tm.CONFIGURATION_OVERRIDE
            ConnectPortRef(tmRpm, "TM_RPM", tm, "MOTOR_RPM");
            ConnectPortRef(voltFeed, "APPLIED_VOLTAGE_EXT_IN", tm, "APPLIED_VOLTAGE");
            // tm.TM_TEMPERATURE

            ApplyMethodToAll<IDE6Defaults>(s => s.ApplyDE6Defaults());

            // Only difference to DE6.
            tm.configurations = tm.configurations.Take(1).ToArray();
            tm.configurations[0].forwardTransition.thresholdValue = 0;
        }
    }
}
