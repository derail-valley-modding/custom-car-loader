using CCL.Types.Components;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    internal class ParticleDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmoDistanceOptimiser(CopyVanillaParticleSystem particles, GizmoType gizmoType)
        {
            Gizmos.color = new Color(0.1f, 0.9f, 1.0f);
            Gizmos.matrix = particles.transform.localToWorldMatrix;

            switch (particles.SystemToCopy)
            {
                case VanillaParticleSystem.SteamerSteamSmoke:
                case VanillaParticleSystem.SteamerSteamSmokeThick:
                case VanillaParticleSystem.SteamerEmberClusters:
                case VanillaParticleSystem.SteamerEmberSparks:
                    Gizmos.DrawLine(Vector3.zero, Vector3.up);
                    break;
                    Gizmos.DrawLine(Vector3.zero, Vector3.down);
                    break;
                default:
                    Gizmos.DrawLine(Vector3.zero, Vector3.forward);
                    break;
            }
        }
    }
}
