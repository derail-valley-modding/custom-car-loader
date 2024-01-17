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
            CreateMap<WheelRotationViaAnimationProxy, WheelRotationViaAnimation>().CacheAndProcessProxyAutomatically();
            CreateMap<WheelRotationViaCodeProxy, WheelRotationViaCode>().CacheAndProcessProxyAutomatically();
            CreateMap<PoweredWheelRotationViaAnimationProxy, PoweredWheelRotationViaAnimation>().CacheAndProcessProxyAutomatically();
            CreateMap<PoweredWheelRotationViaCodeProxy, PoweredWheelRotationViaCode>().CacheAndProcessProxyAutomatically();
        }
    }
}
