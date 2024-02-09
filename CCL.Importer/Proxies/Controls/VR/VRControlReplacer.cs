using AutoMapper;
using CCL.Types.Proxies.Controls.VR;
using DV;

namespace CCL.Importer.Proxies.Controls.VR
{
    internal class VRControlReplacer : Profile
    {
        public VRControlReplacer()
        {
            CreateMap<LineHandSnapperProxy, LineHandSnapper>().AutoCacheAndMap();
            CreateMap<ValveHandSnapperProxy, ValveHandSnapper>().AutoCacheAndMap();
        }
    }
}
