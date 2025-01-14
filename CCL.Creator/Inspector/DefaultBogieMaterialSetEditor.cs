using CCL.Types.Proxies.Customization;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(DefaultBogieMaterialSet))]
    internal class DefaultBogieMaterialSetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component will create a material set for default bogies.\n" +
                "Be aware it may have side effects when using custom bogies.", MessageType.Info);
        }
    }
}
