using CCL.Types.Proxies.Ports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CCL.Importer;

namespace CCL.Tests
{
    [TestClass]
    public class ProxyMappingTests
    {
        [TestMethod]
        public void AllProxyComponentsHaveMappings()
        {
            const string proxyNamespace = "CCL.Types.Proxies";
            var scriptTypes = Assembly.GetAssembly(typeof(SimComponentDefinitionProxy)).GetTypes()
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && t.Namespace.StartsWith(proxyNamespace));

            var typeMaps = Mapper.M.ConfigurationProvider.GetAllTypeMaps();
            var failures = new List<string>();

            foreach (var scriptType in scriptTypes)
            {
                if (scriptType.IsAbstract) continue;

                if (!typeMaps.Any(map => map.SourceType == scriptType))
                {
                    failures.Add($"No mappings configured for proxy script {scriptType.Name}");
                }
            }

            if (failures.Count > 0)
            {
                Assert.Fail(string.Join("\n", failures));
            }
        }
    }
}
