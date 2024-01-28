using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.HUD;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using System;
using UnityEngine;

namespace CCL.Importer.Proxies.Controls
{
    public class OverridableControlReplacer : Profile
    {
        public OverridableControlReplacer()
        {
            CreateMap<OverridableControlProxy, ThrottleControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Throttle);
            CreateMap<OverridableControlProxy, BrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.TrainBrake);
            CreateMap<OverridableControlProxy, ReverserControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Reverser);
            CreateMap<OverridableControlProxy, IndependentBrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.IndBrake);
            //CreateMap<OverridableControlProxy, HandbrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Handbrake);
            CreateMap<OverridableControlProxy, SanderControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Sander);
            CreateMap<OverridableControlProxy, HornControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Horn);
            CreateMap<OverridableControlProxy, HeadlightsControlFront>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.HeadlightsFront);
            CreateMap<OverridableControlProxy, HeadlightsControlRear>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.HeadlightsRear);
            CreateMap<OverridableControlProxy, DynamicBrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.DynamicBrake);
            CreateMap<OverridableControlProxy, PowerOffControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.FuelCutoff);

            CreateMap<InteriorControlsManagerProxy, InteriorControlsManager>().AutoCacheAndMap();
            CreateMap<BaseControlsOverriderProxy, BaseControlsOverrider>().AutoCacheAndMap();
            CreateMap<BaseControlsOverriderProxy.PortSetter, BaseControlsOverrider.PortSetter>();
        }
    }
}
