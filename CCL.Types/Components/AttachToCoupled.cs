using UnityEngine;

namespace CCL.Types.Components
{
    public class AttachToCoupled : MonoBehaviour
    {
        [Header("Connections")]
        [Tooltip("If specified, will attempt to connect to a transform at this path in the front vehicle when that vehicle's front connects to this vehicle's front")]
        public string FrontConnectionTransformFront = string.Empty;
        [Tooltip("If specified, will attempt to connect to a transform at this path in the front vehicle when that vehicle's rear connects to this vehicle's front")]
        public string FrontConnectionTransformRear = string.Empty;
        [Tooltip("If specified, will attempt to connect to a transform at this path in the rear vehicle when that vehicle's front connects to this vehicle's rear")]
        public string RearConnectionTransformFront = string.Empty;
        [Tooltip("If specified, will attempt to connect to a transform at this path in the rear vehicle when that vehicle's rear connects to this vehicle's rear")]
        public string RearConnectionTransformRear = string.Empty;

        [Header("Behaviour")]
        [Tooltip("How this object will behave")]
        public ConnectionMode Mode;
        [Tooltip("Hide gameobject when not uncoupled")]
        public bool HideWhenUncoupled = false;
    }

    public enum ConnectionMode
    {
        [Tooltip("Object will pivot to look towards the attachment point")]
        Rigid,
        [Tooltip("Object will move to the attach point position")]
        Attach,
        [Tooltip("Object will meet another object halfway\n" +
            "You should ensure this object and the target face eachother")]
        HalfMeet
    }
}
