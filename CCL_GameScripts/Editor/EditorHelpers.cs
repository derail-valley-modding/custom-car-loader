using System;
using System.IO;

namespace CCL_GameScripts
{
    public static class EditorHelpers
    {
        /// <summary>
        /// Clears out an entire folder. BE CAREFUL WHEN USING.
        /// </summary>
        /// <param name="directory"></param>
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            //foreach(System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        /// <summary>
        /// Returns a full path with filename without an extension.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPathWithoutExtension(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        }

        public static string GetDefaultSavePath(string subDir = null)
        {
            string steamModPath = "Steam/steamapps/common/Derail Valley/Mods/DVCustomCarLoader";
            if (!string.IsNullOrEmpty(subDir))
            {
                steamModPath = Path.Combine(steamModPath, subDir);
            }

            // search for the user's DV install
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    string driveRoot = drive.RootDirectory.FullName;
                    string potentialPath = Path.Combine(driveRoot, "Program Files", steamModPath);
                    if (Directory.Exists(potentialPath))
                    {
                        return potentialPath;
                    }

                    potentialPath = Path.Combine(driveRoot, "Program Files (x86)", steamModPath);
                    if (Directory.Exists(potentialPath))
                    {
                        return potentialPath;
                    }
                }
                catch (Exception) { }
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
    }
}