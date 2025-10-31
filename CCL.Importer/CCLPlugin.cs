using CCL.Types;
using DV;
using DVLangHelper.Runtime;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityModManagerNet;

namespace CCL.Importer
{
    public static class CCLPluginInfo
    {
        public const string Guid = "cc.foxden.customcarloader";
        public const string Name = "Custom Car Loader";
        public const string Version = "3.1.2";

        public const string ContentFolderName = "content";
        public const string CarFolderName = "cars";
        public const string TranslationCsvFile = "ccl_translation_data.csv";
        public const string TranslationCsvUrl = "https://github.com/derail-valley-modding/custom-car-loader/blob/master/ccl_translation_data.csv";
    }

    public static class CCLPlugin
    {
        public static UnityModManager.ModEntry Instance = null!;
        public static Settings Settings = null!;
        public static bool Enabled => Instance.Active;
        public static string Path = null!;

        public static TranslationInjector Translations { get; private set; } = null!;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Instance = modEntry;
            Translations = new TranslationInjector(CCLPluginInfo.Guid);
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

            Instance.OnGUI += Settings.Draw;
            Instance.OnSaveGUI += Settings.Save;

            if (!VersionCheck())
            {
                Error($"Game version failure!\nGame: {BuildInfo.BUILDBOT_INFO}\nExpected: {ExporterConstants.MINIMUM_DV_BUILD}");
                return false;
            }

            // Build caches before any car is loaded, to only get vanilla resources.
            Processing.GrabberProcessor.BuildAllCaches();

            // Load default translation data.
            Translations.AddTranslationsFromCsv(System.IO.Path.Combine(modEntry.Path, CCLPluginInfo.TranslationCsvFile));
            Translations.AddTranslationsFromWebCsv(CCLPluginInfo.TranslationCsvUrl);

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
            if (!Settings.UseVerboseLogging) return;

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
            if (!Settings.InfoDump) return;

            Write("Cargo IDs", DV.Globals.G.Types.cargos.OrderBy(x => x.v1).Select(x => x.id));
            Write("General Licence IDs", DV.Globals.G.Types.generalLicenses.OrderBy(x => x.v1).Select(x => x.id));
            Write("Job License IDs", DV.Globals.G.Types.jobLicenses.OrderBy(x => x.v1).Select(x => x.id));
            Write("Garage IDs", DV.Globals.G.Types.garages.OrderBy(x => x.v1).Select(x => x.id));
            Write("Resource IDs", DV.Globals.G.Types.resources.OrderBy(x => x.v1).Select(x => x.id));
            Write("Livery IDs", DV.Globals.G.Types.Liveries.OrderBy(x => x.v1).Select(x => x.id));
            Write("Car Type IDs", DV.Globals.G.Types.carTypes.Select(x => x.id));
            Write("Car Kind IDs", DV.Globals.G.Types.CarKinds.Select(x => x.id));

            WriteNoDetail("Cargo to Car",
                DV.Globals.G.Types.CargoToLoadableCarTypes.Select(x => $"{x.Key.id}: {string.Join(", ", x.Value.Select(y => y.id))}"));
            WriteNoDetail("Car to Cargo",
                DV.Globals.G.Types.carTypes.OrderBy(car => car.id).Select(car =>
                    $"{car.id}:\n\"{string.Join("\",    \n\"", DV.Globals.G.Types.cargos.Where(cargo => cargo.loadableCarTypes.Any(l => l.carType.id == car.id)).Select(cargo => cargo.id).OrderBy(cargo => cargo))}\""));
            WriteNoDetail("Cargo ID to Mass", DV.Globals.G.Types.cargos.OrderBy(x => x.v1).Select(x => $"{{ \"{x.id}\", {x.massPerUnit} }}" ));

            static void Write(string title, IEnumerable<string> values)
            {
                Log($"{title}:\n\"{string.Join("\",\n\"", values)}\"");
            }

            static void WriteNoDetail(string title, IEnumerable<string> values)
            {
                Log($"{title}:\n{string.Join("\n", values)}");
            }
        }

        private static bool VersionCheck() => int.Parse(BuildInfo.BUILDBOT_INFO.Substring(5)) >= ExporterConstants.BUILD_INT;
    }
}
