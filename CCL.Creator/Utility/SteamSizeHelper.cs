using CCL.Types;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEngine;

namespace CCL.Creator.Utility
{
    internal class SteamSizeHelper : MonoBehaviour, IEditorComponent
    {
        private const int GIZMO_SEGMENTS = 32;

        public BoilerDefinitionProxy? Boiler;
        public ReciprocatingSteamEngineDefinitionProxy? SteamEngine;

        private void OnDrawGizmos()
        {
            if (Boiler != null)
            {
                Gizmos.color = Color.blue;
                DrawCylinder(Boiler.diameter * 0.5f, Boiler.length);
            }

            if (SteamEngine != null)
            {
                Gizmos.color = Color.red;
                DrawCylinder(SteamEngine.cylinderBore * 0.5f, SteamEngine.pistonStroke);
            }
        }

        private void DrawCylinder(float radius, float height)
        {
            using (GizmoUtil.MatrixScope.LocalTransformNoScale(transform))
            {
                var direction = Vector3.forward * height * 0.5f;
                var offset = Vector3.up * radius;
                var rot = Quaternion.AngleAxis(360.0f / GIZMO_SEGMENTS, Vector3.forward);

                var prev = offset;

                for (int i = 0; i < GIZMO_SEGMENTS; i++)
                {
                    offset = rot * offset;

                    if (i % 2 == 0)
                    {
                        Gizmos.DrawLine(prev + direction, prev - direction);
                    }

                    Gizmos.DrawLine(prev + direction, offset + direction);
                    Gizmos.DrawLine(prev - direction, offset - direction);

                    prev = offset;
                }
            }
        }
    }
}
