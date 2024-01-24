using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;

namespace CCL.Tests
{
    [TestClass]
    public class PortIdTests
    {
        private static readonly BindingFlags ALL_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [TestMethod]
        public void AllPortIdFieldsAreEnumerated()
        {
            var scriptTypes = Assembly.GetAssembly(typeof(PortIdAttribute)).GetTypes()
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t));

            var failures = new List<string>();

            foreach (var scriptType in scriptTypes)
            {
                if (scriptType.IsAbstract) continue;

                var portIds = scriptType.GetFields(ALL_INSTANCE)
                    .Select(f => new KeyValuePair<string, PortIdAttribute>(f.Name, f.GetCustomAttribute<PortIdAttribute>()))
                    .Where(kvp => kvp.Value != null)
                    .ToList();

                if (portIds.Any())
                {
                    if (!typeof(IHasPortIdFields).IsAssignableFrom(scriptType))
                    {
                        failures.Add($"Script \"{scriptType.Name}\" with port IDs must expose them via {nameof(IHasPortIdFields)}");
                        continue;
                    }

                    var instance = Activator.CreateInstance(scriptType);
                    var exposedFields = ((IHasPortIdFields)instance).ExposedPortIdFields.ToList();

                    foreach (var idField in portIds)
                    {
                        var matchingExposed = exposedFields.Find(f => f.FieldName == idField.Key);

                        if (matchingExposed == null)
                        {
                            failures.Add($"Port ID field {idField.Key} must be exposed via {nameof(IHasPortIdFields)}");
                        }
                        else
                        {
                            if (!SetsAreEqual(idField.Value.typeFilters, matchingExposed.TypeFilters))
                            {
                                failures.Add($"Exposed port ID field {idField.Key} type filter does not match definition");
                            }
                            if (!SetsAreEqual(idField.Value.valueTypeFilters, matchingExposed.ValueFilters))
                            {
                                failures.Add($"Exposed port ID field {idField.Key} value type filter does not match definition");
                            }
                        }
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