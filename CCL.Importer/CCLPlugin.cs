using BepInEx;
using HarmonyLib;
using System.Collections;
using System.Reflection;

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

    [BepInPlugin(CCLPluginInfo.Guid, CCLPluginInfo.Name, CCLPluginInfo.Version)]
    public class CCLPlugin : BaseUnityPlugin
    {
        private static CCLPlugin? _instance;
        public static CCLPlugin Instance => _instance!;
        public string Path = null!;


        private string? _carFolderPath;
        public string CarFolderPath => 
            Extensions.GetCached(ref _carFolderPath, 
                () => System.IO.Path.Combine(Paths.BepInExRootPath, CCLPluginInfo.ContentFolderName, CCLPluginInfo.CarFolderName));

        public void Awake()
        {
            _instance = this;
            Path = System.IO.Path.GetDirectoryName(Info.Location);

            var harmony = new Harmony(CCLPluginInfo.Guid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void Log(string message)
        {
            Instance.Logger.LogInfo(message);
        }

        public static void LogVerbose(string message)
        {
            Instance.Logger.LogDebug(message);
        }

        public static void Error(string message)
        {
            Instance.Logger.LogError(message);
        }

        public static void Warning(string message)
        {
            Instance.Logger.LogWarning(message);
        }
    }
}
