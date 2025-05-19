using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Utility
{
    internal class AssetHelper
    {
        public static GameObject[] GetPrefabsAtPath(string path)
        {
            var guids = AssetDatabase.FindAssets("t:GameObject", new[] { path });

            // ???
            return guids
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => AssetDatabase.LoadAssetAtPath<GameObject>(p)).ToArray();
        }

        public static T GetSelectedAsset<T>()
            where T : Object
        {
            var selected = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(selected);

            if (path == null || !(selected is T actual))
            {
                return null!;
            }

            return actual;
        }

        public static void SaveAsset(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        public static string GetFolder(Object asset) => Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset)).Replace('\\', '/');

        public static T CreateScriptableAsset<T>(string path, string name)
            where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.name = name;

            path = $"{Path.Combine(path, name)}.asset";

            AssetDatabase.CreateAsset(instance, path);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
