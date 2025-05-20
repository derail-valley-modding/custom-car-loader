using CCL.Types;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Steam;
using CCL.Types.Proxies.Wheels;
using UnityEngine;

using static CCL.Types.Proxies.Controls.BaseControlsOverriderProxy;
using static CCL.Types.Proxies.Ports.ConfigurablePortsDefinitionProxy;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class SteamerSimCreator : SimCreator
    {
        // TODO:
        // Headlights

        public SteamerSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "S060", "S282" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            // Simulation components.
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var brakeCut = CreateOverridableControl(OverridableControlType.TrainBrakeCutout);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);

            var headlights = CreateSimComponent<MultiplePortDecoderEncoderDefinitionProxy>("headlightDecoder");
            var lightsF = CreateSibling<OverridableControlProxy>(headlights);
            var lightsR = CreateSibling<OverridableControlProxy>(headlights);
            lightsF.ControlType = OverridableControlType.HeadlightsFront;
            lightsR.ControlType = OverridableControlType.HeadlightsRear;

            if (HasTender(basisIndex))
            {
                CreateBroadcastProvider(headlights, "FRONT_HEADLIGHTS_EXT_IN", DVPortForwardConnectionType.COUPLED_REAR, "HEADLIGHTS_FRONT");
                CreateBroadcastProvider(headlights, "REAR_HEADLIGHTS_EXT_IN", DVPortForwardConnectionType.COUPLED_REAR, "HEADLIGHTS_REAR");
            }

            var blower = CreateExternalControl("blower");
            var whistle = CreateExternalControl("whistle");
            var hornControl = CreateSibling<HornControlProxy>(whistle);
            hornControl.portId = FullPortId(whistle, "EXT_IN");
            hornControl.neutralAt0 = true;
            var damper = CreateExternalControl("damper", true, 1.0f);
            var cylCock = CreateExternalControl("cylinderCock", true);
            var fireDoor = CreateExternalControl("fireboxDoor", true);
            var injector = CreateExternalControl("injector", true);
            var blowdown = CreateExternalControl("blowdown", true);

            var reverser = CreateReverserControl(isAnalog: true);
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);

            var poweredAxles = CreateSimComponent<ConfigurablePortDefinitionProxy>("poweredAxles");
            poweredAxles.value = PoweredAxleCount(basisIndex);
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var waterDetector = CreateWaterDetector();

            var coalDump = CreateExternalControl("coalDumpControl", true);
            coalDump.defaultValue = 0.5f;

            var firebox = CreateSimComponent<FireboxDefinitionProxy>("firebox");
            var fireboxSimController = CreateSibling<FireboxSimControllerProxy>(firebox);
            fireboxSimController.ConnectFirebox(firebox);
            fireboxSimController.fireboxDoorPortId = FullPortId(fireDoor, "EXT_IN");
            var engineOn = CreateSibling<EngineOnReaderProxy>(firebox);
            engineOn.portId = FullPortId(firebox, "FIRE_ON");
            var engineOff = CreateSibling<PowerOffControlProxy>(firebox);
            engineOff.portId = FullPortId(firebox, "EXTINGUISH_EXT_IN");
            engineOff.signalClearedBySim = true;
            var environmentDamage = CreateSibling<EnvironmentDamagerProxy>(firebox);
            environmentDamage.damagerPortId = FullPortId(firebox, "COAL_ENV_DAMAGE_METER");
            environmentDamage.environmentDamageResource = BaseResourceType.EnvironmentDamageCoal;
            var fireMass = CreateSibling<ResourceMassPortReaderProxy>(firebox);
            fireMass.resourceMassPortId = FullPortId(firebox, "COAL_LEVEL");

            var steamCalc = CreateSimComponent<MultiplePortSumDefinitionProxy>("steamConsumptionCalculator");
            steamCalc.inputs = new[]
            {
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "EXHAUST"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "COMPRESSOR"),
                new PortReferenceDefinition(DVPortValueType.MASS_RATE, "ENGINE"),
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
            explosion.explosionPrefab = ExplosionPrefab.Boiler;
            explosion.explosionSignalPortId = FullPortId(boiler, "IS_BROKEN");
            explosion.explodeTrainCar = true;
            var boilerMass = CreateSibling<ResourceMassPortReaderProxy>(boiler);
            boilerMass.resourceMassPortId = FullPortId(boiler, "WATER_MASS");

            var compressorControl = CreateOverridableControl(OverridableControlType.AirPump);
            var compressor = CreateSimComponent<SteamCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);
            airController.mainResPressureNormalizedPortId = FullPortId(compressor, "MAIN_RES_PRESSURE_NORMALIZED");

            var dynamoControl = CreateOverridableControl(OverridableControlType.Dynamo);
            var dynamo = CreateSimComponent<DynamoDefinitionProxy>("dynamo");

            var fuseController = CreateSimComponent<FuseControllerDefinitionProxy>("electronicsFuseController");
            fuseController.controllingPort = new PortReferenceDefinition(DVPortValueType.STATE, "CONTROLLING_PORT");

            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var lubricatorControl = CreateExternalControl("lubricatorControl", true);
            var lubricatorSmoothing = CreateSibling<SmoothedOutputDefinitionProxy>(lubricatorControl, "lubricatorControlSmoothing");
            lubricatorSmoothing.smoothTime = 0.2f;
            var lubricator = CreateSimComponent<MechanicalLubricatorDefinitionProxy>("lubricator");

            var oilingPoints = CreateSimComponent<ManualOilingPointsDefinitionProxy>("oilingPoints");

            var bellControl = CreateExternalControl("bellControl", true);
            var bell = CreateSimComponent<SteamBellDefinitionProxy>("bell");

            var steamEngine = CreateSimComponent<ReciprocatingSteamEngineDefinitionProxy>("steamEngine");
            var exhaust = CreateSimComponent<SteamExhaustDefinitionProxy>("exhaust");

            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(poweredAxles, "NUM");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");

            // Handle tender and non tender steam engines.
            SimComponentDefinitionProxy water = null!;
            SimComponentDefinitionProxy coal = null!;

            if (HasTender(basisIndex))
            {
                var tenderWater = CreateSimComponent<ConfigurablePortsDefinitionProxy>("tenderWater");
                tenderWater.Ports = new[]
                {
                    new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "NORMALIZED"), 0),
                    new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "CAPACITY"), 30000),
                    new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.WATER, "AMOUNT"), 0),
                    new PortStartValue(new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.WATER, "CONSUME_EXT_IN"), 0)
                };
                tenderWater.OnValidate();
                CreateBroadcastConsumer(tenderWater, "NORMALIZED", DVPortForwardConnectionType.COUPLED_REAR, "TENDER_WATER_NORMALIZED", 0, false);
                CreateBroadcastConsumer(tenderWater, "CAPACITY", DVPortForwardConnectionType.COUPLED_REAR, "TENDER_WATER_CAPACITY", 1, false);
                CreateBroadcastConsumer(tenderWater, "AMOUNT", DVPortForwardConnectionType.COUPLED_REAR, "TENDER_WATER_AMOUNT", 0, false);
                CreateBroadcastProvider(tenderWater, "CONSUME_EXT_IN", DVPortForwardConnectionType.COUPLED_REAR, "TENDER_WATER_CONSUME");

                var tenderCoal = CreateSimComponent<ConfigurablePortsDefinitionProxy>("tenderCoal");
                tenderCoal.Ports = new[]
                {
                    new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.COAL, "NORMALIZED"), 0),
                    new PortStartValue(new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.COAL, "CAPACITY"), 10000),
                };
                tenderCoal.OnValidate();
                CreateBroadcastConsumer(tenderCoal, "NORMALIZED", DVPortForwardConnectionType.COUPLED_REAR, "TENDER_COAL_NORMALIZED", 0, false);
                CreateBroadcastConsumer(tenderCoal, "CAPACITY", DVPortForwardConnectionType.COUPLED_REAR, "TENDER_COAL_CAPACITY", 1, false);

                water = tenderWater;
                coal = tenderCoal;
            }
            else
            {
                var locoWater = CreateResourceContainer(ResourceContainerType.Water);
                var locoCoal = CreateResourceContainer(ResourceContainerType.Coal);
                CreateCoalPile(locoCoal);

                water = locoWater;
                coal = locoCoal;
            }

            // Fusebox and fuse connections.
            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fuseboxDummy");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false)
            };

            var oilGearLamp = CreateLampDecreasingWarning("oilGearsLamp", DVPortValueType.OIL, 1, 0.5f, 0.001f, 0, false);
            var oilingPointsLamp = CreateLampDecreasingWarning("oilingPointsLamp", DVPortValueType.STATE, 1, 0.05f, 0.001f, 0, false);

            fuseController.fuseId = FullFuseId(fusebox, 0);

            if (HasTender(basisIndex))
            {
                _baseControls.propagateNeutralStateToRear = true;
                CreateBroadcastProvider(dynamo, "DYNAMO_FLOW_NORMALIZED", DVPortForwardConnectionType.COUPLED_REAR, "DYNAMO_FLOW");
            }

            // Neutral states.
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

            // Damage.
            _damageController.bodyHealthStateExternalInPortIds = new[]
            {
                FullPortId(boiler, "BODY_HEALTH_EXT_IN")
            };

            _damageController.mechanicalPTDamagerPortIds = new[]
            {
                FullPortId(steamEngine, "GENERATED_MECHANICAL_DAMAGE"),
                FullPortId(oilingPoints, "MECHANICAL_DAMAGE")
            };
            _damageController.mechanicalPTHealthStateExternalInPortIds = new[]
            {
                FullPortId(steamEngine, "HEALTH_STATE_EXT_IN"),
                FullPortId(lubricator, "MECHANICAL_PT_HEALTH_EXT_IN"),
                FullPortId(oilingPoints, "MECHANICAL_PT_HEALTH_EXT_IN")
            };

            // Port Overrider.
            var portOverrider = AddBasePortsOverrider();
            FillPortOverriderSteamer(portOverrider, boiler, oilingPoints, lubricator);

            // Port connections.
            ConnectPorts(steamEngine, "TORQUE_OUT", traction, "TORQUE_IN");

            // Port reference connections.
            ConnectPortRef(lubricatorSmoothing, "CONTROL", lubricatorControl, "EXT_IN");
            ConnectPortRef(lubricator, "OIL", oil, "AMOUNT");
            ConnectPortRef(lubricator, "OIL_CONSUMPTION", oil, "CONSUME_EXT_IN");
            ConnectPortRef(lubricator, "MANUAL_FILL_RATE_NORMALIZED", lubricatorSmoothing, "OUTPUT");
            ConnectPortRef(lubricator, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");

            ConnectPortRef(oilingPoints, "OIL_STORAGE", oil, "AMOUNT");
            ConnectPortRef(oilingPoints, "OIL_CONSUMPTION", oil, "CONSUME_EXT_IN");
            ConnectPortRef(oilingPoints, "WHEEL_RPM", traction, "WHEEL_RPM_EXT_IN");

            ConnectPortRef(bell, "CONTROL", bellControl, "EXT_IN");
            ConnectPortRef(bell, "STEAM_PRESSURE", boiler, "PRESSURE");

            ConnectPortRef(firebox, "COAL_DUMP_CONTROL", coalDump, "EXT_IN");
            ConnectPortRef(firebox, "INTAKE_WATER_CONTENT", waterDetector, "STATE_EXT_IN");
            ConnectPortRef(firebox, "AIR_FLOW", exhaust, "AIR_FLOW");
            ConnectPortRef(firebox, "FORWARD_SPEED", traction, "FORWARD_SPEED_EXT_IN");
            ConnectPortRef(firebox, "BOILER_PRESSURE", boiler, "PRESSURE");
            ConnectPortRef(firebox, "BOILER_TEMPERATURE", boiler, "TEMPERATURE");
            ConnectPortRef(firebox, "BOILER_BROKEN_STATE", boiler, "IS_BROKEN");

            ConnectPortRef(steamCalc, "EXHAUST", exhaust, "STEAM_CONSUMPTION");
            ConnectPortRef(steamCalc, "COMPRESSOR", compressor, "STEAM_CONSUMPTION");
            ConnectPortRef(steamCalc, "ENGINE", steamEngine, "INTAKE_FLOW");
            ConnectPortRef(steamCalc, "DYNAMO", dynamo, "STEAM_CONSUMPTION");
            ConnectPortRef(steamCalc, "BELL", bell, "STEAM_CONSUMPTION");

            ConnectPortRef(sander, "SAND", sand, "AMOUNT");
            ConnectPortRef(sander, "SAND_CONSUMPTION", sand, "CONSUME_EXT_IN");

            ConnectPortRef(boiler, "INJECTOR", injector, "EXT_IN");
            ConnectPortRef(boiler, "BLOWDOWN", blowdown, "EXT_IN");
            ConnectPortRef(boiler, "HEAT", firebox, "HEAT");
            ConnectPortRef(boiler, "FIREBOX_TEMPERATURE", firebox, "TEMPERATURE");
            if (HasFeedwaterHeater(basisIndex))
            {
                ConnectPortRef(boiler, "FEEDWATER_TEMPERATURE", steamEngine, "EXHAUST_TEMPERATURE");
            }
            else
            {
                ConnectEmptyPortRef(boiler, "FEEDWATER_TEMPERATURE");
            }
            ConnectPortRef(boiler, "STEAM_CONSUMPTION", steamCalc, "OUT");
            ConnectPortRef(boiler, "WATER", water, "AMOUNT");
            ConnectPortRef(boiler, "WATER_CONSUMPTION", water, "CONSUME_EXT_IN");

            ConnectPortRef(compressor, "COMPRESSOR_CONTROL", compressorControl, "EXT_IN");
            ConnectPortRef(compressor, "STEAM_PRESSURE", boiler, "PRESSURE");
            ConnectPortRef(dynamo, "CONTROL", dynamoControl, "EXT_IN");
            ConnectPortRef(dynamo, "STEAM_PRESSURE", boiler, "PRESSURE");

            ConnectPortRef(fuseController, "CONTROLLING_PORT", dynamo, "DYNAMO_FLOW_NORMALIZED");

            ConnectPortRef(steamEngine, "THROTTLE_CONTROL", throttle, "EXT_IN");
            ConnectPortRef(steamEngine, "REVERSER_CONTROL", reverser, "REVERSER");
            ConnectPortRef(steamEngine, "CYLINDER_COCK_CONTROL", cylCock, "EXT_IN");
            ConnectPortRef(steamEngine, "INTAKE_PRESSURE", boiler, "PRESSURE");
            if (HasSuperheater(basisIndex))
            {
                ConnectPortRef(steamEngine, "INTAKE_TEMPERATURE", firebox, "TEMPERATURE");
            }
            else
            {
                ConnectPortRef(steamEngine, "INTAKE_TEMPERATURE", boiler, "TEMPERATURE");
            }
            ConnectPortRef(steamEngine, "INTAKE_QUALITY", boiler, "OUTLET_STEAM_QUALITY");
            ConnectPortRef(steamEngine, "CRANK_RPM", traction, "WHEEL_RPM_EXT_IN");
            ConnectPortRef(steamEngine, "LUBRICATION_NORMALIZED", lubricator, "LUBRICATION_NORMALIZED");

            ConnectPortRef(exhaust, "EXHAUST_FLOW", steamEngine, "EXHAUST_FLOW");
            ConnectPortRef(exhaust, "ENGINE_MAX_FLOW", steamEngine, "MAX_FLOW");
            ConnectPortRef(exhaust, "BOILER_PRESSURE", boiler, "PRESSURE");
            ConnectPortRef(exhaust, "BLOWER_CONTROL", blower, "EXT_IN");
            ConnectPortRef(exhaust, "WHISTLE_CONTROL", whistle, "EXT_IN");
            ConnectPortRef(exhaust, "DAMPER_CONTROL", damper, "EXT_IN");

            ConnectLampRef(oilGearLamp, lubricator, "LUBRICATION_NORMALIZED");
            ConnectLampRef(oilingPointsLamp, oilingPoints, "LOWEST_OIL_LEVEL_NORMALIZED");

            // Apply defaults.
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

            // Shovelling.
            if (!_root.TryGetComponent(out MagicShovellingProxy shovelling))
            {
                shovelling = _root.AddComponent<MagicShovellingProxy>();
            }
        }

        private static int PoweredAxleCount(int basis) => basis switch
        {
            0 => 3,
            1 => 4,
            _ => 0,
        };

        private static bool HasTender(int basis)
        {
            return basis switch
            {
                1 => true,
                _ => false,
            };
        }

        private static bool HasSuperheater(int basis)
        {
            return basis switch
            {
                1 => true,
                _ => false,
            };
        }

        private static bool HasFeedwaterHeater(int basis)
        {
            return basis switch
            {
                1 => true,
                _ => false,
            };
        }
    }
}
