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

		public static CCLSettings Settings { get; private set; }

		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			Enabled = modEntry.Enabled;
			ModEntry = modEntry;

			Settings = UnityModManager.ModSettings.Load<CCLSettings>(ModEntry);
			ModEntry.OnGUI = DrawGUI;
			ModEntry.OnSaveGUI = SaveGUI;

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

				DebugCommands.RegisterCommands();
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

		static void DrawGUI(UnityModManager.ModEntry entry)
		{
			Settings.Draw(entry);
		}

		static void SaveGUI(UnityModManager.ModEntry entry)
		{
			Settings.Save(entry);
		}

		#region Logging

		public static void LogVerbose(string msg)
		{
			if (Settings.VerboseMode)
			{
				ModEntry.Logger.Log(msg);
			}
		}

		public static void LogAlways(string msg) => ModEntry.Logger.Log(msg);

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
