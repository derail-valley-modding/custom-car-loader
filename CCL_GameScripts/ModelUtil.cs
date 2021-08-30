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
            Vector3 massZeroVector = Vector3.forward;

            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            if( renderers.Length > 0 )
            {
                massZeroVector = Vector3.zero;

                foreach( Renderer r in renderers )
                {
                    Vector3 center = gameObject.transform.InverseTransformPoint(r.bounds.center);
                    massZeroVector += center;
                }
            }

            if( massZeroVector == Vector3.zero ) return Vector3.forward;

            return massZeroVector;
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
    }
}
