using CCL.Creator.Utility;
using CCL.Types;
using CCL.Types.Json;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CCL.Creator.Wizards
{
    internal class ExportPackWizard : EditorWindow
    {
        private enum RequirementType
        {
            Required,
            Optional
        }

        private class ModDependencyEntry
        {
            public RequirementType Type;
            public string Id;

            public ModDependencyEntry(string id)
            {
                Id = id;
            }
        }

        private static ExportPackWizard? _window;

        private CustomCarPack _pack = null!;
        private bool _openFolderAfterExport = false;
        private ModDependencyEntry[] _requirements = null!;

        private static string LastExportPath
        {
            get => EditorPrefs.GetString("CCL_LastExportPath");
            set => EditorPrefs.SetString("CCL_LastExportPath", value);
        }

        public static void ShowWindow(CustomCarPack pack)
        {
            // Get existing open window or if none, make a new one:
            _window = GetWindow<ExportPackWizard>();
            _window._pack = pack;
            _window._requirements = OtherMods.GetModRequirements(pack).Select(x => new ModDependencyEntry(x)).ToArray();
            _window.titleContent = new GUIContent("CCL - Export Car Pack");

            _window.Show();
        }

        private void OnGUI()
        {
            DrawRequirements();

            EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("Export Pack"))
            {
                if (_pack == null)
                {
                    EditorUtility.DisplayDialog("ERROR", "Car pack is null!", "Ok");
                    Close();
                    return;
                }

                string startingPath;
                string folderName;

                string lastExport = LastExportPath;
                if (!string.IsNullOrEmpty(lastExport) && Directory.Exists(lastExport))
                {
                    startingPath = Path.GetDirectoryName(lastExport);
                    folderName = Path.GetFileName(lastExport.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                }
                else
                {
                    startingPath = SteamHelper.GetModsDirectory() ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    folderName = _pack.PackId;
                }

                string exportFolderPath = EditorUtility.SaveFolderPanel("Export Car", startingPath, folderName);

                if (!string.IsNullOrWhiteSpace(exportFolderPath))
                {
                    LastExportPath = exportFolderPath;
                    PrepareExport(exportFolderPath);

                    //Goto folder when finished building.
                    if (_openFolderAfterExport)
                    {
                        EditorUtility.RevealInFinder(exportFolderPath);
                    }

                    //Close the window when we are done building.
                    Close();
                    return;
                }
            }

            EditorHelpers.WordWrappedLabel(
                "This button will open a window that allows you to select a folder to export your car. " +
                "If a CCL asset bundle already exists, it will be written over.");

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            _openFolderAfterExport = GUILayout.Toggle(_openFolderAfterExport, "Open build folder after export");

            DrawAdditionalInfo();
        }

        private void PrepareExport(string exportFolderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(exportFolderPath);

            // Prompt to clear folder before exporting.
            if (directory.GetFiles().Length > 0 || directory.GetDirectories().Length > 0)
            {
                if (!EditorUtility.DisplayDialog("Clear Folder",
                    "The directory you selected isn't empty, would you like to clear the files from the folder before proceeding?\n\n" +
                    "WARNING: THIS WILL DELETE ALL FILES (BUT NOT DIRECTORIES) IN THE FOLDER.",
                    "Skip",
                    "Clear Folder"))
                {
                    // DANGEROUS METHOD, DON'T USE WITHOUT CONFIDENCE
                    directory.Empty();
                }
            }

            Close();
            Debug.Log($"Exporting asset bundle to: {exportFolderPath}");

            Export(_pack, exportFolderPath, _requirements);
        }

        private void DrawRequirements()
        {
            if (_requirements == null || _requirements.Length <= 1) return;

            EditorGUILayout.BeginVertical("box");
            EditorHelpers.DrawHeader("Additional Requirements");

            for (int i = 1; i < _requirements.Length; i++)
            {
                _requirements[i].Type = EditorHelpers.EnumPopup(_requirements[i].Id, _requirements[i].Type);
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private static void DrawAdditionalInfo()
        {
            EditorGUILayout.BeginVertical("box");

            EditorHelpers.DrawHeader("HOW TO EXPORT");
            EditorHelpers.WordWrappedLabel(
                "• The tool will attempt to find your Derail Valley installation. If the tool can't find your install path, navigate to it manually.\n" +
                "• Once you are in the installation path, navigate to the Mods/ folder.\n" +
                "• When you are in the Mods folder, create a new folder for your car, name doesn't matter (example: Explody Boy Tanker).\n" +
                "• After creating that new folder, make sure to select it and click 'Save'.");

            EditorGUILayout.Space();
            EditorHelpers.DrawHeader("HOW TO AVOID ERRORS");
            EditorHelpers.WordWrappedLabel(
                "• Do not type any special characters in your folder to avoid Windows path problems.\n" +
                "• Do not overwrite files if you don't want something potentially going wrong.\n" +
                "• If any warnings or errors pop up within Unity, do not continue saving your file.");

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private static void Export(CustomCarPack pack, string exportPath, ModDependencyEntry[] requirements)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            pack.ExporterVersion = ExporterConstants.ExporterVersion.ToString();

            try
            {
                for (int i = 0; i < pack.Cars.Length; i++)
                {
                    if (Progress($"Validating car {i + 1}/{pack.Cars.Length}...", (0.5f * i) / pack.Cars.Length)) return;

                    pack.Cars[i].ForceValidation();
                    EditorUtility.SetDirty(pack.Cars[i]);
                    EditorHelpers.SaveAndRefresh();
                }

                if (Progress($"Exporting assets...", 0.5f)) return;
                AssetBundleHelper.CreateBundle(exportPath, new List<(UnityEngine.Object, string?)>() { (pack, null) });

                sw.Stop();
                Debug.Log($"[{DateTime.Now:HH:mm:ss}] Finished AssetBundle build for pack: {pack.PackId} ({sw.Elapsed.TotalSeconds:F2}s).");

                if (Progress("Creating mod info...", 0.9f)) return;

                var required = requirements
                    .Where(x => x.Type == RequirementType.Required && !string.IsNullOrWhiteSpace(x.Id))
                    .Select(x => x.Id).ToArray();
                var optional = requirements
                    .Where(x => x.Type == RequirementType.Optional && !string.IsNullOrWhiteSpace(x.Id))
                    .Select(x => x.Id).ToArray();

                WriteModInfoFile(pack, exportPath, required, optional);
            }
            catch (Exception ex)
            {
                sw.Stop();
                EditorUtility.DisplayDialog("Export Error",
                    "An exception occurred while exporting your pack. Details:\n\n" + ex.Message,
                    "Close Window");
            }
            finally
            {
                Progress("Finished!", 1.0f);
                System.Threading.Thread.Sleep(2000);
                EditorUtility.ClearProgressBar();
            }
        }

        private static bool Progress(string status, float percent) => EditorUtility.DisplayCancelableProgressBar("Exporting Pack", status, percent);

        private static void WriteModInfoFile(CustomCarPack pack, string exportPath, string[] requirements, string[] optional)
        {
            var outFilePath = Path.Combine(exportPath, ExporterConstants.MOD_INFO_FILENAME);

            var jsonFile = new JSONObject
            {
                { "Id", pack.PackId },
                { "DisplayName", pack.PackName },
                { "Version", pack.Version },
                { "Author", pack.Author },
                { "ManagerVersion", "0.27.3" },
                { "Requirements", JSONObject.CreateFromObject(requirements) },
                { "LoadAfter", JSONObject.CreateFromObject(optional) },
            };

            using StreamWriter stream = new StreamWriter(outFilePath, false);
            stream.Write(jsonFile.ToString(true));
        }
    }
}
