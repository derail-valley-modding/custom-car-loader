using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Proxies
{
    public class TeleportArcPassThroughProxy : MonoBehaviour
    {
        public bool twoSided = true;
        [SerializeField]
        private Collider[] collidersToPassThrough = new Collider[0];
        [NonSerialized]
        public HashSet<Collider> colliders = new HashSet<Collider>();
    }
}
