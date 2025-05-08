using CCL.Types;
using CCL.Types.Proxies.Controllers;
using CCL.Types.Proxies.Simulation.Electric;

namespace CCL.Creator.Wizards.SimSetup
{
    internal abstract partial class SimCreator
    {
        protected SlugsPowerProviderModuleProxy CreateSlugsPowerProviderModule(TractionGeneratorDefinitionProxy tractionGen,
            SlugsPowerCalculatorDefinitionProxy slugPowerCalc)
        {
            var slugPowerProv = CreateSibling<SlugsPowerProviderModuleProxy>(slugPowerCalc);

            slugPowerProv.generatorVoltagePortId = FullPortId(tractionGen, "VOLTAGE");
            slugPowerProv.slugsEffectiveResistancePortId = FullPortId(slugPowerCalc, "EXTERNAL_EFFECTIVE_RESISTANCE_EXT_IN");
            slugPowerProv.slugsTotalAmpsPortId = FullPortId(slugPowerCalc, "EXTERNAL_AMPS_EXT_IN");

            return slugPowerProv;
        }

        protected void CreateTMsExtras(TractionMotorSetDefinitionProxy tm,
            out DeadTractionMotorsControllerProxy deadTMs, out ExplosionActivationOnSignalProxy tmExplosion)
        {
            deadTMs = CreateSibling<DeadTractionMotorsControllerProxy>(tm);
            deadTMs.overheatFuseOffPortId = FullPortId(tm, "OVERHEAT_POWER_FUSE_OFF");

            tmExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(tm);
            tmExplosion.explosionSignalPortId = FullPortId(tm, "OVERSPEED_EXPLOSION_TRIGGER");
            tmExplosion.bodyDamagePercentage = 0.05f;
            tmExplosion.explosionPrefab = ExplosionPrefab.TMOverspeed;
        }
    }
}
