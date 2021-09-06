using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public enum CopiedSwitchType
    {
        RedFuseShunter,
        BlackFuseShunter,
        RockerSwitchShunter,

        FanSwitchDE6,
        BlackFuseDE6,
    }

    public class CopiedToggle : CopiedCabInput
    {
        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new[]
            {
                (BaseTrainCarType.LocoShunter, "C fuse switches/fuse 1 main"),
                (BaseTrainCarType.LocoShunter, "C fuse switches/fuse 3 main"),
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/C fan switch"),

                (BaseTrainCarType.LocoDiesel, "offset/C fan switch"),
                (BaseTrainCarType.LocoDiesel, "offset/C fusebox controls/C fuse_1_switch"),
            };

        public CopiedSwitchType SwitchType;

        public override (BaseTrainCarType, string) GetSourceObject()
        {
            return TargetObjects[(int)SwitchType];
        }

        protected static readonly ToggleGizmoInfo[] GizmoData =
            new[]
            {
                new ToggleGizmoInfo(-52, 32, Vector3.down),
                new ToggleGizmoInfo(-52, 32, Vector3.down),
                new ToggleGizmoInfo(-10, 20, Vector3.up),

                new ToggleGizmoInfo(-10, 20, Vector3.up),
                new ToggleGizmoInfo(-52, 32, Vector3.down),
            };

        protected const float GIZMO_RADIUS = 0.05f;
        protected const int GIZMO_SEGMENTS = 10;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmos()
        {
            var gizmo = GizmoData[(int)SwitchType];
            Vector3 lastVector = transform.position;

            // draw ray segments
            for( int i = 0; i <= GIZMO_SEGMENTS; i++ )
            {
                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);

                float rotateAngle = Mathf.Lerp(gizmo.LimitMin, gizmo.LimitMax, (float)i / GIZMO_SEGMENTS);
                Quaternion rotation = Quaternion.AngleAxis(rotateAngle, gizmo.Axis);

                // projected sweep
                Vector3 nextVector = transform.TransformPoint((rotation * Vector3.forward) * GIZMO_RADIUS);

                if( i == 0 || i == GIZMO_SEGMENTS )
                {
                    Gizmos.DrawLine(transform.position, nextVector);
                }
                if( i != 0 )
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                }

                lastVector = nextVector;
            }
        }

        protected class ToggleGizmoInfo
        {
            public float LimitMin;
            public float LimitMax;
            public Vector3 Axis;

            public ToggleGizmoInfo( float min, float max, Vector3 axis )
            {
                LimitMin = min;
                LimitMax = max;
                Axis = axis;
            }
        }
    }
}