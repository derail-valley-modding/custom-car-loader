using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL_GameScripts
{
    public static class ModelUtil
    {
        /// <summary>
        /// Gets the vector from an object's transform origin to the center of its bounding box
        /// </summary>
        public static Vector3 GetModelCenterline( GameObject gameObject )
        {
            Vector3 massCenter = Vector3.forward;

            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            if( renderers.Length > 0 )
            {
                massCenter = Vector3.zero;

                foreach( Renderer r in renderers )
                {
                    //Vector3 center = r.transform.InverseTransformPoint(r.bounds.center);
                    massCenter += r.bounds.center;
                }

                massCenter /= renderers.Length;
            }

            if( massCenter == Vector3.zero ) return Vector3.forward;

            return massCenter;
        }

        /// <summary>
        /// Gets the centerline vector of a model, and its projection on the plane perpendicular to <paramref name="axis"/>
        /// </summary>
        /// <returns>(center vector, projection vector)</returns>
        public static (Vector3, Vector3) GetModelCenterHingeProjection( GameObject gameObject, Vector3 axis )
        {
            Vector3 massZeroVector = GetModelCenterline(gameObject);
            Vector3 gizmoZeroVector = Vector3.ProjectOnPlane(massZeroVector, axis);

            if( gizmoZeroVector == Vector3.zero )
            {
                gizmoZeroVector = Vector3.forward;
            }

            gizmoZeroVector = gizmoZeroVector.normalized;
            return (gizmoZeroVector, massZeroVector);
        }


        public static Transform FindSafe(this Transform tform, string name)
        {
            var names = new[] { name, $"{name} 1", $"{name} " };

            foreach (var attempt in names)
            {
                try
                {
                    var result = tform.Find(attempt);
                    if (result) return result;
                }
                catch (NullReferenceException)
                {

                }
            }

            return null;
        }
    }
}
