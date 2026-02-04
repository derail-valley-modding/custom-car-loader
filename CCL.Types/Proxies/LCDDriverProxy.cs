using UnityEngine;

namespace CCL.Types.Proxies
{
    [AddComponentMenu("CCL/Proxies/LCD Driver Proxy")]
    public class LCDDriverProxy : MonoBehaviour
    {
        private static readonly Vector3 s_size = new Vector3(1.2f, 0, 1.7f);
        private static readonly Vector3 s_sizeDot = new Vector3(0.2f, 0, 0.2f);
        private static readonly Vector3 s_position = new Vector3(1.59f, 0, 1.43f);
        private static readonly Vector3 s_positionDot = new Vector3(1.001906f, 0, 2.245174f);
        private static readonly Vector3 s_positionColon1 = new Vector3(0.9227163f, 0, 1.670568f);
        private static readonly Vector3 s_positionColon2 = new Vector3(0.8627163f, 0, 1.176568f);

        public enum DigitStyle
        {
            RegularBlack = 0,
            RegularRed = 100,
            TransparentRed = 101,
            Custom = 10000
        }

        [Tooltip("Can display 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, -, +")]
        public string displayedString = string.Empty;
        public DigitStyle model = DigitStyle.RegularRed;
        [EnableIf(nameof(UseCustomStyle))]
        public GameObject? customStyle;
        public int numDigits = 17;
        public float spacing = -1.4f;

        public bool UseCustomStyle => model == DigitStyle.Custom;

        private void OnDrawGizmos()
        {
            using (GizmoUtil.MatrixScope.LocalTransform(transform))
            {
                Gizmos.color = model switch
                {
                    DigitStyle.RegularBlack => Color.black,
                    _ => Color.red,
                };

                for (int i = 0; i < numDigits; i++)
                {
                    var offset = new Vector3(spacing * i, 0, 0);
                    Gizmos.DrawWireCube(s_position + offset, s_size);
                    Gizmos.DrawWireCube(s_positionDot + offset, s_sizeDot);
                    Gizmos.DrawWireCube(s_positionColon1 + offset, s_sizeDot);
                    Gizmos.DrawWireCube(s_positionColon2 + offset, s_sizeDot);
                }
            }
        }
    }
}
