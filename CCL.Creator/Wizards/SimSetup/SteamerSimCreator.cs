using CCL.Types;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEngine;
using static CCL.Types.Proxies.Controls.BaseControlsOverriderProxy;
using static CCL.Types.Proxies.Ports.ConfigurablePortsDefinitionProxy;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class SteamerSimCreator : SimCreator
    {
        // TODO:
        // Whistle control
        // Headlights
        // Power off

        public SteamerSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "S060", "S282" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);

            // Lights.

            var blower = CreateExternalControl("blower");
            var whistle = CreateExternalControl("whistle");
            // Whistle control.
            var damper = CreateExternalControl("damper");
            var cylCock = CreateExternalControl("cylinderCock");
            var fireDoor = CreateExternalControl("fireboxDoor");
            var injector = CreateExternalControl("injector");
            var blowdown = CreateExternalControl("blowdown");

            var reverser = CreateReverserControl();
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);

            var poweredAxles = CreateSimComponent<ConstantPortDefinitionProxy>("poweredAxles");
            poweredAxles.value = basisIndex switch
            {
                0 => 3,
                1 => 4,
                _ => 0
            };
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var coalDump = CreateExternalControl("coalDumpControl");
            var firebox = CreateSimComponent<FireboxDefinitionProxy>("firebox");
            var fireboxSimController = CreateSibling<FireboxSimControllerProxy>(firebox);
            fireboxSimController.ConnectFirebox(firebox);
            fireboxSimController.fireboxDoorPortId = FullPortId(fireDoor, "EXT_IN");
            var engineOn = CreateSibling<EngineOnReaderProxy>(firebox);
            engineOn.portId = FullPortId(firebox, "FIRE_ON");
            // Power Off Control
            var environmentDamage = CreateSibling<EnvironmentDamagerProxy>(firebox);
            environmentDamage.damagerPortId = FullPortId(firebox, "COAL_ENV_DAMAGE_METER");
            environmentDamage.environmentDamageResource = BaseResourceType.EnvironmentDamageCoal;

            var steamCalc = CreateSimComponent<MultiplePortSumDefinitionProxy>("steamConsumptionCalculator");
            steamCalc.inputs = new[]
            {
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "EXHAUST"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "COMPRESSOR"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "ENGINE"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "CYLINDER_DUMP"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "DYNAMO"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "BELL"),
            };
            steamCalc.output = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "OUT");

            var boiler = CreateSimComponent<BoilerDefinitionProxy>("boiler");
            var boilerSimController = CreateSibling<BoilerSimControllerProxy>(boiler);
            boilerSimController.anglePortId = FullPortId(boiler, "BOILER_ANGLE_EXT_IN");
            var explosion = CreateSibling<ExplosionActivationOnSignalProxy>(boiler);
            explosion.bodyDamagePercentage = 1.0f;
            explosion.wheelsDamagePercentage = 1.0f;
            explosion.mechanicalPTDamagePercentage = 1.0f;
            explosion.electricalPTDamagePercentage = 1.0f;
            explosion.explosion = ExplosionPrefab.ExplosionBoiler;
            explosion.explosionSignalPortId = FullPortId(boiler, "IS_BROKEN");
            explosion.explodeTrainCar = true;

            var compressorControl = CreateExternalControl("compressorControl");
            var compressor = CreateSimComponent<SteamCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var dynamoControl = CreateExternalControl("dynamoControl");
            var dynamo = CreateSimComponent<DynamoDefinitionProxy>("dynamo");

            var fuseController = CreateSimComponent<FuseControllerDefinitionProxy>("electronicsFuseController");
            fuseController.controllingPort = new PortReferenceDefinition(DVPortValueType.STATE, "CONTROLLING_PORT");

            var throttleCalc = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("throttleCalculator");
            throttleCalc.aReader = new PortReferenceDefinition(DVPortValueType.PRESSURE, "BOILER_PRESSURE");
            throttleCalc.bReader = new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE");
            throttleCalc.mulReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.PRESSURE, "STEAM_CHEST_PRESSURE");

            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var lubricatorControl = CreateExternalControl("lubricatorControl");
            var lubricator = CreateSimComponent<MechanicalLubricatorDefinitionProxy>("lubricator");

            var bellControl = CreateExternalControl("bellControl");
            var bell = CreateSimComponent<SteamBellDefinitionProxy>("bell");

            var steamEngine = CreateSimComponent<ReciprocatingSteamEngineDefinitionProxy>("steamEngine");
            var exhaust = CreateSimComponent<SteamExhaustDefinitionProxy>("exhaust");

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);

            SimComponentDefinitionProxy water = null!;
            SimComponentDefinitionProxy coal = null!;

            switch (basisIndex)
            {
                case 0:
                    var locoWater = CreateResourceContainer(ResourceContainerType.Water);
                    var locoCoal = CreateResourceContainer(ResourceContainerType.Coal);
                    CreateCoalPile(locoCoal);

                    water = locoWater;
                    coal = locoCoal;
                    break;
                case 1:
                    var tenderWater = CreateSimComponent<ConfigurablePortsDefinitionProxy>("tenderWater");
                    tenderWater.Ports = new[]
                    {
                        new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "NORMALIZED"), 0),
                        new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "CAPACITY"), 30000),
                        new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "AMOUNT"), 0),
                        new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "CONSUME_EXT_IN"), 0)
                    };
                    CreateBroadcastConsumer(tenderWater, "NORMALIZED", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_NORMALIZED", 0, false);
                    CreateBroadcastConsumer(tenderWater, "CAPACITY", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_CAPACITY", 1, false);
                    CreateBroadcastConsumer(tenderWater, "AMOUNT", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_AMOUNT", 0, false);
                    CreateBroadcastProvider(tenderWater, "CONSUME_EXT_IN", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_WATER_CONSUME");

                    var tenderCoal = CreateSimComponent<ConfigurablePortsDefinitionProxy>("tenderCoal");
                    tenderCoal.Ports = new[]
                    {
                        new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "NORMALIZED"), 0),
                        new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "CAPACITY"), 10000),
                    };
                    CreateBroadcastConsumer(tenderCoal, "NORMALIZED", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_COAL_NORMALIZED", 0, false);
                    CreateBroadcastConsumer(tenderCoal, "CAPACITY", DVPortForwardConnectionType.COUPLED_FRONT, "TENDER_COAL_CAPACITY", 1, false);

                    water = tenderWater;
                    coal = tenderCoal;
                    break;
                default:
                    break;
            }

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fuseboxDummy");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false)
            };

            switch (basisIndex)
            {
                case 1:
                    _baseControls.propagateNeutralStateToRear = true;
                    CreateBroadcastProvider(dynamo, "DYNAMO_FLOW_NORMALIZED", DVPortForwardConnectionType.COUPLED_REAR, "DYNAMO_FLOW");
                    break;
                default:
                    break;
            }

            _baseControls.neutralStateSetters = new[]
            {
                new PortSetter(FullPortId(injector, "EXT_IN"), 0),
                new PortSetter(FullPortId(blower, "EXT_IN"), 0),
                new PortSetter(FullPortId(blowdown, "EXT_IN"), 0),
                new PortSetter(FullPortId(bellControl, "EXT_IN"), 0),
                new PortSetter(FullPortId(lubricatorControl, "EXT_IN"), 0),
                new PortSetter(FullPortId(dynamoControl, "EXT_IN"), 0),
                new PortSetter(FullPortId(compressorControl, "EXT_IN"), 0)
            };

            ConnectPorts(steamEngine, "TORQUE_OUT", traction, "TORQUE_IN");

            ConnectPortRef(oil, "AMOUNT", lubricator, "OIL");
            ConnectPortRef(oil, "CONSUME_EXT_IN", lubricator, "OIL_CONSUMPTION");
            ConnectPortRef(lubricatorControl, "EXT_IN", lubricator, "LUBRICATOR_CONTROL");
            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", lubricator, "WHEEL_RPM");

            ConnectPortRef(bellControl, "EXT_IN", bell, "CONTROL");
            ConnectPortRef(boiler, "PRESSURE", bell, "STEAM_PRESSURE");

            ConnectPortRef(damper, "EXT_IN", firebox, "DAMPER_CONTROL");
            ConnectPortRef(coalDump, "EXT_IN", firebox, "COAL_DUMP_CONTROL");
            ConnectPortRef(exhaust, "AIR_FLOW", firebox, "AIR_FLOW");
            ConnectPortRef(traction, "FORWARD_SPEED_EXT_IN", firebox, "FORWARD_SPEED");
            ConnectPortRef(boiler, "PRESSURE", firebox, "BOILER_PRESSURE");
            ConnectPortRef(boiler, "TEMPERATURE", firebox, "BOILER_TEMPERATURE");

            ConnectPortRef(exhaust, "STEAM_CONSUMPTION", steamCalc, "EXHAUST");
            ConnectPortRef(compressor, "STEAM_CONSUMPTION", steamCalc, "COMPRESSOR");
            ConnectPortRef(steamEngine, "STEAM_FLOW", steamCalc, "ENGINE");
            ConnectPortRef(steamEngine, "DUMPED_FLOW", steamCalc, "CYLINDER_DUMP");
            ConnectPortRef(dynamo, "STEAM_CONSUMPTION", steamCalc, "DYNAMO");
            ConnectPortRef(bell, "STEAM_CONSUMPTION", steamCalc, "BELL");

            ConnectPortRef(sand, "AMOUNT", sander, "SAND");
            ConnectPortRef(sand, "CONSUME_EXT_IN", sander, "SAND_CONSUMPTION");

            ConnectPortRef(injector, "EXT_IN", boiler, "INJECTOR");
            ConnectPortRef(blowdown, "EXT_IN", boiler, "BLOWDOWN");
            ConnectPortRef(firebox, "HEAT", boiler, "HEAT");
            ConnectPortRef(firebox, "TEMPERATURE", boiler, "FIREBOX_TEMPERATURE");
            //ConnectPortRef(, "", boiler, "FEEDWATER_TEMPERATURE");
            ConnectPortRef(steamCalc, "OUT", boiler, "STEAM_CONSUMPTION");
            ConnectPortRef(water, "AMOUNT", boiler, "WATER");
            ConnectPortRef(water, "CONSUME_EXT_IN", boiler, "WATER_CONSUMPTION");

            ConnectPortRef(compressorControl, "EXT_IN", compressor, "COMPRESSOR_CONTROL");
            ConnectPortRef(boiler, "PRESSURE", compressor, "STEAM_PRESSURE");
            ConnectPortRef(dynamoControl, "EXT_IN", dynamo, "CONTROL");
            ConnectPortRef(boiler, "PRESSURE", dynamo, "STEAM_PRESSURE");

            ConnectPortRef(dynamo, "DYNAMO_FLOW_NORMALIZED", fuseController, "CONTROLLING_PORT");

            ConnectPortRef(boiler, "PRESSURE", throttleCalc, "BOILER_PRESSURE");
            ConnectPortRef(throttle, "EXT_IN", throttleCalc, "THROTTLE");

            ConnectPortRef(reverser, "REVERSER", steamEngine, "REVERSER_CONTROL");
            ConnectPortRef(cylCock, "EXT_IN", steamEngine, "CYLINDER_COCK_CONTROL");
            ConnectPortRef(throttleCalc, "STEAM_CHEST_PRESSURE", steamEngine, "STEAM_CHEST_PRESSURE");
            ConnectPortRef(firebox, "TEMPERATURE", steamEngine, "STEAM_CHEST_TEMPERATURE");
            ConnectPortRef(boiler, "OUTLET_STEAM_QUALITY", steamEngine, "STEAM_QUALITY");
            ConnectPortRef(traction, "WHEEL_RPM_EXT_IN", steamEngine, "CRANK_RPM");
            ConnectPortRef(lubricator, "LUBRICATION_NORMALIZED", steamEngine, "LUBRICATION_NORMALIZED");

            ConnectPortRef(steamEngine, "STEAM_FLOW", exhaust, "EXHAUST_FLOW");
            ConnectPortRef(boiler, "PRESSURE", exhaust, "BOILER_PRESSURE");
            ConnectPortRef(blower, "EXT_IN", exhaust, "BLOWER_CONTROL");
            ConnectPortRef(whistle, "EXT_IN", exhaust, "WHISTLE_CONTROL");

            switch (basisIndex)
            {
                case 0:
                    ApplyMethodToAll<IS060Defaults>(s => s.ApplyS060Defaults());
                    break;
                case 1:
                    ApplyMethodToAll<IS282Defaults>(s => s.ApplyS282Defaults());
                    break;
                default:
                    break;
            }

            _root.AddComponent<MagicShovellingProxy>();
        }
    }
}
