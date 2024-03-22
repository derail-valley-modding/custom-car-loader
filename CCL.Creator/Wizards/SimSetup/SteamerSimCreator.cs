using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class SteamerSimCreator : SimCreator
    {
        public SteamerSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "S060", "S282" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);

            // Lights.

            var blower = CreateExternalControl("blower");
            // Whistle.
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

            var steamConsume = CreateSimComponent<MultiplePortSumDefinitionProxy>("steamConsumptionCalculator");
            steamConsume.inputs = new[]
            {
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "EXHAUST"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "COMPRESSOR"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "ENGINE"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "CYLINDER_DUMP"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "DYNAMO"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "BELL"),
            };
            steamConsume.output = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.MASS_RATE, "OUT");

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
            CreateBroadcastProvider(dynamo, "DYNAMO_FLOW_NORMALIZED", DVPortForwardConnectionType.COUPLED_REAR, "DYNAMO_FLOW");

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

            SimComponentDefinitionProxy water;
            SimComponentDefinitionProxy coal;

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
                case 0:
                    ApplyMethodToAll<IS060Defaults>(s => s.ApplyS060Defaults());
                    break;
                case 1:
                    ApplyMethodToAll<IS282Defaults>(s => s.ApplyS282Defaults());
                    break;
                default:
                    break;
            }
        }
    }
}
