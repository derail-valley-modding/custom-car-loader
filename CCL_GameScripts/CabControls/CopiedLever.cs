using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class CopiedLever : CopiedCabInput
    {
        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new []
            {
                (BaseTrainCarType.LocoShunter, "C independent_brake_lever"),
                (BaseTrainCarType.LocoShunter, "C train_brake_lever"),
                (BaseTrainCarType.LocoShunter, "C throttle"),
                (BaseTrainCarType.LocoShunter, "C reverser"),
                (BaseTrainCarType.LocoShunter, "C horn"),

                (BaseTrainCarType.LocoDiesel, "offset/C independent_brake_lever"),
                (BaseTrainCarType.LocoDiesel, "offset/C train_brake_lever"),
                (BaseTrainCarType.LocoDiesel, "offset/C throttle"),
                (BaseTrainCarType.LocoDiesel, "offset/C reverser"),
                (BaseTrainCarType.LocoDiesel, "offset/C horn"),
            };

        protected static readonly LeverGizmoInfo[] GizmoData =
            new[]
            {
                new LeverGizmoInfo(-61, 0, 20),
                new LeverGizmoInfo(0, 72, 20),
                new LeverGizmoInfo(-52, 1, 8, true),
                new LeverGizmoInfo(-25, 65, 3, true),
                new LeverGizmoInfo(-27, 27, 3),

                new LeverGizmoInfo(-88.429f, 0, 20, true),
                new LeverGizmoInfo(-90, 0, 20, true),
                new LeverGizmoInfo(-78.815f, 0, 8, true),
                new LeverGizmoInfo(-65.2f, 0, 3, true),
                new LeverGizmoInfo(-12, 12, 3),
            };

        public CopiedLeverType LeverType;

        public override (BaseTrainCarType, string) GetSourceObject()
        {
            return TargetObjects[(int)LeverType];
        }

        protected const float GIZMO_RADIUS = 0.1f;
        protected const int GIZMO_SEGMENTS = 40;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmosSelected()
        {
            var gizmo = GizmoData[(int)LeverType];

            Color startColor = gizmo.Inverted ? END_COLOR : START_COLOR;
            Color endColor = gizmo.Inverted ? START_COLOR : END_COLOR;

            float rayCount = gizmo.Notches - 1;
            // draw ray segments
            for( int i = 0; i <= rayCount; i++ )
            {
                Color segmentColor = Color.Lerp(startColor, endColor, i / rayCount);
                Vector3 rayVector = Quaternion.AngleAxis(
                    Mathf.Lerp(gizmo.LimitMin, gizmo.LimitMax, i / rayCount), Vector3.up)
                    * Vector3.forward * GIZMO_RADIUS;
                rayVector = transform.TransformPoint(rayVector);

                Gizmos.color = segmentColor;
                Gizmos.DrawLine(transform.position, rayVector);
            }
        }

        protected class LeverGizmoInfo
        {
            public float LimitMin;
            public float LimitMax;
            public int Notches;
            public bool Inverted;

            public LeverGizmoInfo( float min, float max, int notches, bool invert = false )
            {
                LimitMin = min;
                LimitMax = max;
                Notches = notches;
                Inverted = invert;
            }
        }
    }

    public enum CopiedLeverType
    {
        IndependentBrakeShunter,
        TrainBrakeShunter,
        ThrottleShunter,
        ReverserShunter,
        HornShunter,

        IndependentBrakeDE6,
        TrainBrakeDE6,
        ThrottleDE6,
        ReverserDE6,
        HornDE6,
    }
}
