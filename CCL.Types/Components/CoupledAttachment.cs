using UnityEngine;

namespace CCL.Types.Components
{
    public class CoupledAttachment : MonoBehaviour
    {
        [Tooltip("The object that will move/rotate")]
        public Transform MovedObject = null!;
        public string ConnectionTag = string.Empty;
        [Tooltip("On which coupled vehicle to look for the tag")]
        public CouplerDirection Direction = CouplerDirection.Front;
        [Tooltip("The direction of the coupled vehicle")]
        public CouplerDirection OtherDirection = CouplerDirection.Rear;

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
