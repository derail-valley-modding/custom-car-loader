using AutoMapper;
using CCL.Types.Proxies.Particles;
using DV.Simulation.Controllers;

namespace CCL.Importer.Proxies.Particles
{
    internal class ParticleProxyReplacer : Profile
    {
        public ParticleProxyReplacer()
        {
            CreateMap<ParticlesPortReadersControllerProxy, ParticlesPortReadersController>().AutoCacheAndMap();

            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader, ParticlesPortReadersController.ParticlePortReader>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition,
                ParticlesPortReadersController.ParticlePortReader.PortParticleUpdateDefinition>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition,
                ParticlesPortReadersController.ParticlePortReader.PropertyChangeDefinition>();

            CreateMap<ParticlesPortReadersControllerProxy.ParticleColorPortReader, ParticlesPortReadersController.ParticleColorPortReader>();

            CreateMap<ParticlesPortReadersControllerProxy.ValueModifier, ParticlesPortReadersController.ValueModifier>();
        }
    }
}
