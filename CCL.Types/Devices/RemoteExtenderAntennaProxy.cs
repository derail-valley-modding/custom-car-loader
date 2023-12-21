using UnityEngine;

namespace CCL.Types.Devices
{
    public class RemoteExtenderAntennaProxy : MonoBehaviour
    {
        // remote antenna dimensions
        private const float ANTENNA_WIDTH = 0.160f;
        private const float ANTENNA_HEIGHT = 0.056f;
        private const float ANTENNA_DEPTH = 0.266f;

        private const float ANTENNA_POLE_X = -0.147f;
        private const float ANTENNA_POLE_Y = 0.114f;
        private const float ANTENNA_POLE_Z = 0.040f;
        private const float ANTENNA_POLE_RADIUS = 0.014f;
        private const float ANTENNA_POLE_HEIGHT = 0.173f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(0, ANTENNA_HEIGHT / 2, 0);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(ANTENNA_DEPTH, ANTENNA_HEIGHT, ANTENNA_WIDTH));

            center = new Vector3(ANTENNA_POLE_X, ANTENNA_POLE_Y, ANTENNA_POLE_Z);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(ANTENNA_POLE_RADIUS, ANTENNA_POLE_HEIGHT, ANTENNA_POLE_RADIUS));

            center = new Vector3(ANTENNA_POLE_X, ANTENNA_POLE_Y, -ANTENNA_POLE_Z);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(ANTENNA_POLE_RADIUS, ANTENNA_POLE_HEIGHT, ANTENNA_POLE_RADIUS));
        }
    }
}
