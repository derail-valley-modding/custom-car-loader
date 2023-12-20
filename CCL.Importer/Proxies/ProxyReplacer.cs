using AutoMapper;
using System;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    public abstract class ProxyReplacer<TSource, TDestination> : Profile, IProxyReplacer
        where TSource : MonoBehaviour
        where TDestination : MonoBehaviour
    {
        public ProxyReplacer() : base() {
            Customize(CreateMap<TSource, TDestination>());
            AddAdditionalMappings();
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
        /// <param name="mappingExpression"></param>
        protected virtual void Customize(IMappingExpression<TSource, TDestination> mappingExpression)
        {
            return;
        }

        protected virtual bool CanReplace(TSource sourceComponent)
        {
            return true;
        }

        public void ReplaceProxies(GameObject prefab)
        {
            prefab.MapComponentsInChildren<TSource, TDestination>(this.CanReplace);
        }
    }
}
