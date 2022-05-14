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

        public static void SetLayersRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].gameObject.layer = layer;
            }
        }

        public static void SetLayersRecursive(GameObject go, DVLayer layer)
        {
            SetLayersRecursive(go, (int)layer);
        }
    }

    public enum DVLayer
    { 
        Default = 1,
        TransparentFX = 2,
        //Ignore Raycast = 3,
        //,
        Water = 5,
        UI = 6,
        //,
        //,
        Terrain = 9,
        Player = 10,
        Train_Big_Collider = 11,
        Train_Walkable = 12,
        Train_Interior = 13,
        Interactable = 14,
        Teleport_Destination = 15,
        Laser_Pointer_Target = 16,
        Camera_Dampening = 17,
        Culling_Sleepers = 18,
        Culling_Anchors = 19,
        Culling_Rails = 20,
        Render_Elements = 21,
        No_Teleport_Interaction = 22,
        Inventory = 23,
        Controller = 24,
        Hazmat = 25,
        PostProcessing = 26,
        Grabbed_Item = 27,
        World_Item = 28,
    }
}
