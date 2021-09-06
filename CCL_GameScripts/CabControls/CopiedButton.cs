using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public enum CopiedButtonType
    {
        FlatRed,
        FlatBlue,

        EmergencyMushroom,
        RecessedYellow,
        RecessedBlue
    }

    public class CopiedButton : CopiedCabInput, IProxyScript
    {
        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new[]
            {
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/C emergency_engine_off button"),
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/C deploy_sand button"),

                (BaseTrainCarType.LocoDiesel, "offset/C emergency_engine_off button"),
                (BaseTrainCarType.LocoDiesel, "offset/C deploy_sand button"),
                (BaseTrainCarType.LocoDiesel, "offset/C bell button"),
            };

        public CopiedButtonType ButtonType;

        public override (BaseTrainCarType, string) GetSourceObject()
        {
            return TargetObjects[(int)ButtonType];
        }
        public CopiedButton()
        {
            ReplaceThisObject = true;
        }

        protected const int GIZMO_SEGMENTS = 40;
        protected const int GIZMO_RADIAL_DIVISOR = 5;

        protected static readonly (float, float, Color)[] GizmoData =
            new[]
            {
                // DE2
                (0.015f, 0.006f, new Color(0.6f, 0, 0)),    // flat red
                (0.015f, 0.006f, new Color(0, 0.5f, 0.6f)),  // flat blue

                // DE6
                (0.023f, 0.017f, new Color(0.6f, 0, 0)),       // shroom
                (0.013f, 0.009f, new Color(0.6f, 0.6f, 0)),    // recess yellow
                (0.013f, 0.009f, new Color(0, 0.4f, 0.6f)),    // recess blue
            };

        public string TargetTypeName => "DV.CabControls.Spec.Button";

        public bool IsOverrideSet( int index ) => false;

        [ProxyField("isToggle")]
        public bool IsToggle = false;

        private void OnDrawGizmos()
        {
            (float radius, float depth, Color color) = GizmoData[(int)ButtonType];

            Vector3 lastVector = transform.position;
            Vector3 backPlaneOffset = Vector3.forward * depth;
            Vector3 lastBackVector = transform.TransformPoint(Vector3.back * depth);

            for( int i = 0; i <= GIZMO_SEGMENTS; i++ )
            {
                Gizmos.color = color;
                Vector3 radialVector = (Quaternion.AngleAxis(
                    Mathf.Lerp(0, 360, (float)i / GIZMO_SEGMENTS), Vector3.forward)
                    * Vector3.right).normalized;

                Vector3 nextVector = transform.TransformPoint(radialVector * radius);
                Vector3 nextBackVector = transform.TransformPoint(radialVector * radius + backPlaneOffset);

                if( i != 0 )
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                    Gizmos.DrawLine(lastBackVector, nextBackVector);
                }
                Gizmos.DrawLine(nextVector, nextBackVector);

                if( (i % GIZMO_RADIAL_DIVISOR) == 0 )
                {
                    Gizmos.DrawLine(transform.position, nextVector);
                }

                lastVector = nextVector;
                lastBackVector = nextBackVector;
            }
        }
    }
}
