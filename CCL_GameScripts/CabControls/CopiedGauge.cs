using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class CopiedGauge : CopiedCabIndicator
    {
        protected static readonly (BaseTrainCarType, string)[] TargetObjects =
            new[]
            {
                (BaseTrainCarType.LocoShunter, "I brake_pipe_meter"),
                (BaseTrainCarType.LocoShunter, "I brake_aux_res_meter"),
                (BaseTrainCarType.LocoShunter, "I speedometer"),
                (BaseTrainCarType.LocoShunter, "C dashboard indicators controller/I engine_temp_meter"),
                (BaseTrainCarType.LocoShunter, "C dashboard indicators controller/I sand_meter"),
                (BaseTrainCarType.LocoShunter, "C dashboard indicators controller/I fuel_meter"),
                (BaseTrainCarType.LocoShunter, "C dashboard indicators controller/I oil_meter"),
            };

        protected static readonly GaugeGizmoInfo[] GizmoData =
            new[]
            {
                new GaugeGizmoInfo(-200, 20, 0.025f),
                new GaugeGizmoInfo(-200, 20, 0.025f),
                new GaugeGizmoInfo(-226, 46, 0.05f),

                new GaugeGizmoInfo(-225, 45, 0.045f),
                new GaugeGizmoInfo(-225, 45, 0.025f),
                new GaugeGizmoInfo(-225, 45, 0.025f),
                new GaugeGizmoInfo(-225, 45, 0.025f),
            };

        public CopiedGaugeType GaugeType;

        public override (BaseTrainCarType, string) GetSourceObject()
        {
            return TargetObjects[(int)GaugeType];
        }

        public CopiedGauge()
        {
            ReplaceThisObject = true;
        }

        protected const int GIZMO_SEGMENTS = 40;
        protected static readonly Color START_COLOR = new Color(0.65f, 0, 0);
        protected static readonly Color END_COLOR = new Color(0, 0.65f, 0);

        private void OnDrawGizmosSelected()
        {
            var gizmo = GizmoData[(int)GaugeType];

            Vector3 lastVector = transform.position;
            for( int i = 0; i <= GIZMO_SEGMENTS; i++ )
            {
                Gizmos.color = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);
                Vector3 nextVector = Quaternion.AngleAxis(
                    Mathf.Lerp(gizmo.LimitMin, gizmo.LimitMax, (float)i / GIZMO_SEGMENTS), Vector3.up)
                    * Vector3.right * gizmo.Radius;
                nextVector = transform.TransformPoint(nextVector);

                Gizmos.DrawLine(transform.position, nextVector);
                if( i != 0 )
                {
                    Gizmos.DrawLine(lastVector, nextVector);
                }

                lastVector = nextVector;
            }
        }

        protected class GaugeGizmoInfo
        {
            public float LimitMin;
            public float LimitMax;
            public float Radius;

            public GaugeGizmoInfo( float min, float max, float radius )
            {
                LimitMin = min;
                LimitMax = max;
                Radius = radius;
            }
        }
    }

    public enum CopiedGaugeType
    {
        DE2BrakePipeMeter,
        DE2BrakeAuxResMeter,
        DE2Speedometer,
        DE2EngineTempMeter,
        DE2SandMeter,
        DE2FuelMeter,
        DE2OilMeter
    }
}