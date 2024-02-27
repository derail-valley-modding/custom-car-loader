using CCL.Types.Proxies;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Resources;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Electric;
using UnityEngine;

namespace CCL.Creator.Wizards.SimSetup
{
    internal class DieselElectricSimCreator : SimCreator
    {
        public DieselElectricSimCreator(GameObject prefabRoot) : base(prefabRoot) { }

        public override string[] SimBasisOptions => new[] { "DE2", "DE6" };

        public override void CreateSimForBasisImpl(int basisIndex)
        {
            var throttle = CreateOverridableControl(OverridableControlType.Throttle);
            var thrtPowr = CreateSimComponent<ThrottleGammaPowerConversionDefinitionProxy>("throttlePower");
            var reverser = CreateReverserControl();
            var trnBrake = CreateOverridableControl(OverridableControlType.TrainBrake);
            var indBrake = CreateOverridableControl(OverridableControlType.IndBrake);
            ExternalControlDefinitionProxy dynBrake;

            if (basisIndex == 1)
            {
                dynBrake = CreateOverridableControl(OverridableControlType.DynamicBrake);
            }

            var fuel = CreateResourceContainer(ResourceContainerType.Fuel);
            var oil = CreateResourceContainer(ResourceContainerType.Oil);
            var sand = CreateResourceContainer(ResourceContainerType.Sand);
            var sander = CreateOverridableControl(OverridableControlType.Sander);

            var engine = CreateSimComponent<DieselEngineDirectDefinitionProxy>("de");
            var compressor = CreateSimComponent<MechanicalCompressorDefinitionProxy>("compressor");
            var airController = CreateSibling<CompressorSimControllerProxy>(compressor);
            airController.activationSignalExtInPortId = FullPortId(compressor, "ACTIVATION_SIGNAL_EXT_IN");
            airController.isPoweredPortId = FullPortId(compressor, "IS_POWERED");
            airController.productionRateOutPortId = FullPortId(compressor, "PRODUCTION_RATE");
            airController.mainReservoirVolumePortId = FullPortId(compressor, "MAIN_RES_VOLUME");
            airController.activationPressureThresholdPortId = FullPortId(compressor, "ACTIVATION_PRESSURE_THRESHOLD");
            airController.compressorHealthStatePortId = FullPortId(compressor, "COMPRESSOR_HEALTH_EXT_IN");

            var tractionGen = CreateSimComponent<TractionGeneratorDefinitionProxy>("tractionGenerator");
            var slugPowerCalc = CreateSimComponent<SlugsPowerCalculatorDefinitionProxy>("slugsPowerCalculator");
            var slugPowerProv = CreateSibling<SlugsPowerProviderModuleProxy>(slugPowerCalc);
            slugPowerCalc.transform.parent = tractionGen.transform;

            var tm = CreateSimComponent<TractionMotorSetDefinitionProxy>("tm");
            var deadTMs = CreateSibling<DeadTractionMotorsControllerProxy>(tm);
            var tmExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(tm);
            tmExplosion.bodyDamagePercentage = 0.05f;

            var cooler = CreateSimComponent<PassiveCoolerDefinitionProxy>("tmPassiveCooler");
            cooler.transform.parent = tm.transform;
            var heat = CreateSimComponent<HeatReservoirDefinitionProxy>("tmHeat");
            heat.transform.parent = tm.transform;

            var tmRpm = CreateSimComponent<ConfigurableMultiplierDefinitionProxy>("tmRpmCalculator");
            var transmission = CreateSimComponent<TransmissionFixedGearDefinitionProxy>("transmission");
            var traction = CreateSimComponent<TractionDefinitionProxy>("traction");
            var tractionFeeders = CreateSibling<TractionPortFeedersProxy>(traction);
            tractionFeeders.forwardSpeedPortId = FullPortId(traction, "FORWARD_SPEED_EXT_IN");
            tractionFeeders.wheelRpmPortId = FullPortId(traction, "WHEEL_RPM_EXT_IN");
            tractionFeeders.wheelSpeedKmhPortId = FullPortId(traction, "WHEEL_SPEED_KMH_EXT_IN");

            var fusebox = CreateSimComponent<IndependentFusesDefinitionProxy>("fusebox");
            fusebox.fuses = new[]
            {
                new FuseDefinition("ELECTRONICS_MAIN", false),
                new FuseDefinition("ENGINE_STARTER", false),
                new FuseDefinition("TM_POWER", false),
            };
            engine.engineStarterFuseId = FullPortId(fusebox, "ENGINE_STARTER");

            switch (basisIndex)
            {
                case 0:
                    ApplyMethodToAll<IDE2Defaults>(s => s.ApplyDE2Defaults());
                    break;
                case 1:
                    ApplyMethodToAll<IDE6Defaults>(s => s.ApplyDE6Defaults());
                    break;
                default:
                    break;
            }
        }
    }
}
