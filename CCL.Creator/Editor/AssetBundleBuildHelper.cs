using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator
{
    public static class AssetBundleBuildHelper
	{
		public static List<AssetBundleBuild> GetBuildsForPaths(HashSet<string> processedBundles, params Object[] assets)
		{
			List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

			// Get asset bundle names from selection
			foreach (var o in assets)
			{
				var assetPath = AssetDatabase.GetAssetPath(o);
				var importer = AssetImporter.GetAtPath(assetPath);

				if (importer == null)
				{
					continue;
				}

				// Get asset bundle name & variant
				var assetBundleName = importer.assetBundleName;
				var assetBundleVariant = importer.assetBundleVariant;
				var assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant) ? assetBundleName : assetBundleName + "." + assetBundleVariant;

				// Only process assetBundleFullName once. No need to add it again.
				if (processedBundles.Contains(assetBundleFullName))
				{
					continue;
				}

				processedBundles.Add(assetBundleFullName);

				AssetBundleBuild build = new AssetBundleBuild();

				build.assetBundleName = assetBundleName;
				build.assetBundleVariant = assetBundleVariant;
				build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName);

				assetBundleBuilds.Add(build);
			}

			return assetBundleBuilds;
		}

		public static AssetBundleBuild GetAssetBundleBuild(string bundleName)
		{
			return new AssetBundleBuild()
			{
				assetBundleName = bundleName,
				assetBundleVariant = null,
				assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
			};
		}

		public static void SetAssetBundle(this Object? asset, string bundleName)
		{
			if (asset != null && asset)
			{
				string assetPath = AssetDatabase.GetAssetPath(asset);
				AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, "");
			}
        }
	}
}