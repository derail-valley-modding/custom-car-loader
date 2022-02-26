using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using CCL_GameScripts;

/// <summary>
/// This window will export a train car for use in Derail Valley.
/// It writes a JSON file with the accompanied assetbundle.
/// </summary>
public class ExportTrainCar : EditorWindow
{

	#region Internal Data
	
	private static ExportTrainCar window = null;
	
	private GUIStyle _boxStyle;
	private GUIStyle BoxStyle => _boxStyle ?? GUI.skin.box;
	
	private TrainCarSetup _trainCarSetup;
	private bool openFolderAfterExport = false;
	
	#endregion


    [InitializeOnLoadMethod]
    static void Init()
    {
	    TrainCarSetup.LaunchExportWindow = ShowWindow;
    }
    
    public static void ShowWindow(TrainCarSetup trainCarSetup)
    {
	    // Get existing open window or if none, make a new one:
	    window = GetWindow<ExportTrainCar>();

	    #region Set Internal Data
	    
	    if (window._boxStyle == null)
	    {
		    window._boxStyle = new GUIStyle {normal = {textColor = Color.white}};
	    }
	    
	    //Set train car
	    window._trainCarSetup = trainCarSetup;
	    
	    #endregion

	    window.Show();
	}

	void OnGUI()
    {
		GUILayout.BeginVertical("box"); // +1
		GUILayout.Box("Export Train Car", BoxStyle);
		GUILayout.BeginHorizontal("box"); // +2

		if (GUILayout.Button("Export Train Car"))
		{
			//Extra check for null.
			if (_trainCarSetup == null)
			{
				EditorUtility.DisplayDialog("ERROR", "TrainCarSetup script is null!", "Ok");
				Close();
				return;
			}

			if (EditorUtility.DisplayDialog("Confirmation",
				$"You are about to export your TrainCar named {_trainCarSetup.Identifier}, are you sure you want to proceed?",
				"Yes", "No"))
			{

				//Basically we store two paths that can potentially have the game existing in it.
				var cPath = Path.Combine("C:/Program Files/Steam/steamapps/common/Derail Valley/Mods/DVCustomCarLoader/Cars");
				var xPath = Path.Combine("X:/Program Files/Steam/steamapps/common/Derail Valley/Mods/DVCustomCarLoader/Cars");
				var ExistAtC = Directory.Exists(cPath);
				var ExistAtX = Directory.Exists(xPath);
					    
				//We check both paths to see if they exist. If not, we just open at the users desktop.
				var exportFolderPath = EditorUtility.SaveFolderPanel(
					"Export Car",
					ExistAtC ? cPath : (ExistAtX ? xPath : System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)),
					_trainCarSetup.Identifier);

				if (!string.IsNullOrWhiteSpace(exportFolderPath))
				{
					ExportCar(exportFolderPath);

					//Goto folder when finished building.
					if (openFolderAfterExport)
					{
						EditorUtility.RevealInFinder(exportFolderPath);
					}
					
					//Close the window when we are done building.
					Close();
				}
			}
		}

		GUILayout.BeginVertical(); // +3

		EditorStyles.label.wordWrap = true;
		EditorGUILayout.LabelField(
			"This button will open a window that allows you to select a folder to export your car. " +
			"If an assetBundle already exists with the name you type, it will be written over." +
			"If a car's settings file already exists, it will be written over." +
			"Be sure to name your assetBundle appropriately as special characters/symbols can cause problems.");

		GUILayout.EndVertical(); // -3
		GUILayout.EndHorizontal(); // -2

		openFolderAfterExport = GUILayout.Toggle(openFolderAfterExport, "Open build folder after export");

		GUILayout.BeginVertical("box"); // +2

		EditorStyles.label.wordWrap = true;
		EditorGUILayout.LabelField(					    
			//How to export
			"[HOW TO EXPORT] \n \n" +
			"-The tool will attempt to find your Derail Valley installation. If the tool can't find your install path, navigate to it manually. \n" +
			"-Once you are in the installation path, navigate to Mods/DVCustomCarLoader/Cars \n" +
			"-When you are in the Cars folder, create a new folder for your new car, name doesn't matter. (Example: UP Autorack Yellow) \n" +
			"-After creating that new folder, make sure to select it and click 'Save'. \n \n" +
					    
			//How to avoid errors
			"[HOW TO AVOID ERRORS] \n \n" +
			"-Do not type any special characters in your folder to avoid Windows path problems. \n" +
			"-Do not overwrite files if you don't want something potentially going wrong. \n" +
			"-If any warnings or errors pop up within Unity, do not continue saving your file.");

		GUILayout.EndVertical(); // -2

		if (GUILayout.Button("Close"))
		{
			Close();
		}

