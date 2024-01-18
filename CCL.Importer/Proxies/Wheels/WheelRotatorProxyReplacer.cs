using AutoMapper;
using CCL.Importer.Types;
using CCL.Types.Proxies.Wheels;
using DV.Wheels;

namespace CCL.Importer.Proxies.Wheels
{
    public class WheelRotationProxyReplacer : Profile
    {
        public WheelRotationProxyReplacer()
        {
            CreateMap<WheelRotationViaAnimationProxy, WheelRotationViaAnimation>().AutoCacheAndMap();
            CreateMap<WheelRotationViaCodeProxy, WheelRotationViaCode>().AutoCacheAndMap();
            CreateMap<PoweredWheelRotationViaAnimationProxy, PoweredWheelRotationViaAnimation>().AutoCacheAndMap();
            CreateMap<PoweredWheelRotationViaCodeProxy, PoweredWheelRotationViaCode>().AutoCacheAndMap();
        }
    }
}
