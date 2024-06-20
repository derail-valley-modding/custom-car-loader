using CCL.Creator.Utility;
using CCL.Types;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(RefreshableChildrenController<>), true)]
    internal class RefreshableChildrenControllerEditor : Editor
    {
        private IRefreshableChildren _controller = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _controller = (IRefreshableChildren)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Refresh Children"))
            {
                _controller.PopulateChildren();
                AssetHelper.SaveAsset(target);
            }
        }
    }
}
