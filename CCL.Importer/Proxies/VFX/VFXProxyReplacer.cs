using AutoMapper;
using CCL.Types.Proxies.VFX;
using DV.Simulation.Controllers;
using DV.VFX;
using UnityEngine;

namespace CCL.Importer.Proxies.VFX
{
    internal class VFXProxyReplacer : Profile
    {
        public VFXProxyReplacer()
        {
            CreateMap<ParticlesPortReadersControllerProxy, ParticlesPortReadersController>().AutoCacheAndMap()
                .ReplaceGOs<ParticlesPortReadersControllerProxy, ParticlesPortReadersController, GameObject>()
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
        }

        private void ParticlesPortReadersControllerAfter(ParticlesPortReadersControllerProxy _, ParticlesPortReadersController comp)
        {
            comp.OnValidate();
        }
    }
}
