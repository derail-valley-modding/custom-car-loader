using CCL.Types;
using System;
using System.IO;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using CCL.Types.Proxies;

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

        public static T ObjectField<T>(GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
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

        private const string SearchFieldControlName = "SearchFieldText";
        private static Vector2 s_searchScroll = Vector2.zero;

        /// <summary>
        /// Draws a string property, and uses its value to search through a list of options.
        /// </summary>
        /// <param name="property">The string property.</param>
        /// <param name="searchOptions">The available options for picking.</param>
        /// <param name="searchMaxHeight">The maximum height of the search box.</param>
        /// <param name="maxResults">The maximum results to display.</param>
        /// <param name="displayIfEmpty">Display results while the search input is empty.</param>
        /// <remarks>Please ensure the property has a string value.</remarks>
        public static void StringWithSearchField(SerializedProperty property, IEnumerable<string> searchOptions, float searchMaxHeight, int maxResults, bool displayIfEmpty = false)
        {
            GUI.SetNextControlName(SearchFieldControlName);
            EditorGUILayout.PropertyField(property);
            string s = property.stringValue;

            if (GUI.GetNameOfFocusedControl() != SearchFieldControlName || (string.IsNullOrEmpty(s) && !displayIfEmpty))
            {
                return;
            }

            s_searchScroll = EditorGUILayout.BeginScrollView(s_searchScroll, false, true, GUILayout.MaxHeight(searchMaxHeight));

            var results = searchOptions.Where(x => x.StartsWith(s, StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x).Take(maxResults);

            if (results.Count() > 0 && results.First() != property.stringValue)
            {
                foreach (var item in results)
                {
                    if (GUILayout.Button(item, EditorStyles.toggle))
                    {
                        property.stringValue = item;
                        GUI.FocusControl("");
                        break;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws an array property with buttons to reorder its elements.
        /// </summary>
        public static void ReorderableArrayField(SerializedProperty property)
        {
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName);

            if (!property.isExpanded)
            {
                return;
            }

            int length = EditorGUILayout.DelayedIntField("Size", property.arraySize);
            property.arraySize = length;

            var width = GUILayout.Width(EditorGUIUtility.singleLineHeight);
            EditorGUIUtility.labelWidth -= EditorGUIUtility.singleLineHeight * 3;

            for (int i = 0; i < length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = i < length - 1;
                if (GUILayout.Button("\u2193", EditorStyles.textField, width))
                {
                    property.MoveArrayElement(i, i + 1);
                }
                GUI.enabled = i > 0;
                if (GUILayout.Button("\u2191", EditorStyles.textField, width))
                {
                    property.MoveArrayElement(i, i - 1);
                }
                GUI.enabled = true;
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = 0;
        }

        /// <summary>
        /// Draws buttons to apply default values to a class that implements a loco defaults interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        public static void DrawLocoDefaultsButtons<T>(T component)
        {
            bool first = true;

            void drawHeader()
            {
                if (first)
                {
                    DrawHeader("Defaults");
                    first = false;
                }
            }

            if (component is IDE2Defaults de2)
            {
                drawHeader();

                if (GUILayout.Button("Apply DE2 Defaults"))
                {
                    de2.ApplyDE2Defaults();
                }
            }

            if (component is IDE6Defaults de6)
            {
                drawHeader();

                if (GUILayout.Button("Apply DE6 Defaults"))
                {
                    de6.ApplyDE6Defaults();
                }
            }

            if (component is IDH4Defaults dh4)
            {
                drawHeader();

                if (GUILayout.Button("Apply DH4 Defaults"))
                {
                    dh4.ApplyDH4Defaults();
                }
            }

            if (component is IDM3Defaults dm3)
            {
                drawHeader();

                if (GUILayout.Button("Apply DM3 Defaults"))
                {
                    dm3.ApplyDM3Defaults();
                }
            }

            if (component is IBE2Defaults be2)
            {
                drawHeader();

                if (GUILayout.Button("Apply BE2 Defaults"))
                {
                    be2.ApplyBE2Defaults();
                }
            }
        }

        public static void DrawHeader(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
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