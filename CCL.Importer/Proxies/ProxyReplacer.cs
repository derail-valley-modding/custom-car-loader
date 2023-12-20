using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    public abstract class ProxyReplacer<TSource, TDestination> : Profile, IProxyReplacer
        where TSource : MonoBehaviour
        where TDestination : MonoBehaviour
    {
        public ProxyReplacer() : base() {
            Customize(CreateMap<TSource, TDestination>());
        }

        /// <summary>
        /// Replace this in a concrete implementation if you need to customize the behavior of the mapper
        /// </summary>
        /// <param name="mappingExpression"></param>
        protected virtual void Customize(IMappingExpression<TSource, TDestination> mappingExpression)
        {
            return;
        }

        public void ReplaceProxies(GameObject prefab)
        {
            prefab.MapComponents<TSource, TDestination>();
        }
    }
}
