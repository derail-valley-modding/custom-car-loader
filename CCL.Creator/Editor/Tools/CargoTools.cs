using CCL.Creator.Utility;
using CCL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor.Tools
{
    internal class CargoTools : EditorWindow
    {
        [MenuItem("CCL/Cargo Tools")]
        public static void ShowWindow()
        {
            var window = GetWindow<CargoTools>();
            window.Show();
            window.titleContent = new GUIContent("Cargo Tools");
            window._currentCar = AssetHelper.GetSelectedAsset<CustomCarType>();
        }

        private static GUIContent[] s_modes = new[]
        {
            new GUIContent("Set Cargo",
                "Pick cargo automatically"),
            new GUIContent("Set Prefabs",
                "Assign prefabs to cargo automatically")
        };

        private SerializedObject _editor = null!;
        private CustomCarType _currentCar = null!;

        private float _scroll = 0.0f;
        [SerializeField]
        private int _mode = -1;

        private void OnAwake()
        {
            if (_currentCar == null)
            {
                _currentCar = AssetHelper.GetSelectedAsset<CustomCarType>();
            }
        }

        private void OnEnable()
        {
            _editor = new SerializedObject(this);
        }

        private void OnGUI()
        {
            _editor.Update();

            _scroll = EditorGUILayout.BeginScrollView(new Vector2(0, _scroll)).y;
            EditorGUI.indentLevel++;

            _currentCar = EditorHelpers.ObjectField("Current Car", _currentCar, false);

            if (GUILayout.Button("Get selected"))
            {
                var selected = AssetHelper.GetSelectedAsset<CustomCarType>();

                if (selected != null)
                {
                    _currentCar = selected;
                }
            }

            if (_currentCar != null)
            {
                EditorGUILayout.LabelField("Cargo Count", _currentCar.CargoTypes.Entries.Count.ToString());
            }
            else
            {
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            EditorGUILayout.Space();
            _mode = GUILayout.SelectionGrid(_mode, s_modes, 2, EditorStyles.miniButtonMid);
            EditorGUILayout.Space();

            switch (_mode)
            {
                case 0:
                    DrawSetCargo();
                    break;
                case 1:
                    DrawSetPrefabs();
                    break;
                default:
                    EditorGUILayout.HelpBox("Please select an option above!", MessageType.Info);
                    break;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();
        }

        private bool RequireNotNull<T>(T obj, string name)
            where T : class
        {
            if (obj == null)
            {
                Debug.LogError($"{name} is not set!");
                return false;
            }
            
            return true;
        }

        private void SaveChanges()
        {
            AssetHelper.SaveAsset(_currentCar);
        }

        #region SET CARGO

        private CarParentType _carParentType = CarParentType.None;
        private float _amount = 1.0f;

        private float _cargoScroll = 0.0f;
        private bool _showCargos = true;

        private void DrawSetCargo()
        {
            _carParentType = EditorHelpers.EnumPopup("Base Car Type", _carParentType);
            _amount = Mathf.Max(EditorGUILayout.FloatField("Amount", _amount), 0.0f);

            EditorGUILayout.Space();

            _cargoScroll = EditorGUILayout.BeginScrollView(new Vector2(0, _cargoScroll)).y;
            _showCargos = EditorGUILayout.Foldout(_showCargos, "Cargo for type");

            if (_showCargos)
            {

                using (new EditorGUI.IndentLevelScope())
                {
                    var cargos = CargoHelper.GetCargosForType(_carParentType);

                    foreach (var item in cargos)
                    {
                        EditorGUILayout.LabelField(Enum.GetName(typeof(BaseCargoType), item));
                    }

                    if (cargos.Length == 0)
                    {
                        EditorGUILayout.HelpBox("None", MessageType.Info);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add to existing cargo"))
            {
                AddCurrentCargoToCar(false);
            }

            if (GUILayout.Button("Overwrite existing cargo"))
            {
                AddCurrentCargoToCar(true);
            }

            if (GUILayout.Button("Clear cargo"))
            {
                if (RequireNotNull(_currentCar, "Current Car"))
                {
                    _currentCar.CargoTypes.Entries.Clear();
                    Debug.Log($"Cleared cargo from {_currentCar.name}");
                    SaveChanges();
                }
            }

            if (GUILayout.Button("Add containers"))
            {
                if (RequireNotNull(_currentCar, "Current Car"))
                {
                    AddCargoToCar(CargoHelper.ContainerCargo);
                }
            }
        }

        private void AddCurrentCargoToCar(bool clear)
        {
            if (!RequireNotNull(_currentCar, "Current Car"))
            {
                return;
            }

            if (clear)
            {
                _currentCar.CargoTypes.Entries.Clear();
            }

            var cargos = CargoHelper.GetCargosForType(_carParentType);
            AddCargoToCar(cargos);
        }

        private void AddCargoToCar(BaseCargoType[] cargos)
        {
            // Turn the cargo into loadable cargo entries.
            List<LoadableCargoEntry> entries = cargos
                .Select(c => new LoadableCargoEntry() { AmountPerCar = _amount, CargoType = c })
                .ToList();

            _currentCar.CargoTypes.Entries.AddRange(entries);

            Debug.Log($"Added {entries.Count} entries: {string.Join(", ", cargos)}");
            SaveChanges();
        }

        #endregion

        #region SET PREFABS

        private string _path = "Assets/_CCL_CARS/";
        [SerializeField]
        private GameObject[] _prefabs = new GameObject[0];
        private bool _emptyOnly = true;

        private float _prefabsScroll = 0.0f;

        private void DrawSetPrefabs()
        {
            _path = EditorGUILayout.TextField(new GUIContent("Asset Folder",
                "The folder where the prefabs you want to use are. Includes child folders."), _path);

            if (GUILayout.Button("Get prefabs"))
            {
                _prefabs = AssetHelper.GetPrefabsAtPath(_path);
            }

            _emptyOnly = EditorGUILayout.Toggle(new GUIContent("Add to empty only",
                "If true, will only assign prefabs to cargo that does not yet have " +
                "a prefab assigned to it."), _emptyOnly);

            EditorGUILayout.Space();

            _prefabsScroll = EditorGUILayout.BeginScrollView(new Vector2(0, _prefabsScroll)).y;
            EditorGUILayout.PropertyField(_editor.FindProperty("_prefabs"));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            _editor.ApplyModifiedProperties();

            if (GUILayout.Button("Auto assign prefabs"))
            {
                AutoAssign();
            }
        }

        public void AutoAssign()
        {
            if (!RequireNotNull(_currentCar, "Current Car"))
            {
                return;
            }

            List<string> assigned = new List<string>();
            int count = 0;

            foreach (var item in _currentCar.CargoTypes.Entries)
            {
                // Assign prefabs if the name contains a cargo type.
                // Containers have some special logic to remove their types,
                // and only keep the brand name.
                var prefabs = _prefabs.Where(x => x.name.Contains(CargoHelper.CleanName(item.CargoType))).ToArray();

                // Only assign prefabs if the array is empty.
                if (_emptyOnly && (item.ModelVariants == null || item.ModelVariants.Length == 0))
                {
                    item.ModelVariants = prefabs;

                    if (prefabs.Length > 0)
                    {
                        assigned.Add($"{item.CargoType} ({prefabs.Length})");
                        count += prefabs.Length;
                    }
                    continue;
                }

                // Add prefabs to the list.
                var list = item.ModelVariants.ToList();
                list.AddRange(prefabs);
                item.ModelVariants = list.ToArray();

                if (prefabs.Length > 0)
                {
                    assigned.Add($"{item.CargoType} ({prefabs.Length})");
                    count += prefabs.Length;
                }
            }

            Debug.Log($"Added {count} prefabs: {string.Join(", ", assigned)}");
            SaveChanges();
        }

        #endregion
    }
}
