using AutoMapper;
using CCL.Types;
using System;
using System.Linq.Expressions;
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

        public static IMappingExpression<TSource, TDestination> WithCachedMember<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> cfg, Expression<Func<TDestination, TMember>> member
        )
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
            where TMember : MonoBehaviour
        {
            return cfg.ForMember(member, opt => opt.MapFrom(s => Mapper.GetFromCache(s)));
        }

        public static IMappingExpression<TSource, TDestination> ReplaceGOs<TSource, TDestination>(this IMappingExpression<TSource, TDestination> cfg)
            where TSource : ICanReplaceInstanced
        {
            return cfg.BeforeMap((proxy, real) => proxy.DoFieldReplacing());
        }
    }
}
