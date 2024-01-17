using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class ControlSpecProxy : MonoBehaviour
    {
        [Header("Common")]
        public bool disallowShortTriggerLockHold;
        public GameObject[] colliderGameObjects;
        public InteractionHandPosesProxy handPosesOverride;

        public abstract int CopiedControlIndex { get; }

        public abstract CopiedControlDescriptor[] CopiedControlData { get; }
    }

    public class CopiedControlDescriptor
    {
        public BaseTrainCarType CarType;
        public string TransformPath;

        public CopiedControlDescriptor(BaseTrainCarType carType, string transformPath)
        {
            CarType = carType;
            TransformPath = transformPath;
        }
    }
}
