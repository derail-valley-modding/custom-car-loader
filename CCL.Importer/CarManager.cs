using CCL.Types;
using DV.JObjectExtstensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CCL.Importer
{
    public static class CarManager
    {
        public static readonly List<CustomCarType> CustomCarTypes = new List<CustomCarType>();

        [AfterStartup]
        public static void LoadCarDefinitions()
        {
            CCLPlugin.Log($"Loading cars from {CCLPlugin.Instance.CarFolderPath}");

            foreach (var carFolder in Directory.GetDirectories(CCLPlugin.Instance.CarFolderPath))
            {
                string jsonPath = Path.Combine(carFolder, ExporterConstants.JSON_FILENAME);
                if (!File.Exists(jsonPath)) continue;

                JObject json;
                try
                {
                    using StreamReader reader = File.OpenText(jsonPath);
                    json = JObject.Parse(reader.ReadToEnd());
                }
                catch (Exception ex)
                {
                    CCLPlugin.Error($"Error loading file {jsonPath}:\n{ex.Message}");
                    continue;
                }

                var carType = LoadCarDefinition(carFolder, json);
                if (carType != null)
                {
                    CustomCarTypes.Add(carType);
                }
            }

            CarTypeInjector.ForceObjectModelUpdate();
        }

        private static CustomCarType? LoadCarDefinition(string directory, JObject jsonFile)
        {
            try
            {
                string carId = jsonFile.GetString(ExporterConstants.IDENTIFIER);
                string versionStr = jsonFile.GetValueOrDefault(ExporterConstants.EXPORTER_VERSION, "1.5");
                var version = new Version(versionStr);

                // make sure car is compatible with current importer
                if (version > ExporterConstants.ExporterVersion)
                {
                    CCLPlugin.Error($"Car {carId} was built with a newer version of CCL: Current Version = {ExporterConstants.ExporterVersion}, Car Version = {version}");
                    return null;
                }
                else if (version < ExporterConstants.MinimumCompatibleVersion)
                {
                    CCLPlugin.Error($"Car {carId} was built with an incompatible version of CCL: Minimum Version = {ExporterConstants.MinimumCompatibleVersion}, Car Version = {version}");
                    return null;
                }

                // Load asset bundle to memory
                var assetBundleName = jsonFile.GetString(ExporterConstants.BUNDLE_NAME);
                var assetBundlePath = Path.Combine(directory, assetBundleName);

                if (!File.Exists(assetBundlePath))
                {
                    CCLPlugin.Error($"AssetBundle for car {carId} is missing");
                    return null;
                }

                CCLPlugin.Log($"Loading AssetBundle: {assetBundleName} at path {assetBundlePath}");
                var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

                if (assetBundle == null)
                {
                    CCLPlugin.Error($"Failed to load AssetBundle: {assetBundleName}");
                    return null;
                }

                // Fetch car types from bundle
                var carType = assetBundle.LoadAllAssets<CustomCarType>().SingleOrDefault();
                if (carType == null)
                {
                    CCLPlugin.Error($"Could not load any car types from AssetBundle {assetBundlePath}");
                    return null;
                }

                carType.AfterAssetLoad();
                CarTypeInjector.SetupTypeLinks(carType);

                // Finalize model & inject
                if (!PrefabWrangler.FinalizeCarTypePrefabs(carType))
                {
                    CCLPlugin.Error($"Failed to wrangle prefab for {carId}");
                    return null;
                }

                if (!CarTypeInjector.RegisterCarType(carType))
                {
                    CCLPlugin.Error($"Failed to register car type for {carId}");
                    return null;
                }

                CCLPlugin.Log($"Successfully loaded car type {carId}");

                return carType;
            }
            catch (Exception ex)
            {
                CCLPlugin.Error($"Error loading car from {directory}:\n{ex}");
                return null;
            }
        }
    }
}
