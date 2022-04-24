using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CCL_GameScripts
{
    [CreateAssetMenu(fileName = "newCargoPack.asset", menuName = "CCL/Custom Cargo Pack")]
    public class CustomCargoPack : ScriptableObject
    {
        public CustomCargo[] Cargos;

        [MethodButton(nameof(Export))]
        [SerializeField]
        protected bool editorFoldout = true;

        private static string LastExportPath
        {
            get => EditorPrefs.GetString("CCL_LastCargoExportPath");
            set => EditorPrefs.SetString("CCL_LastCargoExportPath", value);
        }

        private void Export()
        {
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
                folderName = name;
            }

            string exportFolderPath = EditorUtility.SaveFolderPanel("Export Cargo", startingPath, folderName);

            if (!string.IsNullOrWhiteSpace(exportFolderPath))
            {
                LastExportPath = exportFolderPath;

                var json = new JSONObject(JSONObject.Type.ARRAY);
                foreach (var child in Cargos)
                {
                    json.Add(child.Export());
                }

                //Create JSON file.
                string fullJson = json.ToString(true);
                string jsonFileName = Path.Combine(exportFolderPath, CarJSONKeys.CARGO_JSON_FILE);

                //Write data to JSON file
                using (StreamWriter stream = new StreamWriter(jsonFileName, false))
                {
                    stream.Write(fullJson);
                }

                Debug.Log($"Exported cargo pack {name} to {exportFolderPath}");
            }
        }
    }

    [Serializable]
    public class CustomCargo
    {
        [Tooltip("Unique name, no spaces")]
        public string Identifier = "NewCargo";
        
        [Tooltip("Used on car info plates")]
        public string ShortName = "New Cargo";

        [Tooltip("Used in the job description")]
        public string SpecificName = "A shipment of New Cargo";
        
        public float MassPerUnit = 35000;
        public float ValuePerUnit = 10000;
        [Tooltip("Environmental penalty for 100% cargo damage")]
        public float EnvironmentDamagePrice = 0;

        public BaseJobLicenses RequiredLicense = BaseJobLicenses.Basic;

        public StationYard Sources = StationYard.None;
        public StationYard Destinations = StationYard.None;

        #region Export

        public JSONObject Export()
        {
            var json = new JSONObject();
            json.AddField(CarJSONKeys.IDENTIFIER, Identifier);
            json.AddField(CarJSONKeys.CARGO_SHORT_NAME, ShortName);
            json.AddField(CarJSONKeys.CARGO_LONG_NAME, SpecificName);

            json.AddField(CarJSONKeys.CARGO_MASS, MassPerUnit);
            json.AddField(CarJSONKeys.CARGO_VALUE, ValuePerUnit);
            json.AddField(CarJSONKeys.CARGO_ENVIRONMENT, EnvironmentDamagePrice);

            json.AddField(CarJSONKeys.CARGO_LICENSE, (int)RequiredLicense);
            json.AddField(CarJSONKeys.CARGO_SOURCE, (int)Sources);
            json.AddField(CarJSONKeys.CARGO_DEST, (int)Destinations);
            return json;
        }

        public static CustomCargo Import(JSONObject json)
        {
            try
            {
                var cargo = new CustomCargo
                {
                    Identifier = json[CarJSONKeys.IDENTIFIER].str,
                    ShortName = json[CarJSONKeys.CARGO_SHORT_NAME].str,
                    SpecificName = json[CarJSONKeys.CARGO_LONG_NAME].str,

                    MassPerUnit = json[CarJSONKeys.CARGO_MASS].f,
                    ValuePerUnit = json[CarJSONKeys.CARGO_VALUE].f,
                    EnvironmentDamagePrice = json[CarJSONKeys.CARGO_ENVIRONMENT].f,

                    RequiredLicense = (BaseJobLicenses)json[CarJSONKeys.CARGO_LICENSE].i,
                    Sources = (StationYard)json[CarJSONKeys.CARGO_SOURCE].i,
                    Destinations = (StationYard)json[CarJSONKeys.CARGO_DEST].i
                };

                return cargo;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        #endregion
    }
}
