using CCL.Types;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using CCL.Types.Proxies.Wheels;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselHydraulicSimCreator : SimCreator
    {
        public DieselHydraulicSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DH4" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtPowr = CreateSimComponent<ThrottleGammaPowerConversionDefinitionProxy>("throttlePower");
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            var dynBrake = CreateOverridableControl(OverridableControlType.DynamicBrake);

            var genericHornControl = CreateSimComponent<GenericControlDefinitionProxy>("hornControl");
            genericHornControl.defaultValue = 0;
            genericHornControl.smoothTime = 0.2f;
            var hornControl = CreateSibling<HornControlProxy>(genericHornControl);
            hornControl.portId = FullPortId(genericHornControl, "EXT_IN");
            hornControl.neutralAt0 = true;
            var horn = CreateSimComponent<HornDefinitionProxy>("horn");
            horn.controlNeutralAt0 = true;

            var bellControl = CreateExternalControl("bellControl", true);
            var bell = CreateSimComponent<ElectricBellDefinitionProxy>("bell");
            bell.smoothDownTime = 0.05f;

            // Headlights.

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateSanderControl();

            var engine = CreateSimComponent<DieselEngineDirectDefinitionProxy>("de");
            var engineOff = CreateSibling<OverridableControlProxy>(engine);
            engineOff.portId = FullPortId(engine, "EMERGENCY_ENGINE_OFF_EXT_IN");
            var engineOn = CreateSibling<EngineOnReaderProxy>(engine);
            engineOn.portId = FullPortId(engine, "ENGINE_ON");
            var environmentDamage = CreateSibling<EnvironmentDamagerProxy>(engine);
            environmentDamage.damagerPortId = FullPortId(engine, "FUEL_ENV_DAMAGE_METER");
            environmentDamage.environmentDamageResource = BaseResourceType.EnvironmentDamageFuel;
            var engineExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(engine);
            engineExplosion.explosion = ExplosionPrefab.ExplosionMechanical;
            engineExplosion.bodyDamagePercentage = 0.1f;
            engineExplosion.explosionSignalPortId = FullPortId(engine, "IS_BROKEN");

            var loadTorque = CreateSimComponent<ConfigurableAddDefinitionProxy>("loadTorqueCalculator");
            loadTorque.aReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_0");
            loadTorque.bReader = new PortReferenceDefinition(DVPortValueType.TORQUE, "LOAD_TORQUE_1");
            loadTorque.addReadOut = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.TORQUE, "LOAD_TORQUE_TOTAL");
            loadTorque.transform.parent = engine.transform;

            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateCompressorSim(compressor);

            var fluidCoupler = CreateSimComponent<HydraulicTransmissionDefinitionProxy>("fluidCoupler");
            var couplerExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(fluidCoupler);
            couplerExplosion.explosion = ExplosionPrefab.ExplosionHydraulic;
            couplerExplosion.bodyDamagePercentage = 0.1f;
            couplerExplosion.windowsBreakingDelay = 0.4f;
            couplerExplosion.explosionSignalPortId = FullPortId(fluidCoupler, "IS_BROKEN");

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("fcCooler");
            cooler.transform.parent = fluidCoupler.transform;
            var autoCooler = CreateSimComponent<AutomaticCoolerDefinitionProxy>("fcAutomaticCooler");
            autoCooler.transform.parent = fluidCoupler.transform;
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("coolant");
            heat.transform.parent = fluidCoupler.transform;
            heat.inputCount = 4;
            heat.OnValidate();

            var poweredAxles = CreateSimComponent<ConstantPortDefinitionProxy>("poweredAxles");
            poweredAxles.value = 4;
            poweredAxles.port = new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NUM");

            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateTractionFeeders(traction);
            var wheelslip = CreateSibling<WheelslipControllerProxy>(traction);
            wheelslip.numberOfPoweredAxlesPortId = FullPortId(poweredAxles, "NUM");
            wheelslip.sandCoefPortId = FullPortId(sander, "SAND_COEF");
            wheelslip.engineBrakingActivePortId = FullPortId(fluidCoupler, "HYDRO_DYNAMIC_BRAKE_EFFECT");

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false),
                new FuseDefinition("ENGINE_STARTER", false)
            };

            ApplyMethodToAll<IDH4Defaults>(s => s.ApplyDH4Defaults());
        }
    }
}
