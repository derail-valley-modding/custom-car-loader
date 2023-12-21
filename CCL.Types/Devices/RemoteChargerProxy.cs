using UnityEngine;

namespace CCL.Types.Devices
{
    public class RemoteChargerProxy : MonoBehaviour
    {
        // remote charger dimensions
        private const float CHARGE_WIDTH = 0.421f;
        private const float CHARGE_HEIGHT = 0.092f;
        private const float CHARGE_DEPTH = 0.385f;

        private const float CHARGE_BUMP_DEPTH = 0.110f;
        private const float CHARGE_BUMP_HEIGHT = 0.050f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(0, CHARGE_HEIGHT / 2, 0);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(CHARGE_DEPTH, CHARGE_HEIGHT, CHARGE_WIDTH));

            center = new Vector3((CHARGE_BUMP_DEPTH / 2) - (CHARGE_DEPTH / 2), CHARGE_HEIGHT + (CHARGE_BUMP_HEIGHT / 2), 0);
            GizmoUtil.DrawLocalPrism(transform, center, new Vector3(CHARGE_BUMP_DEPTH, CHARGE_BUMP_HEIGHT, CHARGE_WIDTH));
        }
    }
}
