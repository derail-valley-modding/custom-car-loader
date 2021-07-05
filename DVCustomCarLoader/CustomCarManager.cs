using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DVCustomCarLoader
{
    public class CustomCarManager : MonoBehaviour
    {
        public List<CustomCar> CustomCarsToSpawn;

        private Dictionary<TrainCar, string> SpawnedCustomCarIds = new Dictionary<TrainCar, string>();

        public bool TryGetCustomCarId( TrainCar trainCar, out string id )
        {
            return SpawnedCustomCarIds.TryGetValue(trainCar, out id);
        }

        public void RegisterSpawnedCar( TrainCar car, string identifier )
        {
            SpawnedCustomCarIds[car] = identifier;
        }

        public void DeregisterCar( TrainCar car )
        {
            SpawnedCustomCarIds.Remove(car);
        }
        
        public void Setup()
        {
            
            CustomCarsToSpawn = new List<CustomCar>();
            
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
                        
                        //Print JSON file for debug.
                        Main.ModEntry.Logger.Log($"{jsonFile.ToString(true)}");
                        
                        if (jsonFile.keys.Count>0)
                        {
                            try
                            {
                                var assetBundleName = jsonFile["assetBundleName"].str;
                                var assetBundlePath = Path.Combine(directory, assetBundleName);

                                if (File.Exists(assetBundlePath))
                                {
                                    Main.ModEntry.Logger.Log(
                                        $"Loading AssetBundle: {assetBundleName} at path {assetBundlePath}");

                                    //Try to load asset bundle.
                                    var assetBundle = AssetBundle.LoadFromFile(assetBundlePath);

                                    if (assetBundle == null)
                                    {
                                        Debug.Log($"Failed to load AssetBundle: {assetBundleName}");
                                        break;
                                    }

                                    Main.ModEntry.Logger.Log(
                                        $"Successfully loaded asset bundle. Bundle name is {assetBundleName}");

                                    //Try to get car prefab from asset bundle
                                    var prefabName = jsonFile["carPrefabName"].str;

                                    GameObject carPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

                                    //Unload assetbundle to free up memory.
                                    assetBundle.Unload(false);

                                    if( carPrefab != null )
                                    {

                                        Main.ModEntry.Logger.Log(
                                            $"Successfully loaded prefab from asset bundle. Prefab name is {carPrefab.name}");

                                        var newCar = new CustomCar()
                                        {
                                            CarPrefab = carPrefab,
                                            identifier = jsonFile["identifier"].str,
                                            BaseCarType = (TrainCarType)jsonFile["carType"].i
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

                                        newCar.FinalizePrefab();

                                        CustomCarsToSpawn.Add(newCar);

                                        Main.ModEntry.Logger.Log(
                                            $"Successfully added new car to spawn list: {newCar.identifier}");
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                Main.ModEntry.Logger.Error(e.ToString());
                                break;
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

        /*
        public void Setup()
        {
            CustomCarsToSpawn = new List<CustomCar>();
            
            var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.transform.localScale = new Vector3(3.17f, 5.71f, 28.71f);
            primitive.SetActive(false);
            DontDestroyOnLoad(primitive);
            
            CustomCarsToSpawn.Add(new CustomCar
            {
                identifier = "Autorack_Concept",
                CarPrefab = primitive,
                TrainCarType = TrainCarType.FlatbedEmpty,
                FrontCouplerPosition = new Vector3(0.0f, 1.05f, 14.324f),
                RearCouplerPosition = new Vector3(0.0f, 1.05f, -14.324f),
                FrontBogiePosition = new Vector3(0.0f, 0.0f, 10.493f),
                RearBogiePosition = new Vector3(0.0f, 0.0f, -10.493f)
            });
        }*/
    }
}