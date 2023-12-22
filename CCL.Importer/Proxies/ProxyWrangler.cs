using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    internal class ProxyWrangler
    {
        [ImportMany]
        public IEnumerable<IProxyReplacer> proxyMappers;

        private readonly CompositionContainer _container;
        public static ProxyWrangler Instance = new();
        private ProxyWrangler() {
            try
            {
                // An aggregate catalog that combines multiple catalogs.
                var catalog = new AggregateCatalog();
                // Adds all the parts found in the same assembly as the Program class.
                catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

                // Create the CompositionContainer with the parts in the catalog.
                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                CCLPlugin.Error("Error loading proxy mappers " + compositionException.Message);
            }
        }
        public void MapProxiesOnPrefab(GameObject prefab)
        {
            foreach (var mapper in proxyMappers)
            {
                mapper.ReplaceProxies(prefab);
            }
            foreach (var mapper in proxyMappers)
            {
                mapper.ReplaceProxiesFromCache(prefab);
            }
        }
    }
}
