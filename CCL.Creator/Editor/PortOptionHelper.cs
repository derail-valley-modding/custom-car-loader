using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using System.Linq;
using CCL.Types.Proxies.Ports;
using static System.Net.WebRequestMethods;

namespace CCL.Creator.Editor
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
                    var scene = EditorSceneManager.GetActiveScene();
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
            }
        }

        public static IEnumerable<PortOptionBase> GetOutputPortConnectionOptions(DVPortValueType valueType, IEnumerable<GameObject> sources)
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
            }
        }

        public static IEnumerable<PortOptionBase> GetPortReferenceInputOptions(DVPortValueType valueType, IEnumerable<GameObject> sources)
        {
            foreach (var source in sources)
            {
                foreach (var port in GetPortsInObject(source))
                {
                    if (port.PortValueType == valueType)
                    {
                        yield return port;
                    }
                }
            }
        }
    }
}
