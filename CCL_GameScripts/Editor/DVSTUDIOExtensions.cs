
    public static class DVSTUDIOExtensions
    {
        /// <summary>
        /// Clears out an entire folder. BE CAREFUL WHEN USING.
        /// </summary>
        /// <param name="directory"></param>
        public static void Empty(this System.IO.DirectoryInfo directory)
        {
            foreach(System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach(System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }
    
        /// <summary>
        /// Returns a full path with filename without an extension.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPathWithoutExtension(string path)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileNameWithoutExtension(path));
        }
    }