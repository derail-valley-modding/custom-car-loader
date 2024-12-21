using System;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    public class MaterialSet : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class RendererMaterialReplacement
        {
            public Renderer Renderer;
            public int MaterialIndex;
        }

        public Material OriginalMaterial;
        public RendererMaterialReplacement[] Replacements = new RendererMaterialReplacement[0];

        [SerializeField, HideInInspector]
        private Renderer[] _renderers = new Renderer[0];
        [SerializeField, HideInInspector]
        private int[] _indexes = new int[0];

        [RenderMethodButtons]
        [MethodButton(nameof(AddAllChildRenderers), "Add all child renderers")]
        public bool buttonRender;

        private void AddAllChildRenderers()
        {
            Replacements = Replacements.Concat(GetComponentsInChildren<Renderer>().Select(x =>
                new RendererMaterialReplacement { Renderer = x, MaterialIndex = 0} )).ToArray();
        }

        public void OnValidate()
        {
            int length = Replacements.Length;
            _renderers = new Renderer[length];
            _indexes = new int[length];

            for (int i = 0; i < length; i++)
            {
                _renderers[i] = Replacements[i].Renderer;
                _indexes[i] = Replacements[i].MaterialIndex;
            }
        }

        public void AfterImport()
        {
            int lenght = _renderers.Length;
            Replacements = new RendererMaterialReplacement[lenght];

            for (int i = 0;i < lenght; i++)
            {
                Replacements[i] = new RendererMaterialReplacement
                {
                    Renderer = _renderers[i],
                    MaterialIndex = _indexes[i]
                };
            }
        }
    }
}
