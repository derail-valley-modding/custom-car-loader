using System;
using System.Reflection;
using DVCustomCarLoader.LocoComponents;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace DVCustomCarLoader
{
	public static class Main
	{
		public static UnityModManager.ModEntry ModEntry;
		public static bool Enabled;

		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			Enabled = modEntry.Enabled;
			ModEntry = modEntry;

			Harmony harmony = null;

			try
			{
				//Create sky manager instance.
				ModEntry.Logger.Log("Creating CustomCarManager");

				Application.quitting += AppQuitWatcher.OnAppQuit;

				InitSpecManager.OnStartup();
				Effects.ParticleInitializer.FetchDefaults();
				CustomCarManager.Setup();

				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());
				LocoLights_Patch.TryCreatePatch(harmony);
			}
			catch( Exception ex )
            {
				if( harmony != null )
                {
					harmony.UnpatchAll();
                }

				modEntry.Logger.LogException("Failed to load CustomCarLoader:", ex);

				return false;
            }

			return true;
		}

		private static void OnCarChanged( TrainCar newCar )
        {
#if DEBUG
			if( newCar && CarTypeInjector.IsCustomTypeRegistered(newCar.carType) )
            {
				// diesel autostart
				var locoController = newCar.gameObject.GetComponent<CustomLocoControllerDiesel>();
				if( locoController && !locoController.EngineRunning )
                {
					locoController.EngineRunning = true;
                }
            }
#endif
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
