using AutoMapper;
using CCL.Types.Proxies.Simulation.Diesel;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Simulation
{
    internal class DieselSimulationReplacer : Profile
    {
        public DieselSimulationReplacer() 
        {
            CreateMap<DieselEngineDirectDefinitionProxy, DieselEngineDirectDefinition>().AutoCacheAndMap();
            CreateMap<HydraulicTransmissionDefinitionProxy, HydraulicTransmissionDefinition>().AutoCacheAndMap();
            CreateMap<HydraulicTransmissionDefinitionProxy.HydraulicConfigDefinition, HydraulicTransmissionDefinition.HydraulicConfigDefinition>();

            // air
            CreateMap<MechanicalCompressorDefinitionProxy, MechanicalCompressorDefinition>().AutoCacheAndMap();
        }
    }
}
