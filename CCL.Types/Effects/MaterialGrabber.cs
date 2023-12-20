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
            DoNotReplace = -1,
            // Glasses and windows
            Glass_0 = 0,
            // Passenger car interiors
            TT_CP_PaintBeige = 7000,
            TT_CP_PlasticWhite,
            TT_CP_Metal,
            TT_CP_MetalTrim,
            TT_CP_RubberFloor,
            TT_CP_MetalDirty,
            TT_CP_Seats,
            TT_CP_Curtains,
        }

        [Tooltip("Which renderers will be affected by the material replacer.")]
        public Renderer[] RenderersToAffect = new Renderer[0];
        [Tooltip("The materials that will replace the ones in the renderers." +
            "\nThe index in this array will match the index in the renderer." +
            "\nTo skip an index, use 'DoNotReplace'.")]
        public GrabbableMaterials[] MaterialsToReplace = new GrabbableMaterials[0];
    }
}
