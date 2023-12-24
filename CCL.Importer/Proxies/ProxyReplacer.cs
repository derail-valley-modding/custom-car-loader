using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using static AutoMapper.Internal.ExpressionFactory;

namespace CCL.Importer.Proxies
{
    [InheritedExport(typeof(IProxyReplacer))]
    public abstract class ProxyReplacer : IProxyReplacer
    {
        private bool _NeedsCache = false;
        private IEnumerable<ProxyMapAttribute> proxyAttributesWithoutCache;
        private IEnumerable<ProxyMapAttribute> proxyAttributesWithCache;

        protected bool NeedsCache
        {
            get
            {
                return _NeedsCache;
            }
            set
            {
                _NeedsCache = _NeedsCache || value; // can never turn back false if set to true
            }
        }
        public ProxyReplacer() : base() {
            this.proxyAttributesWithoutCache = from a in this.GetType().GetCustomAttributes<ProxyMapAttribute>()
                                   where a.FieldsFromCache.Count() == 0
                                   select a;
            this.proxyAttributesWithCache = from a in this.GetType().GetCustomAttributes<ProxyMapAttribute>()
                                            where a.FieldsFromCache.Count() > 0
                                            select a;

        }
        public void ReplaceProxies(GameObject prefab)
        {
            foreach (var proxy in this.proxyAttributesWithoutCache)
            {
                prefab.MapComponentsInChildren(proxy.SourceType, proxy.DestinationType, proxy.Predicate);
            }
            foreach (var proxy in this.proxyAttributesWithCache)
            {
                prefab.StoreComponentsInChildrenInCache(proxy.SourceType, proxy.DestinationType, proxy.Predicate);
            }
        }

        public void ReplaceProxiesFromCache(GameObject prefab)
        {
            foreach (var proxy in this.proxyAttributesWithCache)
            {
                prefab.ConvertFromCache(proxy.SourceType, proxy.DestinationType, proxy.Predicate);
            }
        }
    }
}
