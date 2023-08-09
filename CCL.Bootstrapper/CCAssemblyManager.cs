using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace CCL.Bootstrapper
{
    public static class CCAssemblyManager
    {
        private static string GetDisabledBinPath() => Path.Combine(Application.dataPath, Constants.PACKAGE_FOLDER, Constants.BIN_ZIP_FILE);
        private static string GetEnabledBinPath() => Path.Combine(Application.dataPath, Constants.PACKAGE_FOLDER, Constants.BIN_FOLDER);

        public static bool ZippedBinIsNewer()
        {
            string hiddenName = GetDisabledBinPath();
            string enabledName = GetEnabledBinPath();

            var hiddenTime = File.GetLastWriteTime(hiddenName);
            var enabledTime = GetFolderLastModified(enabledName);

            return hiddenTime > enabledTime;
        }

        private static DateTime GetFolderLastModified(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return DateTime.MinValue;

            DateTime lastModified = DateTime.MinValue;
            foreach (string file in Directory.EnumerateFiles(dirPath))
            {
                DateTime fileModified = File.GetLastWriteTime(file);
                if (fileModified > lastModified)
                {
                    lastModified = fileModified;
                }
            }
            return lastModified;
        }

        public static void EnableMainAssembly()
        {
            string hiddenPath = GetDisabledBinPath();
            string enabledPath = GetEnabledBinPath();

            if (File.Exists(hiddenPath))
            {
                if (!Directory.Exists(enabledPath))
                {
                    Directory.CreateDirectory(enabledPath);
                }

                using (var stream = File.OpenRead(hiddenPath))
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            string newFile = Path.Combine(enabledPath, entry.FullName);
                            entry.ExtractToFile(newFile, true);
                        }
                    }
                }
            }
        }

        public static void DisableMainAssembly()
        {
            string enabledPath = GetEnabledBinPath();

            if (Directory.Exists(enabledPath))
            {
                if (!ZippedBinIsNewer())
                {
                    string hiddenPath = GetDisabledBinPath();
                    using (var stream = File.Open(hiddenPath, FileMode.Create))
                    {
                        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                        {
                            foreach (var file in Directory.EnumerateFiles(enabledPath, "*.dll", SearchOption.AllDirectories))
                            {
                                string relPath = file.Replace(enabledPath, "").TrimStart(Path.DirectorySeparatorChar);
                                archive.CreateEntryFromFile(file, relPath);
                            }
                        }
                    }
                }

                Directory.Delete(enabledPath, true);
            }
        }
    }
}
