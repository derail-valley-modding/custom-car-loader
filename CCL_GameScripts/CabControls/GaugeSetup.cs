using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class GaugeSetup : IndicatorSetupBase
    {
        public override string TargetTypeName => "IndicatorGauge";

		[ProxyField("unclamped")]
        public bool Unclamped = false;
		[ProxyField("minAngle")]
		public float MinAngle = -180;
		[ProxyField("maxAngle")]
		public float MaxAngle = 180;

		[ProxyField("rotationAxis")]
		public Vector3 RotationAxis = Vector3.forward;
		[ProxyField("needle")]
		public Transform Needle;

		protected const float GIZMO_RADIUS = 0.1f;
		protected const int GIZMO_SEGMENTS = 20;
		protected static readonly Color START_COLOR = new Color(0, 0, 0.65f);
		protected static readonly Color END_COLOR = new Color(0.65f, 0, 0);

		private void OnDrawGizmos()
		{
			if( !Needle )
			{
				return;
			}

			Vector3 lastVector = Vector3.zero;
			for( int i = 0; i <= GIZMO_SEGMENTS; i++ )
			{
				Color segmentColor = Color.Lerp(START_COLOR, END_COLOR, (float)i / GIZMO_SEGMENTS);
				Vector3 nextVector = Quaternion.AngleAxis(Mathf.Lerp(MinAngle, MaxAngle, (float)i / GIZMO_SEGMENTS), RotationAxis) * Vector3.right * GIZMO_RADIUS;
				nextVector = transform.TransformPoint(nextVector);

				if( i == 0 || i == GIZMO_SEGMENTS )
				{
					Debug.DrawLine(Needle.position, nextVector, segmentColor, 0f, false);
				}

				if( i != 0 )
				{
					Debug.DrawLine(lastVector, nextVector, segmentColor, 0f, false);
				}

				lastVector = nextVector;
			}
		}
	}
}