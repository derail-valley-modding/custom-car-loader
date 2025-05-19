using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using CCL.Types;
using CCL.Types.Json;
using CCL.Creator.Utility;

namespace CCL.Creator.Wizards
{
    internal class CarTypeExporter
    {
        public readonly CustomCarType CarType;
        public readonly string ExportFolderPath;
        public readonly string BundleName;
        public readonly string[] Requirements;

        public readonly CarLiveryExporter[] LiveryExporters;

        public CarTypeExporter(string exportFolderPath, CustomCarType carType)
        {
            CarType = carType;
            ExportFolderPath = exportFolderPath;
            BundleName = $"{carType.id.Replace(' ', '_')}_bundle";

            LiveryExporters = carType.liveries.Select(l => new CarLiveryExporter(this, l)).ToArray();

            List<string> requirements = new List<string>() { ExporterConstants.MOD_ID };

            if (OtherMods.RequiresPassengerJobsMod(carType))
            {
                requirements.Add(OtherMods.PASSENGER_JOBS);
            }

            if (OtherMods.RequiresCustomCargoMod(carType))
            {
                requirements.Add(OtherMods.CUSTOM_CARGO);
            }

            Requirements = requirements.ToArray();
        }

        private static bool Progress(string status, float percent)
        {
            return EditorUtility.DisplayCancelableProgressBar("Exporting Car", status, percent);
        }

        public void Export()
        {
            int liveryCount = LiveryExporters.Length;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Prep
                for (int i = 0; i < liveryCount; i++)
                {
                    if (Progress($"Preparing livery \"{LiveryExporters[i].Livery.id}\" for export...", 0.05f + 0.50f * (i / liveryCount))) return;

                    LiveryExporters[i].PrepareForExport();
                }

                CarType.ForceValidation();
                EditorUtility.SetDirty(CarType);
                EditorHelpers.SaveAndRefresh();

                string carTypePath = AssetDatabase.GetAssetPath(CarType);
                AssetImporter.GetAtPath(carTypePath).SetAssetBundleNameAndVariant(BundleName, "");

                // Export
                if (Progress("Exporting assets...", 0.55f)) return;

                //Build assetBundle.
                AssetDatabase.RemoveUnusedAssetBundleNames();
                var processedBundles = new HashSet<string>();
                var trainCarBundleBuild = AssetBundleBuildHelper.GetBuildsForPaths(processedBundles, CarType);

                Debug.Log($"Build bundle {BundleName} to {ExportFolderPath}");
                string builds = string.Join(", ", trainCarBundleBuild.SelectMany(b => b.assetNames));
                Debug.Log($"assets: {builds}");

                BuildPipeline.BuildAssetBundles(ExportFolderPath, trainCarBundleBuild.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
                sw.Stop();
                Debug.Log($"[{DateTime.Now:HH:mm:ss}] Finished AssetBundle build for car: {CarType.id} ({sw.Elapsed.TotalSeconds:F2}s).");

                // Create car.json file.
                if (Progress("Writing car properties...", 0.80f)) return;
                WriteCarSettingsFile();
                WriteModInfoFile();
            }
            catch (Exception ex)
            {
                sw.Stop();
                EditorUtility.DisplayDialog("Export Error",
                    "An exception occurred while exporting your car. Details:\n\n" + ex.Message,
                    "Close Window");
            }
            finally
            {
                // clean up whether we succeeded, failed, or canceled
                Progress("Finished!", 1.00f);
                System.Threading.Thread.Sleep(2000);
                EditorUtility.ClearProgressBar();
            }
        }

        private void WriteCarSettingsFile()
        {
            var outFilePath = Path.Combine(ExportFolderPath, ExporterConstants.JSON_FILENAME);

            //Create master JSONObject
            var jsonfile = new JSONObject
            {
                { ExporterConstants.IDENTIFIER, CarType.id },
                { ExporterConstants.BUNDLE_NAME, BundleName },
                { ExporterConstants.EXPORTER_VERSION, ExporterConstants.ExporterVersion.ToString() }
            };

            //Create JSON file.
            var fullJson = jsonfile.ToString();

            //Write data to JSON file
            using StreamWriter newTask = new StreamWriter(outFilePath, false);
            newTask.Write(fullJson);
        }

        private void WriteModInfoFile()
        {
            var outFilePath = Path.Combine(ExportFolderPath, ExporterConstants.MOD_INFO_FILENAME);

            var jsonFile = new JSONObject
            {
                { "Id", CarType.id },
                { "DisplayName", CarType.NameTranslations.Items[0].Value },
                { "Version", CarType.version },
                { "Author", CarType.author },
                { "ManagerVersion", "0.27.3" },
                { "Requirements", JSONObject.CreateFromObject(Requirements) },
            };

            using StreamWriter stream = new StreamWriter(outFilePath, false);
            stream.Write(jsonFile.ToString(true));
        }
    }

    internal class CarLiveryExporter
    {
        public readonly CarTypeExporter Parent;
        public readonly CustomCarVariant Livery;

        public CarLiveryExporter(CarTypeExporter parent, CustomCarVariant livery)
        {
            Parent = parent;
            Livery = livery;
        }

        public bool PrepareForExport()
        {
            Debug.Log($"Creating temp prefab from car object {Livery.prefab!.name}");

            Livery.interiorPrefab?.SetAssetBundle(Parent.BundleName);
            Livery.explodedInteriorPrefab?.SetAssetBundle(Parent.BundleName);
            Livery.externalInteractablesPrefab?.SetAssetBundle(Parent.BundleName);
            Livery.explodedExternalInteractablesPrefab?.SetAssetBundle(Parent.BundleName);

            Livery.icon?.SetAssetBundle(Parent.BundleName);

            // Create prefabs for cargo models
            //var modelSetupScripts = exportInfo.Livery.gameObject.GetComponents<CargoModelSetup>();

            //if (modelSetupScripts.Any())
            //{
            //	if (Progress("Generating Cargo Prefabs...", 0.20f)) return;
            //}

            //foreach (var modelSetup in modelSetupScripts)
            //{
            //	if (modelSetup.Model && !PrefabUtility.IsPartOfPrefabAsset(modelSetup.Model))
            //	{
            //		modelSetup.ValidateColliders();

            //		Debug.Log($"Creating temp prefab for cargo model {modelSetup}");

            //		string tempModelPath = Path.Combine(exportInfo.AssetFolder, GetCargoModelPrefabName(exportInfo.Livery.Identifier, modelSetup))
            //			.Replace('\\', '/');

            //		exportInfo.OriginalCargoModels.Add(new OriginalCargoModelInfo(modelSetup, modelSetup.Model, tempModelPath));

            //		var tempModelPrefab = PrefabUtility.SaveAsPrefabAsset(modelSetup.Model, tempModelPath);
            //		modelSetup.Model = tempModelPrefab;

            //		AssetImporter.GetAtPath(tempModelPath).SetAssetBundleNameAndVariant(exportInfo.BundleName, "");
            //	}
            //}

            //Change name of asset bundle on the temp prefab
            if (!Livery.prefab)
            {
                Debug.LogError($"Missing prefab for livery {Livery.name}, abandoning build");
                return false;
            }

            Livery.prefab.SetAssetBundle(Parent.BundleName);
            Livery.SetAssetBundle(Parent.BundleName);

            return true;
        }
    }
}