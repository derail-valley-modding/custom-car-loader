using System.IO;
using UnityEditor;
using UnityEngine;
using CCL.Types;

namespace CCL.Creator
{
    /// <summary>
    /// This window will export a train car for use in Derail Valley.
    /// It writes a JSON file with the accompanied assetbundle.
    /// </summary>
    public class ExportTrainCar : EditorWindow
	{
		private static ExportTrainCar? window = null;
		private static string LastExportPath
		{
			get => EditorPrefs.GetString("CCL_LastExportPath");
			set => EditorPrefs.SetString("CCL_LastExportPath", value);
		}

		private GUIStyle? _boxStyle = null;
		private GUIStyle BoxStyle
		{
			get
			{
				_boxStyle ??= new GUIStyle()
				{
					normal = { textColor = Color.white }
				};
				return _boxStyle;
			}
		}

		private CustomCarType? carSetup;
		private bool openFolderAfterExport = false;

		public static void ShowWindow(CustomCarType carType)
		{
			// Get existing open window or if none, make a new one:
			window = GetWindow<ExportTrainCar>();
			window.carSetup = carType;
			window.titleContent = new GUIContent("CCL - Export Car Type");
			
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
				if (carSetup == null)
				{
					EditorUtility.DisplayDialog("ERROR", "Selected CarType script is null!", "Ok");
					Close();
					return;
				}

				//if (EditorUtility.DisplayDialog("Confirmation",
				//	$"You are about to export your TrainCar named {carSetup.name}, are you sure you want to proceed?",
				//	"Yes", "No"))
				//{
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
						startingPath = EditorHelpers.GetDefaultSavePath();
						folderName = carSetup.id;
					}

					string exportFolderPath = EditorUtility.SaveFolderPanel("Export Car", startingPath, folderName);

					if (!string.IsNullOrWhiteSpace(exportFolderPath))
					{
						LastExportPath = exportFolderPath;
						ExportCar(exportFolderPath);

						//Goto folder when finished building.
						if (openFolderAfterExport)
						{
							EditorUtility.RevealInFinder(exportFolderPath);
						}

						//Close the window when we are done building.
						Close();
						return;
					}
				//}
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
				"-Once you are in the installation path, navigate to BepInEx/content/cars \n" +
				"-When you are in the Cars folder, create a new folder for your new car, name doesn't matter. (Example: Explody Boy Tanker) \n" +
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
				return;
			}

			GUILayout.EndVertical(); // -1
		}

		#region Export Functions


		//private static string GetCargoModelPrefabName(string identifier, CargoModelSetup modelSetup)
		//{
		//	string cargo = !string.IsNullOrEmpty(modelSetup.CustomCargo) ? modelSetup.CustomCargo : modelSetup.CargoType.ToString();
		//	return $"exported_{identifier.Replace(' ', '_')}_cargo_{cargo}.prefab";
		//}

		//private struct OriginalCargoModelInfo
		//{
		//	public CargoModelSetup ModelSetup;
		//	public GameObject OriginalModel;
		//	public string TempPrefabPath;

		//	public OriginalCargoModelInfo(CargoModelSetup modelSetup, GameObject originalModel, string tempPrefabPath)
		//	{
		//		ModelSetup = modelSetup;
		//		OriginalModel = originalModel;
		//		TempPrefabPath = tempPrefabPath;
		//	}
		//}

		private void ExportCar(string exportFolderPath)
		{
			DirectoryInfo directory = new DirectoryInfo(exportFolderPath);

			//Prompt to clear folder before exporting.
			if (directory.GetFiles().Length > 0 || directory.GetDirectories().Length > 0)
			{
				if (!EditorUtility.DisplayDialog("Clear Folder",
					"The directory you selected isn't empty, would you like to clear the files from the folder before proceeding? \n \n WARNING: THIS WILL DELETE ALL FILES (BUT NOT DIRECTORIES) IN THE FOLDER.",
					"Skip",
					"Clear Folder"))
				{
					//DANGEROUS METHOD, DONT USE WITHOUT CONFIDENCE
					directory.Empty();
				}
			}

			Close();

			var exportInfo = new CarTypeExporter(exportFolderPath, carSetup!);
			Debug.Log($"Exporting assetBundle to: {exportFolderPath} - bundle name: {exportInfo.BundleName}");

			exportInfo.Export();
		}

		#endregion
	}
}