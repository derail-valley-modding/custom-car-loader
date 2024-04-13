using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    public class RefreshableChildrenController<T> : MonoBehaviour, IRefreshableChildren
        where T : MonoBehaviour
    {
        public T[] entries = new T[0];

        public virtual void OnValidate()
        {
            PopulateChildren();
        }

        public void PopulateChildren()
        {
            var children = GetComponentsInChildren<T>();
            var list = entries.ToList();

            foreach (var entry in children)
            {
                if (!list.Contains(entry))
                {
                    list.Add(entry);
                }
            }
        }
    }

    public interface IRefreshableChildren
    {
        public void PopulateChildren();
    }
}
