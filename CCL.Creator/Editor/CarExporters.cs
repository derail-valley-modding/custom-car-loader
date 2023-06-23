using UnityEditor;
using UnityEngine;
using DV.ThingTypes;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CCL.Types;

namespace CCL.Creator
{
    internal class CarTypeExporter
    {
        public readonly CustomCarType CarType;
        public readonly string ExportFolderPath;
        public readonly string BundleName;

        public readonly CarLiveryExporter[] LiveryExporters;

        public CarTypeExporter(string exportFolderPath, CustomCarType carType)
        {
            CarType = carType;
            ExportFolderPath = exportFolderPath;
            BundleName = $"{carType.id.Replace(' ', '_')}_bundle";

            LiveryExporters = carType.liveries.Select(l => new CarLiveryExporter(this, l)).ToArray();
        }

        private static bool Progress(string status, float percent)
        {
            return EditorUtility.DisplayCancelableProgressBar("Exporting Car", status, percent);
        }

        public void Export()
        {
            CarType.ExporterVersion = ExporterConstants.ExporterVersion;
            int liveryCount = LiveryExporters.Length;

            try
            {
                // Prep
                for (int i = 0; i < liveryCount; i++)
                {
                    if (Progress($"Preparing livery \"{LiveryExporters[i].Livery.id}\" for export...", 0.05f + 0.50f * (i / liveryCount))) return;

                    LiveryExporters[i].PrepareForExport();
                }

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
                Debug.Log($"Finished AssetBundle build for car: {CarType.id}.");

                // Create car.json file.
                if (Progress("Writing car properties...", 0.80f)) return;
                WriteCarSettingsFile();
            }
            catch (Exception ex)
            {
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
            var jsonfile = new JObject
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
    }

    internal class CarLiveryExporter
	{
		public readonly CarTypeExporter Parent;
		public readonly TrainCarLivery Livery;

		public CarLiveryExporter(CarTypeExporter parent, TrainCarLivery livery)
		{
			Parent = parent;
			Livery = livery;
		}

        public bool PrepareForExport()
        {
            Debug.Log($"Creating temp prefab from car object {Livery.prefab.name}");

            if (Livery.interiorPrefab)
            {
                string interiorPath = AssetDatabase.GetAssetPath(Livery.interiorPrefab);
                AssetImporter.GetAtPath(interiorPath).SetAssetBundleNameAndVariant(Parent.BundleName, "");
            }

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
            string prefabPath = AssetDatabase.GetAssetPath(Livery.prefab);
            AssetImporter.GetAtPath(prefabPath).SetAssetBundleNameAndVariant(Parent.BundleName, "");

            string liveryPath = AssetDatabase.GetAssetPath(Livery);
            AssetImporter.GetAtPath(liveryPath).SetAssetBundleNameAndVariant(Parent.BundleName, "");

            return true;
        }
	}
}