using CCL.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Utility
{
    internal static class AssetBundleHelper
    {
        public static string GetAssetPath(Object asset) => Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));

        public static void CreateBundle(string path, IEnumerable<(Object Asset, string? Name)> assets)
        {
            BuildPipeline.BuildAssetBundles(path,
                GetAssetBuilds(assets),
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows64);
        }

        public static void DeleteBundle(string path, string bundlePath)
        {
            File.Delete(bundlePath);
            File.Delete(bundlePath + ".manifest");

            // Delete the 2nd bundle too.
            bundlePath = Path.GetFileName(path);
            bundlePath = Path.Combine(path, bundlePath);

            File.Delete(bundlePath);
            File.Delete(bundlePath + ".manifest");
        }

        public static AssetBundleBuild[] GetAssetBuilds(IEnumerable<(Object Asset, string? Name)> assets)
        {
            List<string> paths = new List<string>();

            foreach (var (asset, _) in assets)
            {
                paths.Add(AssetDatabase.GetAssetPath(asset));
            }

            var build = new AssetBundleBuild
            {
                assetBundleName = ExporterConstants.EXPORTED_BUNDLE_NAME,
                assetNames = paths.ToArray(),
                addressableNames = assets.Select(x => x.Name).ToArray(),
            };

            return new[] { build };
        }

        public static string GetFullPath(Object obj)
        {
            string path = Application.dataPath;
            string assetPath = AssetDatabase.GetAssetPath(obj);
            path = path + "/" + assetPath.Substring(7);
            return Path.GetDirectoryName(path);
        }
    }
}
