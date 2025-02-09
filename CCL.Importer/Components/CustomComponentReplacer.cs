using AutoMapper;
using CCL.Importer.Components.Headlights;
using CCL.Types.Components;
using CCL.Types.Components.Headlights;

namespace CCL.Importer.Components
{
    internal class CustomComponentReplacer : Profile
    {
        public CustomComponentReplacer()
        {
            CreateMap<CarAutoCoupler, CarAutoCouplerInternal>().AutoCacheAndMap();
            CreateMap<RigidCoupler, RigidCouplerInternal>().AutoCacheAndMap();
            CreateMap<VirtualHandbrakeOverrider, VirtualHandbrakeOverriderInternal>().AutoCacheAndMap();
            CreateMap<DuplicateHandbrakeOverrider, DuplicateHandbrakeOverriderInternal>().AutoCacheAndMap();

            MapHeadlights();
        }

        private void MapHeadlights()
        {
            CreateMap<FrontConnectedDualCarAutomaticHeadlightsController, FrontConnectedDualCarAutomaticHeadlightsControllerInternal>().AutoCacheAndMap();
            CreateMap<FrontAndRearConnectedTriCarAutomaticHeadlightsController, FrontAndRearConnectedTriCarAutomaticHeadlightsControllerInternal>().AutoCacheAndMap();
        }
    }
}
