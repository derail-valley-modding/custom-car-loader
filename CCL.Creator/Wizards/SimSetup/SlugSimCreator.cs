using CCL.Types.Proxies;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Wheels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class SlugSimCreator : SimCreator
    {
        public SlugSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DE6 Slug" };

        public override IEnumerable<string> GetSimFeatures(int basisIndex)
        {
            yield return "6 Traction Motors";
        }

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var reverser = CreateReverserControl();
            var dynBrake = CreateOverridableControl(OverridableControlType.DynamicBrake);

            var lightsR = CreateOverridableControl(OverridableControlType.HeadlightsRear, defaultValue: 0.4f);
            var lightsF = CreateOverridableControl(OverridableControlType.HeadlightsFront, defaultValue: 0.4f);

            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");

            var voltFeed = CreateSibling<ConfigurablePortDefinitionProxy>(tm, "slugVoltageFeed");
            voltFeed.port = new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, "APPLIED_VOLTAGE_EXT_IN");
            var slug = CreateSibling<SlugModuleProxy>(tm);
            slug.appliedVoltagePortId = FullPortId(voltFeed, voltFeed.port.ID);
            slug.effectiveResistancePortId = FullPortId(tm, "EFFECTIVE_RESISTANCE");
            slug.totalAmpsPortId = FullPortId(tm, "TOTAL_AMPS");

            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            tmRpm.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            tmRpm.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO");
            tmRpm.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TM_RPM");
            tmRpm.transform.parent = tm.transform;

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
                new FuseDefinition("PROVIDER_POWER", false, 0)
            };

            var sandLamp = CreateLampDecreasingWarning("sandLamp", DVPortValueType.SAND, 1f, 0.1f, 0.05f, 0f);
            var lightsRLamp = CreateLampHeadlightControl("headlightsRLamp");
            var lightsFLamp = CreateLampHeadlightControl("headlightsFLamp");
            var sanderLamp = CreateLampBasicControl("sanderLamp");
            var tmOffLamp = CreateLamp("tmOfflineLamp", DVPortValueType.STATE, 0.1f, 1f, -1f, -0.1f, -0.1f, 0.1f);
            var ampLamp = CreateLampIncreasingWarning("ampLamp", DVPortValueType.AMPS, 0, 600, 1200, float.PositiveInfinity);

            var waterDetector = CreateWaterDetector();

            sander.powerFuseId = FullFuseId(fusebox, 0);
            tm.powerFuseId = FullFuseId(fusebox, 0);
            slug.powerFuseId = FullFuseId(fusebox, 0);
            sandLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsRLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsFLamp.powerFuseId = FullFuseId(fusebox, 0);
            sanderLamp.powerFuseId = FullFuseId(fusebox, 0);
            tmOffLamp.powerFuseId = FullFuseId(fusebox, 0);
            ampLamp.powerFuseId = FullFuseId(fusebox, 0);

            _damageController.electricalPTDamagerPortIds = new[] { FullPortId(tm, "GENERATED_DAMAGE") };
            _damageController.electricalPTHealthStateExternalInPortIds = new[] { FullPortId(tm, "HEALTH_STATE_EXT_IN") };

            ConnectPorts(tm, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(tmRpm, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(tmRpm, "GEAR_RATIO", transmission, "GEAR_RATIO");

            ConnectPortRef(tm, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(tm, "REVERSER", reverser, "REVERSER");
            ConnectPortRef(tm, "DYNAMIC_BRAKE", dynBrake, "EXT_IN");
            ConnectEmptyPortRef(tm, "CONFIGURATION_OVERRIDE");
            ConnectPortRef(tm, "MOTOR_RPM", tmRpm, "TM_RPM");
            ConnectPortRef(tm, "APPLIED_VOLTAGE", voltFeed, "APPLIED_VOLTAGE_EXT_IN");
            ConnectEmptyPortRef(tm, "TM_TEMPERATURE");
            ConnectPortRef(tm, "ENVIRONMENT_WATER_STATE", waterDetector, "STATE_EXT_IN");

            ConnectLampRef(sandLamp, sand, "NORMALIZED");
            ConnectLampRef(lightsFLamp, lightsF, "EXT_IN");
            ConnectLampRef(lightsRLamp, lightsR, "EXT_IN");
            ConnectLampRef(sanderLamp, sander, "CONTROL_EXT_IN");
            ConnectLampRef(tmOffLamp, tm, "TMS_STATE");
            ConnectLampRef(ampLamp, tm, "AMPS_PER_TM");

            AddWheelSlideObserver();

            ApplyMethodToAll<IDE6Defaults>(s => s.ApplyDE6Defaults());

            // Only difference to DE6.
            tm.configurations = tm.configurations.Take(1).ToArray();
            tm.configurations[0].forwardTransition.thresholdValue = 0;
        }
    }
}
