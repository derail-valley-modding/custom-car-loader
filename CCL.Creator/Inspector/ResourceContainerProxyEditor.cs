using CCL.Creator.Utility;
using CCL.Types.Proxies.Resources;
using UnityEditor;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(ResourceContainerProxy)), CanEditMultipleObjects]
    internal class ResourceContainerProxyEditor : Editor
    {
        private ResourceContainerProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (ResourceContainerProxy)target;

            var mass = _proxy.type switch
            {
                ResourceContainerType.Fuel => _proxy.capacity * 0.85f,
                ResourceContainerType.Oil => _proxy.capacity * 0.90f,
                ResourceContainerType.ElectricCharge => 0,
                _ => _proxy.capacity,
            };

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Resource Mass", $"{mass:F1} kg");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
