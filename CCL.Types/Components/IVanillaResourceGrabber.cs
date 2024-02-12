using System;
using UnityEngine;

namespace CCL.Types.Components
{
    public interface IVanillaResourceGrabber
    {
        public GameObject gameObject { get; }

        public Component GetScript();

        public Type GetTypeOfResource();

        public Type GetTypeOfResourceIList();

        public bool IsSupportedType(Type type, out bool isIList);

        public string[] GetNames();

        public ResourceReplacement[] GetReplacements();
    }
}
