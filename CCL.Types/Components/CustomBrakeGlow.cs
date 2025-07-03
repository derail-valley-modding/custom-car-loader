using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Custom Brake Glow")]
    public class CustomBrakeGlow : MonoBehaviour
    {
        [GradientUsage(true)]
        public Gradient ColourGradient = new Gradient
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(new Color(0, 0, 0, 1), 0),
                new GradientColorKey(new Color(5, 5, 5, 1), 1)
            }
        };
    }
}
