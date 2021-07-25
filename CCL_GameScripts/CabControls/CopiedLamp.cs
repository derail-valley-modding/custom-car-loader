using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class CopiedLamp : CopiedCabControl
    {
        public enum CopiedLampType
        {
            DE2SmallRed,
            DE2SmallBlue,
            DE2SmallYellow,
            DE2LargeRed,
            DE2LargeGreen
        }

        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new[]
            {
                (BaseTrainCarType.LocoShunter, "I brake_aux_res_lamp"),
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/I fan_lamp"),
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/I deploy_sand_lamp"),
                (BaseTrainCarType.LocoShunter, "C dashboard indicators controller/I service_engine_lamp"),
                (BaseTrainCarType.LocoShunter, "C dashboard buttons controller/I power_fuse_lamp"),
            };

        protected static readonly (float, Color)[] GizmoData =
            new[]
            {
                (0.011f, new Color(0.6f, 0, 0)),
                (0.01f, new Color(0, 0, 0.6f)),
                (0.009f, new Color(0.6f, 0.6f, 0)),
                (0.02f, new Color(0.6f, 0, 0)),
                (0.02f, new Color(0, 0.6f, 0)),
            };


        public SimEventType SimBinding;
        public CopiedLampType LampType;

        [Header("Simulation Binding")]
        public SimThresholdDirection ThresholdDirection;
        public SimAmount SolidThreshold;
        public bool UseBlinkMode;
        public SimAmount BlinkThreshold;

        public override (BaseTrainCarType, string) GetSourceObject()
        {
            return TargetObjects[(int)LampType];
        }

        public CopiedLamp()
        {
            ReplaceThisObject = true;
        }

        protected const int GIZMO_SEGMENTS = 40;
        //protected static readonly Color GIZMO_COLOR = new Color(0.65f, 0, 0);

        private void OnDrawGizmosSelected()
        {
            (float radius, Color color) = GizmoData[(int)LampType];

            Vector3 lastVector = transform.position;
            for( int i = 0; i <= GIZMO_SEGMENTS; i++ )
            {
                Gizmos.color = color;
                Vector3 nextVector = Quaternion.AngleAxis(
                    Mathf.Lerp(0, 360, (float)i / GIZMO_SEGMENTS), Vector3.forward)
                    * Vector3.right * radius;
                nextVector = transform.TransformPoint(nextVector);

                if( i != 0 )
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                }

                lastVector = nextVector;
            }
        }
    }
}