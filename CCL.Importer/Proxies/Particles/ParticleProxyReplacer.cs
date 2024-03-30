﻿using AutoMapper;
using CCL.Types.Proxies.Particles;
using DV.Simulation.Controllers;
using DV.VFX;

namespace CCL.Importer.Proxies.Particles
{
    internal class ParticleProxyReplacer : Profile
    {
        public ParticleProxyReplacer()
        {
            CreateMap<ParticlesPortReadersControllerProxy, ParticlesPortReadersController>().AutoCacheAndMap()
                .AfterMap(ParticlesPortReadersControllerAfter);
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader, ParticlesPortReadersController.ParticlePortReader>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition,
                ParticlesPortReadersController.ParticlePortReader.PortParticleUpdateDefinition>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition,
                ParticlesPortReadersController.ParticlePortReader.PropertyChangeDefinition>();
            CreateMap<ParticlesPortReadersControllerProxy.ParticleColorPortReader, ParticlesPortReadersController.ParticleColorPortReader>();
            CreateMap<ParticlesPortReadersControllerProxy.ValueModifier, ParticlesPortReadersController.ValueModifier>();

            CreateMap<CylinderCockParticlePortReaderProxy, CylinderCockParticlePortReader>().AutoCacheAndMap();
            CreateMap<SteamSmokeParticlePortReaderProxy, SteamSmokeParticlePortReader>().AutoCacheAndMap();

            CreateMap<WorldMoverParticleSimulationSpaceProxy, WorldMoverParticleSimulationSpace>().AutoCacheAndMap();
        }

        private void ParticlesPortReadersControllerAfter(ParticlesPortReadersControllerProxy _, ParticlesPortReadersController comp)
        {
            comp.OnValidate();
        }
    }
}
