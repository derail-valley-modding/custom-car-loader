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
                                            TrainCarType = (TrainCarType)jsonFile["carType"].i
                                        };

                                        //Bogies
                                        newCar.FrontBogiePosition = JSONTemplates.ToVector3(jsonFile["frontBogiePos"]);
                                        newCar.RearBogiePosition = JSONTemplates.ToVector3(jsonFile["rearBogiePos"]);

                                        // Custom Front Bogie
                                        if( jsonFile.GetField(out string customFrontBogieName, "frontBogieReplacement", null) )
                                        {
                                            newCar.FrontBogieReplacement = customFrontBogieName;

                                            if( jsonFile.GetField("frontBogieParams") is JSONObject fbs )
                                            {
                                                newCar.FrontBogieConfig = CustomBogieParams.FromJSON(fbs);
                                            }
                                        }

                                        // Custom Rear Bogie
                                        if( jsonFile.GetField(out string customRearBogieName, "rearBogieReplacement", null) )
                                        {
                                            newCar.RearBogieReplacement = customRearBogieName;

                                            if( jsonFile.GetField("rearBogieParams") is JSONObject rbs )
                                            {
                                                newCar.RearBogieConfig = CustomBogieParams.FromJSON(rbs);
                                            }
                                        }

                                        //Couplers
                                        newCar.FrontCouplerPosition = JSONTemplates.ToVector3(jsonFile["frontCouplerPos"]);
                                        newCar.RearCouplerPosition = JSONTemplates.ToVector3(jsonFile["rearCouplerPos"]);

                                        //Chains
                                        newCar.FrontChainPosition =JSONTemplates.ToVector3(jsonFile["frontChainPos"]);
                                        newCar.RearChainPosition = JSONTemplates.ToVector3(jsonFile["rearChainPos"]);

                                        //Hoses
                                        newCar.FrontHosePosition = JSONTemplates.ToVector3(jsonFile["frontHosePos"]);
                                        newCar.RearHosePosition =JSONTemplates.ToVector3(jsonFile["rearHosePos"]);

                                        //Buffers
                                        newCar.FrontBufferPosition = JSONTemplates.ToVector3(jsonFile["frontBufferPos"]);
                                        newCar.RearBufferPosition = JSONTemplates.ToVector3(jsonFile["rearBufferPos"]);

                                        //Name Plates
                                        newCar.SidePlate1Position = JSONTemplates.ToVector3(jsonFile["sidePlate1Pos"]);
                                        newCar.SidePlate2Position = JSONTemplates.ToVector3(jsonFile["sidePlate2Pos"]);

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