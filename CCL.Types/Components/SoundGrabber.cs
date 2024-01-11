using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Components
{
    public class SoundGrabber : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public struct SoundReplacement
        {
            public string FieldName;
            public string SoundName;
        }

        public MonoBehaviour ScriptToAffect = null!;
        public SoundReplacement[] Replacements = new SoundReplacement[0];

        private string? json;

        public void OnValidate()
        {
            json = JSONObject.ToJson(Replacements);
        }

        public void AfterImport()
        {
            if (json != null)
            {
                Replacements = JSONObject.FromJson<SoundReplacement[]>(json);
            }
        }
    }
}
