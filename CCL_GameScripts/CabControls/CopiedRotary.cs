using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public enum CopiedRotaryType
    {
        StarterShunter,
        LightKnobShunter,

        StarterDE6,
        LightKnobDE6,

        InjectorSH282,
        BigValveSH282,
        SmallValveSH282,
    }

    public class CopiedRotary : CopiedCabInput
    {
        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new[]
            {
                (BaseTrainCarType.LocoShunter, "C starter_rotary"),
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/C headlights dash_rotary02"),

                (BaseTrainCarType.LocoDiesel, "offset/C engine controls/C engine_ignition"),
                (BaseTrainCarType.LocoDiesel, "offset/C headlights dash_rotary01"),

                (BaseTrainCarType.LocoSteamHeavy, "C injector"),
                (BaseTrainCarType.LocoSteamHeavy, "C valve controller/C valve 1"),
                (BaseTrainCarType.LocoSteamHeavy, "S_Valve01"),
            };

        public CopiedRotaryType RotaryType;

        public override (BaseTrainCarType, string) GetSourceObject()
        {
            return TargetObjects[(int)RotaryType];
        }

        protected static readonly RotaryGizmoInfo[] GizmoData =
            new[]
            {
                // DE2
                new RotaryGizmoInfo(-60, 60, 0, Vector3.up),        // starter
                new RotaryGizmoInfo(-22, 22, -22, Vector3.up),      // lights

                // DE6
                new RotaryGizmoInfo(-60, 60, 0, Vector3.down),      // starter
                new RotaryGizmoInfo(-22, 22, -22, Vector3.down),    // lights

                // SH282
                new RotaryGizmoInfo(0, 180, 0, Vector3.forward),    // injector
                new RotaryGizmoInfo(0, 180, 0, Vector3.down),    // large valve
                new RotaryGizmoInfo(0, 180, 0, Vector3.down),    // small valve
            };

        protected const int GIZMO_SLICE_SIZE = 10;
        protected const float GIZMO_RADIUS = 0.02f;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            var gizmo = GizmoData[(int)RotaryType];

            bool singleSweep = (gizmo.LimitMax - gizmo.LimitMin) >= 180;

            DrawSweep(gizmo, gizmo.LimitMin, singleSweep);
            DrawSweep(gizmo, gizmo.LimitMax, singleSweep);
        }

        private void DrawSweep(RotaryGizmoInfo gizmo, float end, bool single)
        {
            if (gizmo.StartPos == end) return;

            Vector3 lastVector = transform.position;
            Vector3 lastOpp = transform.position;

            float totalSweep = Mathf.Abs(end - gizmo.StartPos);
            int gizmoSegments = Mathf.FloorToInt(totalSweep / GIZMO_SLICE_SIZE);

            Quaternion axisTransform = Quaternion.FromToRotation(Vector3.up, gizmo.Axis);
            Vector3 startVector = axisTransform * Vector3.forward;

            for (int i = 0; i <= gizmoSegments; i++)
            {
                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / gizmoSegments);
                Vector3 radialVector = (Quaternion.AngleAxis(
                    Mathf.Lerp(gizmo.StartPos, end, (float)i / gizmoSegments), gizmo.Axis)
                    * startVector).normalized;

                Vector3 nextVector = transform.TransformPoint(radialVector * GIZMO_RADIUS);

                if (i != 0)
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                }
                lastVector = nextVector;

                Gizmos.DrawLine(transform.position, nextVector);

                if (!single)
                {
                    Vector3 oppositeVector = transform.TransformPoint(radialVector * -GIZMO_RADIUS);

                    if (i != 0)
                    {
                        Gizmos.DrawLine(lastOpp, oppositeVector);
                    }

                    lastOpp = oppositeVector;
                    Gizmos.DrawLine(transform.position, oppositeVector);
                }
            }
        }

        protected class RotaryGizmoInfo
        {
            public float LimitMin;
            public float LimitMax;
            public float StartPos;
            public Vector3 Axis;

            public RotaryGizmoInfo( float min, float max, float start, Vector3 axis )
            {
                LimitMin = min;
                LimitMax = max;
                StartPos = start;
                Axis = axis;
            }
        }
    }
}