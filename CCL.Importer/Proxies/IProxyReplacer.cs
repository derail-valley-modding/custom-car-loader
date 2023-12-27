using UnityEngine;

namespace CCL.Importer.Proxies
{
    public interface IProxyReplacer
    {
        /// <summary>
        /// Simple replae step - called first.  It is possible that the replacement cache is not fully built by the time this gets called, so it should realistically
        /// only be used to add to the cache and do the replace
        /// <see cref="Mapper.StoreComponentsInChildrenInCache{TSource, TDestination}(GameObject, System.Func{TSource, bool})"/>
        /// </summary>
        /// <param name="prefab">The prefab to look for replacements on</param>
        /// <remarks>This function should *always* at least create the destination component and register it to the cache.  The only exception is if 
        /// an alternative implementation will be doing so.  By the time all instances of this function have been called, the cache is assumed complete.</remarks>
        void CacheAndReplaceProxies(GameObject prefab);

        /// <summary>
        /// Replace proxies that shouldn't be interacting with the monobehaviour cache.  This supports highly custom replacers such as swapping out a proxy for an entire prefab or vanilla resource
        /// </summary>
        /// <param name="prefab"></param>
        void ReplaceProxiesUncached(GameObject prefab);

        /// <summary>
        /// Called in a second pass.  Replacement cache has been fully built by this point and therefore cache can be used to complete the mapping process.
        /// </summary>
        /// <param name="prefab">The prefab to look for remaining replacements on</param>
        void MapProxies(GameObject prefab);
    }
}