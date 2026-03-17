using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Keep Coupled Interior Loaded")]
    public class KeepCoupledInteriorLoaded : MonoBehaviour
    {
        public bool KeepFrontCoupledLoaded = true;
        public bool KeepRearCoupledLoaded = false;
    }
}
