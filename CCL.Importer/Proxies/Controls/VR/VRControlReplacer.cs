using AutoMapper;
using CCL.Types.Proxies.Controls.VR;
using DV;
using DV.CabControls;

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

            CreateMap<SpeedZoneControlTouchBehaviourProxy, SpeedZoneControlTouchBehaviour>().AutoCacheAndMap();
        }
    }
}
