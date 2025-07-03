using UnityEngine;

namespace CCL.Types.Proxies.Indicators
{
    [AddComponentMenu("CCL/Proxies/Indicators/Indicator Scaler Proxy")]
    public class IndicatorScalerProxy : IndicatorProxy
    {
        public Transform indicatorToScale = null!;
        public Vector3 startScale = Vector3.one;
        public Vector3 endScale = Vector3.one;

        [Header("Editor visualization")]
        public bool scaleFromModel = true;
        public bool drawAsBox = true;
        public Vector3 sizeMultiplier = Vector3.one;

        private static Vector3 s_flatCube = new Vector3(0.1f, 0.0f, 0.1f);

        public void OnDrawGizmos()
        {
            if (!indicatorToScale) return;

            Gizmos.matrix = Matrix4x4.TRS(indicatorToScale.position, indicatorToScale.rotation, scaleFromModel ?
                ModelUtil.GetModelBoundsWithInverse(indicatorToScale.gameObject, indicatorToScale.position).size :
                sizeMultiplier);

            if (drawAsBox)
            {
                // Bleh
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(startScale.x, startScale.y, startScale.z) * 0.5f, new Vector3(endScale.x, endScale.y, endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(startScale.x, startScale.y, -startScale.z) * 0.5f, new Vector3(endScale.x, endScale.y, -endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(startScale.x, -startScale.y, startScale.z) * 0.5f, new Vector3(endScale.x, -endScale.y, endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(startScale.x, -startScale.y, -startScale.z) * 0.5f, new Vector3(endScale.x, -endScale.y, -endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(-startScale.x, startScale.y, startScale.z) * 0.5f, new Vector3(-endScale.x, endScale.y, endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(-startScale.x, startScale.y, -startScale.z) * 0.5f, new Vector3(-endScale.x, endScale.y, -endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(-startScale.x, -startScale.y, startScale.z) * 0.5f, new Vector3(-endScale.x, -endScale.y, endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(-startScale.x, -startScale.y, -startScale.z) * 0.5f, new Vector3(-endScale.x, -endScale.y, -endScale.z) * 0.5f);

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(Vector3.zero, startScale);
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(Vector3.zero, endScale);
            }
            else
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(-endScale.x, 0, 0) * 0.5f, new Vector3(endScale.x, 0, 0) * 0.5f);
                Gizmos.DrawLine(new Vector3(0, -endScale.y, 0) * 0.5f, new Vector3(0, endScale.y, 0) * 0.5f);
                Gizmos.DrawLine(new Vector3(0, 0, -endScale.z) * 0.5f, new Vector3(0, 0, endScale.z) * 0.5f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(startScale.x, -0.1f, 0) * 0.5f, new Vector3(startScale.x, 0.1f, 0) * 0.5f);
                Gizmos.DrawLine(new Vector3(-startScale.x, -0.1f, 0) * 0.5f, new Vector3(-startScale.x, 0.1f, 0) * 0.5f);
                Gizmos.DrawLine(new Vector3(0, -0.1f, startScale.z) * 0.5f, new Vector3(0, 0.1f, startScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(0, -0.1f, -startScale.z) * 0.5f, new Vector3(0, 0.1f, -startScale.z) * 0.5f);
                Gizmos.DrawWireCube(Vector3.up * startScale.y * 0.5f, s_flatCube);
                Gizmos.DrawWireCube(Vector3.down * startScale.y * 0.5f, s_flatCube);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(endScale.x, -0.1f, 0) * 0.5f, new Vector3(endScale.x, 0.1f, 0) * 0.5f);
                Gizmos.DrawLine(new Vector3(-endScale.x, -0.1f, 0) * 0.5f, new Vector3(-endScale.x, 0.1f, 0) * 0.5f);
                Gizmos.DrawLine(new Vector3(0, -0.1f, endScale.z) * 0.5f, new Vector3(0, 0.1f, endScale.z) * 0.5f);
                Gizmos.DrawLine(new Vector3(0, -0.1f, -endScale.z) * 0.5f, new Vector3(0, 0.1f, -endScale.z) * 0.5f);
                Gizmos.DrawWireCube(Vector3.up * endScale.y * 0.5f, s_flatCube);
                Gizmos.DrawWireCube(Vector3.down * endScale.y * 0.5f, s_flatCube);
            }
        }
    }
}
