using AutoMapper;
using System;
using UnityEngine;

namespace CCL.Importer
{
    internal static class AutoMapperExtensions
    {
        /// <summary>
        /// Calling this on a mapping configuration will make it so the mapping of these components is fully automatic.
        /// </summary>
        /// <typeparam name="TSource">The proxy component.</typeparam>
        /// <typeparam name="TDestination">The real component.</typeparam>
        /// <param name="cfg">The mapping expression.</param>
        /// <returns>Itself</returns>
        public static IMappingExpression<TSource, TDestination> AutoCacheAndMap<TSource, TDestination>(this IMappingExpression<TSource, TDestination> cfg)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            Mapper.AddConfig<TSource, TDestination>();
            return cfg;
        }

        /// <summary>
        /// Calling this on a mapping configuration will make it so the mapping of these components is fully automatic.
        /// Mapping will only happen for proxies that meet the criteria in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TSource">The proxy component.</typeparam>
        /// <typeparam name="TDestination">The real component.</typeparam>
        /// <param name="cfg">The mapping expression.</param>
        /// <param name="predicate">The conditions that have to be met for a proxy to be replaced.</param>
        /// <returns>Itself</returns>
        public static IMappingExpression<TSource, TDestination> AutoCacheAndMap<TSource, TDestination>(this IMappingExpression<TSource, TDestination> cfg,
            Predicate<TSource> predicate)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            Mapper.AddConfig<TSource, TDestination>(predicate);
            return cfg;
        }
    }
}
