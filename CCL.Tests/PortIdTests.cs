using Microsoft.VisualStudio.TestTools.UnitTesting;

using CCL.Types.Components.Controllers;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Ports;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Tests
{
    [TestClass]
    public class PortIdTests
    {
        private static readonly BindingFlags ALL_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static IEnumerable<object[]> ScriptsWithPorts
        {
            get
            {
                var scriptTypes = Assembly.GetAssembly(typeof(PortIdAttribute)).GetTypes()
                    .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t));

                foreach (var scriptType in scriptTypes)
                {
                    if (scriptType.IsAbstract || (scriptType.GetCustomAttribute<NotExposedAttribute>() != null)) continue;

                    var portIds = scriptType.GetFields(ALL_INSTANCE)
                        .Select(f => new KeyValuePair<string, PortIdAttribute>(f.Name, f.GetCustomAttribute<PortIdAttribute>()))
                        .Where(kvp => kvp.Value != null)
                        .ToList();

                    if (portIds.Any())
                    {
                        yield return new object[] { scriptType, portIds };
                    }
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(ScriptsWithPorts))]
        public void AllPortIdFieldsAreEnumerated(Type scriptType, IEnumerable<KeyValuePair<string, PortIdAttribute>> portIds)
        {
            var failures = new List<string>();

            if (!typeof(IHasPortIdFields).IsAssignableFrom(scriptType))
            {
                failures.Add($"Script \"{scriptType.Name}\" with port IDs must expose them via {nameof(IHasPortIdFields)}");
            }
            else
            {
                // Ignore this script type as it uses dynamic ports.
                if (scriptType == typeof(ResourceSharerController)) return;

                var instance = Activator.CreateInstance(scriptType);
                var exposedFields = ((IHasPortIdFields)instance).ExposedPortIdFields.ToList();

                foreach (var idField in portIds)
                {
                    var matchingExposed = exposedFields.Find(f => f.FieldName == idField.Key);

                    if (matchingExposed == null)
                    {
                        failures.Add($"Port ID field {scriptType.Name}.{idField.Key} must be exposed via {nameof(IHasPortIdFields)}");
                    }
                    else
                    {
                        if (!SetsAreEqual(idField.Value.typeFilters, matchingExposed.TypeFilters))
                        {
                            failures.Add($"Exposed port ID field {scriptType.Name}.{idField.Key} type filter does not match definition");
                        }
                        if (!SetsAreEqual(idField.Value.valueTypeFilters, matchingExposed.ValueFilters))
                        {
                            failures.Add($"Exposed port ID field {scriptType.Name}.{idField.Key} value type filter does not match definition");
                        }
                    }
                }
            }

            if (failures.Count > 0)
            {
                Assert.Fail("\n" + string.Join("\n", failures));
            }
        }

        public static IEnumerable<object[]> ScriptsWithFuses
        {
            get
            {
                var scriptTypes = Assembly.GetAssembly(typeof(FuseIdAttribute)).GetTypes()
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t));


                foreach (var scriptType in scriptTypes)
                {
                    if (scriptType.IsAbstract) continue;

                    var fuseIds = scriptType.GetFields(ALL_INSTANCE)
                        .Select(f => new KeyValuePair<string, FuseIdAttribute>(f.Name, f.GetCustomAttribute<FuseIdAttribute>()))
                        .Where(kvp => kvp.Value != null)
                        .ToList();

                    if (!fuseIds.Any()) continue;

                    yield return new object[] { scriptType, fuseIds };
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(ScriptsWithFuses))]
        public void AllFuseIdFieldsAreEnumerated(Type scriptType, IEnumerable<KeyValuePair<string, FuseIdAttribute>> fuseIds)
        {
            var failures = new List<string>();

            if (!typeof(IHasFuseIdFields).IsAssignableFrom(scriptType))
            {
                failures.Add($"Script \"{scriptType.Name}\" with fuse IDs must expose them via {nameof(IHasFuseIdFields)}");
            }
            else
            {
                var instance = Activator.CreateInstance(scriptType);
                var exposedFields = ((IHasFuseIdFields)instance).ExposedFuseIdFields.ToList();

                foreach (var idField in fuseIds)
                {
                    var matchingExposed = exposedFields.Find(f => f.FieldName == idField.Key);

                    if (matchingExposed == null)
                    {
                        failures.Add($"Fuse ID field {scriptType.Name}.{idField.Key} must be exposed via {nameof(IHasFuseIdFields)}");
                    }
                }
            }

            if (failures.Count > 0)
            {
                Assert.Fail("\n" + string.Join("\n", failures));
            }
        }

        private static bool SetsAreEqual<T>(IEnumerable<T>? a, IEnumerable<T>? b)
        {
            if ((a == null) || (b == null))
            {
                // true if both null, false if only one null
                return !((a == null) ^ (b == null));
            }

            return new HashSet<T>(a).SetEquals(b);
        }
    }
}