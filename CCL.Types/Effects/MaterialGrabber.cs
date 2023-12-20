using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Effects
{
    public class MaterialGrabber : MonoBehaviour
    {
        public enum GrabbableMaterials
        {
            DoNotReplace,
            Glass_0
        }

        [Tooltip("Which renderers will be affected by the material replacer.")]
        public Renderer[] RenderersToAffect = new Renderer[0];
        [Tooltip("The materials that will replace the ones in the renderers." +
            "\nThe index in this array will match the index in the renderer." +
            "\nTo skip an index, use 'DoNotReplace'.")]
        public GrabbableMaterials[] MaterialsToReplace = new GrabbableMaterials[0];
    }
}
