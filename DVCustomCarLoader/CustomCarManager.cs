using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using CCL_GameScripts;

namespace DVCustomCarLoader
{
    public static class CustomCarManager
    {
        public static List<CustomCar> CustomCarTypes = new List<CustomCar>();

        public static IEnumerable<KeyValuePair<TrainCarType, string>> GetCustomCarList()
        {
            return CustomCarTypes.Select(car => new KeyValuePair<TrainCarType, string>(car.CarType, car.identifier));
        }

        public static void Setup()
        {
            //Load all json files
            string bundlePath = Path.Combine(Main.ModEntry.Path, "Cars");
            
            Main.LogVerbose($"Mod path: {bundlePath}");
            
            if (!Directory.Exists(bundlePath))
                Directory.CreateDirectory(bundlePath);

            var Folders = Directory.GetDirectories(bundlePath);

            Main.LogVerbose($"Folder count: {Folders.Length}");
            
            foreach (var directory in Folders)
            {
                Main.LogVerbose($"Reading directory: {directory}");

                string file = Path.Combine(directory, CarJSONKeys.JSON_FILENAME);
                if (File.Exists(file))
                {
                    Main.LogVerbose($"Reading JSON file: {file}");

                    try
                    {
                        using (StreamReader reader = File.OpenText(file))
                        {
                            var jsonText = reader.ReadToEnd();

                            //Load JSON
                            JSONObject jsonFile = new JSONObject(jsonText);

                            if (jsonFile.keys.Count > 0)
                            {
                                CustomCar newCar = CreateCustomCar(directory, jsonFile);

                                if (newCar != null)
                                {
                                    CustomCarTypes.Add(newCar);
                                    Main.LogAlways($"Successfully added new car to spawn list: {newCar.identifier}");
                                }
                                else
                                {
                                    Main.Error($"Failed to load custom car from {directory}");
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.Error($"Error loading file {file}:\n{ex.Message}");
                    }
                }
            }
        }

        private static CustomCar CreateCustomCar(string directory, JSONObject jsonFile)
        {
            try
            {
                var assetBundleName = jsonFile[CarJSONKeys.BUNDLE_NAME].str;
                var assetBundlePath = Path.Combine(directory, assetBundleName);

                if (File.Exists(assetBundlePath))
                {
                    Main.LogVerbose($"Loading AssetBundle: {assetBundleName} at path {assetBundlePath}");

                    //Try to load asset bundle.
                    var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

                    if (assetBundle == null)
                    {
                        Main.Warning($"Failed to load AssetBundle: {assetBundleName}");
                        return null;
                    }

                    Main.LogVerbose($"Successfully loaded asset bundle. Bundle name is {assetBundleName}");

                    //Try to get car prefab from asset bundle
                    var prefabName = jsonFile[CarJSONKeys.PREFAB_NAME].str;

                    GameObject carPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

                    //Unload assetbundle to free up memory.
                    assetBundle.Unload(false);

                    if (carPrefab != null)
                    {
                        Main.LogVerbose($"Successfully loaded prefab from asset bundle. Prefab name is {carPrefab.name}");

                        jsonFile.GetField(out string version, CarJSONKeys.EXPORTER_VERSION, "1.5");

                        var newCar = new CustomCar()
                        {
                            CarPrefab = carPrefab,
                            identifier = jsonFile[CarJSONKeys.IDENTIFIER].str,
                            BaseCarType = (TrainCarType)jsonFile[CarJSONKeys.CAR_TYPE].i,
                            ExporterVersion = new Version(version),
                        };

                        //Bogies
                        // Custom Front Bogie
                        if (jsonFile.GetField(CarJSONKeys.FRONT_BOGIE_PARAMS) is JSONObject fbs)
                        {
                            newCar.FrontBogieConfig = CustomBogieParams.FromJSON(fbs);
                        }

                        // Custom Rear Bogie
                        if (jsonFile.GetField(CarJSONKeys.REAR_BOGIE_PARAMS) is JSONObject rbs)
                        {
                            newCar.RearBogieConfig = CustomBogieParams.FromJSON(rbs);
                        }

                        if (newCar.FinalizePrefab())
                        {
                            if (CarTypeInjector.RegisterCustomCarType(newCar) != TrainCarType.NotSet)
                            {
                                CargoModelInjector.RegisterCargo(newCar);

                                return newCar;
                            }
                            else
                            {
                                // TODO: Destroy failed car?
                                return null;
                            }
                        }
                        else return null;
                    }
                }
            }
            catch (Exception e)
            {
                Main.Error(e.ToString());
            }

            return null;
        }
    }
}