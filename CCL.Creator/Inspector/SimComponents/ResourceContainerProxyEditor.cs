using CCL.Creator.Utility;
using CCL.Types.Proxies.Resources;
using UnityEditor;

namespace CCL.Creator.Inspector.SimComponents
{
    [CustomEditor(typeof(ResourceContainerProxy)), CanEditMultipleObjects]
    internal class ResourceContainerProxyEditor : Editor
    {
        private ResourceContainerProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (ResourceContainerProxy)target;

            var mass = _proxy.capacity * _proxy.type switch
            {
                ResourceContainerType.Fuel =>  0.85f,
                ResourceContainerType.Oil => 0.90f,
                ResourceContainerType.ElectricCharge => 0,
                _ => 1,
            };

            var price = _proxy.capacity * _proxy.type switch
            {
                ResourceContainerType.Coal => 0.70f,
                ResourceContainerType.Fuel => 3.36f,
                ResourceContainerType.Oil => 9.87f,
                ResourceContainerType.Sand => 2.67f,
                ResourceContainerType.Water => 0.09f,
                ResourceContainerType.ElectricCharge => 7.50f,
                _ => 1,
            };

            EditorHelpers.DrawHeader("Calculated Values");
            EditorGUILayout.LabelField("Resource Mass", $"{mass:F1} kg");
            EditorGUILayout.LabelField("Resource Price", $"${price:F2}");

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
