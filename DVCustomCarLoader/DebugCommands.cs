using CommandTerminal;
using DVCustomCarLoader.LocoComponents.Steam;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class DebugCommands
    {
        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        public static void RegisterCommands()
        {
            try
            {
                Terminal.Shell.AddCommand("CCL.ForceSteamUp", ForceSteamUp, 0, 0, "");
                Terminal.Autocomplete.Register("CCL.ForceSteamUp");

                Terminal.Shell.AddCommand("CCL.SteamDebug", SteamDebug, 0, 1, "");
                Terminal.Autocomplete.Register("CCL.SteamDebug");

                //MethodInfo registerDevCommands = AccessTools.Method(typeof(DV.Console), "RegisterDevCommands");
                //registerDevCommands.Invoke(null, new object[] { });
            }
            catch (Exception ex)
            {
                Main.Error("Failed to register debug commands: " + ex.ToString());
            }
        }

        public static void SteamDebug(CommandArg[] args)
        {
            TrainCar currentCar = PlayerManager.Car;
            if (currentCar == null)
            {
                Debug.Log("Player is not currently on a car");
                return;
            }

            if (!CarTypes.IsSteamLocomotive(currentCar.carType))
            {
                Debug.Log("Current car is not a steam locomotive");
                return;
            }

            if (CarTypeInjector.IsInCustomRange(currentCar.carType))
            {
                var sim = currentCar.gameObject.GetComponent<CustomLocoSimSteam>();
                if (sim != null)
                {
                    bool on = args.Length == 0 || args[0].Int > 0;

                    var debugGUI = currentCar.gameObject.GetComponent<CustomSteamDebugGUI>();
                    if (!debugGUI)
                    {
                        if (!on) return;
                        debugGUI = currentCar.gameObject.AddComponent<CustomSteamDebugGUI>();
                    }

                    debugGUI.enabled = on;
                }
                else
                {
                    Debug.Log("Custom loco sim not found");
                }
            }
        }

        public static void ForceSteamUp(CommandArg[] args)
        {
            TrainCar currentCar = PlayerManager.Car;
            if (currentCar == null)
            {
                Debug.Log("Player is not currently on a car");
                return;
            }

            if (!CarTypes.IsSteamLocomotive(currentCar.carType))
            {
                Debug.Log("Current car is not a steam locomotive");
                return;
            }

            if (CarTypeInjector.IsInCustomRange(currentCar.carType))
            {
                var sim = currentCar.gameObject.GetComponent<CustomLocoSimSteam>();
                if (sim != null)
                {
                    sim.DebugForceSteamUp();
                }
                else
                {
                    Debug.Log("Custom loco sim not found");
                }
            }
            else
            {
                var sim = currentCar.gameObject.GetComponent<SteamLocoSimulation>();
                if (sim != null)
                {
                    sim.DebugForceSteamCreation();
                }
                else
                {
                    Debug.Log("Custom loco sim not found");
                }
            }
        }
    }
}
