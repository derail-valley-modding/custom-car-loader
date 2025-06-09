using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    public class SetPhysicsMaterial : MonoBehaviour
    {
        public enum PhysicsMaterial
        {
            //Asphalt     = 0,
            Ballast     = 1,
            //Liquid      = 2
        }

        public PhysicsMaterial Material = PhysicsMaterial.Ballast;
        public List<Collider> Colliders = new List<Collider>();

        [RenderMethodButtons, SerializeField]
        [MethodButton(nameof(AddChildren), "Add Child Colliders")]
        private bool _buttons;

        private void AddChildren()
        {
            foreach (var child in GetComponentsInChildren<Collider>(true))
            {
                if (!Colliders.Contains(child))
                {
                    Colliders.Add(child);
                }
            }
        }
    }
}
