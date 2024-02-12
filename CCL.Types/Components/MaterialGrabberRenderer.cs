using CCL.Types.Json;
using System;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Components
{
    public class MaterialGrabberRenderer : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class IndexToName
        {
            public int RendererIndex;
            public string ReplacementName = string.Empty;
        }

        public MeshRenderer[] RenderersToAffect = new MeshRenderer[0];
        public IndexToName[] Replacements = new IndexToName[0];

        [HideInInspector]
        [SerializeField]
        private string? _json = string.Empty;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Replacements);
        }

        public void AfterImport()
        {
            if (!string.IsNullOrEmpty(_json))
            {
                Replacements = JSONObject.FromJson<IndexToName[]>(_json);
            }
        }

        public void PickChildren()
        {
            var renderers = RenderersToAffect.ToList();
            renderers.AddRange(GetComponentsInChildren<MeshRenderer>());
            RenderersToAffect = renderers.ToArray();
        }
    }
}
