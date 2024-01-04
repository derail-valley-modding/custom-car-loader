using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.HUD;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Proxies.Controls
{
    [Export(typeof(IProxyReplacer))]
    public class OverridableControlReplacer : Profile, IProxyReplacer
    {
        public OverridableControlReplacer()
        {
            foreach (var targetType in _controlToImplMap.Values)
            {
                CreateMap(typeof(OverridableControlProxy), targetType);
            }
        }

        public void CacheAndReplaceProxies(GameObject prefab)
        {
            foreach (var overridableProxy in prefab.GetComponentsInChildren<OverridableControlProxy>(true))
            {
                if (_controlToImplMap.TryGetValue(overridableProxy.ControlType, out Type targetType))
                {
                    prefab.StoreComponentsInChildrenInCache(typeof(OverridableControlProxy), targetType, _ => true);
                }
            }
        }

        public void MapProxies(GameObject prefab)
        {
            foreach (var overridableProxy in prefab.GetComponentsInChildren<OverridableControlProxy>(true))
            {
                if (_controlToImplMap.TryGetValue(overridableProxy.ControlType, out Type targetType))
                {
                    prefab.ConvertFromCache(typeof(OverridableControlProxy), targetType, _ => true);
                }
            }
        }

        public void ReplaceProxiesUncached(GameObject prefab)
        {

        }

        private static readonly Dictionary<OverridableControlType, Type> _controlToImplMap = new()
        {
            { OverridableControlType.Throttle, typeof(ThrottleControl) },
            { OverridableControlType.TrainBrake, typeof(BrakeControl) },
            { OverridableControlType.Reverser, typeof(ReverserControl) },
            { OverridableControlType.IndBrake, typeof(IndependentBrakeControl) },
            { OverridableControlType.Handbrake, typeof(HandbrakeControl) },
            { OverridableControlType.Sander, typeof(SanderControl) },
            { OverridableControlType.Horn, typeof(HornControl) },
            { OverridableControlType.HeadlightsFront, typeof(HeadlightsControlFront) },
            { OverridableControlType.HeadlightsRear, typeof(HeadlightsControlRear) },
            { OverridableControlType.DynamicBrake, typeof(DynamicBrakeControl) },
            { OverridableControlType.FuelCutoff, typeof(PowerOffControl) },
        };
    }

    [ProxyMap(typeof(InteriorControlsManagerProxy), typeof(InteriorControlsManager))]
    [ProxyMap(typeof(BaseControlsOverriderProxy), typeof(BaseControlsOverrider))]
    public class InteriorControlReplacer : ProxyReplacer
    {

    }
}
