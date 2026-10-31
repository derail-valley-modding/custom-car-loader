using DV.ThingTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Importer
{
    internal class DummyScriptToInspectNonObjects : MonoBehaviour
    {
        private StationSpawnChanceData _dummyData = new();

        private void PrintContainerGroups()
        {
            PrintCargoGroupsWithCargo(CargoType.EmptySunOmni);
            PrintCargoGroupsWithCargo(CargoType.EmptyIskar);
            PrintCargoGroupsWithCargo(CargoType.EmptyObco);
            PrintCargoGroupsWithCargo(CargoType.EmptyGoorsk);
            PrintCargoGroupsWithCargo(CargoType.EmptyKrugmann);
            PrintCargoGroupsWithCargo(CargoType.EmptyBrohm);
            PrintCargoGroupsWithCargo(CargoType.EmptyAAG);
            PrintCargoGroupsWithCargo(CargoType.EmptySperex);
            PrintCargoGroupsWithCargo(CargoType.EmptyNovae);
            PrintCargoGroupsWithCargo(CargoType.EmptyTraeg);
            PrintCargoGroupsWithCargo(CargoType.EmptyChemlek);
            PrintCargoGroupsWithCargo(CargoType.EmptyNeoGamma);

            PrintCargoGroupsWithCargo(CargoType.ElectronicsAAG);
            PrintCargoGroupsWithCargo(CargoType.ElectronicsIskar);
            PrintCargoGroupsWithCargo(CargoType.ElectronicsKrugmann);
            PrintCargoGroupsWithCargo(CargoType.ElectronicsNovae);
            PrintCargoGroupsWithCargo(CargoType.ElectronicsTraeg);

            PrintCargoGroupsWithCargo(CargoType.ToolsIskar);
            PrintCargoGroupsWithCargo(CargoType.ToolsBrohm);
            PrintCargoGroupsWithCargo(CargoType.ToolsAAG);
            PrintCargoGroupsWithCargo(CargoType.ToolsNovae);
            PrintCargoGroupsWithCargo(CargoType.ToolsTraeg);

            PrintCargoGroupsWithCargo(CargoType.ClothingObco);
            PrintCargoGroupsWithCargo(CargoType.ClothingNeoGamma);
            PrintCargoGroupsWithCargo(CargoType.ClothingNovae);
            PrintCargoGroupsWithCargo(CargoType.ClothingTraeg);

            PrintCargoGroupsWithCargo(CargoType.ChemicalsIskar);
            PrintCargoGroupsWithCargo(CargoType.ChemicalsSperex);
        }

        public static void PrintCargoGroupsWithCargo(CargoType cargo, string? id = null)
        {
            List<string> routes = new();

            foreach (var controller in StationController.allStations)
            {
                foreach (var group in controller.proceduralJobsRuleset.outputCargoGroups)
                {
                    if (group.cargoTypes.Contains(cargo))
                    {
                        foreach (var station in group.stations)
                        {
                            routes.Add($"{controller.name} -> {station.name}");
                        }
                    }
                }

                foreach (var group in controller.proceduralJobsRuleset.inputCargoGroups)
                {
                    if (group.cargoTypes.Contains(cargo))
                    {
                        foreach (var station in group.stations)
                        {
                            routes.Add($"{station.name} -> {controller.name}");
                        }
                    }
                }
            }

            if (routes.Count < 1)
            {
                CCLPlugin.Warning($"No routes for cargo {cargo}");
                return;
            }

            CCLPlugin.Log($"Routes for cargo {(string.IsNullOrEmpty(id) ? cargo : id)}:\n{string.Join(",\n", routes.Distinct().OrderBy(x => x))}");
        }
    }

    internal class ScriptForFailures : MonoBehaviour
    {
        private const int WindowId = 9000;

        private static readonly GUILayoutOption s_width = GUILayout.MaxWidth(380);

        private static bool s_spamFlag = false;

        private Rect _windowRect = new(10, 10, 400, 200);
        private Vector2 _scroll = Vector2.zero;

        public static readonly List<string> StaticFailures = new();
        public static readonly List<string> LoadFailures = new();

        private void Start()
        {
            if (LoadFailures.Count == 0 && StaticFailures.Count == 0)
            {
                CloseWindow();
            }
        }

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(WindowId, _windowRect, FailuresWindow, "CCL Failures");
        }

        private void FailuresWindow(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            GUILayout.BeginVertical();

            foreach (var failure in StaticFailures)
            {
                GUILayout.Label(failure, s_width);
            }

            foreach (var failure in LoadFailures)
            {
                GUILayout.Label(failure, s_width);
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                CloseWindow();
            }

            if (s_spamFlag)
            {
                if (GUILayout.Button("Close And Disable Spam Warning"))
                {
                    CloseWindow();
                    DV.ErrorSoundLogHandler.SoundEnabled = false;
                    CCLPlugin.Settings.SpamWarningDisabled = true;
                    CCLPlugin.Settings.Save(CCLPlugin.Instance);
                }
            }
        }

        private void CloseWindow()
        {
            Destroy(this);
        }

        public static void SetSpamFlag()
        {
            s_spamFlag = true;
        }
    }
}
