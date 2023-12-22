using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static AutoMapper.Internal.ExpressionFactory;

namespace CCL.Importer.Proxies
{
    public abstract class ProxyReplacer<TSource, TDestination> : Profile, IProxyReplacer
        where TSource : MonoBehaviour
        where TDestination : MonoBehaviour
    {
        private bool needsCache = false;
        public ProxyReplacer() : base() {
            var cfg = CreateMap<TSource, TDestination>();
            ApplyCacheOverrides(cfg);
            Customize(cfg);
            AddAdditionalMappings();
        }

        private void ApplyCacheOverrides(IMappingExpression<TSource, TDestination> cfg)
        {
            var cacheList = FieldsToGetFromCache();
            if (cacheList.Count > 0)
            {
                needsCache = true;
            }
            foreach (var cache in cacheList)
            {
                cfg.ForMember(cache.DestProvider, (IMemberConfigurationExpression <TSource, TDestination, MonoBehaviour> memberCfg) => {
                    memberCfg.MapFrom((src, dest) => Mapper.GetFromCache(cache.SourceProvider(src)));
                });
            }
        }

        /// <summary>
        /// Replace this in a concrete implementation to decare types that need to be replaced from cache
        /// instead of simply being replaced and converted.
        /// </summary>
        /// <returns>A list of source field mappers and destination expressions to map by replacing from cache</returns>
        protected virtual List<(Func<TSource, MonoBehaviour> SourceProvider, Expression<Func<TDestination, MonoBehaviour>> DestProvider)> FieldsToGetFromCache()
        {
            return new();
        }

        /// <summary>
        /// Replace this in a concrete implementation if you need to add additional mappers - In this function you will have access
        /// to all instance functionality on Profile to create mappers and it is guaranteed to be called before mappers are finalized
        /// </summary>
        protected virtual void AddAdditionalMappings()
        {
            return;
        }

        /// <summary>
        /// Replace this in a concrete implementation if you need to customize the behavior of the mapper
        /// </summary>
        /// <param name="mappingExpression">The AutoMapper expression for the base type pair of this replacer</param>
        protected virtual void Customize(IMappingExpression<TSource, TDestination> mappingExpression)
        {
            return;
        }

        /// <summary>
        /// Implement this to allow configuration of whether or not this source should be mapped by this converter
        /// this can be used to add logic to allow for a single source to go to multiple possible destinations
        /// </summary>
        /// <param name="sourceComponent">The actual component being replaced at runtime</param>
        /// <returns>true to allow replacement to proceed, false to prevent replacement</returns>
        /// 
        /// <remarks>If no implementation exists that returns true, the source component will never be replaced and will simply stay on the prefab</remarks>
        protected virtual bool CanReplace(TSource sourceComponent)
        {
            return true;
        }

        public void ReplaceProxies(GameObject prefab)
        {
            if (!needsCache)
            {
                prefab.MapComponentsInChildren<TSource, TDestination>(this.CanReplace);
            } else
            {
                prefab.StoreComponentsInChildrenInCache<TSource, TDestination> (this.CanReplace);
            }
        }

        public void ReplaceProxiesFromCache(GameObject prefab)
        {
            if (needsCache)
            {
                prefab.ConvertFromCache<TSource, TDestination>(this.CanReplace);
            }
        }
    }
}
