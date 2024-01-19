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

            // heat
            CreateMap<PassiveCoolerDefinitionProxy, PassiveCoolerDefinition>().AutoCacheAndMap();
            CreateMap<DirectionalCoolerDefinitionProxy, DirectionalMovementCoolerDefinition>().AutoCacheAndMap();
            CreateMap<HeatReservoirDefinitionProxy, HeatReservoirDefinition>().AutoCacheAndMap();

            // air
            CreateMap<MechanicalCompressorDefinitionProxy, MechanicalCompressorDefinition>().AutoCacheAndMap();
        }
    }
}
