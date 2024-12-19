using AutoMapper;
using CCL.Types.Proxies.Simulation.Steam;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Simulation
{
    internal class SteamSimulationReplacer : Profile
    {
        public SteamSimulationReplacer()
        {
            // Main.
            CreateMap<FireboxDefinitionProxy, FireboxDefinition>().AutoCacheAndMap();
            CreateMap<BoilerDefinitionProxy, BoilerDefinition>().AutoCacheAndMap();
            CreateMap<ReciprocatingSteamEngineDefinitionProxy, ReciprocatingSteamEngineDefinition>().AutoCacheAndMap();
            CreateMap<SteamExhaustDefinitionProxy, SteamExhaustDefinition>().AutoCacheAndMap();
            // Appliances.
            CreateMap<SteamCompressorDefinitionProxy, SteamCompressorDefinition>().AutoCacheAndMap();
            CreateMap<DynamoDefinitionProxy, DynamoDefinition>().AutoCacheAndMap();
            CreateMap<SteamBellDefinitionProxy, SteamBellDefinition>().AutoCacheAndMap();
            // Lubrication.
            CreateMap<ManualOilingPointsDefinitionProxy, ManualOilingPointsDefinition>().AutoCacheAndMap();
            CreateMap<ManualOilingPointsDefinitionProxy.OilingPointDefinition, ManualOilingPointsDefinition.OilingPointDefinition>();
            CreateMap<MechanicalLubricatorDefinitionProxy, MechanicalLubricatorDefinition>().AutoCacheAndMap();
            CreateMap<LubricatorRatchetProxy, LubricatorRatchet>().AutoCacheAndMap();
            CreateMap<LubricatorRatchetDriverProxy, LubricatorRatchetDriver>().AutoCacheAndMap();
        }
    }
}
