using CCL.Types.Catalog.Diagram;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector.Catalog
{
    internal class DiagramComponentDrawer
    {
        private static Color s_empty = new Color(0, 0, 0, 0);
        private static bool s_drawBase = false;

        public static void DrawCircle(Vector3 position, float radius)
        {
            Handles.DrawWireArc(position, Vector3.forward, Vector3.up, 360.0f, radius);
        }

        public static void DrawFilledCircle(Vector3 position, float radius)
        {
            Handles.DrawSolidArc(position, Vector3.forward, Vector3.up, 360.0f, radius);
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
        private static void DrawGizmoDiagram(DiagramComponent diagram, GizmoType gizmoType)
        {
            if (diagram.AutoAlign && gizmoType.HasFlag(GizmoType.Active))
            {
                diagram.AlignToGrid();
            }

            SceneView.beforeSceneGui += BeforeSceneDiagramCallback;

            if (!s_drawBase)
            {
                return;
            }

            Handles.DrawSolidRectangleWithOutline(DiagramComponent.FocusPoints, s_empty, Color.white);
            s_drawBase = false;
        }

        private static void BeforeSceneDiagramCallback(SceneView sceneView)
        {
            s_drawBase = true;
            SceneView.beforeSceneGui -= BeforeSceneDiagramCallback;
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawGizmoBogieLayout(BogieLayout bogie, GizmoType gizmoType)
        {
            if (bogie.Wheels.Length == 0)
            {
                return;
            }

            Handles.color = new Color(0.5f, 0.5f, 0.5f);

            int length = bogie.Wheels.Length;
            var middleSpace = length % 2 == 0;
            var position = bogie.transform.position + new Vector3(
                -BogieLayout.RADIUS * (length - 1) - (middleSpace ? BogieLayout.MIDDLE_SPACE / 2 : 0),
                0, 0);

            for (int i = 0; i < length; i++)
            {
                if (bogie.Wheels[i])
                {
                    DrawFilledCircle(position, BogieLayout.RADIUS);
                }
                else
                {
                    DrawCircle(position, BogieLayout.RADIUS);
                }

                position += Vector3.right * 2 * BogieLayout.RADIUS;

                if (middleSpace && i + 1 == length / 2)
                {
                    position += Vector3.right * BogieLayout.MIDDLE_SPACE;
                }
            }

            if (bogie.Pivots)
            {
                Handles.DrawLine(bogie.transform.position, bogie.transform.position + Vector3.up * BogieLayout.RADIUS * 1.5f);
            }
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawGizmoTechIcon(TechnologyIcon icon, GizmoType gizmoType)
        {
            if (icon.Icon == Types.Catalog.TechIcon.None)
            {
                return;
            }

            var color = icon.Icon switch
            {
                Types.Catalog.TechIcon.Generic => FromHex("808080"),
                Types.Catalog.TechIcon.ClosedCab => FromHex("967FAD"),
                Types.Catalog.TechIcon.OpenCab => FromHex("967FAD"),
                Types.Catalog.TechIcon.CrewCompartment => FromHex("967FAD"),
                Types.Catalog.TechIcon.CompressedAirBrakeSystem => FromHex("B45656"),
                Types.Catalog.TechIcon.DirectBrakeSystem => FromHex("B45656"),
                Types.Catalog.TechIcon.DynamicBrakeSystem => FromHex("DBB56B"),
                Types.Catalog.TechIcon.ElectricPowerSupplyAndTransmission => FromHex("5583AB"),
                Types.Catalog.TechIcon.ExternalControlInterface => FromHex("DBB56B"),
                Types.Catalog.TechIcon.HeatManagement => FromHex("439C7B"),
                Types.Catalog.TechIcon.HydraulicTransmission => FromHex("439C7B"),
                Types.Catalog.TechIcon.InternalCombustionEngine => FromHex("C6916C"),
                Types.Catalog.TechIcon.MechanicalTransmission => FromHex("808080"),
                Types.Catalog.TechIcon.PassengerCompartment => FromHex("5583AB"),
                Types.Catalog.TechIcon.SpecializedEquipment => FromHex("C6916C"),
                Types.Catalog.TechIcon.SteamEngine => FromHex("808080"),
                Types.Catalog.TechIcon.UnitEffect => FromHex("79C7E4"),
                Types.Catalog.TechIcon.CrewDelivery => FromHex("DBB56B"),
                _ => Color.white,
            };

            Handles.DrawSolidRectangleWithOutline(GetRect(icon.transform.position), color, s_empty);

            Vector3[] GetRect(Vector3 position)
            {
                return new[]
                {
                    new Vector3(position.x - TechnologyIcon.SIZE, position.y - TechnologyIcon.SIZE, position.z),
                    new Vector3(position.x - TechnologyIcon.SIZE, position.y + TechnologyIcon.SIZE, position.z),
                    new Vector3(position.x + TechnologyIcon.SIZE, position.y + TechnologyIcon.SIZE, position.z),
                    new Vector3(position.x + TechnologyIcon.SIZE, position.y - TechnologyIcon.SIZE, position.z)
                };
            }

            Color FromHex(string hex)
            {
                ColorUtility.TryParseHtmlString($"#{hex}", out var color);
                return color;
            }
        }
    }
}
