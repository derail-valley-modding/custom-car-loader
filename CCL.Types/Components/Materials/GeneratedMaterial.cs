using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components.Materials
{
    public interface IGeneratedMaterial
    {
        void Assign();
    }

    public abstract class GeneratedMaterial<T> : MonoBehaviour, IGeneratedMaterial
        where T : GeneratedMaterial<T>
    {
        private static List<T> s_cache = new List<T>();

        private Material _generated = null!;

        public Renderer[] RenderersToAssign = new Renderer[0];

        public abstract bool AreSameSettings(T other);

        public void Cache(Material generated)
        {
            _generated = generated;
            s_cache.Add((T)this);
        }

        public void Assign()
        {
            foreach (var renderer in RenderersToAssign)
            {
                renderer.sharedMaterial = _generated;
            }
        }

        public static bool TryGetAlreadyGenerated(T comp, out Material mat)
        {
            for (int i = 0; i < s_cache.Count; i++)
            {
                var cached = s_cache[i];

                if (cached == null)
                {
                    s_cache.RemoveAt(i);
                    i--;
                    continue;
                }

                if (comp.AreSameSettings(cached))
                {
                    comp._generated = cached._generated;
                    mat = cached._generated;
                    return true;
                }
            }

            mat = null!;
            return false;
        }
    }
}
