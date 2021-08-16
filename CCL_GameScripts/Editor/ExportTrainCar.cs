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
	
	#endregion
	
	private enum state
    {
        //Main,
        Settings,
        Export
    }

    /// <summary>
    /// The current state of the menu.
    /// </summary>
    private state State = state.Settings;
    

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
	    
	    //Reset window when we open it (just in case)
	    window.ResetWindow();
	    
	    //Set train car
	    window._trainCarSetup = trainCarSetup;
	    
	    #endregion

	    window.Show();
    }

    void OnGUI()
    {
	    switch (State)
	    {
			#region State.Main
			/*
		    case state.Main:
			    GUILayout.BeginVertical("box");
			    GUILayout.Box("Train Car Tools", BoxStyle);
			    GUILayout.BeginHorizontal("box");

			    if (_trainCarSetup == null)
			    {
				    GUI.enabled = false;
				    EditorGUILayout.LabelField(
					    "This button is disabled because a TrainCarSetup script couldn't be found!");
			    }

			    if (GUILayout.Button("Prepare TrainCar for Export"))
			    {
				    State = state.Settings;
			    }

			    GUI.enabled = true;

			    GUILayout.BeginVertical();

			    EditorStyles.label.wordWrap = true;
			    EditorGUILayout.LabelField(
				    "This tool will prepare a TrainCar for export. You'll be able to choose various settings to setup your car before exporting.");

			    GUILayout.EndVertical();
			    GUILayout.EndHorizontal();
			    GUILayout.EndVertical();
			    break;
			*/
			#endregion

			#region State.Settings

			case state.Settings:

			    GUILayout.BeginVertical("box");
			    GUILayout.Box("Prepare TrainCar for Export", BoxStyle);

			    EditorStyles.label.wordWrap = true;
			    EditorGUILayout.LabelField(
				    "Confirm the settings for your train car before proceeding");

			    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			    GUILayout.BeginVertical();

			    EditorGUILayout.LabelField("Identifier:", _trainCarSetup.Identifier);
			    EditorGUILayout.LabelField("Car Underlying Type:", _trainCarSetup.BaseCarType.ToString());

			    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			    if (_trainCarSetup != null)
			    {
				    EditorGUILayout.ObjectField("Selected TrainCar: ", _trainCarSetup.gameObject, typeof(GameObject),
					    false);
			    }
			    else
			    {
				    EditorStyles.label.wordWrap = true;
				    EditorGUILayout.LabelField("No TrainCar found!.");
			    }

			    GUI.enabled = true;

			    var lastColor = GUI.backgroundColor;
			    GUI.backgroundColor = Color.green;

			    GUI.enabled = _trainCarSetup != null;
			    if (GUILayout.Button("Finalize TrainCar settings."))
			    {
				    State = state.Export;
			    }

			    GUI.enabled = true;

			    GUI.backgroundColor = lastColor;

			    GUILayout.EndVertical();

			    GUILayout.EndVertical();
			    break;

		    #endregion

			#region State.Export

			case state.Export:
			    GUILayout.BeginVertical("box"); // +1
			    GUILayout.Box("Export Train Car", BoxStyle);
			    GUILayout.BeginHorizontal("box"); // +2

			    if (GUILayout.Button("Export Train Car"))
			    {

				    //Extra check for null.
				    if (_trainCarSetup == null)
				    {
					    EditorUtility.DisplayDialog("ERROR", "TrainCarSetup script is null!", "Ok");
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
					    var assetBundleFullpath = EditorUtility.SaveFolderPanel(
						    "Export Car",
						    ExistAtC ? cPath : (ExistAtX ? xPath : System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)),
						    _trainCarSetup.Identifier);

					    if (assetBundleFullpath.Length != 0)
					    {

						    DirectoryInfo directory = new DirectoryInfo(assetBundleFullpath);
						    
						    //Prompt to clear folder before exporting.
						    if (directory.GetFiles().Length > 0 || directory.GetDirectories().Length>0)
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

						    var carFullPath = Path.Combine(assetBundleFullpath, "car.json");

						    Debug.Log($"Exporting assetBundle to: {assetBundleFullpath}");

						    #region Build Asset Bundles

						    //Build assetBundle.
						    //[OLD METHOD] Not used because it builds ALL asset bundles in the project.
						    //BuildPipeline.BuildAssetBundles (assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

						    //Correct
						    var trainCarBuild = _trainCarSetup.gameObject;
						    var trainCarOriginalObject = PrefabUtility.GetCorrespondingObjectFromSource(trainCarBuild);
						    var trainCarAssetBundleName = AssetImporter
							    .GetAtPath(AssetDatabase.GetAssetPath(trainCarOriginalObject)).assetBundleName;
						    var processedBundles = new HashSet<string>();
						    var trainCarBundleBuild = AssetBundleBuildHelper
							    .GetBuildsForPaths(processedBundles, trainCarOriginalObject).ToArray();

						    if (trainCarOriginalObject == null)
						    {
							    Debug.LogError("You must make your TrainCar a prefab before attempting to export it!");
							    return;
						    }

						    if (trainCarBundleBuild.Length <= 0)
						    {
							    Debug.LogError("Failed to create build for AssetBundle.");
						    }
						    
						    BuildPipeline.BuildAssetBundles(assetBundleFullpath, trainCarBundleBuild,
							    BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

						    Debug.Log($"Finished AssetBundle build for car: {_trainCarSetup.Identifier}.");

							#endregion

							// Create car.json file.
							ExportCarSettings(trainCarAssetBundleName, carFullPath);

						    //Goto folder when finished building.
						    if(EditorUtility.DisplayDialog("Finished Build", $"Finished building car {_trainCarSetup.Identifier} to path ({assetBundleFullpath}). Would you like to open the build folder?", "Yes", "No"))
						    {
							    EditorUtility.RevealInFinder(assetBundleFullpath);
						    }
						    
						    //Close the window when we are done building.
						    Close();
					    }
				    }
				    else
				    {
					    return;
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
			    
			    GUILayout.BeginVertical("box"); // +2
			    //GUILayout.BeginHorizontal();
			    
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

				//GUILayout.EndHorizontal();
				GUILayout.EndVertical(); // -2
			    
			    //GUILayout.EndVertical(); // -1
			    break;

		    #endregion
	    }


		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

		if( State == state.Settings )
        {
			if( GUILayout.Button("Close") )
			{
				ResetWindow();
				Close();
			}
		}
		else
		{
			if (GUILayout.Button("Back"))
			{
				State = state.Settings;
			}
		}
	}

	private void ExportCarSettings( string assetBundleName, string outFilePath )
    {
		//Create master JSONObject
		JSONObject jsonfile = new JSONObject();

		jsonfile.AddField(CarJSONKeys.BUNDLE_NAME, assetBundleName);
		jsonfile.AddField(CarJSONKeys.PREFAB_NAME, _trainCarSetup.gameObject.name);
		jsonfile.AddField(CarJSONKeys.IDENTIFIER, _trainCarSetup.Identifier);
		jsonfile.AddField(CarJSONKeys.CAR_TYPE, (int)_trainCarSetup.BaseCarType);

		//Bogies
		jsonfile.AddField(CarJSONKeys.REPLACE_FRONT_BOGIE, _trainCarSetup.ReplaceFrontBogie);
		if( _trainCarSetup.ReplaceFrontBogie )
		{
			if( _trainCarSetup.FrontBogie.GetComponent<BogieSetup>() is BogieSetup fbs )
			{
				jsonfile.AddField(CarJSONKeys.FRONT_BOGIE_PARAMS, fbs.GetJSON());
			}
		}

		jsonfile.AddField(CarJSONKeys.REPLACE_REAR_BOGIE, _trainCarSetup.ReplaceRearBogie);
		if( _trainCarSetup.ReplaceRearBogie )
		{
			if( _trainCarSetup.RearBogie.GetComponent<BogieSetup>() is BogieSetup rbs )
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

	private void ResetWindow()
    {
	    //Set state to main when
	    State = state.Settings;

	    //Reset internal data
	    _trainCarSetup = null;
    }
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