using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Vehicle Audio Controller")]
    public class VehicleAudioController : MonoBehaviour
    {
        public Transform? CustomWheelAudioPosition1;
        public Transform? CustomWheelAudioPosition2;

        public Transform? BrakeCylinderExhaustPosition;
        public Transform? BrakeAirflowPosition;
    }
}
