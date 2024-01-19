using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Types.Components
{
    public class ShaderGrabber : MonoBehaviour, ICustomSerialized
    {
        public enum GrabbableShader
        {
            TransparencyWithFog,
            DVModularBuildings
        }

        [Serializable]
        public struct ShaderIndex
        {
            public int Index;
            public GrabbableShader Shader;
        }

        [Tooltip("Which renderers will be affected by the shader replacer.")]
        public Renderer[] RenderersToAffect = new Renderer[0];
        [Tooltip("The shaders that will replace the ones in the renderers.")]
        public ShaderIndex[] ShadersToReplace = new ShaderIndex[0];

        [HideInInspector]
        [SerializeField]
        private string? json;

        public void OnValidate()
        {
            json = JSONObject.ToJson(ShadersToReplace);
        }

        public void AfterImport()
        {
            if (!string.IsNullOrEmpty(json))
            {
                ShadersToReplace = JSONObject.FromJson<ShaderIndex[]>(json);
            }
        }
    }
}
