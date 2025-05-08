using CCL.Importer;
using CCL.Types.Proxies.Ports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using PortDef = LocoSim.Definitions.PortDefinition;
using PortRefDef = LocoSim.Definitions.PortReferenceDefinition;
using FuseDef = LocoSim.Definitions.FuseDefinition;

namespace CCL.Tests
{
    internal static class PortTestExtensions
    {
        internal static IEnumerable<T> NotNull<T>(this IEnumerable<T> source) => source.Where(x => x is not null);
    }

    [TestClass]
    public class PortMatchingTests
    {
        private static readonly BindingFlags ALL_INSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type SimCompDefType = typeof(SimComponentDefinitionProxy);
        private static readonly Type PortType = typeof(PortDef);
        private static readonly Type PortRefType = typeof(PortRefDef);
        private static readonly Type FuseType = typeof(FuseDef);

        private static readonly Type[] RequireManualCheckTypes = new[]
        {
            typeof(ConfigurablePortDefinitionProxy),
            typeof(MultiplePortDecoderEncoderDefinitionProxy),
            typeof(Types.Proxies.Resources.ResourceContainerProxy),
        };

        public static IEnumerable<object[]> SimComponentTypeMaps
        {
            get
            {
                var scriptTypes = Mapper.M.ConfigurationProvider.GetAllTypeMaps();

                foreach (var map in scriptTypes)
                {
                    if (map.SourceType.IsAbstract || !map.SourceType.IsSubclassOf(SimCompDefType)) continue;
                    yield return new object[] { map };
                }
            }
        }

        public static string GetTestName(MethodInfo _, object[] testArgs)
        {
            var map = (AutoMapper.TypeMap)testArgs[0];
            return $"Sim Ports Match: {map.SourceType.Name} -> {map.DestinationType.Name}";
        }

        [TestMethod]
        [DynamicData(nameof(SimComponentTypeMaps), DynamicDataDisplayName = nameof(GetTestName))]
        public void CheckIfSimPortsMatch(AutoMapper.TypeMap map)
        {
            var fail = new List<string>();

            if (RequireManualCheckTypes.Contains(map.SourceType))
            {
                Assert.Inconclusive($"\n{map.SourceType.Name} requires manual checking of ports");
                return;
            }

            // Proxy.
            // Skip test in case the proxy component could not be instanced.
            SimComponentDefinitionProxy proxy;
            try
            {
                proxy = (SimComponentDefinitionProxy)Activator.CreateInstance(map.SourceType);
            }
            catch (TargetInvocationException)
            {
                Assert.Inconclusive($"\nCould not create an instance of {map.SourceType.Name} to compare ports");
                return;
            }

            // Check for correct port exposition.
            var proxyPorts = proxy.ExposedPorts;
            var proxyPortRefs = proxy.ExposedPortReferences;
            var proxyFuses = proxy.ExposedFuses;

            if (proxyPorts == null)
            {
                Assert.Fail($"\n{map.SourceType.Name} failed to expose ports correctly");
            }

            if (proxyPortRefs == null)
            {
                Assert.Fail($"\n{map.SourceType.Name} failed to expose port references correctly");
            }

            if (proxyFuses == null)
            {
                Assert.Fail($"\n{map.SourceType.Name} failed to expose fuses correctly");
            }

            // Real.
            // Skip test in case the real component could not be instanced.
            object real;
            try
            {
                real = Activator.CreateInstance(map.DestinationType);
            }
            catch (TargetInvocationException)
            {
                Assert.Inconclusive($"\nCould not create an instance of {map.DestinationType.Name} to compare ports");
                return;
            }

            var realPorts = map.DestinationType.GetFields(ALL_INSTANCE)
                .Where(x => x.FieldType == PortType)
                .Select(x => (PortDef)x.GetValue(real))
                .NotNull()
                .ToList();

            PortCheck(fail, map, proxyPorts, realPorts);

            var realPortRefs = map.DestinationType.GetFields(ALL_INSTANCE)
                .Where(x => x.FieldType == PortRefType)
                .Select(x => (PortRefDef)x.GetValue(real))
                .NotNull()
                .ToList();

            PortReferenceCheck(fail, map, proxyPortRefs, realPortRefs);

            var realFuses = map.DestinationType.GetFields(ALL_INSTANCE)
                .Where(x => x.FieldType == FuseType)
                .Select(x => (FuseDef)x.GetValue(real))
                .NotNull()
                .ToList();

            FuseCheck(fail, map, proxyFuses, realFuses);

            if (fail.Count > 0)
            {
                Assert.Fail("\n" + string.Join("\n", fail));
            }
        }

        private static void PortCheck(List<string> fail, AutoMapper.TypeMap map,
            IEnumerable<PortDefinition> proxyPorts, IEnumerable<PortDef> realPorts)
        {
            foreach (var port in realPorts)
            {
                var matched = proxyPorts.FirstOrDefault(x => x.ID == port.ID);

                if (matched == null)
                {
                    fail.Add($"{map.SourceType.Name} is missing port [{port.ID}]");
                }
                else
                {
                    if ((int)matched.type != (int)port.type)
                    {
                        fail.Add($"{map.SourceType.Name} port type mismatch [{port.type}/{matched.type}]");
                    }
                    if ((int)matched.valueType != (int)port.valueType)
                    {
                        fail.Add($"{map.SourceType.Name} port value type mismatch [{port.valueType}/{matched.valueType}]");
                    }
                }
            }

            foreach (var port in proxyPorts)
            {
                var matched = realPorts.FirstOrDefault(x => x != null && x.ID == port.ID);

                if (matched == null)
                {
                    fail.Add($"{map.SourceType.Name} has extra port [{port.ID}]");
                }
            }
        }

        private static void PortReferenceCheck(List<string> fail, AutoMapper.TypeMap map,
            IEnumerable<PortReferenceDefinition> proxyPortRefs, IEnumerable<PortRefDef> realPortRefs)
        {
            foreach (var port in realPortRefs)
            {
                var matched = proxyPortRefs.FirstOrDefault(x => x.ID == port.ID);

                if (matched == null)
                {
                    fail.Add($"{map.SourceType.Name} is missing port reference [{port.ID}]");
                }
                else
                {
                    if ((int)matched.valueType != (int)port.valueType)
                    {
                        fail.Add($"{map.SourceType.Name} port reference value type mismatch [{port.valueType}/{matched.valueType}]");
                    }
                }
            }

            foreach (var port in proxyPortRefs)
            {
                var matched = realPortRefs.FirstOrDefault(x => x != null && x.ID == port.ID);

                if (matched == null)
                {
                    fail.Add($"{map.SourceType.Name} has extra port reference [{port.ID}]");
                }
            }
        }


        private static void FuseCheck(List<string> fail, AutoMapper.TypeMap map,
            IEnumerable<FuseDefinition> proxyFuses, IEnumerable<FuseDef> realFuses)
        {
            foreach (var fuse in realFuses)
            {
                var matched = proxyFuses.FirstOrDefault(x => x.id == fuse.id);

                if (matched == null)
                {
                    fail.Add($"{map.SourceType.Name} is missing port [{fuse.id}]");
                }
            }

            foreach (var fuse in proxyFuses)
            {
                var matched = realFuses.FirstOrDefault(x => x != null && x.id == fuse.id);

                if (matched == null)
                {
                    fail.Add($"{map.SourceType.Name} has extra port [{fuse.id}]");
                }
            }
        }
    }
}
