using CCL.Types;
using System;
using System.IO;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CCL.Creator.Utility
{
    public static class EditorHelpers
    {
        public static class Colors
        {
            public static readonly Color DEFAULT = Color.white;
            public static readonly Color WARNING = new Color32(255, 230, 128, 255);
            public static readonly Color DELETE_ACTION = new Color32(255, 153, 153, 255);
        }

        /// <summary>
        /// Clears out an entire folder. BE CAREFUL WHEN USING.
        /// </summary>
        /// <param name="directory"></param>
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            //foreach(System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        /// <summary>
        /// Returns a full path with filename without an extension.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        private static string SearchDisksForPath(string subPath)
        {
            // search for the user's DV install
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    string driveRoot = drive.RootDirectory.FullName;
                    string potentialPath = Path.Combine(driveRoot, "Program Files", subPath);
                    if (Directory.Exists(potentialPath))
                    {
                        return potentialPath;
                    }

                    potentialPath = Path.Combine(driveRoot, "Program Files (x86)", subPath);
                    if (Directory.Exists(potentialPath))
                    {
                        return potentialPath;
                    }
                }
                catch (Exception) { }
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static string GetDefaultSavePath(string? subDir = null)
        {
            string steamModPath = "Steam/steamapps/common/Derail Valley/Mods/DVCustomCarLoader";
            if (!string.IsNullOrEmpty(subDir))
            {
                steamModPath = Path.Combine(steamModPath, subDir);
            }

            return SearchDisksForPath(steamModPath);
        }

        public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options);
        }

        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
        }

        public static T EnumPopup<T>(string label, T selected, params GUILayoutOption[] options)
            where T : Enum
        {
            return (T)EditorGUILayout.EnumPopup(label, selected, options);
        }

        public static string? GetSelectionPath()
        {
            if (Selection.activeGameObject)
            {
                return Selection.activeGameObject.GetPath();
            }
            return null;
        }

        public static void SaveAndRefresh(Action? extraAction = null)
        {
            AssetDatabase.SaveAssets();
            string? selectionPath = GetSelectionPath();
            EditorApplication.delayCall += () => DelayedRefresh(selectionPath, extraAction);
        }

        private static void DelayedRefresh(string? selectionPath, Action? extraAction)
        {
            GameObject? root = null;
            var parts = selectionPath?.Split(new[] { '/' }, 2);

            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage)
            {
                root = stage.prefabContentsRoot;
            }
            else
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                var roots = scene.GetRootGameObjects();

                if (parts != null)
                {
                    root = roots.FirstOrDefault(r => r.name == parts[0]);
                }
            }

            if (parts != null && root)
            {
                if (parts.Length == 1)
                {
                    Selection.activeGameObject = root;
                }
                else
                {
                    Transform? target = root!.transform.FindSafe(parts[1]);
                    if (target)
                    {
                        Selection.activeGameObject = target!.gameObject;
                    }
                }
            }

            extraAction?.Invoke();
        }
    }

    internal class GUIColorScope : IDisposable
    {
        private readonly Color _entryColor;
        private readonly Color _entryBackground;
        private readonly Color _entryContent;

        public GUIColorScope(Color? newColor = null, Color? newBackground = null, Color? newContent = null)
        {
            _entryColor = GUI.color;
            _entryBackground = GUI.backgroundColor;
            _entryContent = GUI.contentColor;

            if (newColor.HasValue) GUI.color = newColor.Value;
            if (newBackground.HasValue) GUI.backgroundColor = newBackground.Value;
            if (newContent.HasValue) GUI.contentColor = newContent.Value;
        }

        public void Dispose()
        {
            GUI.color = _entryColor;
            GUI.backgroundColor = _entryBackground;
            GUI.contentColor = _entryContent;
        }
    }
}