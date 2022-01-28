using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL_GameScripts
{
    public class FireboxSetup : MonoBehaviour
    {
        protected static readonly Vector3 FIREBOX_OFFSET = new Vector3(0, -0.282f, -1.335f);
        protected static readonly Vector3 FIREBOX_COLLIDER_CENTER = new Vector3(0, 2.18f, 2.051f);

        public void OnDrawGizmos()
        {
            // coal collider
            Gizmos.color = Color.red;
            Vector3 coalSize = transform.TransformVector(Vector3.one);
            Vector3 coalPos = transform.TransformPoint(FIREBOX_COLLIDER_CENTER);
            Gizmos.DrawWireCube(coalPos, coalSize);
        }
    }
}
