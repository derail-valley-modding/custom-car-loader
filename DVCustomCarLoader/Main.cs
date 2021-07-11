using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace DVCustomCarLoader
{
	public static class Main
	{
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			Main.Enabled = modEntry.Enabled;
			Main.ModEntry = modEntry;
			
			//Create sky manager instance.
			ModEntry.Logger.Log("Creating CustomCarManager");

			Application.quitting += AppQuitWatcher.OnAppQuit;

			var nsmgr = new GameObject("[CustomCarManagerInstance]");
			Object.DontDestroyOnLoad(nsmgr);
			nsmgr.transform.SetSiblingIndex(0);
			CustomCarManagerInstance = nsmgr.AddComponent<CustomCarManager>();
			CustomCarManagerInstance.Setup();

			return true;
		}

		public static CustomCarManager CustomCarManagerInstance;
		public static CommsRadioCustomCarManager CommsRadioCustomCarManager;
		public static UnityModManager.ModEntry ModEntry;
		public static bool Enabled;

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
