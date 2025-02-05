using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Wheels;
using UnityEngine;
using static CCL.Types.Proxies.Controls.ControlBlockerProxy.BlockerDefinition;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselMechSimCreator : SimCreator
    {
        public DieselMechSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DM3", "DM1U" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            switch (basisIndex)
            {
                case 0:
                    CreateDM3();
                    break;
                case 1:
                    CreateDM1U();
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void CreateDM3()
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var brakeCut = CreateOverridableControl(OverridableControlType.TrainBrakeCutout);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            var retarder = CreateOverridableControl(OverridableControlType.DynamicBrake, "retarder");

            var gearA = CreateSimComponent<ManualTransmissionInputDefinitionProxy>("gearInputA");
            var gearB = CreateSimComponent<ManualTransmissionInputDefinitionProxy>("gearInputB");
            var gearAB = CreateSimComponent<ConfigurableAddDefinitionProxy>("gearInputAB");
            gearAB.aReader = new PortReferenceDefinition(DVPortValueType.CONTROL, "GEAR_A");
            gearAB.bReader = new PortReferenceDefinition(DVPortValueType.CONTROL, "GEAR_B");
            gearAB.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.CONTROL, "GEAR");

            var genericHornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            genericHornControl.defaultValue = 0;
            genericHornControl.smoothTime = 0.2f;
            var hornControl = CreateSibling<HornControlProxy>(genericHornControl);
            hornControl.portId = FullPortId(genericHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;

            var waterDetector = CreateWaterDetector();

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var driveRpmCalc = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("driveShaftRpmCalculator");
            driveRpmCalc.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            driveRpmCalc.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");
            driveRpmCalc.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "DRIVE_SHAFT_RPM");

            var engine = CreateDieselEngine(out var engineOff, out var engineOn, out var environmentDamage, out var engineExplosion);

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

            var coolerPassive = CreateSimComponent<PassiveCoolerDefinitionProxy>("fcCoolerPassive");
            coolerPassive.transform.parent = fluidCoupler.transform;
            var coolerDirection = CreateSimComponent<DirectionalCoolerDefinitionProxy>("fcCoolerDirectional");
            coolerDirection.transform.parent = fluidCoupler.transform;
            var coolant = CreateSimComponent<HeatReservoirDefinitionProxy>("coolant");
            coolant.transform.parent = fluidCoupler.transform;
            coolant.inputCount = 4;
            coolant.OnValidate();

            var transmissionA = CreateSimComponent<SmoothTransmissionDefinitionProxy>("transmissionA");
            transmissionA.ApplyDM3BoxADefaults();
            var transmissionB = CreateSimComponent<SmoothTransmissionDefinitionProxy>("transmissionB");
            transmissionB.ApplyDM3BoxBDefaults();
            var transmissionAB = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("transmissionAB");
            transmissionAB.aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO_A");
            transmissionAB.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "GEAR_RATIO_B");
            transmissionAB.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");

            var gearRatioCalc = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("gearRatioCalculator");
            gearRatioCalc.aReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "HYDRAULIC_GEAR_RATIO");
            gearRatioCalc.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");
            gearRatioCalc.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "OVERALL_GEAR_RATIO");

            var poweredAxles = CreateSimComponent<ConstantPortDefinitionProxy>("poweredAxles");
            poweredAxles.value = 3;
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(poweredAxles, "NUM");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");
            wheelslip.engineBrakingActivePortId = FullPortId(engine, "RETARDER_BRAKE_EFFECT");
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

            horn.powerFuseId = FullFuseId(fusebox, 0);
            sander.powerFuseId = FullFuseId(fusebox, 0);
            engine.engineStarterFuseId = FullFuseId(fusebox, 1);

            // Damage.
            _damageController.mechanicalPTDamagerPortIds = new[]
            {
                FullPortId(engine, "GENERATED_ENGINE_DAMAGE"),
                FullPortId(fluidCoupler, "GENERATED_DAMAGE"),
                FullPortId(transmissionA, "GENERATED_DAMAGE"),
                FullPortId(transmissionB, "GENERATED_DAMAGE")
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
            ConnectPorts(fluidCoupler, "OUTPUT_SHAFT_TORQUE", transmissionA, "TORQUE_IN");
            ConnectPorts(transmissionA, "TORQUE_OUT", transmissionB, "TORQUE_IN");
            ConnectPorts(transmissionB, "TORQUE_OUT", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(gearA, "NUM_OF_GEARS", transmissionA, "NUM_OF_GEARS");
            ConnectPortRef(gearB, "NUM_OF_GEARS", transmissionB, "NUM_OF_GEARS");
            ConnectPortRef(gearAB, "GEAR_A", gearA, "GEAR");
            ConnectPortRef(gearAB, "GEAR_B", gearB, "GEAR");

            ConnectPortRef(horn, "HORN_CONTROL", genericHornControl, "CONTROL");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(driveRpmCalc, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(driveRpmCalc, "MECHANICAL_GEAR_RATIO", transmissionAB, "MECHANICAL_GEAR_RATIO");

            ConnectPortRef(engine, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(engine, "RETARDER", retarder, "EXT_IN");
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
            ConnectEmptyPortRef(fluidCoupler, "HYDRODYNAMIC_BRAKE");
            ConnectPortRef(fluidCoupler, "REVERSER", reverser, "REVERSER");
            ConnectPortRef(fluidCoupler, "INPUT_SHAFT_RPM", engine, "RPM");
            ConnectPortRef(fluidCoupler, "MAX_RPM", engine, "MAX_RPM");
            ConnectPortRef(fluidCoupler, "OUTPUT_SHAFT_RPM", driveRpmCalc, "DRIVE_SHAFT_RPM");
            ConnectPortRef(fluidCoupler, "TEMPERATURE", coolant, "TEMPERATURE");

            ConnectPortRef(coolerPassive, "TEMPERATURE", coolant, "TEMPERATURE");
            ConnectEmptyPortRef(coolerPassive, "TARGET_TEMPERATURE");
            ConnectPortRef(coolerDirection, "SPEED", traction, "FORWARD_SPEED_EXT_IN");
            ConnectPortRef(coolerDirection, "TEMPERATURE", coolant, "TEMPERATURE");
            ConnectEmptyPortRef(coolerDirection, "TARGET_TEMPERATURE");

            ConnectPortRef(loadTorque, "LOAD_TORQUE_0", compressor, "LOAD_TORQUE");
            ConnectPortRef(loadTorque, "LOAD_TORQUE_1", fluidCoupler, "INPUT_SHAFT_TORQUE");

            ConnectHeatRef(coolant, 0, engine, "HEAT_OUT");
            ConnectHeatRef(coolant, 1, fluidCoupler, "HEAT_OUT");
            ConnectHeatRef(coolant, 2, coolerPassive, "HEAT_OUT");
            ConnectHeatRef(coolant, 3, coolerDirection, "HEAT_OUT");

            ConnectPortRef(transmissionA, "GEAR", gearA, "GEAR");
            ConnectPortRef(transmissionA, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(transmissionA, "RETARDER", retarder, "EXT_IN");
            ConnectPortRef(transmissionA, "ENGINE_RPM", engine, "RPM");

            ConnectPortRef(transmissionB, "GEAR", gearB, "GEAR");
            ConnectPortRef(transmissionB, "THROTTLE", throttle, "EXT_IN");
            ConnectPortRef(transmissionB, "RETARDER", retarder, "EXT_IN");
            ConnectPortRef(transmissionB, "ENGINE_RPM", engine, "RPM");

            ConnectPortRef(transmissionAB, "GEAR_RATIO_A", transmissionA, "GEAR_RATIO");
            ConnectPortRef(transmissionAB, "GEAR_RATIO_B", transmissionB, "GEAR_RATIO");

            ConnectPortRef(gearRatioCalc, "HYDRAULIC_GEAR_RATIO", fluidCoupler, "GEAR_RATIO");
            ConnectPortRef(gearRatioCalc, "MECHANICAL_GEAR_RATIO", transmissionAB, "MECHANICAL_GEAR_RATIO");

            // Apply defaults.
            ApplyMethodToAll<IDM3Defaults>(s => s.ApplyDM3Defaults());

            // Control blockers.
            AddControlBlocker(throttle, retarder, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                .blockedControlPortId = FullPortId(throttle, "EXT_IN");

            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", 20, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", -20, BlockType.BLOCK_ON_BELOW_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");

            AddControlBlocker(retarder, throttle, "EXT_IN", 0, BlockType.BLOCK_ON_ABOVE_THRESHOLD)
                .blockedControlPortId = FullPortId(retarder, "EXT_IN");
        }

        private void CreateDM1U()
        {
            // Simulation components.
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var brakeCut = CreateOverridableControl(OverridableControlType.TrainBrakeCutout);
            var gearSelect = CreateSimComponent<ManualTransmissionInputDefinitionProxy>("gearSelect");
            var reverser = CreateReverserControl();
            
            var externalHornControl = CreateExternalControl("hornControl");
            var hornControl = CreateSibling<HornControlProxy>(externalHornControl);
            hornControl.portId = FullPortId(externalHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;

            var waterDetector = CreateWaterDetector();

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var driveRpmCalc = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("driveShaftRpmCalculator");
            driveRpmCalc.aReader = new PortReferenceDefinition(DVPortValueType.RPM, "WHEEL_RPM");
            driveRpmCalc.bReader = new PortReferenceDefinition(DVPortValueType.GENERIC, "MECHANICAL_GEAR_RATIO");
            driveRpmCalc.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.RPM, "DRIVE_SHAFT_RPM");

            var engine = CreateDieselEngine(out var engineOff, out var engineOn, out var environmentDamage, out var engineExplosion);

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

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("fcCoolerPassive");
            cooler.transform.parent = fluidCoupler.transform;
            var coolant = CreateSimComponent<HeatReservoirDefinitionProxy>("coolant");
            coolant.transform.parent = fluidCoupler.transform;
            coolant.inputCount = 3;
            coolant.OnValidate();

            var transmission = CreateSimComponent<SmoothTransmissionDefinitionProxy>("transmission");

            var poweredAxles = CreateSimComponent<ConstantPortDefinitionProxy>("poweredAxles");
            poweredAxles.value = 1;
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(poweredAxles, "NUM");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");
            wheelslip.engineBrakingActivePortId = FullPortId(engine, "RETARDER_BRAKE_EFFECT");
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

            horn.powerFuseId = FullFuseId(fusebox, 0);
            sander.powerFuseId = FullFuseId(fusebox, 0);
            engine.engineStarterFuseId = FullFuseId(fusebox, 1);

            // Damage.
            _damageController.mechanicalPTDamagerPortIds = new[]
            {
                FullPortId(engine, "GENERATED_ENGINE_DAMAGE"),
                FullPortId(fluidCoupler, "GENERATED_DAMAGE"),
                FullPortId(transmission, "GENERATED_DAMAGE")
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
            ConnectPorts(fluidCoupler, "OUTPUT_SHAFT_TORQUE", transmission, "TORQUE_IN");
            ConnectPorts(transmission, "TORQUE_OUT", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(gearSelect, "REVERSER", reverser, "REVERSER");
            ConnectPortRef(gearSelect, "NUM_OF_GEARS", transmission, "NUM_OF_GEARS");

            ConnectPortRef(horn, "HORN_CONTROL", externalHornControl, "EXT_IN");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(driveRpmCalc, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(driveRpmCalc, "MECHANICAL_GEAR_RATIO", transmission, "GEAR_RATIO");

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
            ConnectEmptyPortRef(fluidCoupler, "HYDRODYNAMIC_BRAKE");
            ConnectPortRef(fluidCoupler, "REVERSER", gearSelect, "REVERSER_OUT");
            ConnectPortRef(fluidCoupler, "INPUT_SHAFT_RPM", engine, "RPM");
            ConnectPortRef(fluidCoupler, "MAX_RPM", engine, "MAX_RPM");
            ConnectPortRef(fluidCoupler, "OUTPUT_SHAFT_RPM", driveRpmCalc, "DRIVE_SHAFT_RPM");
            ConnectPortRef(fluidCoupler, "TEMPERATURE", coolant, "TEMPERATURE");

            ConnectPortRef(cooler, "TEMPERATURE", coolant, "TEMPERATURE");
            ConnectEmptyPortRef(cooler, "TARGET_TEMPERATURE");

            ConnectPortRef(loadTorque, "LOAD_TORQUE_0", compressor, "LOAD_TORQUE");
            ConnectPortRef(loadTorque, "LOAD_TORQUE_1", fluidCoupler, "INPUT_SHAFT_TORQUE");

            ConnectHeatRef(coolant, 0, engine, "HEAT_OUT");
            ConnectHeatRef(coolant, 1, fluidCoupler, "HEAT_OUT");
            ConnectHeatRef(coolant, 2, cooler, "HEAT_OUT");

            ConnectPortRef(transmission, "GEAR", gearSelect, "GEAR");
            ConnectPortRef(transmission, "THROTTLE", throttle, "EXT_IN");
            ConnectEmptyPortRef(transmission, "RETARDER");
            ConnectPortRef(transmission, "ENGINE_RPM", engine, "RPM");

            // Apply defaults.
            ApplyMethodToAll<IDM1UDefaults>(s => s.ApplyDM1UDefaults());

            // Control blockers.
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", 20, BlockType.BLOCK_ON_ABOVE_THRESHOLD);
            AddControlBlocker(reverser, traction, "WHEEL_RPM_EXT_IN", -20, BlockType.BLOCK_ON_BELOW_THRESHOLD)
                .blockedControlPortId = FullPortId(reverser, "CONTROL_EXT_IN");
        }
    }
}
