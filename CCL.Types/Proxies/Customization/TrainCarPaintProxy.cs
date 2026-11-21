using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types.Proxies.Customization
{
    [AddComponentMenu("CCL/Proxies/Customization/Train Car Paint Proxy")]
    public class TrainCarPaintProxy : MonoBehaviour, ISelfValidation
    {
        public enum Target : byte
        {
            Exterior,
            Interior
        }

        [PaintField]
        public string CurrentTheme = IdV2.Paints[0];
        public Target targetArea = Target.Exterior;
        public List<MaterialSet> Sets = new List<MaterialSet>();

        [RenderMethodButtons]
        [MethodButton(nameof(AddSet), "Add Set")]
        [MethodButton(nameof(AddSetChild), "Add Set as Child")]
        public bool buttonRender;

        private void AddSet()
        {
            Sets.Add(gameObject.AddComponent<MaterialSet>());
            OnValidate();
        }

        private void AddSetChild()
        {
            var go = new GameObject("Material Set (Exterior)");
            go.transform.parent = transform;
            Sets.Add(go.AddComponent<MaterialSet>());
            OnValidate();
        }

        private void OnValidate()
        {
            // Just because Unity doesn't like the method above.
        }

        public SelfValidationResult Validate(out string message, out string? highlight)
        {
            if (Sets.Any(x => x == null))
            {
                return this.FailForNullEntries(nameof(Sets), out message, out highlight);
            }

            return this.Pass(out message, out highlight);
        }
    }
}
