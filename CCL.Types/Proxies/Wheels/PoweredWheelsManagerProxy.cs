﻿using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class PoweredWheelsManagerProxy : MonoBehaviour
    {
        public PoweredWheelProxy[] poweredWheels;
        public bool GetWheelsFromDefaultBogies = true;
    }

    public class PoweredWheelProxy : MonoBehaviour
    {
        public Transform wheelTransform;
        public Vector3 localRotationAxis = Vector3.right;
    }
}
