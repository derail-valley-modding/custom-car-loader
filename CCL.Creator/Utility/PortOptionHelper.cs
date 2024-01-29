using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Linq;
using CCL.Types.Proxies.Ports;
using static System.Net.WebRequestMethods;

namespace CCL.Creator.Utility
{
    internal static class PortOptionHelper
    {
        public static IEnumerable<GameObject> GetAvailableSources(Component location, bool local)
        {
            if (local)
            {
                // if local, only look at ports on the current prefab
                return new GameObject[] { location.transform.root.gameObject };
            }
            else
            {
                // otherwise we want to list ports on all parts of the car
                string? prefabAssetPath = GetCurrentPrefabPath(location);
                if (prefabAssetPath != null)
                {
                    return GetSiblingPrefabs(prefabAssetPath);
                }
                else
                {
                    var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                    return scene.GetRootGameObjects();
                }
            }
        }

        private static string? GetCurrentPrefabPath(Object contextObj)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(contextObj))
            {
                return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(contextObj);
            }

            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage)
            {
                return stage.prefabAssetPath;
            }

            return null;
        }

        private static IEnumerable<GameObject> GetSiblingPrefabs(string prefabPath)
        {
            string curFolder = Path.GetDirectoryName(prefabPath);

            return AssetDatabase.FindAssets("t:prefab", new string[] { curFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => p != null)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>);
        }

        public static IEnumerable<PortIdField> GetPortIdFields(IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var hasFields in source.GetComponentsInChildren<IHasPortIdFields>())
                {
                    foreach (var field in hasFields.ExposedPortIdFields)
                    {
                        yield return field;
                    }
                }
            }
        }

        public static IEnumerable<FuseIdField> GetFuseIdFields(IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var hasFields in source.GetComponentsInChildren<IHasFuseIdFields>())
                {
                    foreach (var field in hasFields.ExposedFuseIdFields)
                    {
                        yield return field;
                    }
                }
            }
        }


        public static IEnumerable<PortOptionBase> GetPortOptions(PortIdAttribute filter, IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var port in GetPortsInObject(source))
                {
                    if (port.MatchesType(filter.typeFilters) && port.MatchesValueType(filter.valueTypeFilters))
                    {
                        yield return port;
                    }
                }
            }
        }

        public static IEnumerable<PortOptionBase> GetFuseOptions(IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var fuse in GetFusesInObject(source))
                {
                    yield return fuse;
                }
            }
        }

        public static IEnumerable<PortOptionBase> GetPortOptions(IEnumerable<GameObject> sources, DVPortType[]? typeFilters, DVPortValueType[]? valueFilters)
        {
            foreach (var source in sources)
            {
                foreach (var port in GetPortsInObject(source))
                {
                    if (port.MatchesType(typeFilters) && port.MatchesValueType(valueFilters))
                    {
                        yield return port;
                    }
                }
            }
        }

        public static IEnumerable<PortOptionBase> GetPortReferenceOptions(IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var reference in GetPortReferencesInObject(source))
                {
                    yield return reference;
                }
            }
        }

        private static IEnumerable<PortOption> GetPortsInObject(GameObject root)
        {
            foreach (var component in root.GetComponentsInChildren<SimComponentDefinitionProxy>())
            {
                foreach (var portDef in component.ExposedPorts)
                {
                    yield return new PortOption(root.name, component.ID, portDef.ID, portDef.type, portDef.valueType);
                }
            }
        }

        private static IEnumerable<PortReferenceOption> GetPortReferencesInObject(GameObject root)
        {
            foreach (var component in root.GetComponentsInChildren<SimComponentDefinitionProxy>())
            {
                foreach (var portDef in component.ExposedPortReferences)
                {
                    yield return new PortReferenceOption(root.name, component.ID, portDef.ID, portDef.valueType);
                }
            }
        }

        private static IEnumerable<FuseOption> GetFusesInObject(GameObject root)
        {
            foreach (var component in root.GetComponentsInChildren<SimComponentDefinitionProxy>())
            {
                foreach (var fuseDef in component.ExposedFuses)
                {
                    yield return new FuseOption(root.name, component.ID, fuseDef.id);
                }
            }
        }


        public static IEnumerable<PortOptionBase> GetInputPortConnectionOptions(DVPortValueType valueType, IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var port in GetPortsInObject(source))
                {
                    if (port.PortType == DVPortType.OUT && port.PortValueType == valueType)
                    {
                        yield return port;
                    }
                }

                foreach (var hasFields in source.GetComponentsInChildren<IHasPortIdFields>())
                {
                    foreach (var field in hasFields.ExposedPortIdFields)
                    {
                        if (field.TypeFilters == null || field.TypeFilters.Contains(DVPortType.IN))
                        {
                            if (field.ValueFilters == null || field.ValueFilters.Contains(valueType))
                            {
                                yield return new IdFieldOption(source.name, field, valueType);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<PortOptionBase> GetOutputPortConnectionOptions(DVPortType portType, DVPortValueType valueType, IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var port in GetPortsInObject(source))
                {
                    if (port.PortType == DVPortType.IN && port.PortValueType == valueType)
                    {
                        yield return port;
                    }
                }

                foreach (var reference in GetPortReferencesInObject(source))
                {
                    if (reference.PortValueType == valueType)
                    {
                        yield return reference;
                    }
                }

                foreach (var hasFields in source.GetComponentsInChildren<IHasPortIdFields>())
                {
                    foreach (var field in hasFields.ExposedPortIdFields)
                    {
                        if (field.CanConnect(portType) && field.CanConnect(valueType))
                        {
                            yield return new IdFieldOption(source.name, field, valueType);
                        }
                    }
                }
            }
        }

        public static IEnumerable<PortOptionBase> GetPortReferenceInputOptions(DVPortValueType valueType, IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var port in GetPortsInObject(source))
                {
                    if (valueType == DVPortValueType.GENERIC || port.PortValueType == valueType || port.PortValueType == DVPortValueType.GENERIC)
                    {
                        yield return port;
                    }
                }
            }
        }

        public static IEnumerable<PortOptionBase> GetFuseConnectionOptions(IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var hasFuseIds in source.GetComponentsInChildren<IHasFuseIdFields>())
                {
                    foreach (var fuseIdField in hasFuseIds.ExposedFuseIdFields)
                    {
                        yield return new IdFieldOption(source.name, fuseIdField, DVPortValueType.GENERIC);
                    }
                }
            }
        }
    }
}
