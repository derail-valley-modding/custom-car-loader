using UnityEngine;

namespace CCL.Importer.Proxies
{
    public interface IProxyReplacer
    {
        /// <summary>
        /// Simple replae step - called first.  It is possible that the replacement cache is not fully built by the time this gets called.
        /// </summary>
        /// <param name="prefab">The prefab to look for replacements on</param>
        /// <remarks>This function should *always* at least create the destination component and register it to the cache.  The only exception is if 
        /// an alternative implementation will be doing so.  By the time all instances of this function have been called, the cache is assumed complete.</remarks>
        void ReplaceProxies(GameObject prefab);

        /// <summary>
        /// Called in a second pass.  Replacement cache has been fully built by this point and therefore cache can be used to complete mapping.
        /// </summary>
        /// <param name="prefab">The prefab to look for remaining replacements on</param>
        void ReplaceProxiesFromCache(GameObject prefab);
    }
}