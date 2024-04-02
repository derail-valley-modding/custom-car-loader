using UnityEngine;

namespace CCL.Types
{
    public interface ICanReplaceInstanced<T> where T : Object
    {
        public void CheckReplaceableFields();
    }

    public interface IInstancedObject<T> where T : Object
    {
        public T? InstancedObject { get; set; }
        public bool CanReplace { get; }
    }
}
