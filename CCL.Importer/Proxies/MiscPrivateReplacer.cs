using AutoMapper;
using CCL.Types.Proxies;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [ProxyMap(typeof(ExplosionModelHandlerProxy), typeof(ExplosionModelHandler))]
    [ProxyMap(typeof(ExplosionModelHandlerProxy.MaterialSwapData), typeof(ExplosionModelHandler.MaterialSwapData))]
    [ProxyMap(typeof(ExplosionModelHandlerProxy.GameObjectSwapData), typeof(ExplosionModelHandler.GameObjectSwapData))]
    [Export(typeof(IProxyReplacer))]
    internal class MiscPrivateReplacer : Profile, IProxyReplacer
    {
        public MiscPrivateReplacer()
        {
            ShouldMapField = f => f.IsPublic | f.IsPrivate;
        }

        public void CacheAndReplaceProxies(GameObject prefab) { }

        public void MapProxies(GameObject prefab) { }

        public void ReplaceProxiesUncached(GameObject prefab) { }
    }
}
