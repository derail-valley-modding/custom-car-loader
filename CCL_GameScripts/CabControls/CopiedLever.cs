using CCL_GameScripts.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
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
        MainBreakerSwitchDE6,
        EngineThrottleDE6,

        ThrottleSH282,
        LightLeverSH282,
        SandValveSH282,
    }

    public class CopiedLever : CopiedCabInput, IProxyScript
    {
        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new []
            {
                // DE2
                (BaseTrainCarType.LocoShunter, "C independent_brake_lever"),
                (BaseTrainCarType.LocoShunter, "C train_brake_lever"),
                (BaseTrainCarType.LocoShunter, "C throttle"),
                (BaseTrainCarType.LocoShunter, "C reverser"),
                (BaseTrainCarType.LocoShunter, "C horn"),

                // DE6
                (BaseTrainCarType.LocoDiesel, "offset/C independent_brake_lever"),
                (BaseTrainCarType.LocoDiesel, "offset/C train_brake_lever"),
                (BaseTrainCarType.LocoDiesel, "offset/C throttle"),
                (BaseTrainCarType.LocoDiesel, "offset/C reverser"),
                (BaseTrainCarType.LocoDiesel, "offset/C horn"),
                (BaseTrainCarType.LocoDiesel, "offset/C fusebox controls/C main_battery_switch"),
                (BaseTrainCarType.LocoDiesel, "offset/C engine controls/C engine_thottle"),

                // SH282
                (BaseTrainCarType.LocoSteamHeavy, "C throttle regulator"),
                (BaseTrainCarType.LocoSteamHeavy, "C light lever"),
                (BaseTrainCarType.LocoSteamHeavy, "C sand valve"),
            };

        protected static readonly LeverGizmoInfo[] GizmoData =
            new[]
            {
                // DE2
                new LeverGizmoInfo(-61, 0, 20),
                new LeverGizmoInfo(0, 72, 20),
                new LeverGizmoInfo(-52, 1, 8, true),
                new LeverGizmoInfo(-25, 65, 3, true),
                new LeverGizmoInfo(-27, 27, 3),

                // DE6
                new LeverGizmoInfo(-88.429f, 0, 20, true),
                new LeverGizmoInfo(-90, 0, 20, true),
                new LeverGizmoInfo(-78.815f, 0, 8, true),
                new LeverGizmoInfo(-65.2f, 0, 3, true),
                new LeverGizmoInfo(-12, 12, 3),
                new LeverGizmoInfo(0, 180, 2),
                new LeverGizmoInfo(0, 22, 0, axis: Vector3.down),

                // SH282
                new LeverGizmoInfo(75.82f, 90, 15, true),
                new LeverGizmoInfo(-60, 60, 5, true, Vector3.right),
                new LeverGizmoInfo(0, 60, 20, true, Vector3.right),
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

        public string TargetTypeName => "DV.CabControls.Spec.Lever";
        public bool IsOverrideSet(int index) => 
            ((index == 1) && OverrideNotches) || 
            ((index == 2) && OverrideDirection);

        public bool OverrideNotches = false;

        [ProxyField("useSteppedJoint", 1)]
        public bool UseNotches = true; // useSteppedJoint
        [ProxyField(1)]
        public int notches = 20;

        public bool OverrideDirection = false;
        [ProxyField(2)]
        public bool invertDirection = false;

        private void OnDrawGizmos()
        {
            var gizmo = GizmoData[(int)LeverType];

            Color startColor = gizmo.Inverted ? END_COLOR : START_COLOR;
            Color endColor = gizmo.Inverted ? START_COLOR : END_COLOR;

            Vector3 axis = gizmo.Axis ?? Vector3.up;
            Quaternion axisTransform = Quaternion.FromToRotation(Vector3.up, axis);
            Vector3 startVector = axisTransform * Vector3.forward;

            float rayCount = gizmo.Notches - 1;

            // draw ray segments
            for ( int i = 0; i <= rayCount; i++ )
            {
                Color segmentColor = Color.Lerp(startColor, endColor, i / rayCount);
                Vector3 rayVector = Quaternion.AngleAxis(
                    Mathf.Lerp(gizmo.LimitMin, gizmo.LimitMax, i / rayCount), axis)
                    * startVector * GIZMO_RADIUS;
                rayVector = transform.TransformPoint(rayVector);

                Gizmos.color = segmentColor;
                Gizmos.DrawLine(transform.position, rayVector);
            }
        }

        protected class LeverGizmoInfo
        {
            public Vector3? Axis;
            public float LimitMin;
            public float LimitMax;
            public int Notches;
            public bool Inverted;

            public LeverGizmoInfo(float min, float max, int notches, bool invert = false, Vector3? axis = null)
            {
                LimitMin = min;
                LimitMax = max;
                Notches = notches;
                Inverted = invert;
                Axis = axis;
            }
        }
    }
}
