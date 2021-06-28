using System;
using System.Collections.Generic;
using DV;
using HarmonyLib;

namespace DVCustomCarLoader
{
    [HarmonyPatch(typeof(CommsRadioCarSpawner), "Awake")]
    public class CommsRadioCarSpawner_Awake_Patch
    {
        static void Postfix(CommsRadioCarSpawner __instance)
        {
            if (!Main.Enabled)
                return;
            
            try
            {
                //Add trains to car spawner
                __instance.UpdateCarTypesToSpawn(true);

            }
            catch(Exception e)
            {
                Main.ModEntry.Logger.Error(e.ToString());
            }
        }  
    }
}