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

        /// <summary>
        /// (Step 1/3) For each proxy, creates the new destination script, and caches the link between proxy and "real" script
        /// </summary>
        /// <param name="prefab"></param>
        public virtual void CacheAndReplaceProxies(GameObject prefab)
        {
            foreach (var proxy in proxyAttributes)
            {
                foreach (MonoBehaviour source in prefab.GetComponentsInChildren(proxy.SourceType).Cast<MonoBehaviour>())
                {
                    if (proxy.Predicate(source))
                    {
                        CreateAndCacheTargetScript(source, proxy.SourceType, proxy.DestinationType);
                    }
                }
            }
        }

        /// <summary>
        /// Override this to add custom logic when creating/caching particular scripts
        /// </summary>
        protected virtual MonoBehaviour? CreateAndCacheTargetScript(MonoBehaviour source, Type sourceType, Type destType)
        {
            return Mapper.CreateProxyTargetAndCache(source, sourceType, destType);
        }

        /// <summary>
        /// (Step 2/3) By default, this does nothing - custom implementations might want to use this step
        /// </summary>
        public virtual void ReplaceProxiesUncached(GameObject prefab)
        {
            return;
        }

        /// <summary>
        /// (Step 3/3) Handles the actual mapping source -> dest and deletes the proxy scripts in the prefab (by default)
        /// </summary>
        public virtual void MapProxies(GameObject prefab)
        {
            foreach (var proxy in proxyAttributes)
            {
                foreach (MonoBehaviour source in prefab.GetComponentsInChildren(proxy.SourceType).Cast<MonoBehaviour>())
                {
                    if (proxy.Predicate(source))
                    {
                        MapSingleProxyScript(source, proxy.SourceType, proxy.DestinationType);
                    }
                }
            }
        }

        /// <summary>
        /// Override this to add custom logic when replacing particular scripts
        /// </summary>
        protected virtual MonoBehaviour? MapSingleProxyScript(MonoBehaviour source, Type sourceType, Type destType)
        {
            return Mapper.ConvertSingleFromCache(source, sourceType, destType);
        }
    }
}
