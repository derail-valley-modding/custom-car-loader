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
        private IEnumerable<ProxyMapAttribute> proxyAttributes;

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
            this.proxyAttributes = this.GetType().GetCustomAttributes<ProxyMapAttribute>();
        }
        public virtual void CacheAndReplaceProxies(GameObject prefab)
        {
            foreach (var proxy in this.proxyAttributes)
            {
                prefab.StoreComponentsInChildrenInCache(proxy.SourceType, proxy.DestinationType, proxy.Predicate);
            }
        }
        //By default, this does nothing - custom implementations might want to use this step
        public virtual void ReplaceProxiesUncached(GameObject prefab)
        {
            return;
        }
        public virtual void MapProxies(GameObject prefab)
        {
            foreach (var proxy in this.proxyAttributes)
            {
                prefab.ConvertFromCache(proxy.SourceType, proxy.DestinationType, proxy.Predicate);
            }
        }
    }
}
