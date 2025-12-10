using AutoMapper;
using CCL.Types.Proxies.Controls.VR;
using DV;
using DV.CabControls;
using DV.Interaction;

namespace CCL.Importer.Proxies.Controls.VR
{
    internal class VRControlReplacer : Profile
    {
        public VRControlReplacer()
        {
            CreateMap<LineHandSnapperProxy, LineHandSnapper>().AutoCacheAndMap();
            CreateMap<ValveHandSnapperProxy, ValveHandSnapper>().AutoCacheAndMap();
            CreateMap<CircleHandSnapperProxy, CircleHandSnapper>().AutoCacheAndMap();
            CreateMap<PointHandSnapperProxy, PointHandSnapper>().AutoCacheAndMap();

            CreateMap<SnapperTargetReassignerProxy, SnapperTargetReassigner>().AutoCacheAndMap()
                .ForMember(d => d.snapper, o => o.MapFrom(s => Mapper.GetFromCache(s.snapper)));

            CreateMap<SpeedZoneControlTouchBehaviourProxy, SpeedZoneControlTouchBehaviour>().AutoCacheAndMap();
            CreateMap<DoorClosedVRTouchDisableProxy, DoorClosedVRTouchDisable>().AutoCacheAndMap();
        }
    }
}
