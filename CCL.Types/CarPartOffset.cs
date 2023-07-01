using UnityEngine;

namespace CCL.Types
{
    public static class CarPartOffset
    {
        public static readonly Vector3 HOOK_PLATE_F = new Vector3(0, -0.07841396f, -0.3319998f);
        public static readonly Vector3 HOOK_PLATE_R = new Vector3(0, -0.07841396f, 0.3319998f);

        public static readonly Vector3 BUFFER_PAD_F = new Vector3(0, 0, 0.2145f);
        public static readonly Vector3 BUFFER_PAD_R = new Vector3(0, 0, -0.2145f);

        public static readonly Vector3 COUPLER_FRONT = new Vector3(0, 0, 0.249f);
        public static readonly Vector3 COUPLER_REAR = new Vector3(0, 0, -0.249f);

        public static readonly Vector3 FIREBOX_OFFSET = new Vector3(0, -0.282f, -1.335f);
        public static readonly Vector3 FIREBOX_COLLIDER_CENTER = new Vector3(0, -1.898f, -0.716f);

        public static readonly Vector3 CENTER_TO_LEFT_RAIL = new Vector3(-0.75f, 0, 0);
        public static readonly Vector3 CENTER_TO_RIGHT_RAIL = new Vector3(0.75f, 0, 0);
    }
}
