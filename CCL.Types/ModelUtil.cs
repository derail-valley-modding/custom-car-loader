using System;
using System.Collections.Generic;
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
        public static (Vector3, Vector3) GetModelCenterHingeProjection(GameObject gameObject, Vector3 axis)
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

        public static Bounds GetModelBounds(GameObject gameObject)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return new Bounds();
            }

            Bounds bounds = renderers[0].bounds;

            for(int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        public static Bounds GetModelBoundsWithInverse(GameObject gameObject, Vector3 point)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return new Bounds();
            }

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            Bounds bounds2 = bounds;
            bounds.center = point - (bounds.center - point);
            bounds.Encapsulate(bounds2);

            return bounds;
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

        public static void SetLayersNonRecursive(this GameObject go, DVLayer layer)
        {
            SetLayersNonRecursive(go.transform, layer);
        }

        public static void SetLayersNonRecursive(this Transform transform, DVLayer layer)
        {
            foreach (var item in transform.GetComponentsInChildren<Transform>())
            {
                item.gameObject.SetLayer(layer);
            }
        }

        public static void SetLayersRecursive(this GameObject go, int layer)
        {
            SetLayerRecursiveInternal(go.transform, (_) => (false, false), layer);
        }

        public static void SetLayersRecursive(this GameObject go, DVLayer layer)
        {
            SetLayerRecursiveInternal(go.transform, (_) => (false, false), (int)layer);
        }

        public static void SetLayersRecursive(this GameObject go, Func<Transform, (bool ExcludeSelf, bool ExcludeChildren)> excludeCondition, DVLayer layer)
        {
            SetLayerRecursiveInternal(go.transform, excludeCondition, (int)layer);
        }

        public static void SetLayersRecursiveAndExclude(GameObject go, DVLayer layer, DVLayer exclude)
        {
            SetLayerRecursiveInternal(go.transform, (_) => (false, false), (int)layer, (int)exclude);
        }

        public static void SetLayersRecursiveAndExclude(GameObject go, Func<Transform, (bool ExcludeSelf, bool ExcludeChildren)> excludeCondition, DVLayer layer, DVLayer exclude)
        {
            SetLayerRecursiveInternal(go.transform, excludeCondition, (int)layer, (int)exclude);
        }

        private static void SetLayerRecursiveInternal(Transform tform, Func<Transform, (bool ExcludeSelf, bool ExcludeChildren)> excludeCondition, int layer, int? exclude = null)
        {
            if (!exclude.HasValue || exclude.Value != tform.gameObject.layer)
            {
                var (excludeSelf, excludeChildren) = excludeCondition(tform);

                if (!excludeSelf)
                {
                    tform.gameObject.layer = layer;
                }
                else if (excludeChildren)
                {
                    return;
                }
            }

            for (int i = 0; i < tform.childCount; i++)
            {
                SetLayerRecursiveInternal(tform.GetChild(i), excludeCondition, layer, exclude);
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
}
