using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts
{
    [RequireComponent(typeof(TrainCarSetup))]
    public class BrakeCarSetup : ComponentInitSpec, ISimSetup
    {
        public LocoParamsType SimType => LocoParamsType.Caboose;

        public override string TargetTypeName => "DVCustomCarLoader.LocoComponents.Utility.BrakeCarController";
        public override bool DestroyAfterCreation => true;

        [ProxyField]
        public bool HasRemoteRangeExtender = false;
    }

    public class BrakeCarInteriorSetup : MonoBehaviour
    {
        public Transform CareerManagerLocation = null;

        [Header("Locomotive Remote Gadgets")]
        public Transform RemoteChargerLocation = null;

        public Transform RemoteExtenderAntennaLocation = null;

        // career manager dimensions
        private const float CM_HEIGHT = 1.765f;
        private const float CM_BOTTOM_HEIGHT = 0.946f;
        private const float CM_TOP_HEIGHT = 0.819f;
        private const float CM_WIDTH = 0.722f;
        private const float CM_DEPTH = 0.568f;
        private const float CM_TOP_DEPTH = 0.391f;

        // remote charger dimensions
        private const float CHARGE_WIDTH = 0.421f;
        private const float CHARGE_HEIGHT = 0.092f;
        private const float CHARGE_DEPTH = 0.385f;

        private const float CHARGE_BUMP_DEPTH = 0.110f;
        private const float CHARGE_BUMP_HEIGHT = 0.050f;

        // remote antenna dimensions
        private const float ANTENNA_WIDTH = 0.160f;
        private const float ANTENNA_HEIGHT = 0.056f;
        private const float ANTENNA_DEPTH = 0.266f;

        private const float ANTENNA_POLE_X = -0.147f;
        private const float ANTENNA_POLE_Y = 0.114f;
        private const float ANTENNA_POLE_Z = 0.040f;
        private const float ANTENNA_POLE_RADIUS = 0.014f;
        private const float ANTENNA_POLE_HEIGHT = 0.173f;

        private void DrawCubeScaled(Transform pivot, Vector3 center, Vector3 size)
        {
            Gizmos.matrix = pivot.localToWorldMatrix;
            Gizmos.DrawWireCube(center, size);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 center;

            if (CareerManagerLocation)
            {
                center = new Vector3(0, CM_BOTTOM_HEIGHT / 2, 0);
                DrawCubeScaled(CareerManagerLocation, center, new Vector3(CM_DEPTH, CM_BOTTOM_HEIGHT, CM_WIDTH));

                center = new Vector3((CM_TOP_DEPTH / 2) - (CM_DEPTH / 2), CM_HEIGHT - (CM_TOP_HEIGHT / 2), 0);
                DrawCubeScaled(CareerManagerLocation, center, new Vector3(CM_TOP_DEPTH, CM_TOP_HEIGHT, CM_WIDTH));
            }

            if (RemoteChargerLocation)
            {
                center = new Vector3(0, CHARGE_HEIGHT / 2, 0);
                DrawCubeScaled(RemoteChargerLocation, center, new Vector3(CHARGE_DEPTH, CHARGE_HEIGHT, CHARGE_WIDTH));

                center = new Vector3((CHARGE_BUMP_DEPTH / 2) - (CHARGE_DEPTH / 2), CHARGE_HEIGHT + (CHARGE_BUMP_HEIGHT / 2), 0);
                DrawCubeScaled(RemoteChargerLocation, center, new Vector3(CHARGE_BUMP_DEPTH, CHARGE_BUMP_HEIGHT, CHARGE_WIDTH));
            }

            if (RemoteExtenderAntennaLocation)
            {
                center = new Vector3(0, ANTENNA_HEIGHT / 2, 0);
                DrawCubeScaled(RemoteExtenderAntennaLocation, center, new Vector3(ANTENNA_DEPTH, ANTENNA_HEIGHT, ANTENNA_WIDTH));

                center = new Vector3(ANTENNA_POLE_X, ANTENNA_POLE_Y, ANTENNA_POLE_Z);
                DrawCubeScaled(RemoteExtenderAntennaLocation, center, new Vector3(ANTENNA_POLE_RADIUS, ANTENNA_POLE_HEIGHT, ANTENNA_POLE_RADIUS));

                center = new Vector3(ANTENNA_POLE_X, ANTENNA_POLE_Y, -ANTENNA_POLE_Z);
                DrawCubeScaled(RemoteExtenderAntennaLocation, center, new Vector3(ANTENNA_POLE_RADIUS, ANTENNA_POLE_HEIGHT, ANTENNA_POLE_RADIUS));
            }
        }
    }
}
