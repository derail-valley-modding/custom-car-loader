using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Effects
{
    public class ShaderGrabber : MonoBehaviour
    {
        public enum GrabbableShaders
        {
            DoNotReplace = -1,
            TransparencyWithFog,
            DVModularBuildings
        }

        [Tooltip("Which renderers will be affected by the shader replacer.")]
        public Renderer[] RenderersToAffect = new Renderer[0];
        [Tooltip("The shaders that will replace the ones in the renderers.\n" +
            "The index in this array will match the index in the renderer.\n" +
            "To skip an index, use 'DoNotReplace'.")]
        public GrabbableShaders[] ShadersToReplace = new GrabbableShaders[0];
    }
}
