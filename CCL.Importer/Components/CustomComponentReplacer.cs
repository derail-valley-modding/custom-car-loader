using AutoMapper;
using CCL.Types.Components;

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
        }
    }
}
