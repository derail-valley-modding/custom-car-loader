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

        public static void SetLayersRecursiveAndExclude(GameObject go, DVLayer layer, DVLayer exclude)
        {
            var toReplace = go.GetComponentsInChildren<Transform>().Where(tform => tform.gameObject.layer != (int)exclude);
            foreach (var tform in toReplace)
            {
                tform.gameObject.layer = (int)layer;
            }
        }
    }

    public enum DVLayer
    { 
        Default = 0,
        TransparentFX = 1,
        //Ignore Raycast = 2,
        //,
        Water = 4,
        UI = 5,
        //,
        //,
        Terrain = 8,
        Player = 9,
        Train_Big_Collider = 10,
        Train_Walkable = 11,
        Train_Interior = 12,
        Interactable = 13,
        Teleport_Destination = 14,
        Laser_Pointer_Target = 15,
        Camera_Dampening = 16,
        Culling_Sleepers = 17,
        Culling_Anchors = 18,
        Culling_Rails = 19,
        Render_Elements = 20,
        No_Teleport_Interaction = 21,
        Inventory = 22,
        Controller = 23,
        Hazmat = 24,
        PostProcessing = 25,
        Grabbed_Item = 26,
        World_Item = 27,
    }
}
