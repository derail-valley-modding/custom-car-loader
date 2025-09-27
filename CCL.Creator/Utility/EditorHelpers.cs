using CCL.Types;
using System;
using System.IO;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
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
            public static readonly Color CONFIRM_ACTION = new Color(0.50f, 1.80f, 0.75f);
            public static readonly Color WARNING = new Color(1.75f, 1.40f, 0.25f);
            public static readonly Color DELETE_ACTION = new Color(2.00f, 0.75f, 0.75f);
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

        public static T ObjectField<T>(T? obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options);
        }

        public static T ObjectField<T>(GUIContent label, T? obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
        }

        public static T ObjectField<T>(string label, T? obj, bool allowSceneObjects, params GUILayoutOption[] options)
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
        public static void StringWithSearchField(SerializedProperty property, IEnumerable<string> searchOptions, float searchMaxHeight, int maxResults,
            bool displayIfEmpty = false)
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
                        GUI.FocusControl(string.Empty);
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

            foreach (var item in VehicleDefaultsHelper.GetActionsForType(component))
            {
                if (first)
                {
                    DrawHeader("Defaults");
                    first = false;
                }

                if (GUILayout.Button(item.ActionName))
                {
                    item.Action();

                    if (component is UnityEngine.Object obj)
                    {
                        AssetHelper.SaveAsset(obj);
                    }
                }
            }
        }

        public static void DrawHeader(string title)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        public static void DrawHeader(string title, string tooltip)
        {
            DrawHeader(new GUIContent(title, tooltip));
        }

        public static void DrawHeader(GUIContent content)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
        }

        public static ReorderableList CreateReorderableList(SerializedObject obj, SerializedProperty elements, bool draggable, bool displayHeader,
            bool displayButtons, string header)
        {
            return new ReorderableList(obj, elements, draggable, displayHeader, displayButtons, displayButtons)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, header);
                }
            };
        }

        public static bool DrawStringArray(string label, ref string[] array, bool expanded)
        {
            return DrawStringArray(new GUIContent(label), ref array, expanded);
        }

        public static bool DrawStringArray(string label, string tooltip, ref string[] array, bool expanded)
        {
            return DrawStringArray(new GUIContent(label, tooltip), ref array, expanded);
        }

        public static bool DrawStringArray(GUIContent label, ref string[] array, bool expanded)
        {
            expanded = EditorGUILayout.Foldout(expanded, label);

            if (expanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    int size = Mathf.Max(0, EditorGUILayout.DelayedIntField("Size", array.Length));

                    if (array.Length != size)
                    {
                        Array.Resize(ref array, size);
                    }

                    for (int i = 0; i < size; i++)
                    {
                        array[i] = EditorGUILayout.TextField($"Item {i}", array[i]);
                    }
                }
            }

            return expanded;
        }

        private static GUIStyle? s_wordWrapLabel;
        private static GUIStyle WordWrapLabel
        {
            get
            {
                if (s_wordWrapLabel == null)
                {
                    s_wordWrapLabel = new GUIStyle(EditorStyles.label);
                    s_wordWrapLabel.wordWrap = true;
                }

                return s_wordWrapLabel;
            }
        }

        public static void WordWrappedLabel(string label, params GUILayoutOption[] options)
        {
            GUILayout.Label(label, WordWrapLabel, options);
        }

        public static void DrawSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public static GUIStyle StyleWithTextColour(Color c)
        {
            return new GUIStyle() { normal = new GUIStyleState() { textColor = c } };
        }

        // This allows fitting with other styles properly, instead of defaulting to GUIStyle.none.
        public static GUIStyle StyleWithTextColour(Color c, GUIStyle original)
        {
            return new GUIStyle(original) { normal = new GUIStyleState() { textColor = c } };
        }
    }

    internal class WordWrapScope : IDisposable
    {
        private readonly bool _wrap;

        public WordWrapScope(bool wrap)
        {
            _wrap = EditorStyles.label.wordWrap;
            EditorStyles.label.wordWrap = wrap;
        }

        public void Dispose()
        {
            EditorStyles.label.wordWrap = _wrap;
        }
    }

    // Unity is Unity.
    internal class ResetIndentScope : IDisposable
    {
        private readonly int _indent;

        public ResetIndentScope()
        {
            _indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = _indent;
        }
    }
}