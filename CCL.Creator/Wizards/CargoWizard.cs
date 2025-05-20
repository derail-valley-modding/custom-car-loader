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
                "Assign prefabs to cargo automatically"),
            new GUIContent("Mass Visualiser",
                "Check cargo masses and multipliers")
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
            int modesPerRow = Mathf.Max(1, Mathf.CeilToInt(position.width / (s_modes.Length * 120.0f)));
            _mode = GUILayout.SelectionGrid(_mode, s_modes, modesPerRow, EditorStyles.miniButtonMid, GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            switch (_mode)
            {
                case 0:
                    DrawSetCargo();
                    break;
                case 1:
                    DrawSetPrefabs();
                    break;
                case 2:
                    DrawMassVisualiser();
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

        #region Mass Visualiser

        private static Dictionary<string, float> s_massMap = new Dictionary<string, float>()
        {
            { "Coal", 56000 },
            { "IronOre", 62000 },
            { "CrudeOil", 26000 },
            { "Diesel", 26000 },
            { "Gasoline", 25000 },
            { "Methane", 23000 },
            { "Logs", 42000 },
            { "Boards", 35000 },
            { "Plywood", 28000 },
            { "RailwaySleepers", 35000 },
            { "Wheat", 42000 },
            { "Corn", 45000 },
            { "SunflowerSeeds", 48000 },
            { "Flour", 36000 },
            { "Pigs", 13000 },
            { "Cows", 16000 },
            { "Poultry", 9000 },
            { "Sheep", 12000 },
            { "Goats", 11000 },
            { "Fish", 25000 },
            { "Bread", 15000 },
            { "DairyProducts", 31000 },
            { "MeatProducts", 30000 },
            { "CannedFood", 34000 },
            { "CatFood", 29000 },
            { "TemperateFruits", 36000 },
            { "Vegetables", 42000 },
            { "Milk", 19000 },
            { "Eggs", 25000 },
            { "Cotton", 12000 },
            { "Wool", 18000 },
            { "TropicalFruits", 31000 },
            { "SteelRolls", 52000 },
            { "SteelBillets", 38000 },
            { "SteelSlabs", 43000 },
            { "SteelBentPlates", 41000 },
            { "SteelRails", 46000 },
            { "ScrapMetal", 40000 },
            { "WoodScrap", 30000 },
            { "WoodChips", 36000 },
            { "ScrapContainers", 6000 },
            { "ElectronicsIskar", 35000 },
            { "ElectronicsKrugmann", 35000 },
            { "ElectronicsAAG", 35000 },
            { "ElectronicsNovae", 35000 },
            { "ElectronicsTraeg", 35000 },
            { "ToolsIskar", 37000 },
            { "ToolsBrohm", 37000 },
            { "ToolsAAG", 37000 },
            { "ToolsNovae", 37000 },
            { "ToolsTraeg", 37000 },
            { "Furniture", 34000 },
            { "Pipes", 40000 },
            { "ClothingObco", 31000 },
            { "ClothingNeoGamma", 31000 },
            { "ClothingNovae", 31000 },
            { "ClothingTraeg", 31000 },
            { "Medicine", 34000 },
            { "ChemicalsIskar", 30000 },
            { "ChemicalsSperex", 30000 },
            { "NewCars", 22000 },
            { "ImportedNewCars", 24000 },
            { "Tractors", 6000 },
            { "Excavators", 25000 },
            { "MiningTrucks", 45000 },
            { "CityBuses", 11000 },
            { "SemiTrailers", 7000 },
            { "CraneParts", 8000 },
            { "Trams", 18000 },
            { "ForestryTrailers", 7000 },
            { "Alcohol", 23000 },
            { "Acetylene", 19000 },
            { "CryoOxygen", 22000 },
            { "CryoHydrogen", 22000 },
            { "Argon", 24000 },
            { "Nitrogen", 22000 },
            { "Ammonia", 25000 },
            { "SodiumHydroxide", 23000 },
            { "AmmoniumNitrate", 36000 },
            { "SpentNuclearFuel", 52000 },
            { "Ammunition", 31000 },
            { "Biohazard", 8000 },
            { "Tanks", 41000 },
            { "MilitaryTrucks", 16000 },
            { "MilitarySupplies", 35000 },
            { "AttackHelicopsters", 7000 },
            { "Missiles", 7000 },
            { "MilitaryCars", 9000 },
            { "TrainPartsDE2", 6000 },
            { "TrainPartsDE6", 16000 },
            { "TrainPartsDH4", 7000 },
            { "TrainPartsDM3", 7000 },
            { "TrainPartsS060", 6000 },
            { "TrainPartsS282A", 12000 },
            { "EmptySunOmni", 6000 },
            { "EmptyIskar", 6000 },
            { "EmptyObco", 6000 },
            { "EmptyGoorsk", 6000 },
            { "EmptyKrugmann", 6000 },
            { "EmptyBrohm", 6000 },
            { "EmptyAAG", 6000 },
            { "EmptySperex", 6000 },
            { "EmptyNovae", 6000 },
            { "EmptyTraeg", 6000 },
            { "EmptyChemlek", 6000 },
            { "EmptyNeoGamma", 6000 }
        };

        private void DrawMassVisualiser()
        {
            EditorGUI.BeginChangeCheck();

            foreach (var item in _setup.Entries)
            {
                DrawMass(item);
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveChanges();
            }
        }

        private void DrawMass(CargoEntry entry)
        {
            EditorGUILayout.BeginHorizontal();

            entry.AmountPerCar = EditorGUILayout.FloatField(entry.AmountPerCar, GUILayout.Width(60.0f));

            if (s_massMap.TryGetValue(entry.CargoId, out float mass))
            {
                mass /= 1000.0f;
                EditorGUILayout.LabelField(entry.CargoId, $"{mass * entry.AmountPerCar:F2}/{mass:F2} t");
            }
            else
            {
                EditorGUILayout.LabelField(entry.CargoId, $"Unknown Mass");
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
