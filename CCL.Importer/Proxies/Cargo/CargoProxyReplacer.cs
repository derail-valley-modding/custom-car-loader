using AutoMapper;
using CCL.Types.Proxies.Cargo;
using DV.Cargo;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Proxies.Cargo
{
    [ProxyMap(typeof(CargoBoundsProxy), typeof(CargoBounds))]
    [ProxyMap(typeof(CargoWaterDamageProxy), typeof(CargoWaterDamage))]
    [ProxyMap(typeof(CargoReactionToDamageProxy), typeof(CargoReactionToDamage))]
    [Export(typeof(IProxyReplacer))]
    internal class CargoProxyReplacer : Profile, IProxyReplacer
    {
        public void CacheAndReplaceProxies(GameObject prefab) { }

        public void MapProxies(GameObject prefab) { }

        public void ReplaceProxiesUncached(GameObject prefab) { }
    }
}
