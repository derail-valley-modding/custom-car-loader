using CCL.Types.Json;
using System;
using UnityEngine;
using static CCL.Types.Proxies.Wheels.PoweredWheelRotationViaAnimationProxy;

namespace CCL.Types.Components
{
    public class SoundGrabber : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class SoundReplacement
        {
            public string SoundName = string.Empty;
            public string FieldName = string.Empty;
            public bool IsArray = false;
            public int Index = 0;
        }

        public MonoBehaviour ScriptToAffect = null!;
        public SoundReplacement[] Replacements = new SoundReplacement[0];

        //[HideInInspector]
        //[SerializeField]
        //private string[] _fields = new string[0];
        //[HideInInspector]
        //[SerializeField]
        //private string[] _sounds = new string[0];
        //[HideInInspector]
        //[SerializeField]
        //private bool[] _arrays = new bool[0];
        //[HideInInspector]
        //[SerializeField]
        //private int[] _index = new int[0];

        [HideInInspector]
        [SerializeField]
        private string? _json;

        public void OnValidate()
        {
            //int length = Replacements.Length;
            //_fields = new string[length];
            //_sounds = new string[length];
            //_arrays = new bool[length];
            //_index = new int[length];

            //for (int i = 0; i < length; i++)
            //{
            //    _fields[i] = Replacements[i].FieldName;
            //    _sounds[i] = Replacements[i].SoundName;
            //    _arrays[i] = Replacements[i].IsArray;
            //    _index[i] = Replacements[i].Index;
            //}

            _json = JSONObject.ToJson(Replacements);
        }

        public void AfterImport()
        {
            //int length = Mathf.Min(_fields.Length, _sounds.Length, _arrays.Length, _index.Length);
            //Replacements = new SoundReplacement[length];

            //for (int i = 0; i < length; i++)
            //{
            //    Replacements[i] = new SoundReplacement()
            //    {
            //        FieldName = _fields[i],
            //        SoundName = _sounds[i],
            //        IsArray = _arrays[i],
            //        Index = _index[i]
            //    };
            //}

            if (_json != null)
            {
                Replacements = JSONObject.FromJson<SoundReplacement[]>(_json);
            }
        }
    }
}
