using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVCustomCarLoader.LocoComponents;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader
{
    [HarmonyPatch(typeof(TrainCar))]
    public static class TrainCar_LoadInterior_Patch
    {
        [HarmonyPatch(nameof(TrainCar.LoadInterior))]
        public static void Postfix(TrainCar __instance, GameObject ___loadedInterior)
        {
            if (!___loadedInterior) return;

            if (!___loadedInterior.activeSelf)
            {
                ___loadedInterior.gameObject.SetActive(true);
                Main.Log($"Activating interior on {___loadedInterior.gameObject.name}");

                var eventMgr = __instance.gameObject.GetComponent<LocoEventManager>();
                if (eventMgr)
                {
                    eventMgr.OnInteriorLoaded(___loadedInterior);
                }
            }
        }

        [HarmonyPatch(nameof(TrainCar.UnloadInterior))]
        public static void Prefix(TrainCar __instance)
        {
            var eventMgr = __instance.gameObject.GetComponent<LocoEventManager>();
            if (eventMgr)
            {
                eventMgr.OnInteriorUnloaded();
            }
        }
    }
}
