using AutoMapper;
using CCL.Types.Proxies.Controllers;
using DV.Damage;
using DV.Simulation.Brake;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;

namespace CCL.Importer.Proxies.Controllers
{
    internal class MiscControllerReplacer : Profile
    {
        public MiscControllerReplacer()
        {
            CreateMap<DamageControllerProxy, DamageController>().AutoCacheAndMap();
            CreateMap<CompressorSimControllerProxy, CompressorSimController>().AutoCacheAndMap();
            CreateMap<DeadTractionMotorsControllerProxy, DeadTractionMotorsController>().AutoCacheAndMap()
                .AfterMap(DeadTractionMotorsControllerAfter);
            CreateMap<ExplosionActivationOnSignalProxy, ExplosionActivationOnSignal>().AutoCacheAndMap()
                .ForMember(d => d.explosionPrefab, o => o.MapFrom(s => QuickAccess.Explosions.GetExplosionPrefab(s.explosionPrefab)));
            CreateMap<EngineOnReaderProxy, EngineOnReader>().AutoCacheAndMap();
            CreateMap<EnvironmentDamagerProxy, EnvironmentDamager>().AutoCacheAndMap();

            CreateMap<MagicShovellingProxy, MagicShoveling>().AutoCacheAndMap();
            CreateMap<CoalPileSimControllerProxy, CoalPileSimController>().AutoCacheAndMap();
            CreateMap<FireboxSimControllerProxy, FireboxSimController>().AutoCacheAndMap();
            CreateMap<BoilerSimControllerProxy, BoilerSimController>().AutoCacheAndMap();

            CreateMap<WindowsBreakingControllerProxy, WindowsBreakingController>().AutoCacheAndMap();
            CreateMap<BlowbackParticlePortReaderProxy, BlowbackParticlePortReader>().AutoCacheAndMap()
                .ForMember(d => d.blowbackParticlesPrefab, o => o.MapFrom(s => QuickAccess.Explosions.GetExplosionPrefab(s.blowbackParticlesPrefab)));

            CreateMap<ClapperControllerProxy, ClapperController>().AutoCacheAndMap();
            CreateMap<GearShifterProxy, GearShifter>().AutoCacheAndMap();

            CreateMap<HandcarControllerProxy, HandcarController>().AutoCacheAndMap();
            CreateMap<HandcarHandbrakeControllerProxy, HandcarHandbrakeController>().AutoCacheAndMap();
            CreateMap<HandcarBarControllerProxy, HandcarBarController>().AutoCacheAndMap();

            CreateMap<LightIntensityPortModifierProxy, LightIntensityPortModifier>().AutoCacheAndMap()
                .ForMember(d => d.cabLightsController, o => o.MapFrom(s => Mapper.GetFromCache(s.cabLightsController)));
        }

        private void DeadTractionMotorsControllerAfter(DeadTractionMotorsControllerProxy _, DeadTractionMotorsController controller)
        {
            controller.firePrefab = QuickAccess.Explosions.DE6DeadTM.firePrefab;
            controller.sparksPrefab = QuickAccess.Explosions.DE6DeadTM.sparksPrefab;
            controller.tmBlowPrefab = QuickAccess.Explosions.DE6DeadTM.tmBlowPrefab;
        }
    }
}
