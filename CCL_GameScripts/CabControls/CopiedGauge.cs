using System.Collections;
using UnityEngine;
using CCL_GameScripts.Attributes;

namespace CCL_GameScripts.CabControls
{
    public enum CopiedGaugeType
    {
        ShunterBrakePipeMeter,
        ShunterBrakeAuxResMeter,
        ShunterSpeedometer,
        ShunterEngineTempMeter,
        ShunterSandMeter,
        ShunterFuelMeter,
        ShunterOilMeter,

        DE6BrakePipeMeter,
        DE6BrakeResMeter,
        DE6IndBrakeMeter,
        DE6IndBrakeResMeter,
        DE6EngineRPMMeter,
        DE6EngineTempMeter,
        DE6Ammeter,
        DE6Speedometer,
        DE6SandMeter,
        DE6FuelMeter,
        DE6OilMeter,
    }

    public class CopiedGauge : CopiedCabIndicator, IProxyScript
    {
        public string TargetTypeName => "IndicatorGauge";

        public bool IsOverrideSet( int index ) => (index == 1) && OverrideScaleLimits;

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

                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I brake_aux_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I brake_res_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I ind_brake_aux_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I ind_brake_res_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I rpm_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I temperature_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I voltage_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I speedometer"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I sand_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I fuel_meter"),
                (BaseTrainCarType.LocoDiesel, "offset/I Indicator meters/I oil_meter"),
            };

        protected static readonly GaugeGizmoInfo[] GizmoData =
            new[]
            {
                // shunter
                new GaugeGizmoInfo(-200, 20, 0.025f),   // brake pipe
                new GaugeGizmoInfo(-200, 20, 0.025f),   // brake res
                new GaugeGizmoInfo(-226, 46, 0.05f),    // speedometer

                new GaugeGizmoInfo(-225, 45, 0.045f),   // temp
                new GaugeGizmoInfo(-225, 45, 0.025f),   // sand
                new GaugeGizmoInfo(-225, 45, 0.025f),   // fuel
                new GaugeGizmoInfo(-225, 45, 0.025f),   // oil

                // DE6
                new GaugeGizmoInfo(-205, 25, 0.025f),   // brake pipe
                new GaugeGizmoInfo(-205, 25, 0.025f),   // brake res
                new GaugeGizmoInfo(-205, 25, 0.025f),   // ind pipe
                new GaugeGizmoInfo(-205, 25, 0.025f),   // ind res

                new GaugeGizmoInfo(-225, 43, 0.04f),    // rpm
                new GaugeGizmoInfo(-225, 43, 0.04f),    // temp
                new GaugeGizmoInfo(-225, 43, 0.04f),    // ammeter
                new GaugeGizmoInfo(-226, 46, 0.05f),    // speedometer

                new GaugeGizmoInfo(-15, 115, 0.025f),   // sand
                new GaugeGizmoInfo(-15, 115, 0.025f),   // fuel
                new GaugeGizmoInfo(-15, 115, 0.025f),   // oil
            };

        public CopiedGaugeType GaugeType;

        [Header("Dial Limits")]
        public bool OverrideScaleLimits = false; // override 1

        [ProxyField("minValue", 1)]
        public float MinValue = 0;
        [ProxyField("maxValue", 1)]
        public float MaxValue = 4;

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
}