using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CCL_GameScripts;

namespace DVCustomCarLoader
{
    public static class CustomCarManager
    {
        public static List<CustomCar> CustomCarTypes = new List<CustomCar>();
        
        public static void Setup()
        {
            //Load all json files
            string bundlePath = Path.Combine(Main.ModEntry.Path, "Cars");
            
            Main.ModEntry.Logger.Log($"Mod path: {bundlePath}");
            
            if (!Directory.Exists(bundlePath))
                Directory.CreateDirectory(bundlePath);

            var Folders = Directory.GetDirectories(bundlePath);

            Main.ModEntry.Logger.Log($"Folder count: {Folders.Length}");
            
            foreach (var directory in Folders)
            {
                Main.ModEntry.Logger.Log($"Reading directory: {directory}");
                
                foreach (var file in Directory.GetFiles(directory, "*.json"))
                {
                    Main.ModEntry.Logger.Log($"Reading JSON file: {file}");
                    
                    using (StreamReader reader = File.OpenText(file))
                    {
                        var jsonText = reader.ReadToEnd();

                        //Load JSON
                        JSONObject jsonFile = new JSONObject(jsonText);

#if DEBUG
                        //Print JSON file for debug.
                        Main.ModEntry.Logger.Log($"{jsonFile.ToString(true)}");
#endif

                        if (jsonFile.keys.Count>0)
                        {
                            CustomCar newCar = CreateCustomCar(directory, jsonFile);

                            if( newCar != null )
                            {
                                CustomCarTypes.Add(newCar);
                                Main.ModEntry.Logger.Log($"Successfully added new car to spawn list: {newCar.identifier}");
                            }
                            else
                            {
                                Main.ModEntry.Logger.Error($"Failed to load custom car from {directory}");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static CustomCar CreateCustomCar( string directory, JSONObject jsonFile )
        {
            try
            {
                var assetBundleName = jsonFile[CarJSONKeys.BUNDLE_NAME].str;
                var assetBundlePath = Path.Combine(directory, assetBundleName);

                if( File.Exists(assetBundlePath) )
                {
                    Main.ModEntry.Logger.Log(
                        $"Loading AssetBundle: {assetBundleName} at path {assetBundlePath}");

                    //Try to load asset bundle.
                    var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

                    if( assetBundle == null )
                    {
                        Debug.Log($"Failed to load AssetBundle: {assetBundleName}");
                        return null;
                    }

                    Main.ModEntry.Logger.Log($"Successfully loaded asset bundle. Bundle name is {assetBundleName}");

                    //Try to get car prefab from asset bundle
                    var prefabName = jsonFile[CarJSONKeys.PREFAB_NAME].str;

                    GameObject carPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

                    //Unload assetbundle to free up memory.
                    assetBundle.Unload(false);

                    if( carPrefab != null )
                    {
                        Main.ModEntry.Logger.Log($"Successfully loaded prefab from asset bundle. Prefab name is {carPrefab.name}");

                        var newCar = new CustomCar()
                        {
                            CarPrefab = carPrefab,
                            identifier = jsonFile[CarJSONKeys.IDENTIFIER].str,
                            BaseCarType = (TrainCarType)jsonFile[CarJSONKeys.CAR_TYPE].i,
                        };

                        //Bogies
                        // Custom Front Bogie
                        jsonFile.GetField(out newCar.HasCustomFrontBogie, CarJSONKeys.REPLACE_FRONT_BOGIE, false);
                        if( newCar.HasCustomFrontBogie )
                        {
                            if( jsonFile.GetField(CarJSONKeys.FRONT_BOGIE_PARAMS) is JSONObject fbs )
                            {
                                newCar.FrontBogieConfig = CustomBogieParams.FromJSON(fbs);
                            }
                        }

                        // Custom Rear Bogie
                        jsonFile.GetField(out newCar.HasCustomRearBogie, CarJSONKeys.REPLACE_REAR_BOGIE, false);
                        if( newCar.HasCustomRearBogie )
                        {
                            if( jsonFile.GetField(CarJSONKeys.REAR_BOGIE_PARAMS) is JSONObject rbs )
                            {
                                newCar.RearBogieConfig = CustomBogieParams.FromJSON(rbs);
                            }
                        }

                        if( newCar.FinalizePrefab() )
                        {
                            if( CarTypeInjector.RegisterCustomCarType(newCar) != TrainCarType.NotSet )
                            {
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
            catch( Exception e )
            {
                Main.ModEntry.Logger.Error(e.ToString());
            }

            return null;
        }
    }
}