using UnityEditor;
using UnityEngine;

namespace CCL.Creator
{
    internal class CCLEditorSettings : ScriptableObject
    {
        #region Static Asset

        public static readonly string DefaultAssetPath = "Assets/CarCreator/EditorSettings.asset";

        private static CCLEditorSettings? s_settings;

        public static CCLEditorSettings Settings => s_settings != null ? s_settings : CreateSettings();

        private static CCLEditorSettings CreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<CCLEditorSettings>(DefaultAssetPath);

            if (settings == null)
            {
                settings = CreateInstance<CCLEditorSettings>();
                AssetDatabase.CreateAsset(settings, DefaultAssetPath);
                AssetDatabase.SaveAssets();
            }

            s_settings = settings;
            return settings;
        }

        #endregion

        [Header("Port Display")]
        [Tooltip("Whether or not to display the port code on fields ([Port], [Fuse]...)")]
        public bool DisplayCodeOnPortFields = true;
        [Tooltip("Displays ports with a folder structure instead of a simple list")]
        public bool UseFolderSystemOnPortFields = false;

        [Space, Header("Extra IDs")]
        [Tooltip("Extra Cargo IDs to display in cargo fields\n" +
            "Only works on fields that support custom values")]
        public string[] ExtraCargos = new string[0];
        [Tooltip("Extra General License IDs to display in license fields\n" +
            "Only works on fields that support custom values")]
        public string[] ExtraGeneralLicenses = new string[0];
        [Tooltip("Extra Job License IDs to display in license fields\n" +
            "Only works on fields that support custom values")]
        public string[] ExtraJobLicenses = new string[0];
    }
}
