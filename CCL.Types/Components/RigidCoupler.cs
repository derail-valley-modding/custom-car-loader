using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Rigid Coupler")]
    public class RigidCoupler : MonoBehaviour
    {
        public CouplerDirection Direction;

        public bool AlwaysRigid = false;
        [CarKindField]
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];
    }
}
