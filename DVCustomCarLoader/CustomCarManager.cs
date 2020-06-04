using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DVCustomCarLoader
{
    public class CustomCarManager : MonoBehaviour
    {
        public List<CustomCar> CustomCarsToSpawn;
        
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

                                    if (carPrefab != null)
                                    {

                                        Main.ModEntry.Logger.Log(
                                            $"Successfully loaded prefab from asset bundle. Prefab name is {carPrefab.name}");

                                        var id = jsonFile["identifier"].str;
                                        var carType = (TrainCarType)jsonFile["carType"].i;
                                        //Bogies
                                        var frontBogiePos = JSONTemplates.ToVector3(jsonFile["frontBogiePos"]);
                                        var rearBogiePos = JSONTemplates.ToVector3(jsonFile["rearBogiePos"]);
                                        //Couplers
                                        var frontCouplerPos = JSONTemplates.ToVector3(jsonFile["frontCouplerPos"]);
                                        var rearCouplerPos = JSONTemplates.ToVector3(jsonFile["rearCouplerPos"]);
                                        //Chains
                                        var frontChainPos =JSONTemplates.ToVector3(jsonFile["frontChainPos"]);
                                        var rearChainPos = JSONTemplates.ToVector3(jsonFile["rearChainPos"]);
                                        //Hoses
                                        var frontHosePos = JSONTemplates.ToVector3(jsonFile["frontHosePos"]);
                                        var rearHosePos =JSONTemplates.ToVector3(jsonFile["rearHosePos"]);
                                        //Buffers
                                        var frontBufferPos = JSONTemplates.ToVector3(jsonFile["frontBufferPos"]);
                                        var rearBufferPos = JSONTemplates.ToVector3(jsonFile["rearBufferPos"]);
                                        //Name Plates
                                        var sidePlate1Pos = JSONTemplates.ToVector3(jsonFile["sidePlate1Pos"]);
                                        var sidePlate2Pos = JSONTemplates.ToVector3(jsonFile["sidePlate2Pos"]);

                                        CustomCar newCar = new CustomCar()
                                        {
                                            CarPrefab = carPrefab,
                                            identifier = id,
                                            TrainCarType = carType,
                                            //Bogies
                                            FrontBogiePosition = frontBogiePos,
                                            RearBogiePosition = rearBogiePos,
                                            //Couplers
                                            FrontCouplerPosition = frontCouplerPos,
                                            RearCouplerPosition = rearCouplerPos,
                                            //Chains
                                            FrontChainPosition = frontChainPos,
                                            RearChainPosition = rearChainPos,
                                            //Hoses
                                            FrontHosePosition = frontHosePos,
                                            RearHosePosition = rearHosePos,
                                            //Buffers
                                            FrontBufferPosition = frontBufferPos,
                                            RearBufferPosition = rearBufferPos,
                                            //Name Plates
                                            SidePlate1Position = sidePlate1Pos,
                                            SidePlate2Position = sidePlate2Pos
                                        };

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