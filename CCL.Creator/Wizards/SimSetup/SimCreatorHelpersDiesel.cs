using CCL.Types.Proxies.Controllers;
using CCL.Types;
using CCL.Types.Proxies.Controls;
using CCL.Types.Proxies.Simulation.Diesel;

namespace CCL.Creator.Wizards.SimSetup
{
    internal abstract partial class SimCreator
    {
        protected DieselEngineDirectDefinitionProxy CreateDieselEngine(out PowerOffControlProxy engineOff, out EngineOnReaderProxy engineOn,
            out EnvironmentDamagerProxy environmentDamage, out ExplosionActivationOnSignalProxy engineExplosion)
        {
            var engine = CreateSimComponent<DieselEngineDirectDefinitionProxy>("de");

            engineOff = CreateSibling<PowerOffControlProxy>(engine);
            engineOff.portId = FullPortId(engine, "EMERGENCY_ENGINE_OFF_EXT_IN");

            engineOn = CreateSibling<EngineOnReaderProxy>(engine);
            engineOn.portId = FullPortId(engine, "ENGINE_ON");

            environmentDamage = CreateSibling<EnvironmentDamagerProxy>(engine);
            environmentDamage.damagerPortId = FullPortId(engine, "FUEL_ENV_DAMAGE_METER");
            environmentDamage.environmentDamageResource = BaseResourceType.EnvironmentDamageFuel;

            engineExplosion = CreateSibling<ExplosionActivationOnSignalProxy>(engine);
            engineExplosion.explosion = ExplosionPrefab.Mechanical;
            engineExplosion.bodyDamagePercentage = 0.1f;
            engineExplosion.explosionSignalPortId = FullPortId(engine, "IS_BROKEN");

            return engine;
        }
    }
}
