using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselMechSimCreator : SimCreator
    {
        public DieselMechSimCreator(GameObject prefabRoot) : base(prefabRoot)
        {
        }

        public override string[] SimBasisOptions => new[] { "DM3" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // engine + transmission
            var engine = CreateSimComponent<DieselEngineDirectDefinitionProxy>("engine");
            var fluidCoupler = CreateSimComponent<HydraulicTransmissionDefinitionProxy>("fluidCoupler");
            
            var transmissionA = CreateSimComponent<SmoothTransmissionDefinitionProxy>("transmissionA");
            transmissionA.ApplyDM3BoxADefaults();
            var transmissionB = CreateSimComponent<SmoothTransmissionDefinitionProxy>("transmissionB");
            transmissionB.ApplyDM3BoxBDefaults();
            var transmissionAB = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("transmissionAB");
            transmissionAB.aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO_A");
            transmissionAB.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO_B");
            transmissionAB.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");

            var driveShaftRpmCalculator = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("driveshaftRpmCalc");
            driveShaftRpmCalculator.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            driveShaftRpmCalculator.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");
            driveShaftRpmCalculator.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "DRIVE_SHAFT_RPM");

            var gearRatioCalculator = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("gearRatioCalc");
            gearRatioCalculator.aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");
            gearRatioCalculator.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "HYDRAULIC_GEAR_RATIO");
            gearRatioCalculator.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OVERALL_GEAR_RATIO");

            // traction
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateSibling<TractionPortFeedersProxy>(traction);
            tractionFeeders.forwardSpeedPortId = FullPortId(traction, "FORWARD_SPEED_EXT_IN");
            tractionFeeders.wheelRpmPortId = FullPortId(traction, "WHEEL_RPM_EXT_IN");
            tractionFeeders.wheelSpeedKmhPortId = FullPortId(traction, "WHEEL_SPEED_KMH_EXT_IN");

            // radiator
            var coolant = CreateSimComponent<HeatReservoirDefinitionProxy>("coolant");
            coolant.inputCount = 4;
            coolant.OnValidate();

            var passiveCool = CreateSibling<PassiveCoolerDefinitionProxy>(coolant, "passiveCool");
            var directionCool = CreateSibling<DirectionalCoolerDefinitionProxy>(coolant, "directionCool");

            // engine load adder
            var loadTorque = CreateSimComponent<ConfigurableAddDefinitionProxy>("loadTorque");
            loadTorque.aReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_0");
            loadTorque.bReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_1");
            loadTorque.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE_TOTAL");

            // compressor
            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateSibling<CompressorSimControllerProxy>(compressor);
            airController.activationSignalExtInPortId = FullPortId(compressor, "ACTIVATION_SIGNAL_EXT_IN");
            airController.isPoweredPortId = FullPortId(compressor, "IS_POWERED");
            airController.productionRateOutPortId = FullPortId(compressor, "PRODUCTION_RATE");
            airController.mainReservoirVolumePortId = FullPortId(compressor, "MAIN_RES_VOLUME");
            airController.activationPressureThresholdPortId = FullPortId(compressor, "ACTIVATION_PRESSURE_THRESHOLD");
            airController.compressorHealthStatePortId = FullPortId(compressor, "COMPRESSOR_HEALTH_EXT_IN");

            // resources
            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);

            // controls
            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false),
                new FuseDefinition("ENGINE_STARTER", false),
            };
            engine.engineStarterFuseId = FullPortId(fusebox, "ENGINE_STARTER");
            
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var reverser = CreateReverserControl();
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            var brake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var retarder = CreateOverridableControl(OverridableControlType.DynamicBrake, "retarder");

            var hornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            hornControl.defaultValue = 0.5f;
            hornControl.smoothTime = 0.2f;
            hornControl.saveState = false;

            var hornOvControl = CreateSibling<HornControlProxy>(hornControl);
            hornOvControl.neutralAt0 = false;
            hornOvControl.portId = FullPortId(hornControl, "EXT_IN");

            var horn = CreateSibling<HornDefinitionProxy>(hornControl, "horn");
            horn.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");

            var sander = CreateSimComponent<SanderDefinitionProxy>("sander");
            sander.powerFuseId = FullPortId(fusebox, "ELECTRONICS_MAIN");

            var gearInputA = CreateSimComponent<ManualTransmissionInputDefinitionProxy>("gearInputA");
            var gearInputB = CreateSimComponent<ManualTransmissionInputDefinitionProxy>("gearInputB");
            var gearInputAB = CreateSimComponent<ConfigurableAddDefinitionProxy>("gearInputAB");
            gearInputAB.aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_A");
            gearInputAB.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_B");
            gearInputAB.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "GEAR");

            // connections
            _damageController.mechanicalPTDamagerPortIds = new[]
            {
                FullPortId(engine, "GENERATED_ENGINE_DAMAGE"),
                FullPortId(fluidCoupler, "GENERATED_DAMAGE"),
                FullPortId(transmissionA, "GENERATED_DAMAGE"),
                FullPortId(transmissionB, "GENERATED_DAMAGE"),
            };
            _damageController.mechanicalPTHealthStateExternalInPortIds = new[]
            {
                FullPortId(engine, "ENGINE_HEALTH_STATE_EXT_IN"),
                FullPortId(fluidCoupler, "MECHANICAL_PT_HEALTH_EXT_IN"),
            };
            _damageController.mechanicalPTOffExternalInPortIds = new[]
            {
                FullPortId(engine, "COLLISION_ENGINE_OFF_EXT_IN"),
            };

            ConnectPorts(fluidCoupler, "OUTPUT_SHAFT_TORQUE", transmissionA, "TORQUE_IN");
            ConnectPorts(transmissionA, "TORQUE_OUT", transmissionB, "TORQUE_IN");
            ConnectPorts(transmissionB, "TORQUE_OUT", traction, "TORQUE_IN");

            ConnectPortRef(engine, "RPM", fluidCoupler, "INPUT_SHAFT_RPM");
            ConnectPortRef(engine, "MAX_RPM", fluidCoupler, "MAX_RPM");
            ConnectPortRef(engine, "RPM_NORMALIZED", compressor, "ENGINE_RPM_NORMALIZED");

            ConnectPortRef(transmissionA, "NUM_OF_GEARS", gearInputA, "NUM_OF_GEARS");
            ConnectPortRef(transmissionB, "NUM_OF_GEARS", gearInputB, "NUM_OF_GEARS");
            ConnectPortRef(gearInputA, "GEAR", gearInputAB, "GEAR_A");
            ConnectPortRef(gearInputB, "GEAR", gearInputAB, "GEAR_B");

            ConnectPortRef(fluidCoupler, "TURBINE_RPM", engine, "DRIVEN_RPM");

            ConnectPortRef(hornControl, "CONTROL", horn, "HORN_CONTROL");

            ConnectPortRef(fuel, "AMOUNT", engine, "FUEL");
            ConnectPortRef(fuel, "CONSUME_EXT_IN", engine, "FUEL_CONSUMPTION");
            ConnectPortRef(oil, "AMOUNT", engine, "OIL");
            ConnectPortRef(oil, "CONSUME_EXT_IN", engine, "OIL_CONSUMPTION");
            ConnectPortRef(sand, "AMOUNT", sander, "SAND");
            ConnectPortRef(sand, "CONSUME_EXT_IN", sander, "SAND_CONSUMPTION");


            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", driveShaftRpmCalculator, "WHEEL_RPM");
            ConnectPortRef(transmissionAB, "MECHANICAL_GEAR_RATIO", driveShaftRpmCalculator, "MECHANICAL_GEAR_RATIO");
            ConnectPortRef(throttle, "EXT_IN", engine, "THROTTLE");
            ConnectPortRef(retarder, "EXT_IN", engine, "RETARDER");
            
            
            ConnectPortRef(loadTorque, "LOAD_TORQUE_TOTAL", engine, "LOAD_TORQUE");
            ConnectPortRef(coolant, "TEMPERATURE", engine, "TEMPERATURE");

            ConnectPortRef(reverser, "REVERSER", fluidCoupler, "REVERSER");
            ConnectPortRef(driveShaftRpmCalculator, "DRIVE_SHAFT_RPM", fluidCoupler, "OUTPUT_SHAFT_RPM");
            ConnectPortRef(coolant, "TEMPERATURE", fluidCoupler, "TEMPERATURE");
            ConnectPortRef(coolant, "TEMPERATURE", passiveCool, "TEMPERATURE");
            ConnectPortRef(traction, "FORWARD_SPEED_EXT_IN", directionCool, "SPEED");
            ConnectPortRef(coolant, "TEMPERATURE", directionCool, "TEMPERATURE");
            
            ConnectPortRef(compressor, "LOAD_TORQUE", loadTorque, "LOAD_TORQUE_0");
            ConnectPortRef(fluidCoupler, "INPUT_SHAFT_TORQUE", loadTorque, "LOAD_TORQUE_1");
            ConnectPortRef(engine, "HEAT_OUT", coolant, "HEAT_IN_0");
            ConnectPortRef(fluidCoupler, "HEAT_OUT", coolant, "HEAT_IN_1");
            ConnectPortRef(passiveCool, "HEAT_OUT", coolant, "HEAT_IN_2");
            ConnectPortRef(directionCool, "HEAT_OUT", coolant, "HEAT_IN_3");
            ConnectPortRef(gearInputA, "GEAR", transmissionA, "GEAR");
            ConnectPortRef(throttle, "EXT_IN", transmissionA, "THROTTLE");
            ConnectPortRef(retarder, "EXT_IN", transmissionA, "RETARDER");
            ConnectPortRef(engine, "RPM", transmissionA, "ENGINE_RPM");
            ConnectPortRef(gearInputB, "GEAR", transmissionB, "GEAR");
            ConnectPortRef(throttle, "EXT_IN", transmissionB, "THROTTLE");
            ConnectPortRef(retarder, "EXT_IN", transmissionB, "RETARDER");
            ConnectPortRef(engine, "RPM", transmissionB, "ENGINE_RPM");
            ConnectPortRef(transmissionA, "GEAR_RATIO", transmissionAB, "GEAR_RATIO_A");
            ConnectPortRef(transmissionB, "GEAR_RATIO", transmissionAB, "GEAR_RATIO_B");
            ConnectPortRef(fluidCoupler, "GEAR_RATIO", gearRatioCalculator, "HYDRAULIC_GEAR_RATIO");
            ConnectPortRef(transmissionAB, "MECHANICAL_GEAR_RATIO", gearRatioCalculator, "MECHANICAL_GEAR_RATIO");

            ApplyMethodToAll<IDM3Defaults>(s => s.ApplyDM3Defaults());
        }
    }
}
