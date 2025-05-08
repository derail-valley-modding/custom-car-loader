using CCL.Types.Proxies;
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
        public BatteryElectricSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "BE2" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtCurv = CreateSimComponent<PowerFunctionDefinitionProxy>("throttleCurve");
            thrtCurv.input.valueType = DVPortValueType.CONTROL;
            thrtCurv.output.valueType = DVPortValueType.CONTROL;
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var brakeCut = CreateOverridableControl(OverridableControlType.TrainBrakeCutout);

            var headlights = CreateSimComponent<MultiplePortDecoderEncoderDefinitionProxy>("headlightDecoder");
            var lightsF = CreateSibling<OverridableControlProxy>(headlights);
            lightsF.portId = FullPortId(headlights, "FRONT_HEADLIGHTS_EXT_IN");
            lightsF.ControlType = OverridableControlType.HeadlightsFront;
            var lightsR = CreateSibling<OverridableControlProxy>(headlights);
            lightsR.portId = FullPortId(headlights, "REAR_HEADLIGHTS_EXT_IN");
            lightsR.ControlType = OverridableControlType.HeadlightsRear;

            var cabLight = CreateOverridableControl(OverridableControlType.CabLight);

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

            var waterDetector = CreateWaterDetector();

            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            tmRpm.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            tmRpm.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO");
            tmRpm.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "TM_RPM");

            var voltRegulator = CreateSimComponent<VoltageRegulatorDefinitionProxy>("voltageRegulator");

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");
            CreateTMsExtras(tm, out var deadTMs, out var tmExplosion);

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("tmPassiveCooler");
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("tmHeat");
            heat.inputCount = 2;
            heat.OnValidate();
            ReparentComponents(tm, cooler, heat);

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

            var cabLamp = CreateLampBasicControl("cabLightLamp", 0.2f);
            var tmOffLamp = CreateLamp("tmOfflineLamp", DVPortValueType.STATE, "INPUT", 0.1f, 1f, -1f, -0.1f, -0.1f, 0.1f);
            var batLamp = CreateLampDecreasingWarning("batteryLamp", DVPortValueType.ELECTRIC_CHARGE, "INPUT", 1f, 0.25f, 0.05f, 0f);
            var tmHeatLamp = CreateLampIncreasingWarning("tmOverheatLamp", DVPortValueType.AMPS, "INPUT", 0, 90, 105, audio: true);
            var sanderLamp = CreateLampBasicControl("sanderLamp");
            var sandLamp = CreateLampDecreasingWarning("sandLamp", DVPortValueType.SAND, "INPUT", 1f, 0.1f, 0.05f, 0f);
            var lightsRLamp = CreateLampBasicControl("headlightsFLamp", 0.01f);
            var lightsFLamp = CreateLampBasicControl("headlightsRLamp", 0.01f);
            var ampLamp = CreateLampIncreasingWarning("ampLamp", DVPortValueType.AMPS, "INPUT", 0, 400, 700);

            horn.powerFuseId = FullFuseId(fusebox, 0);
            batteryController.powerFuseId = FullFuseId(fusebox, 0);
            sander.powerFuseId = FullFuseId(fusebox, 0);
            horn.powerFuseId = FullFuseId(fusebox, 0);
            tm.powerFuseId = FullFuseId(fusebox, 1);
            deadTMs.tmFuseId = FullFuseId(fusebox, 1);
            compressor.powerFuseId = FullFuseId(fusebox, 0);
            cabLamp.powerFuseId = FullFuseId(fusebox, 0);
            tmOffLamp.powerFuseId = FullFuseId(fusebox, 0);
            batLamp.powerFuseId = FullFuseId(fusebox, 0);
            tmHeatLamp.powerFuseId = FullFuseId(fusebox, 0);
            sanderLamp.powerFuseId = FullFuseId(fusebox, 0);
            sandLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsRLamp.powerFuseId = FullFuseId(fusebox, 0);
            lightsFLamp.powerFuseId = FullFuseId(fusebox, 0);
            ampLamp.powerFuseId = FullFuseId(fusebox, 0);

            // Damage.
            _damageController.electricalPTDamagerPortIds = new[] { FullPortId(tm, "GENERATED_DAMAGE") };
            _damageController.electricalPTHealthStateExternalInPortIds = new[] { FullPortId(tm, "HEALTH_STATE_EXT_IN") };

            // Port connections.
            ConnectPorts(tm, "TORQUE_OUT", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(thrtCurv, "IN", throttle, "EXT_IN");
            ConnectPortRef(horn, "HORN_CONTROL", genericHornControl, "CONTROL");

            ConnectPortRef(batteryController, "NORMALIZED_CHARGE", batteryCharge, "NORMALIZED");
            ConnectPortRef(batteryController, "CHARGE_CONSUMPTION", batteryCharge, "CONSUME_EXT_IN");
            ConnectPortRef(batteryController, "POWER", pwrConsumCalc, "POWER_CONSUMPTION_TOTAL");

            ConnectPortRef(pwrConsumCalc, "POWER_CONSUMPTION_0", tm, "POWER_IN");
            ConnectPortRef(pwrConsumCalc, "POWER_CONSUMPTION_1", compressor, "POWER_CONSUMPTION");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(compressor, "VOLTAGE_NORMALIZED", batteryController, "VOLTAGE_NORMALIZED");

            ConnectPortRef(tmRpm, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(tmRpm, "GEAR_RATIO", transmission, "GEAR_RATIO");

            ConnectPortRef(voltRegulator, "THROTTLE", thrtCurv, "OUT");
            ConnectPortRef(voltRegulator, "SUPPLY_VOLTAGE", batteryController, "VOLTAGE");
            ConnectPortRef(voltRegulator, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE", tm, "SINGLE_MOTOR_EFFECTIVE_RESISTANCE");

            ConnectPortRef(tm, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(tm, "REVERSER", reverser, "REVERSER");
            ConnectEmptyPortRef(tm, "DYNAMIC_BRAKE");
            ConnectEmptyPortRef(tm, "CONFIGURATION_OVERRIDE");
            ConnectPortRef(tm, "MOTOR_RPM", tmRpm, "TM_RPM");
            ConnectPortRef(tm, "APPLIED_VOLTAGE", voltRegulator, "OUTPUT_VOLTAGE");
            ConnectPortRef(tm, "TM_TEMPERATURE", heat, "TEMPERATURE");
            ConnectPortRef(tm, "ENVIRONMENT_WATER_STATE", waterDetector, "STATE_EXT_IN");

            ConnectPortRef(cooler, "TEMPERATURE", heat, "TEMPERATURE");
            ConnectEmptyPortRef(cooler, "TARGET_TEMPERATURE");

            ConnectHeatRef(heat, 0, tm, "HEAT_OUT");
            ConnectHeatRef(heat, 1, cooler, "HEAT_OUT");

            ConnectLampRef(cabLamp, cabLight, "EXT_IN");
            ConnectLampRef(tmOffLamp, tm, "TMS_STATE");
            ConnectLampRef(batLamp, batteryCharge, "NORMALIZED");
            ConnectLampRef(tmHeatLamp, heat, "TEMPERATURE");
            ConnectLampRef(sanderLamp, sander, "CONTROL_EXT_IN");
            ConnectLampRef(sandLamp, sand, "NORMALIZED");
            ConnectLampRef(lightsRLamp, headlights, "FRONT_HEADLIGHTS_EXT_IN");
            ConnectLampRef(lightsFLamp, headlights, "REAR_HEADLIGHTS_EXT_IN");
            ConnectLampRef(ampLamp,tm, "AMPS_PER_TM");

            // Apply defaults.
            ApplyMethodToAll<IBE2Defaults>(s => s.ApplyBE2Defaults());

            // Control blockers.
            AddControlBlocker(reverser, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");
        }
    }
}
