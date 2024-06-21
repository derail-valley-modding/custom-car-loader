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
using CCL.Types.Proxies;

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
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && t.Namespace.StartsWith(proxyNamespace))
                .ToList();

            var typeMaps = Mapper.M.ConfigurationProvider.GetAllTypeMaps();
            var failures = new List<string>();

            CheckMappingExists(scriptTypes, typeMaps, failures);

            if (failures.Count > 0)
            {
                Assert.Fail("\n" + string.Join("\n", failures));
            }
        }

        private static void CheckMappingExists(IEnumerable<Type> scriptTypes, IEnumerable<AutoMapper.TypeMap> typeMaps, List<string> failures, string? parentType = null)
        {
            foreach (var scriptType in scriptTypes)
            {
                if (scriptType.IsAbstract || scriptType.IsEnum || (scriptType.GetCustomAttribute<NotProxiedAttribute>() != null)) continue;

                if (!typeMaps.Any(map => map.SourceType == scriptType))
                {
                    string typeName = scriptType.Name;
                    if (parentType != null) typeName = $"{parentType}.{typeName}";
                    failures.Add($"No mappings configured for proxy script {typeName}");
                }
                else
                {
                    var subTypes = scriptType.GetNestedTypes();
                    if (subTypes.Any())
                    {
                        string prefix = scriptType.Name;
                        if (parentType != null) prefix = $"{parentType}.{prefix}";
                        CheckMappingExists(subTypes, typeMaps, failures, prefix);
                    }
                }
            }
        }
    }
}
