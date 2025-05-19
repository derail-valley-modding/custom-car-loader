using CCL.Creator.Utility;
using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class CargoWizard : EditorWindow
    {
        [MenuItem("CCL/Cargo Wizard", priority = MenuOrdering.MenuBar.CargoWizard)]
        public static void ShowWindow()
        {
            ShowWindowForSetup(AssetHelper.GetSelectedAsset<CargoSetup>());
        }

        public static void ShowWindowForSetup(CargoSetup setup)
        {
            var window = GetWindow<CargoWizard>();
            window.Show();
            window.titleContent = new GUIContent("CCL - Cargo Wizard");
            window._setup = setup;
        }

        private static GUIContent[] s_modes = new[]
{
            new GUIContent("Set Cargo",
                "Pick cargo automatically"),
            new GUIContent("Set Prefabs",
                "Assign prefabs to cargo automatically")
        };

        private SerializedObject _editor = null!;
        private CargoSetup _setup = null!;

        private float _scroll = 0.0f;
        [SerializeField]
        private int _mode = -1;

        private bool HasSetup => _setup != null;

        private void Awake()
        {
            if (_setup == null)
            {
                _setup = AssetHelper.GetSelectedAsset<CargoSetup>();
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

            _setup = EditorHelpers.ObjectField("Current Setup", _setup, false);

            if (GUILayout.Button("Get selected"))
            {
                var selected = AssetHelper.GetSelectedAsset<CargoSetup>();

                if (selected != null)
                {
                    _setup = selected;
                }
            }

            if (_setup != null)
            {
                EditorGUILayout.LabelField("Entries", _setup.Entries.Count.ToString());
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

        private bool RequireNotNull<T>(T? obj, string name)
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
            AssetHelper.SaveAsset(_setup);
        }

        #region Set Cargo

        private CarCargoSet _carParentType = CarCargoSet.None;
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
                    var cargos = CargoHelper.CarToCargo(_carParentType);

                    foreach (var item in cargos)
                    {
                        EditorGUILayout.LabelField(item);
                    }

                    if (cargos.Length == 0)
                    {
                        EditorGUILayout.HelpBox("None", MessageType.Info);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            GUI.enabled = HasSetup;

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
                if (RequireNotNull(_setup, "Current Setup"))
                {
                    _setup.Entries.Clear();
                    Debug.Log($"Cleared cargo from {_setup.name}");
                    SaveChanges();
                }
            }

            if (GUILayout.Button("Add containers"))
            {
                if (RequireNotNull(_setup, "Current Setup"))
                {
                    AddCargoToCar(CargoHelper.ContainerCargo);
                }
            }

            GUI.enabled = true;
        }

        private void AddCurrentCargoToCar(bool clear)
        {
            if (!RequireNotNull(_setup, "Current Setup"))
            {
                return;
            }

            if (clear)
            {
                _setup.Entries.Clear();
            }

            var cargos = CargoHelper.CarToCargo(_carParentType);
            AddCargoToCar(cargos);
        }

        private void AddCargoToCar(string[] ids)
        {
            // Turn the cargo IDs into cargo entries.
            List<CargoEntry> entries = ids
                .Select(c => new CargoEntry() { CargoId = c, AmountPerCar = _amount })
                .ToList();

            _setup.Entries.AddRange(entries);

            Debug.Log($"Added {entries.Count} entries: {string.Join(", ", ids)}");
            SaveChanges();
        }

        #endregion

        #region Set Prefabs

        private string _path = "Assets/_CCL_CARS/";
        [SerializeField]
        private GameObject[] _prefabs = new GameObject[0];
        private bool _emptyOnly = true;

        private float _prefabsScroll = 0.0f;

        private void DrawSetPrefabs()
        {
            _path = EditorGUILayout.TextField(new GUIContent("Asset Folder",
                "The folder where the prefabs you want to use are\nIncludes child folders"), _path);

            if (GUILayout.Button("Set path to current"))
            {
                if (RequireNotNull(_setup, "Current Setup"))
                {
                    _path = AssetHelper.GetFolder(_setup);
                }
            }

            if (GUILayout.Button("Get prefabs"))
            {
                _prefabs = AssetHelper.GetPrefabsAtPath(_path);
            }

            _emptyOnly = EditorGUILayout.Toggle(new GUIContent("Add to empty only",
                "If true, will only assign prefabs to cargo that does not yet have " +
                "a prefab assigned to it"), _emptyOnly);

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

            if (GUILayout.Button("Clear all models"))
            {
                if (RequireNotNull(_setup, "Current Setup"))
                {
                    foreach (var item in _setup.Entries)
                    {
                        item.Models = new GameObject[0];
                    }
                }
            }
        }

        public void AutoAssign()
        {
            if (!RequireNotNull(_setup, "Current Setup"))
            {
                return;
            }

            List<string> assigned = new List<string>();
            int count = 0;

            foreach (var item in _setup.Entries)
            {
                // Assign prefabs if the name contains a cargo type.
                // Containers have some special logic to remove their types,
                // and only keep the brand name.
                var prefabs = _prefabs.Where(x => CheckName(x.name, item)).ToArray();

                // Skip when there are no prefabs.
                // Also skip if the array is not empty when only empty is selected.
                if (prefabs.Length == 0 || (_emptyOnly && item.Models.Length > 0)) continue;

                var list = item.Models.ToList();
                list.AddRange(prefabs);
                item.Models = list.ToArray();

                if (prefabs.Length > 0)
                {
                    assigned.Add($"{item.CargoId} ({prefabs.Length})");
                    count += prefabs.Length;
                }
            }

            Debug.Log($"Added {count} prefabs: {string.Join(", ", assigned)}");
            SaveChanges();
        }

        private static bool CheckName(string name, CargoEntry cargo)
        {
            return name.Contains(CargoHelper.CleanName(cargo.CargoId));
        }

        #endregion
    }
}
