using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Editor
{
    [CustomPropertyDrawer(typeof(PortIdAttribute))]
    public class PortIdEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var portData = (PortIdAttribute)attribute;
            
            string? currentValue = property.stringValue;
            if (string.IsNullOrEmpty(currentValue))
            {
                currentValue = null;
            }

            var component = property.serializedObject.targetObject;
            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            IEnumerable<GameObject> sources;

            if (portData.local)
            {
                // if local, only look at ports on the current prefab
                sources = new GameObject[] { ((Component)component).transform.root.gameObject };
            }
            else
            {
                // otherwise we want to list ports on all parts of the car
                string? prefabAssetPath = GetCurrentPrefabPath(component);
                if (prefabAssetPath != null)
                {
                    sources = GetSiblingPrefabs(prefabAssetPath);
                }
                else
                {
                    var scene = EditorSceneManager.GetActiveScene();
                    sources = scene.GetRootGameObjects();
                }
            }

            var options = GetPortOptions(portData, sources);
            int selected = options.FindIndex(p => p.ID == currentValue);

            if ((selected < 0) && !string.IsNullOrEmpty(currentValue))
            {
                options.Add(new PortOption(currentValue));
                selected = options.Count - 1;
            }

            string[] optionNames = options.Select(p => p.Description).ToArray();

            int newIndex = EditorGUI.Popup(position, Math.Max(selected, 0), optionNames);

            if (newIndex != selected)
            {
                property.stringValue = options[newIndex].ID;
            }

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        private static string? GetCurrentPrefabPath(UnityEngine.Object contextObj)
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

        private static List<PortOption> GetPortOptions(PortIdAttribute filter, IEnumerable<GameObject> sources)
        {
            var ids = new List<PortOption>
            {
                new PortOption(null, "Not Set")
            };

            foreach (var source in sources)
            {
                foreach (PortOption port in GetPortsInObject(source))
                {
                    if (port.MatchesType(filter.typeFilters) && port.MatchesValueType(filter.valueTypeFilters))
                    {
                        ids.Add(port);
                    }
                }
            }
            return ids;
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

        private readonly struct PortOption
        {
            public readonly string PrefabName;
            public readonly string? ID;
            public readonly DVPortType PortType;
            public readonly DVPortValueType PortValueType;

            public PortOption(string prefabName, string compId, string portId, DVPortType portType, DVPortValueType valueType)
            {
                PrefabName = prefabName;
                ID = $"{compId}.{portId}";
                PortType = portType;
                PortValueType = valueType;
            }

            public PortOption(string? fullId, string prefabName = "Unknown")
            {
                PrefabName = prefabName;
                ID = fullId;
                PortType = DVPortType.IN;
                PortValueType = DVPortValueType.GENERIC;
            }

            public bool MatchesType(DVPortType[]? filters)
            {
                return (filters == null) || (filters.Length == 0) || filters.Contains(PortType);
            }

            public bool MatchesValueType(DVPortValueType[]? filters)
            {
                return (filters == null) || (filters.Length == 0) || filters.Contains(PortValueType);
            }

            public string Description => $"{ID} ({PrefabName})";
        }
    }
}