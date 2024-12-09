using UnityEngine;

namespace CCL.Types.Proxies.Ports
{
    public class AnimatorPortReaderProxy : MonoBehaviour
    {
        public enum UpdateType
        {
            SET_NORMALIZED_TIME,
            SET_PARAMETER
        }

        public UpdateType updateType;
        [PortId(null, null, false)]
        public string portId;
        //[ShowIf("updateType", UpdateType.SET_PARAMETER)]
        public string parameterName;

        [Header("Value modifiers")]
        public float valueMultiplier = 1f;
        public float valueOffset;
    }
}
