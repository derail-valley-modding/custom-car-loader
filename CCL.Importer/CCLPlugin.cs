using UnityModManagerNet;
using HarmonyLib;
using System.Reflection;
using DVLangHelper.Runtime;
using System.Linq;
using DV.Utils;

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

            // Build caches before any car is loaded, to only get vanilla resources.
            Processing.GrabberProcessor.BuildAllCaches();

            CarManager.ScanLoadedMods();
            UnityModManager.toggleModsListen += CarManager.HandleModToggled;

            var harmony = new Harmony(CCLPluginInfo.Guid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            InfoDump();

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

        private static void InfoDump()
        {
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.cargos.OrderBy(x => x.v1).Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.generalLicenses.OrderBy(x => x.v1).Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.jobLicenses.OrderBy(x => x.v1).Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.garages.OrderBy(x => x.v1).Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.resources.OrderBy(x => x.v1).Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.Liveries.OrderBy(x => x.v1).Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.carTypes.Select(x => x.id))}\"");
            //Log($"\"{string.Join("\",\n\"", DV.Globals.G.Types.CarKinds.Select(x => x.id))}\"");
            //Log($"{string.Join("\",\n\"", DV.Globals.G.Types.CargoToLoadableCarTypes.Select(x => $"{x.Key.id}: {string.Join(", ", x.Value.Select(y => y.id))}"))}");
            //Log($"{string.Join("\n", DV.Globals.G.Types.carTypes.OrderBy(car => car.id).Select(car => $"- {car.id}:\n\"{string.Join("\",\n\"", DV.Globals.G.Types.cargos.Where(cargo => cargo.loadableCarTypes.Any(l => l.carType.id == car.id)).Select(cargo => cargo.id).OrderBy(cargo => cargo)) }\""))}");
        }
    }
}
