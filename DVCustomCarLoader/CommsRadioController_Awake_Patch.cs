using System;
using System.Collections.Generic;
using DV;
using Harmony12;

namespace DVCustomCarLoader
{
    /// <summary>
    /// After CommsRadioControler finished it's Awake() call, we add our own comms mode after that.
    /// </summary>
    [HarmonyPatch(typeof(CommsRadioController), "Awake")]
    public class CommsRadioController_Awake_Patch
    {
        static void Postfix(CommsRadioController __instance, ref List<ICommsRadioMode> ___allModes, ref CommsRadioCarSpawner ___carSpawnerControl)
        {
            if (!Main.Enabled)
                return;
            
            try
            {

                if (Main.CustomCarManagerInstance.CustomCarsToSpawn.Count <= 0)
                    return;
                
                //Add our spawner to the comms radio
                var ccm = Main.CommsRadioCustomCarManager = __instance.gameObject.AddComponent<CommsRadioCustomCarManager>();
                
                //We need to get info from car spawner because we don't have these naturally.
                ccm.display = ___carSpawnerControl.display;
                ccm.validMaterial = ___carSpawnerControl.validMaterial;
                ccm.invalidMaterial = ___carSpawnerControl.invalidMaterial;
                ccm.lcdArrow = ___carSpawnerControl.lcdArrow;
                ccm.spawnModeEnterSound = ___carSpawnerControl.spawnModeEnterSound;
                ccm.spawnVehicleSound = ___carSpawnerControl.spawnVehicleSound;
                ccm.confirmSound = ___carSpawnerControl.confirmSound;
                ccm.cancelSound = ___carSpawnerControl.cancelSound;
                ccm.destinationHighlighterGO = ___carSpawnerControl.destinationHighlighterGO;
                ccm.directionArrowsHighlighterGO = ___carSpawnerControl.directionArrowsHighlighterGO;
                
                //setup script after we add references.
                ccm.Setup();
                
                //Add our custom mode to comms radio
                ___allModes.Add(Main.CommsRadioCustomCarManager);
            }
            catch(Exception e)
            {
                Main.ModEntry.Logger.Error(e.ToString());
            }
        }  
    }
}
