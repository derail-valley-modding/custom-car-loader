using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public class LocoControlsReaderProxy : MonoBehaviour
    {
        public GameObject cabLight = null!;
        public GameObject wipers = null!;
        public GameObject fuelCutoff = null!;
        public GameObject indHeadlights1Front = null!;
        public GameObject indHeadlights2Front = null!;
        public GameObject indHeadlightsTypeRear = null!;
        public GameObject indHeadlights1Rear = null!;
        public GameObject indHeadlights2Rear = null!;
        public GameObject indWipers1 = null!;
        public GameObject indWipers2 = null!;
        public GameObject indCabLight = null!;
        public GameObject indDashLight = null!;
        public GameObject headlightsFront = null!;
        public GameObject headlightsRear = null!;
        public GameObject gearboxA = null!;
        public GameObject gearboxB = null!;

        [Header("Steam")]
        public GameObject cylCock = null!;
        public GameObject injector = null!;
        public GameObject firedoor = null!;
        public GameObject blower = null!;
        public GameObject damper = null!;
        public GameObject blowdown = null!;
        public GameObject coalDump = null!;
        public GameObject dynamo = null!;
        public GameObject airPump = null!;
        public GameObject lubricator = null!;
        public GameObject bell = null!;
    }
}
