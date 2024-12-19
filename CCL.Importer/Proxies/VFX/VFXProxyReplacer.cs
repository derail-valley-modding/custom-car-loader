using AutoMapper;
using CCL.Types.Proxies.VFX;
using DV.Simulation.Controllers;
using DV.VFX;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Proxies.VFX
{
    internal class VFXProxyReplacer : Profile
    {
        public VFXProxyReplacer()
        {
            CreateMap<ParticlesPortReadersControllerProxy, ParticlesPortReadersController>().AutoCacheAndMap()
                .ReplaceInstancedObjects()
                .AfterMap(ParticlesPortReadersControllerAfter);
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader, ParticlesPortReadersController.ParticlePortReader>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition,
                ParticlesPortReadersController.ParticlePortReader.PortParticleUpdateDefinition>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition,
                ParticlesPortReadersController.ParticlePortReader.PropertyChangeDefinition>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticleColorPortReader, ParticlesPortReadersController.ParticleColorPortReader>();
            CreateMap<ParticlesPortReadersControllerProxy.ValueModifier, ParticlesPortReadersController.ValueModifier>();

            CreateMap<CylinderCockParticlePortReaderProxy, CylinderCockParticlePortReader>().AutoCacheAndMap();
            CreateMap<CylinderCockParticlePortReaderProxy.CylinderSetup, CylinderCockParticlePortReader.CylinderSetup>();
            CreateMap<SteamSmokeParticlePortReaderProxy, SteamSmokeParticlePortReader>().AutoCacheAndMap();

            CreateMap<WorldMoverParticleSimulationSpaceProxy, WorldMoverParticleSimulationSpace>().AutoCacheAndMap();

            CreateMap<TunnelParticleDampeningProxy, TunnelParticleDampening>().AutoCacheAndMap()
                .ForMember(d => d.bogie, o => o.Ignore())
                .ForMember(d => d.systems, o => o.Ignore())
                .AfterMap(TunnelParticleDampeningAfter);

            CreateMap<LightShadowQualityProxy, LightShadowQuality>().AutoCacheAndMap();
            CreateMap<LightShadowQualityProxy.LightShadowQualitySettings, LightShadowQuality.LightShadowQualitySettings>();
        }

        private void ParticlesPortReadersControllerAfter(ParticlesPortReadersControllerProxy _, ParticlesPortReadersController comp)
        {
            comp.RefreshChildren();
        }

        private void TunnelParticleDampeningAfter(TunnelParticleDampeningProxy proxy, TunnelParticleDampening dampening)
        {
            // We can't link the bogies directly, so a workaround is needed. Using
            // the transform does not work either because it gets deleted when
            // using a default bogie.
            var cars = proxy.GetComponentsInParent<TrainCar>(true);

            if (cars.Length == 0)
            {
                CCLPlugin.Error("Could not find car for TunnelParticleDampening.");
                Object.Destroy(dampening);
                return;
            }

            var bogies = cars[0].GetComponentsInChildren<Bogie>().OrderBy(x => x.transform.localPosition.z);

            if (bogies.Count() == 0)
            {
                CCLPlugin.Error("Could not find bogies for TunnelParticleDampening.");
                Object.Destroy(dampening);
                return;
            }

            dampening.bogie = proxy.bogie == TunnelParticleDampeningProxy.Bogie.Rear ?
                bogies.First() :
                bogies.Last();

            // Since it's possible to use the system copy, can't link systems directly,
            // so instead the systems are obtained from the GameObjects.
            List<ParticleSystem> systems = new();

            foreach (var item in proxy.systems)
            {
                systems.AddRange(item.GetComponentsInChildren<ParticleSystem>());
            }

            dampening.systems = systems.ToArray();
        }
    }
}
