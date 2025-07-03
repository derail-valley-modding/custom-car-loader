using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    [AddComponentMenu("CCL/Proxies/Controls/Gizmo Proxy")]
    public class GizmoProxy : ControlSpecProxy
    {
        public enum ItemCollisionSoundCategory
        {
            Generic,
            NeverPlayCollisionSounds,
            Shovel,
            Coal
        }

        public bool behaveAsItem;
        public bool carryingPosition;
        public bool precisionGrab;

        [Header("VR")]
        public bool telegrabbable;

        [Header("Audio")]
        public AudioClip collision = null!;
        public ItemCollisionSoundCategory itemCollisionSoundCategory;
        public ItemCollisionSoundCategory ignoredCollisionSoundCategory;
    }
}
