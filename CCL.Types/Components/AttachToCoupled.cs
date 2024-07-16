using UnityEngine;

namespace CCL.Types.Components
{
    public class AttachToCoupled : MonoBehaviour
    {
        [Tooltip("Connect when coupled to the following direction")]
        public CouplingDirection Direction = CouplingDirection.Front;
        [Tooltip("Connect when coupled to the other car's direction")]
        public CouplingDirection OtherDirection = CouplingDirection.Rear;
        [Tooltip("How this object will behave")]
        public ConnectionMode Mode;
        [Tooltip("Hide gameobject when not uncoupled")]
        public bool HideWhenUncoupled = false;
        [Tooltip("The path to the attachment point")]
        public string TransformPath = string.Empty;

    }

    public enum CouplingDirection
    {
        Front,
        Rear
    }

    public enum ConnectionMode
    {
        [Tooltip("Object will pivot to look towards the attachment point")]
        Rigid,
        [Tooltip("Object will move to the attach point position")]
        Attach,
        [Tooltip("Object will meet another object halfway")]
        HalfMeet
    }
}