		GUILayout.EndVertical(); // -1
	}

    #region Export Functions

    private static string GetBundleName(string identifier)
    {
		return identifier.Replace(' ', '_') + "_bundle";
    }

	private static string GetPrefabName(string identifier)
    {
		return $"exported_{identifier.Replace(' ', '_')}.prefab";
    }

	private static string GetInteriorPrefabName(string identifier)
    {
		return $"exported_{identifier.Replace(' ', '_')}_interior.prefab";
    }

	private void ExportCar(string exportFolderPath)
    {
		DirectoryInfo directory = new DirectoryInfo(exportFolderPath);

		//Prompt to clear folder before exporting.
		if (directory.GetFiles().Length > 0 || directory.GetDirectories().Length > 0)
		{
			if (EditorUtility.DisplayDialog("Clear Folder",
				"The directory you selected isn't empty, would you like to clear the folder before proceeding? \n \n WARNING: THIS WILL DELETE EVERYTHING IN THE FOLDER.",
				"Clear Folder",
				"Cancel"))
			{
				//DANGEROUS METHOD, DONT USE WITHOUT CONFIDENCE
				directory.Empty();
			}
		}

		string bundleName = GetBundleName(_trainCarSetup.Identifier);
		Debug.Log($"Exporting assetBundle to: {exportFolderPath} - bundle name: {bundleName}");

		// Create a temp prefab object
		string assetFolder = Path.GetDirectoryName(_trainCarSetup.gameObject.scene.path);
		string tempPrefabPath = Path.Combine(assetFolder, GetPrefabName(_trainCarSetup.Identifier)).Replace('\\', '/');
		var tempPrefab = PrefabUtility.SaveAsPrefabAsset(_trainCarSetup.gameObject, tempPrefabPath);

		string tempInteriorPath = null;
		if (_trainCarSetup.InteriorPrefab)
		{
			tempInteriorPath = Path.Combine(assetFolder, GetInteriorPrefabName(_trainCarSetup.Identifier)).Replace('\\', '/');
			var tempInteriorPrefab = PrefabUtility.SaveAsPrefabAsset(_trainCarSetup.InteriorPrefab, tempInteriorPath);
			tempPrefab.GetComponent<TrainCarSetup>().InteriorPrefab = tempInteriorPrefab;

			AssetImporter.GetAtPath(tempInteriorPath).SetAssetBundleNameAndVariant(bundleName, "");
		}

		//Change name of asset bundle to this GameObject.
		AssetImporter.GetAtPath(tempPrefabPath).SetAssetBundleNameAndVariant(bundleName, "");
		AssetDatabase.RemoveUnusedAssetBundleNames();
		
		if (tempPrefab == null)
		{
			Debug.LogError("Failed to create temporary prefab for export, abandoning build");
			return;
		}

		//Build assetBundle.
		var processedBundles = new HashSet<string>();
		var trainCarBundleBuild = AssetBundleBuildHelper.GetBuildsForPaths(processedBundles, tempPrefab).ToArray();

		if (trainCarBundleBuild.Length <= 0)
		{
			Debug.LogError("Failed to create build for AssetBundle.");
		}

		BuildPipeline.BuildAssetBundles(exportFolderPath, trainCarBundleBuild, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
		Debug.Log($"Finished AssetBundle build for car: {_trainCarSetup.Identifier}.");

		AssetDatabase.DeleteAsset(tempPrefabPath);
		if (tempInteriorPath != null)
		{
			AssetDatabase.DeleteAsset(tempInteriorPath);
		}

		// Create car.json file.
		var jsonFile = Path.Combine(exportFolderPath, "car.json");
		WriteCarSettingsFile(bundleName, jsonFile);
	}

	private void WriteCarSettingsFile( string assetBundleName, string outFilePath )
    {
		//Create master JSONObject
		JSONObject jsonfile = new JSONObject();

		jsonfile.AddField(CarJSONKeys.BUNDLE_NAME, assetBundleName);
		jsonfile.AddField(CarJSONKeys.PREFAB_NAME, GetPrefabName(_trainCarSetup.Identifier));
		jsonfile.AddField(CarJSONKeys.IDENTIFIER, _trainCarSetup.Identifier);
		jsonfile.AddField(CarJSONKeys.CAR_TYPE, (int)_trainCarSetup.BaseCarType);

		//Bogies
		if( _trainCarSetup.UseCustomFrontBogie )
		{
			var frontBogie = _trainCarSetup.GetFrontBogie();
			if (frontBogie && frontBogie.GetComponent<BogieSetup>() is BogieSetup fbs)
			{
				jsonfile.AddField(CarJSONKeys.FRONT_BOGIE_PARAMS, fbs.GetJSON());
			}
		}

		if( _trainCarSetup.UseCustomRearBogie )
		{
			var rearBogie = _trainCarSetup.GetRearBogie();
			if (rearBogie && rearBogie.GetComponent<BogieSetup>() is BogieSetup rbs )
			{
				jsonfile.AddField(CarJSONKeys.REAR_BOGIE_PARAMS, rbs.GetJSON());
			}
		}

		//Create JSON file.
		var fullJson = jsonfile.ToString(true);

		//Write data to JSON file
		using( StreamWriter newTask = new StreamWriter(outFilePath, false) )
		{
			newTask.Write(fullJson);
		}
	}

    #endregion
}


#region Build Helper

public static class AssetBundleBuildHelper
{
	public static List<AssetBundleBuild> GetBuildsForPaths(HashSet<string> processedBundles , params Object[] assets )
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
}

class MyCustomBuildProcessor : IPostprocessBuildWithReport
{
	public int callbackOrder
	{
		get { return 0; }
	}

	public void OnPostprocessBuild(BuildReport report)
	{
		Debug.Log("Finished building asset bundles");
	}
}

#endregion