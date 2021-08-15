using System;
using System.Reflection;
using DVCustomCarLoader.LocoComponents;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace DVCustomCarLoader
{
	public static class Main
	{
		public static CommsRadioCustomCarManager CommsRadioCustomCarManager;
		public static UnityModManager.ModEntry ModEntry;
		public static bool Enabled;

		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			LocoLights_Patch.TryCreatePatch(harmony);

			Enabled = modEntry.Enabled;
			ModEntry = modEntry;
			
			//Create sky manager instance.
			ModEntry.Logger.Log("Creating CustomCarManager");

			Application.quitting += AppQuitWatcher.OnAppQuit;
			CustomCarManager.Setup();

			PlayerManager.CarChanged += OnCarChanged;

			return true;
		}

		private static void OnCarChanged( TrainCar newCar )
        {
			if( newCar && CustomCarManager.IsRegisteredCustomCar(newCar) )
            {
				// diesel autostart
				var locoController = newCar.gameObject.GetComponent<CustomLocoControllerDiesel>();
				if( locoController && !locoController.EngineRunning )
                {
					locoController.EngineRunning = true;
                }
            }
        }

		#region Logging

		public static void Log( string msg ) => ModEntry.Logger.Log(msg);
		public static void Warning( string msg ) => ModEntry.Logger.Warning(msg);
		public static void Error( string msg ) => ModEntry.Logger.Error(msg);

        #endregion
    }

    internal static class AppQuitWatcher
    {
		public static bool isQuitting { get; private set; } = false;

		public static void OnAppQuit()
        {
			isQuitting = true;
        }
    }
}
