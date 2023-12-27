using AutoMapper;
using AutoMapper.Internal;
using CCL.Types.Proxies;
using DV;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [Export(typeof(IProxyReplacer))]
    public class InternalExternalSnapshotSwitcherReplacer : ProxyReplacer<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher> { }
    
    [Export(typeof(IProxyReplacer))]
    public class PlayerDistanceGameObjectsDisablerReplacer : ProxyReplacer<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler> { }
    
    [Export(typeof(IProxyReplacer))]
    public class PlayerDistanceMultipleGameObjectsOptimizerReplacer : ProxyReplacer<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>
    {
        protected override void Customize(IMappingExpression<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer> cfg)
        {
            cfg.ForMember(d => d.scriptsToDisable, o => o.MapFrom(s => Mapper.GetEnumerableFromCache(s.scriptsToDisable)));
        }
    }
}
