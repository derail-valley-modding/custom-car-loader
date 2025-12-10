using CCL.Creator.Utility;
using CCL.Types;
using DVLangHelper.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    public class CarWizard : EditorWindow
    {
        private enum CargoRole
        {
            None,

            [Tooltip("Car designed to transport bulk material")]
            BulkMaterial,

            [Tooltip("Flatbed car designed to transport large objects")]
            Flatbed,
            [Tooltip("Flatbed car with stakes designed to transport stackable objects")]
            FlatbedStakes,
            [Tooltip("Flatbed car with centerbeam designed to transport tied objects")]
            FlatbedCenterbeam,
            [Tooltip("Flatbed car with bulkheads designed to transport heavy large objects")]
            FlatbedBulkhead,

            [Tooltip("Enclosed car designed to transport small objects")]
            Enclosed,
            [Tooltip("Enclosed car with air conditioning designed to transport small objects")]
            Refrigerated,

            [Tooltip("Tank car designed to transport fossil fuel derivatives in liquid form")]
            LiquidOilDerivatives,
            [Tooltip("Tank car designed to transport fossil fuel derivatives in gas form")]
            GasOilDerivatives,
            [Tooltip("Tank car designed to transport chemicals in liquid form")]
            LiquidChemicals,
            [Tooltip("Tank car designed to transport food in liquid form")]
            LiquidFood,

            [Tooltip("Car designed to transport powders")]
            Powders,
            [Tooltip("Car designed to transport road vehicles")]
            RoadVehicles,
            [Tooltip("Car designed to transport animals")]
            Livestock,
            [Tooltip("Car designed to transport passengers")]
            Passenger,

            [Tooltip("Enclosed military car designed to transport boxes")]
            MilitaryEnclosed,
            [Tooltip("Military flatbed car designed to transport large objects")]
            MilitaryFlatbed,
            [Tooltip("Military car designed to transport nuclear flasks")]
            MilitaryNuclearFlasks,

            [Tooltip("For any different kind of transport")]
            Other,
        }

        private const string CAR_FOLDER = "_CCL_CARS";
        private const string CAR_TEMPLATE_PATH = "Assets/CarCreator/Prefabs/car_template.prefab";
        private const string FREIGHT_INTERACTABLES_PATH = "Assets/CarCreator/Prefabs/freight_interactables.prefab";

        private static CarWizard? _window;
        private static string GetCarFolder() => Path.Combine(Application.dataPath, CAR_FOLDER);

        private CarSettings _carSettings = new CarSettings();
        private PackSettings _packSettings = new PackSettings();
        private Vector2 _scrollPosition = Vector2.zero;
        private bool _createPack = true;

        [MenuItem("CCL/Car Wizard", priority = MenuOrdering.MenuBar.CarWizard)]
        private static void ShowWindow()
        {
            _window = GetWindow<CarWizard>();
            _window.Refresh();
            _window.Show();
        }

        private void Refresh()
        {
            titleContent = new GUIContent("CCL - New Car Type");
            _carSettings = new CarSettings();
            _createPack = true;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorHelpers.WordWrappedLabel(
                "This wizard will automagically create car type and livery assets for your new vehicle. " +
                "Simply fill out the fields below to get started!");

            EditorHelpers.DrawSeparator();

            _carSettings.Name = RenderTextbox(
                "Pick an in-game name for your car - you will be able to add translations later",
                "Car Name", _carSettings.Name);

            _carSettings.ID = RenderTextbox(
                "This will be the unique identifier for your car - we will also apply it to the default livery",
                "Car ID", _carSettings.ID);

            _carSettings.Kind = RenderEnum(
                "This is the \"category\" of the vehicle",
                "Kind", _carSettings.Kind);

            _carSettings.BaseCarType = RenderEnum(
                "Pick the base car type that you would like to use bogies and buffers from",
                "Base Type", _carSettings.BaseCarType);

            using (new EditorGUI.DisabledScope(_carSettings.RoleDisabled))
            {
                _carSettings.Role = RenderEnum(
                    "Pick the role of the car",
                    "Role", _carSettings.Role);
            }

            EditorHelpers.DrawSeparator();

            _createPack = RenderToggle(
                "You will need at least 1 pack to export any number of cars",
                "Create Pack", _createPack);

            using (new EditorGUI.DisabledScope(!_createPack))
            {
                _packSettings.Author = RenderTextbox(
                    "Your (user)name for publishing the car pack",
                    "Author", _packSettings.Author);
            }


            EditorGUILayout.EndScrollView();

            EditorHelpers.DrawSeparator();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            var validPack = !_createPack || _packSettings.IsValid;
            using (new EditorGUI.DisabledScope(!(_carSettings.IsValid && validPack)))
            {
                if (GUILayout.Button("Create Car"))
                {
                    CreateNewCar(_carSettings, _createPack ? _packSettings : null);
                    Close();
                    return;
                }
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
                return;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private static string RenderTextbox(string help, string label, string? value)
        {
            EditorGUILayout.Space();
            string val = EditorGUILayout.TextField(label, value);
            EditorHelpers.WordWrappedLabel(help);
            return val;
        }

        private static T RenderEnum<T>(string help, string label, T value) where T : Enum
        {
            EditorGUILayout.Space();
            T val = (T)EditorGUILayout.EnumPopup(label, value);
            EditorHelpers.WordWrappedLabel(help);
            return val;
        }

        private static bool RenderToggle(string help, string label, bool value)
        {
            EditorGUILayout.Space();
            bool val = EditorGUILayout.Toggle(label, value);
            EditorHelpers.WordWrappedLabel(help);
            return val;
        }

        private static void CreateNewCar(CarSettings settings, PackSettings? packSettings)
        {
            string carFolder = Path.Combine(GetCarFolder(), settings.Name);
            Directory.CreateDirectory(carFolder);

            string relativeCarFolder = Path.Combine("Assets", CAR_FOLDER, settings.Name);

            // Create scene.
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            string scenePath = Path.Combine(relativeCarFolder, $"{settings.ID}_scene.unity");

            // Create prefab.
            string carPrefabPath = Path.Combine(relativeCarFolder, $"{settings.ID}_template.prefab");
            AssetDatabase.CopyAsset(CAR_TEMPLATE_PATH, carPrefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(carPrefabPath);

            PrefabUtility.InstantiatePrefab(prefab);

            // Create interactables prefab.
            GameObject interactables = CreateExternalInteractables(settings.ID, relativeCarFolder);

            // Create car type.
            var carType = AssetHelper.CreateScriptableAsset<CustomCarType>(relativeCarFolder, $"{settings.ID}_cartype");
            carType.id = settings.ID;
            carType.KindSelection = settings.Kind;
            carType.NameTranslations = TranslationData.Default(settings.Name);
            carType.carIdPrefix = GetCarIdPrefix(settings.Name, settings.Kind, settings.Role);
            carType.mass = 25000;
            carType.wheelRadius = 0.459f;

            bool isLoco = settings.Kind == DVTrainCarKind.Loco;
            carType.brakes = new CustomCarType.BrakesSetup()
            {
                hasCompressor = isLoco,
                brakeValveType = isLoco ? CustomCarType.BrakesSetup.TrainBrakeType.SelfLap : CustomCarType.BrakesSetup.TrainBrakeType.None,
                TrainBrakeCurveType = isLoco ? CustomCarType.BrakesSetup.BrakeCurveType.LocoDefault : CustomCarType.BrakesSetup.BrakeCurveType.Linear,
                IndBrakeCurveType = isLoco ? CustomCarType.BrakesSetup.BrakeCurveType.LocoDefault : CustomCarType.BrakesSetup.BrakeCurveType.Linear,
                hasIndependentBrake = isLoco,
                hasHandbrake = true,
            };

            // Create livery.
            var livery = AssetHelper.CreateScriptableAsset<CustomCarVariant>(relativeCarFolder, $"{settings.ID}_livery");
            livery.id = settings.ID;
            livery.parentType = carType;
            livery.NameTranslations = TranslationData.Default(settings.Name);
            livery.prefab = prefab;
            livery.externalInteractablesPrefab = interactables;
            livery.BufferType = GetBufferFromBase(settings.BaseCarType);
            livery.FrontBogie = GetBogieFromBase(settings.BaseCarType);
            livery.RearBogie = livery.FrontBogie;
            livery.parentType = carType;

            carType.liveries = new List<CustomCarVariant>() { livery };
            carType.ForceValidation();

            AssetHelper.SaveAsset(livery);
            AssetHelper.SaveAsset(carType);

            // Create pack.
            if (packSettings != null)
            {
                var pack = AssetHelper.CreateScriptableAsset<CustomCarPack>(relativeCarFolder, $"{settings.ID}_pack");
                pack.Author = packSettings.Author;
                pack.PackId = settings.ID;
                pack.PackName = settings.Name;
                pack.Cars = new[] { carType };

                AssetHelper.SaveAsset(pack);
            }

            EditorSceneManager.SaveScene(scene, scenePath);
            Selection.activeObject = carType;
        }

        private static GameObject CreateExternalInteractables(string carId, string relativeCarFolder)
        {
            string interactablesPath = Path.Combine(relativeCarFolder, $"{carId}_interactables.prefab");
            AssetDatabase.CopyAsset(FREIGHT_INTERACTABLES_PATH, interactablesPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(interactablesPath);

            PrefabUtility.InstantiatePrefab(prefab);
            return prefab;
        }

        private static string GetCarIdPrefix(string name, DVTrainCarKind kind, CargoRole role)
        {
            switch (kind)
            {
                case DVTrainCarKind.Car:
                    if (role == CargoRole.None) goto default;
                    if (role == CargoRole.Other) return(GetInitials(name));
                    return GetDesignationId(role);
                default:
                    return "-";
            }
        }

        private static string GetInitials(string value)
        {
            return string.Concat(value
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length >= 1 && char.IsLetter(x[0]))
                .Select(x => char.ToUpper(x[0])));
        }

        private static BogieType GetBogieFromBase(BaseTrainCarType carType)
        {
            switch (carType)
            {
                case BaseTrainCarType.LocoDE2:
                    return BogieType.DE2;
                case BaseTrainCarType.LocoS282:
                    return BogieType.S282;
                case BaseTrainCarType.LocoDE6:
                case BaseTrainCarType.LocoDE6Slug:
                    return BogieType.DE6;
                case BaseTrainCarType.LocoDH4:
                    return BogieType.DH4;
                case BaseTrainCarType.LocoMicroshunter:
                    return BogieType.Microshunter;
                case BaseTrainCarType.Handcar:
                    return BogieType.Handcar;

                case BaseTrainCarType.Custom:
                    return BogieType.Custom;
                default:
                    return BogieType.Default;
            }
        }

        private static BufferType GetBufferFromBase(BaseTrainCarType carType)
        {
            switch (carType)
            {
                case BaseTrainCarType.Boxcar:
                    return BufferType.Buffer02;

                case BaseTrainCarType.LocoDE2:
                case BaseTrainCarType.LocoDH4:
                case BaseTrainCarType.LocoDM3:
                case BaseTrainCarType.Stock:
                    return BufferType.Buffer03;

                case BaseTrainCarType.LocoS060:
                case BaseTrainCarType.Gondola:
                    return BufferType.Buffer04;

                case BaseTrainCarType.LocoDE6:
                case BaseTrainCarType.LocoDE6Slug:
                case BaseTrainCarType.LocoMicroshunter:
                    return BufferType.Buffer05;

                case BaseTrainCarType.BoxcarMilitary:
                case BaseTrainCarType.Refrigerator:
                    return BufferType.Buffer06;

                case BaseTrainCarType.Caboose:
                case BaseTrainCarType.TankGas:
                case BaseTrainCarType.TankOil:
                case BaseTrainCarType.TankChem:
                case BaseTrainCarType.TankFood:
                    return BufferType.Buffer07;

                case BaseTrainCarType.Passenger:
                    return BufferType.Buffer08;

                case BaseTrainCarType.LocoS282:
                    return BufferType.S282A;

                case BaseTrainCarType.S282Tender:
                    return BufferType.S282B;

                case BaseTrainCarType.Custom:
                    return BufferType.Custom;

                default:
                    return BufferType.Buffer09;
            }
        }

        private static string GetDesignationId(CargoRole role) => role switch
        {
            CargoRole.BulkMaterial => "BK",
            CargoRole.Flatbed => "FF",
            CargoRole.FlatbedStakes => "FS",
            CargoRole.FlatbedCenterbeam => "FC",
            CargoRole.FlatbedBulkhead => "FB",
            CargoRole.Enclosed => "BX",
            CargoRole.Refrigerated => "RF",
            CargoRole.LiquidOilDerivatives => "OL",
            CargoRole.GasOilDerivatives => "GS",
            CargoRole.LiquidChemicals => "CH",
            CargoRole.LiquidFood => "FD",
            CargoRole.Powders => "PD",
            CargoRole.RoadVehicles => "VT",
            CargoRole.Livestock => "LS",
            CargoRole.Passenger => "PS",
            CargoRole.MilitaryEnclosed => "XB",
            CargoRole.MilitaryFlatbed => "XF",
            CargoRole.MilitaryNuclearFlasks => "XN",
            _ => string.Empty,
        };

        private class CarSettings
        {
            public DVTrainCarKind Kind;
            public string ID = string.Empty;
            public string Name = string.Empty;
            public BaseTrainCarType BaseCarType = BaseTrainCarType.Flatbed;
            public CargoRole Role = CargoRole.Flatbed;

            public bool IsValid =>
                !string.IsNullOrWhiteSpace(ID) &&
                !string.IsNullOrWhiteSpace(Name);

            public bool RoleDisabled => Kind != DVTrainCarKind.Car;
        }

        private class PackSettings
        {
            public string Author = string.Empty;

            public bool IsValid =>
                !string.IsNullOrWhiteSpace(Author);
        }
    }
}