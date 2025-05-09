using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCL.Importer;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;

namespace CCL.Tests
{
    [TestClass]
    public class ProxyMappingTests
    {
        public static IEnumerable<object[]> ScriptTypes
        {
            get
            {
                const string proxyNamespace = "CCL.Types.Proxies";

                var scriptTypes = Assembly.GetAssembly(typeof(SimComponentDefinitionProxy)).GetTypes()
                    .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && t.Namespace.StartsWith(proxyNamespace));

                var typeMaps = Mapper.M.ConfigurationProvider.GetAllTypeMaps();

                return scriptTypes.Select(t => new object[] { t, typeMaps });
            }
        }

        public static string GetTestName(MethodInfo _, object[] testArgs)
        {
            var scriptType = (Type)testArgs[0];
            return $"{scriptType.Name} has mapping";
        }

        [TestMethod]
        [DynamicData(nameof(ScriptTypes), DynamicDataDisplayName = nameof(GetTestName))]
        public void AllProxyComponentsHaveMappings(Type scriptType, AutoMapper.TypeMap[] typeMaps)
        {
            //const string proxyNamespace = "CCL.Types.Proxies";
            //var scriptTypes = Assembly.GetAssembly(typeof(SimComponentDefinitionProxy)).GetTypes()
            //    .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) && t.Namespace.StartsWith(proxyNamespace))
            //    .ToList();

            //var typeMaps = Mapper.M.ConfigurationProvider.GetAllTypeMaps();
            var failures = new List<string>();

            CheckMappingExists(new[] { scriptType }, typeMaps, failures);

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
