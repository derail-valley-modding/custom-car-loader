using CCL.Types;
using CCL.Types.Proxies.Simulation.Steam;
using UnityEngine;

namespace CCL.Creator.Utility
{
    [AddComponentMenu("CCL Editor/Steam Size Helper")]
    internal class SteamSizeHelper : MonoBehaviour, IEditorComponent
    {
        private const int GIZMO_SEGMENTS = 32;

        private static readonly Color s_orange = Color.Lerp(Color.red, Color.yellow, 0.4f);

        public BoilerDefinitionProxy? Boiler;
        [Range(0, 1)]
        public float WaterLevel = 0.75f;
        [Space]
        public ReciprocatingSteamEngineDefinitionProxy? SteamEngine;
        public float CylinderOffset = 1;
        public float PistonLead = 0;

        private void OnDrawGizmos()
        {
            using (GizmoUtil.MatrixScope.LocalTransformNoScale(transform))
            {
                if (Boiler != null)
                {
                    Gizmos.color = Color.blue;
                    DrawCylinder(Boiler.diameter * 0.5f, Boiler.length, Vector3.zero);

                    Gizmos.color = Color.cyan;
                    DrawRectangle(Boiler.diameter * Mathf.Sin(WaterLevel * Mathf.PI), Boiler.length,
                        Vector3.down * Boiler.diameter * 0.5f * Mathf.Cos(WaterLevel * Mathf.PI));
                }

                if (SteamEngine != null)
                {
                    if (SteamEngine.numCylinders <= 1)
                    {
                        Gizmos.color = Color.red;
                        DrawCylinder(SteamEngine.cylinderBore * 0.5f, SteamEngine.pistonStroke, Vector3.zero);
                    }
                    else
                    {
                        var strokeOffset = Vector3.forward * SteamEngine.pistonStroke * 0.5f;

                        for (int i = 0; i < SteamEngine.numCylinders; i++)
                        {
                            Gizmos.color = Color.red;

                            var offset = Vector3.right * CylinderOffset +
                                Vector3.left * 2 * CylinderOffset * (i / (SteamEngine.numCylinders - 1.0f));

                            DrawCylinder(SteamEngine.cylinderBore * 0.5f, SteamEngine.pistonStroke, offset);

                            Gizmos.color = s_orange;

                            var lead = SteamEngine.numCylinders == 3 ? i * 0.33333333333333f : i * 0.25f;

                            DrawCylinder(SteamEngine.cylinderBore * 0.5f, 0.03f, offset +
                                 strokeOffset * Mathf.Cos((lead + PistonLead) * Mathf.PI * 2));
                        }
                    }
                }
            }
        }

        private void DrawCylinder(float radius, float height, Vector3 offset)
        {
            var direction = Vector3.forward * height * 0.5f;
            var cross = Vector3.up * radius;
            var rot = Quaternion.AngleAxis(360.0f / GIZMO_SEGMENTS, Vector3.forward);

            var prev = cross;

            for (int i = 0; i < GIZMO_SEGMENTS; i++)
            {
                cross = rot * cross;

                if (i % 2 == 0)
                {
                    Gizmos.DrawLine(prev + direction + offset, prev - direction + offset);
                }

                Gizmos.DrawLine(prev + direction + offset, cross + direction + offset);
                Gizmos.DrawLine(prev - direction + offset, cross - direction + offset);

                prev = cross;
            }
        }

        private void DrawRectangle(float width, float height, Vector3 offset)
        {
            var halfW = width / 2;
            var halfH = height / 2;

            Gizmos.DrawLine(new Vector3(halfW, 0, halfH) + offset, new Vector3(halfW, 0, -halfH) + offset);
            Gizmos.DrawLine(new Vector3(halfW, 0, halfH) + offset, new Vector3(-halfW, 0, halfH) + offset);
            Gizmos.DrawLine(new Vector3(-halfW, 0, halfH) + offset, new Vector3(-halfW, 0, -halfH) + offset);
            Gizmos.DrawLine(new Vector3(-halfW, 0, -halfH) + offset, new Vector3(halfW, 0, -halfH) + offset);
        }
    }
}
