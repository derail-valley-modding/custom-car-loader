using CCL.Types.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    public class VanillaResourceGrabber<T> : MonoBehaviour, IVanillaResourceGrabber, ICustomSerialized
        where T : UnityEngine.Object
    {
        public Component ScriptToAffect = null!;
        public ResourceReplacement[] Replacements = new ResourceReplacement[0];

        [HideInInspector]
        [SerializeField]
        private string? _json;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Replacements);
        }

        public void AfterImport()
        {
            if (_json != null)
            {
                Replacements = JSONObject.FromJson<ResourceReplacement[]>(_json);
            }
        }

        public Component GetScript()
        {
            return ScriptToAffect;
        }

        public Type GetTypeOfResource()
        {
            return typeof(T);
        }

        public Type GetTypeOfResourceIList()
        {
            return typeof(IList<T>);
        }

        public bool IsSupportedType(Type type, out bool isIList)
        {
            if (type == GetTypeOfResource())
            {
                isIList = false;
                return true;
            }
            else if (GetTypeOfResourceIList().IsAssignableFrom(type))
            {
                isIList = true;
                return true;
            }

            isIList = false;
            return false;
        }

        public virtual string[] GetNames()
        {
            return new string[0];
        }
    }
}
