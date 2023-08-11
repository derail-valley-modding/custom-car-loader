using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace CCL.Bootstrapper
{
    internal class Constants
    {
        public const string DLL_FOLDER = "DLL_Links";
        public const string CAR_FOLDER = "_CCL_CARS";
        
        public const string PACKAGE_FOLDER = "CarCreator";

        public const string BIN_ZIP_FILE = "CCL.Creator.zip";
        public const string BIN_FOLDER = "Bin";
    }

    [InitializeOnLoad]
    public class CCBootstrapper
    {
        private static readonly string[] _dllNames =
        {
            "DV.Simulation",
            "DV.ThingTypes",
            "DV.Utils",
        };

        private static readonly string[] _packages =
        {
            "com.unity.collections@0.9.0-preview.6",
            "com.unity.nuget.newtonsoft-json@3.0",
        };

        private static string LastDLLPath
        {
            get => EditorPrefs.GetString("CCL_LastDVDLLPath");
            set => EditorPrefs.SetString("CCL_LastDVDLLPath", value);
        }

        private static AddRequest _packageRequest = null;
        private const string PACKAGE_RUN_KEY = "CCL_packageInstalling";
        private const string PACKAGE_IDX_KEY = "CCL_packageIdx";

        static CCBootstrapper()
        {
            bool installingPkgs = EditorPrefs.GetBool(PACKAGE_RUN_KEY, false);
            if (installingPkgs)
            {
                EditorApplication.update += WaitForPackageInstall;
            }
        }

        private static void Progress(string message, float progress) => EditorUtility.DisplayProgressBar("Initializing CCL Creator...", message, progress);

        [MenuItem("CCL/Initialize Creator Package")]
        public static void Initialize(MenuCommand _)
        {
            _packageRequest = null;
            EditorPrefs.SetBool(PACKAGE_RUN_KEY, true);
            EditorPrefs.SetInt(PACKAGE_IDX_KEY, 0);
            EditorApplication.update += WaitForPackageInstall;
        }

        private static void WaitForPackageInstall()
        {
            int currentIdx = EditorPrefs.GetInt(PACKAGE_IDX_KEY, 0);

            // no active request
            if (_packageRequest == null)
            {
                if (currentIdx < _packages.Length)
                {
                    string pkg = _packages[currentIdx];
                    Progress($"Installing required package: {pkg}", (float)(currentIdx + 1) / (_packages.Length + 1));

                    _packageRequest = Client.Add(pkg);
                    Debug.Log($"Add package {pkg}");
                }
                else
                {
                    EditorPrefs.SetBool(PACKAGE_RUN_KEY, false);
                    EditorApplication.update -= WaitForPackageInstall;
                    _packageRequest = null;

                    AssetDatabase.Refresh();
                    FinishInitialization();
                }
            }
            // request in flight
            else if (_packageRequest.IsCompleted)
            {
                _packageRequest = null;
                EditorPrefs.SetInt(PACKAGE_IDX_KEY, currentIdx + 1);
            }
        }

        private static void FinishInitialization()
        {
            if (!SetupDLLs())
            {
                EditorUtility.DisplayDialog(
                    "DV DLLs not linked",
                    "Derail Valley libraries were not linked - you will not be able to build cars until they are. " +
                    "Select CCL -> Initialize Creator Package from the menu bar to try again.",
                    "OK");
                return;
            }

            if (CCAssemblyManager.ZippedBinIsNewer())
            {
                Progress("Enabling CCL Binaries...", 0.8f);
                CCAssemblyManager.EnableMainAssembly();
            }

            CreateCarFolders();

            Progress("Refreshing Assets...", 0.9f);
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
        }

        [MenuItem("CCL/Repack Creator Package (dev only)")]
        public static void DeInitialize(MenuCommand _)
        {
            CCAssemblyManager.DisableMainAssembly();

            AssetDatabase.Refresh();
        }

        private static bool DLLsNeedUpdated()
        {
            string localDLLFolder = Path.Combine(Application.dataPath, Constants.DLL_FOLDER);
            if (!Directory.Exists(localDLLFolder))
            {
                return true;
            }

            foreach (string dllName in _dllNames)
            {
                string destination = Path.Combine(localDLLFolder, $"{dllName}.dll");

                if (!File.Exists(destination))
                {
                    return true;
                }
            }

            foreach (string file in Directory.EnumerateFiles(localDLLFolder))
            {
                if ((Path.GetExtension(file) != ".meta") && !_dllNames.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SetupDLLs()
        {
            if (!DLLsNeedUpdated()) return true;

            bool result;

            result = EditorUtility.DisplayDialog("Setup DV References",
                "You will be asked to select the folder where the Derail Valley libraries (DLL files) are located. " +
                "The tool will attempt to find your Derail Valley installation. If the tool can't find your install path, navigate to it manually. " +
                "Once you are in the installation path, select the DerailValley_Data/Managed folder.",
                "Proceed");

            if (!result) return false;

            string startingPath, folderName;
            string lastPath = LastDLLPath;
            if (!string.IsNullOrEmpty(lastPath) && Directory.Exists(lastPath))
            {
                startingPath = Path.GetDirectoryName(lastPath);
                folderName = Path.GetFileName(lastPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            }
            else
            {
                startingPath = GetDefaultDLLFolder();
                folderName = "";
            }

            string dllPath = EditorUtility.SaveFolderPanel("Managed DLL Folder", startingPath, folderName);

            if (!string.IsNullOrWhiteSpace(dllPath) && Directory.Exists(dllPath))
            {
                LastDLLPath = dllPath;
                LinkDLLs(dllPath);
                return true;
            }

            return false;
        }

        private static void LinkDLLs(string dllPath)
        {
            string localDLLFolder = Path.Combine(Application.dataPath, Constants.DLL_FOLDER);
            Directory.CreateDirectory(localDLLFolder);

            foreach (var file in Directory.EnumerateFiles(localDLLFolder))
            {
                if ((Path.GetExtension(file) != ".meta") && !_dllNames.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    File.Delete(file);
                }
            }

            for (int i = 0; i < _dllNames.Length; i++)
            {
                Progress($"Linking DLL: {_dllNames[i]}", (float)i / _dllNames.Length);

                string dllName = $"{_dllNames[i]}.dll";
                string source = Path.Combine(dllPath, dllName);
                string destination = Path.Combine(localDLLFolder, dllName);

                if (!File.Exists(destination) || (File.GetLastWriteTime(source) > File.GetLastWriteTime(destination)))
                {
                    if (File.Exists(destination)) 
                    {
                        File.Delete(destination);
                    }

                    File.Copy(source, destination, true);
                }
            }
        }

        private static string GetDefaultDLLFolder()
        {
            string dllPath = SteamHelper.GetDLLDirectory();
            if (dllPath != null) return dllPath;

            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private static void CreateCarFolders()
        {
            string carFolder = Path.Combine(Application.dataPath, Constants.CAR_FOLDER);
            Directory.CreateDirectory(carFolder);
        }
    }
}
