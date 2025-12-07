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

    internal class ScriptForLoadFailures : MonoBehaviour
    {
        private const int WindowId = 9000;

        private Rect _windowRect = new(10, 10, 400, 200);
        private Vector2 _scroll = Vector2.zero;

        private void Start()
        {
            if (CarManager.LoadFailures.Count == 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(WindowId, _windowRect, FailuresWindow, "CCL Load Failures");
        }

        private void FailuresWindow(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            GUILayout.BeginVertical();

            foreach (var item in CarManager.LoadFailures)
            {
                GUILayout.Label(item, GUILayout.MaxWidth(380));
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                Destroy(gameObject);
            }
        }
    }
}
