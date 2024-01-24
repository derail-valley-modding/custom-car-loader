using AutoMapper;
using CCL.Importer.Types;
using CCL.Types.Proxies.Wheels;
using DV;
using DV.Wheels;
using LocoSim.Implementations.Wheels;
using System.Linq;

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

            CreateMap<PoweredWheelsManagerProxy, PoweredWheelsManager>().AutoCacheAndMap()
                .ForMember(s => s.poweredWheels, o => o.MapFrom(s => Mapper.GetFromCache(s.poweredWheels).ToArray()));
            CreateMap<PoweredWheelProxy, PoweredWheel>().AutoCacheAndMap();

            CreateMap<WheelslipControllerProxy, WheelslipController>().AutoCacheAndMap();
            CreateMap<PoweredWheelRotationViaAnimationProxy.AnimatorStartTimeOffsetPair, PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair>();
            CreateMap<PoweredWheelRotationViaCodeProxy.TransformRotationConfig, TransformRotationConfig>();
        }
    }
}
