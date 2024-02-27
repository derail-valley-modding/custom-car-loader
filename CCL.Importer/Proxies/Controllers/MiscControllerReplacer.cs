using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Damage;
using DV.Simulation.Brake;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using UnityEngine;

namespace CCL.Importer.Proxies.Controllers
{
    internal class MiscControllerReplacer : Profile
    {
        private static DeadTractionMotorsController? s_de6deadTM;
        private static GameObject? s_tmExplosion;

        private static DeadTractionMotorsController DE6DeadTM => Extensions.GetCached(ref s_de6deadTM,
            () => TrainCarType.LocoDiesel.ToV2().prefab.GetComponentInChildren<DeadTractionMotorsController>());
        private static GameObject TMExplosion => Extensions.GetCached(ref s_tmExplosion,
            () => DE6DeadTM.GetComponent<ExplosionActivationOnSignal>().explosionPrefab);

        public MiscControllerReplacer()
        {
            CreateMap<DamageControllerProxy, DamageController>().AutoCacheAndMap();
            CreateMap<CompressorSimControllerProxy, CompressorSimController>().AutoCacheAndMap();
            CreateMap<DeadTractionMotorsControllerProxy, DeadTractionMotorsController>().AutoCacheAndMap()
                .AfterMap(DeadTractionMotorsControllerAfter);
            CreateMap<ExplosionActivationOnSignalProxy, ExplosionActivationOnSignal>().AutoCacheAndMap()
                .AfterMap(ExplosionActivationOnSignalAfter);
        }

        private void DeadTractionMotorsControllerAfter(DeadTractionMotorsControllerProxy _, DeadTractionMotorsController controller)
        {
            controller.firePrefab = DE6DeadTM.firePrefab;
            controller.sparksPrefab = DE6DeadTM.sparksPrefab;
            controller.tmBlowPrefab = DE6DeadTM.tmBlowPrefab;
        }

        private void ExplosionActivationOnSignalAfter(ExplosionActivationOnSignalProxy _, ExplosionActivationOnSignal explosion)
        {
            explosion.explosionPrefab = TMExplosion;
        }
    }
}
