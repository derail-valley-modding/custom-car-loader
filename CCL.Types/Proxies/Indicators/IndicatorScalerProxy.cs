using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    public class IndicatorScalerProxy : IndicatorProxy
    {
        public Transform indicatorToScale = null!;
        public Vector3 startScale = Vector3.one;
        public Vector3 endScale = Vector3.one;

        [Header("Editor visualization")]
        public bool scaleFromModel = true;
        public Vector3 sizeMultiplier = Vector3.one;

        public void OnDrawGizmos()
        {
            if (!indicatorToScale) return;

            Gizmos.matrix = Matrix4x4.TRS(indicatorToScale.position, indicatorToScale.rotation,
                scaleFromModel ? ModelUtil.GetModelBounds(indicatorToScale.gameObject).size : sizeMultiplier);

            // Bleh
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(startScale.x, startScale.y, startScale.z) * 0.5f, new Vector3(endScale.x, endScale.y, endScale.z) * 0.5f);
            Gizmos.DrawLine(new Vector3(startScale.x, startScale.y, -startScale.z) * 0.5f, new Vector3(endScale.x, endScale.y, -endScale.z)  * 0.5f);
            Gizmos.DrawLine(new Vector3(startScale.x, -startScale.y, startScale.z) * 0.5f, new Vector3(endScale.x, -endScale.y, endScale.z)  * 0.5f);
            Gizmos.DrawLine(new Vector3(startScale.x, -startScale.y, -startScale.z) * 0.5f, new Vector3(endScale.x, -endScale.y, -endScale.z) * 0.5f);
            Gizmos.DrawLine(new Vector3(-startScale.x, startScale.y, startScale.z) * 0.5f, new Vector3(-endScale.x, endScale.y, endScale.z)  * 0.5f);
            Gizmos.DrawLine(new Vector3(-startScale.x, startScale.y, -startScale.z) * 0.5f, new Vector3(-endScale.x, endScale.y, -endScale.z) * 0.5f);
            Gizmos.DrawLine(new Vector3(-startScale.x, -startScale.y, startScale.z) * 0.5f, new Vector3(-endScale.x, -endScale.y, endScale.z) * 0.5f);
            Gizmos.DrawLine(new Vector3(-startScale.x, -startScale.y, -startScale.z) * 0.5f, new Vector3(-endScale.x, -endScale.y, -endScale.z) * 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, startScale);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, endScale);
        }
    }
}
