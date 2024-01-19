using CCL.Types.Proxies;
using UnityEditor;
using static CCL.Types.Proxies.WetDecalProxy;
using UnityEngine;

namespace CCL.Creator.Editor
{
    internal class WetDecalDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawWetDecalBounds(WetDecalProxy proxy, GizmoType gizmo)
        {
            Color color = new Color(0f, 0.7f, 1f, 1f);
            Gizmos.matrix = proxy.transform.localToWorldMatrix;
            if (proxy.Settings.Shape == DecalShape.Cube || proxy.Settings.Shape == DecalShape.Mesh)
            {
                color.a = (gizmo == GizmoType.Selected ? 0.3f : 0.1f);
                color.a *= (proxy.isActiveAndEnabled ? 0.15f : 0.1f);
                Gizmos.color = color;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
            else if (proxy.Settings.Shape == DecalShape.Sphere)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0f);
                Gizmos.DrawSphere(Vector3.zero, 0.5f);
            }
            else
            {
                Debug.LogError($"Unknown decal shape: '{proxy.Settings.Shape}'");
            }

            color.a = (gizmo == GizmoType.Selected ? 0.5f : 0.2f);
            color.a *= (proxy.isActiveAndEnabled ? 1f : 0.75f);
            Gizmos.color = color;
            if (proxy.Settings.Shape == DecalShape.Cube || proxy.Settings.Shape == DecalShape.Mesh)
            {
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
            else if (proxy.Settings.Shape == DecalShape.Sphere)
            {
                Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
            }
        }
    }
}
