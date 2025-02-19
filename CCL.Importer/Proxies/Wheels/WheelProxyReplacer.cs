using AutoMapper;
using CCL.Importer.Components;
using CCL.Types.Proxies.Wheels;
using DV;
using DV.Wheels;
using LocoSim.Implementations.Wheels;
using System.Linq;

namespace CCL.Importer.Proxies.Wheels
{
    public class WheelProxyReplacer : Profile
    {
        public WheelProxyReplacer()
        {
            CreateMap<WheelRotationViaAnimationProxy, WheelRotationViaAnimation>().AutoCacheAndMap();
            CreateMap<WheelRotationViaCodeProxy, WheelRotationViaCode>().AutoCacheAndMap();
            CreateMap<PoweredWheelRotationViaAnimationProxy, PoweredWheelRotationViaAnimation>().AutoCacheAndMap();
            CreateMap<PoweredWheelRotationViaCodeProxy, PoweredWheelRotationViaCode>().AutoCacheAndMap();

            CreateMap<PoweredWheelsManagerProxy, PoweredWheelsManager>().AutoCacheAndMap()
                .ForMember(d => d.poweredWheels, o => o.MapFrom(s => Mapper.GetFromCache(s.poweredWheels).ToArray()))
                .AfterMap(PoweredWheelsManagerProxyAfter);
            CreateMap<PoweredWheelProxy, PoweredWheel>().AutoCacheAndMap();

            CreateMap<WheelslipControllerProxy, WheelslipController>().AutoCacheAndMap();
            CreateMap<PoweredWheelRotationViaAnimationProxy.AnimatorStartTimeOffsetPair, PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair>();
            CreateMap<PoweredWheelRotationViaCodeProxy.TransformRotationConfig, TransformRotationConfig>();
            CreateMap<WheelslipSparksControllerProxy, WheelslipSparksController>().AutoCacheAndMap();
            CreateMap<WheelslipSparksControllerProxy.WheelSparksDefinition, WheelslipSparksController.WheelSparksDefinition>()
                .ForMember(d => d.poweredWheel, o => o.MapFrom(s => Mapper.GetFromCache(s.poweredWheel)));

            CreateMap<DirectDriveMaxWheelslipRpmCalculatorProxy, DirectDriveMaxWheelslipRpmCalculator>().AutoCacheAndMap();
        }

        private void PoweredWheelsManagerProxyAfter(PoweredWheelsManagerProxy proxy, PoweredWheelsManager real)
        {
            if (proxy.GetWheelsFromDefaultBogies)
            {
                var wheels = real.poweredWheels.ToList();
                wheels.AddRange(real.transform.root.GetComponentsInChildren<PoweredWheel>());
                real.poweredWheels = wheels.ToArray();
            }
        }
    }
}
