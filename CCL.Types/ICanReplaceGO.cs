using UnityEngine;

namespace CCL.Types
{
    public interface ICanReplaceGO
    {
        public void CheckGOFields();
    }

    public interface IInstancedGO
    {
        public GameObject? InstancedGO { get; set; }
        public bool CanReplace { get; }
    }
}
