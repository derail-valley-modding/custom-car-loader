using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV.JObjectExtstensions;
using HarmonyLib;
using Newtonsoft.Json.Linq;

namespace DVCustomCarLoader
{
    static class SaveConstants
    {
        public const string CUSTOM_CAR_KEY = "customcar";
    }

    [HarmonyPatch(typeof(CarsSaveManager), "GetCarSaveData")]
    public static class CarsSaveManager_GetSaveData_Patch
    {
        public static void Postfix( TrainCar car, ref JObject __result )
        {
            if( Main.CustomCarManagerInstance.TryGetCustomCarId(car, out string id) )
            {
                // custom car detected, save its type
                __result.SetString(SaveConstants.CUSTOM_CAR_KEY, id);
            }
        }
    }

    [HarmonyPatch(typeof(CarsSaveManager), "InstantiateCar")]
    public static class CarsSaveManager_InstantiateCar_Patch
    {
        public static void Postfix( JObject carData, TrainCar __result )
        {
            string customType = carData.GetString(SaveConstants.CUSTOM_CAR_KEY);
            if( customType != null )
            {
                CustomCar match = Main.CustomCarManagerInstance.CustomCarsToSpawn.Find(cc => cc.identifier == customType);
                if( match != null )
                {
                    //match.Spawn(__result);
                    Main.ModEntry.Logger.Warning($"Reverting {match.identifier} to its base type");
                }
                else
                {
                    Main.ModEntry.Logger.Warning("Tried to instantiate a custom car of unknown type, reverting to its base type");
                }
            }
        }
    }
}
