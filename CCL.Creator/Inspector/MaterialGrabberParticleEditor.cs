using CCL.Creator.Utility;
using CCL.Types.Components;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(MaterialGrabberParticle))]
    internal class MaterialGrabberParticleEditor : Editor
    {
        private SerializedProperty _system = null!;
        private SerializedProperty _replacement = null!;

        private void OnEnable()
        {
            // Need to add a reference to the module otherwise.
            _system = serializedObject.FindProperty("ParticleSystem");
            _replacement = serializedObject.FindProperty(nameof(MaterialGrabberParticle.Replacement));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_system);
            EditorHelpers.StringWithSearchField(_replacement, MaterialGrabber.MaterialNames, EditorGUIUtility.singleLineHeight * 4, 40);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
