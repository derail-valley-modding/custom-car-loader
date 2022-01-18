using CommandTerminal;
using DVCustomCarLoader.LocoComponents.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class DebugCommands
    {
        public static void RegisterCommands()
        {
            Terminal.Shell.AddCommand("CCL.ForceSteamUp", ForceSteamUp, 0, 0, "");
            Terminal.Autocomplete.Register("CCL.ForceSteamUp");
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
