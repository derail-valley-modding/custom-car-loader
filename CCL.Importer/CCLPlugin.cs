using UnityModManagerNet;
using HarmonyLib;
using System.Collections;
using System.IO;
using System.Reflection;
using DVLangHelper.Runtime;

namespace CCL.Importer
{
    public static class CCLPluginInfo
    {
        public const string Guid = "cc.foxden.customcarloader";
        public const string Name = "Custom Car Loader";
        public const string Version = "2.0.0";

        public const string ContentFolderName = "content";
        public const string CarFolderName = "cars";
    }

    public static class CCLPlugin
    {
        public static UnityModManager.ModEntry Instance = null!;
        public static bool Enabled => Instance.Active;
        public static string Path = null!;

        public static TranslationInjector Translations { get; private set; } = null!;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;

            Translations = new TranslationInjector(CCLPluginInfo.Guid);

            CarManager.ScanLoadedMods();
            UnityModManager.toggleModsListen += CarManager.HandleModToggled;

            var harmony = new Harmony(CCLPluginInfo.Guid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }

        public static void Log(string message)
        {
            Instance.Logger.Log(message);
        }

        public static void LogVerbose(string message)
        {
            Instance.Logger.Log(message);
        }

        public static void Error(string message)
        {
            Instance.Logger.Error(message);
        }

        public static void Warning(string message)
        {
            Instance.Logger.Warning(message);
        }
    }
}
