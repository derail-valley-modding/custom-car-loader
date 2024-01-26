using CCL.Types;
using System;
using System.IO;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CCL.Creator
{
    public static class EditorHelpers
    {
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

        public static T ObjectField<T>(T obj, bool allowSceneObjects, params UnityEngine.GUILayoutOption[] options) where T : UnityEngine.Object
        {
            return (T)UnityEditor.EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options);
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
                var scene = EditorSceneManager.GetActiveScene();
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

        public GUIColorScope()
        {
            _entryColor = GUI.color;
        }

        public GUIColorScope(Color newColor) : this()
        {
            GUI.color = newColor;
        }

        public void Dispose()
        {
            GUI.color = _entryColor;
        }
    }
}