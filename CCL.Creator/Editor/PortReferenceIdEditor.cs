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
    [CustomPropertyDrawer(typeof(PortReferenceIdAttribute))]
    public class PortReferenceIdEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string? currentValue = property.stringValue;
            if (string.IsNullOrEmpty(currentValue))
            {
                currentValue = null;
            }

            var component = property.serializedObject.targetObject;
            
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            IEnumerable<GameObject> sources;

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

            var options = GetPortOptions(sources);
            int selected = options.FindIndex(p => p.ID == currentValue);

            if ((selected < 0) && !string.IsNullOrEmpty(currentValue))
            {
                options.Add(new PortReferenceOption(currentValue));
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

        private static List<PortReferenceOption> GetPortOptions(IEnumerable<GameObject> sources)
        {
            var ids = new List<PortReferenceOption>
            {
                new PortReferenceOption(null, "Not Set")
            };

            foreach (var source in sources)
            {
                ids.AddRange(GetPortsInObject(source));
            }
            return ids;
        }

        private static IEnumerable<PortReferenceOption> GetPortsInObject(GameObject root)
        {
            foreach (var component in root.GetComponentsInChildren<SimComponentDefinitionProxy>())
            {
                foreach (var portDef in component.ExposedPortReferences)
                {
                    yield return new PortReferenceOption(root.name, component.ID, portDef.ID);
                }
            }
        }

        private readonly struct PortReferenceOption
        {
            public readonly string PrefabName;
            public readonly string? ID;

            public PortReferenceOption(string prefabName, string compId, string portId)
            {
                PrefabName = prefabName;
                ID = $"{compId}.{portId}";
            }

            public PortReferenceOption(string? fullId, string prefabName = "Unknown")
            {
                PrefabName = prefabName;
                ID = fullId;
            }

            public string Description => $"{ID} ({PrefabName})";
        }
    }
}