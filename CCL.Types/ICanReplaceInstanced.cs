using UnityEngine;

namespace CCL.Types
{
    public interface ICanReplaceInstanced
    {
        public void DoFieldReplacing();
    }

    public interface IInstancedObject<T> where T : Object
    {
        public T? InstancedObject { get; set; }
        public bool CanReplace { get; }
    }
}
