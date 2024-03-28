using CCL.Types.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CCL.Types
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


        public static Transform? FindSafe(this Transform? tform, string name)
        {
            if (tform == null) return null;

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

        public static void SetLayer(this GameObject go, DVLayer layer)
        {
            go.layer = (int)layer;
        }

        public static void SetLayersRecursive(this GameObject go, int layer)
        {
            SetLayerRecursiveInternal(go.transform, layer);
        }

        public static void SetLayersRecursive(this GameObject go, DVLayer layer)
        {
            SetLayerRecursiveInternal(go.transform, (int)layer);
        }

        public static void SetLayersRecursiveAndExclude(GameObject go, DVLayer layer, DVLayer exclude)
        {
            SetLayerRecursiveInternal(go.transform, (int)layer, (int)exclude);
        }

        private static void SetLayerRecursiveInternal(Transform tform, int layer, int? exclude = null)
        {
            if ((!exclude.HasValue || exclude.Value != tform.gameObject.layer) &&
                !tform.TryGetComponent(out InteriorNonStandardLayerProxy _))
            {
                tform.gameObject.layer = layer;
            }

            for (int i = 0; i < tform.childCount; i++)
            {
                SetLayerRecursiveInternal(tform.GetChild(i), layer, exclude);
            }
        }

        public static string GetPath(this GameObject go)
        {
            var parts = new Stack<string>();

            Transform current = go.transform;
            while (current)
            {
                parts.Push(current.name);
                current = current.parent;
            }

            var sb = new StringBuilder();
            while (parts.Count > 0)
            {
                if (sb.Length > 0)
                {
                    sb.Append('/');
                }
                sb.Append(parts.Pop());
            }

            return sb.ToString();
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
        Reflection_Probe_Only = 28,
    }
}
