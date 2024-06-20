using UnityEngine;

namespace CCL.Types
{
    /// <summary>
    /// This interface can be used on objects that allow both real or placeholder objects in them.
    /// </summary>
    public interface ICanReplaceInstanced
    {
        /// <summary>
        /// Use this to perform replacing of fields that support it.
        /// </summary>
        public void DoFieldReplacing();
    }

    /// <summary>
    /// This interface is used on placeholder objects to allow them to be replaced by another object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInstancedObject<T> where T : Object
    {
        /// <summary>
        /// The object instanced by this script.
        /// </summary>
        public T? InstancedObject { get; set; }
        /// <summary>
        /// If a field is allowed to replace its value with the instanced object.
        /// </summary>
        public bool CanReplace { get; }
    }
}
