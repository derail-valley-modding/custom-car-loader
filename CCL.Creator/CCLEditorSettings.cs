using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator
{
    internal class CCLEditorSettings : ScriptableObject
    {
        #region Static Asset

        private static CCLEditorSettings? s_settings;

        public static CCLEditorSettings Settings => s_settings != null ? s_settings : CreateSettings();

        private static CCLEditorSettings CreateSettings()
        {
            var settings = CreateInstance<CCLEditorSettings>();

            AssetDatabase.CreateAsset(settings, $"Assets/CarCreator/EditorSettings.asset");
            AssetDatabase.SaveAssets();

            s_settings = settings;
            return settings;
        }

        #endregion

        public bool DisplayCodeOnPortFields = true;
    }
}
