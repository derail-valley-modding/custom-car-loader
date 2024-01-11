using CCL.Types.Json;
using System;
using UnityEngine;
using static CCL.Types.Proxies.Wheels.PoweredWheelRotationViaAnimationProxy;

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

        [HideInInspector]
        [SerializeField]
        private string[] _fields = new string[0];
        [HideInInspector]
        [SerializeField]
        private string[] _sounds = new string[0];

        public void OnValidate()
        {
            int length = Replacements.Length;
            _fields = new string[length];
            _sounds = new string[length];

            for (int i = 0; i < length; i++)
            {
                _fields[i] = Replacements[i].FieldName;
                _sounds[i] = Replacements[i].SoundName;
            }
        }

        public void AfterImport()
        {
            int length = Mathf.Min(_fields.Length, _sounds.Length);
            Replacements = new SoundReplacement[length];

            for (int i = 0; i < length; i++)
            {
                Replacements[i] = new SoundReplacement()
                {
                    FieldName = _fields[i],
                    SoundName = _sounds[i]
                };
            }
        }
    }
}
