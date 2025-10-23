using CCL.Types.Json;
using System;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Grabbers/Material Grabber (Renderer)")]
    public class MaterialGrabberRenderer : MonoBehaviour, ICustomSerialized, ICustomGrabberValidation
    {
        [Serializable]
        public class IndexToName
        {
            public int RendererIndex;
            public string ReplacementName = string.Empty;
        }

        public Renderer[] RenderersToAffect = new Renderer[0];
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
            renderers.AddRange(GetComponentsInChildren<Renderer>());
            RenderersToAffect = renderers.ToArray();
        }

        public bool IsValid(out string error)
        {
            foreach (var item in RenderersToAffect)
            {
                if (item == null)
                {
                    error = $"MaterialGrabberRenderer in {gameObject.GetPath()} has null entries.";
                    return false;
                }
            }

            foreach (var item in Replacements)
            {
                if (!MaterialGrabber.MaterialNames.Contains(item.ReplacementName))
                {
                    error = $"MaterialGrabberRenderer in {gameObject.GetPath()} does not have a valid replacement ({item.ReplacementName}).";
                    return false;
                }

                if (RenderersToAffect.Any(x => item.RendererIndex >= x.sharedMaterials.Length))
                {
                    error = $"MaterialGrabberRenderer in {gameObject.GetPath()} has an index ({item.RendererIndex} out of range.";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
