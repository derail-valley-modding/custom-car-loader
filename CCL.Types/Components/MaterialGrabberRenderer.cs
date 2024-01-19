using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Components
{
    public class MaterialGrabberRenderer : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class IndexToIndex
        {
            public int RendererIndex;
            public int NameIndex;
        }

        public MeshRenderer[] RenderersToAffect = new MeshRenderer[0];
        public IndexToIndex[] Indeces = new IndexToIndex[0];

        [HideInInspector]
        [SerializeField]
        private string? _json = string.Empty;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Indeces);
        }

        public void AfterImport()
        {
            if (!string.IsNullOrEmpty(_json))
            {
                Indeces = JSONObject.FromJson<IndexToIndex[]>(_json);
            }
        }
    }
}
