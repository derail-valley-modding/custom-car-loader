using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    /// <summary>
    /// This default implementation does nothing but effectively makes the second pass of the interface optional
    /// </summary>
    public abstract class ProxyReplacerBase : IProxyReplacer
    {
        public abstract void ReplaceProxies(GameObject prefab);

        public virtual void ReplaceProxiesFromCache(GameObject prefab)
        {
            return;
        }
    }
}
