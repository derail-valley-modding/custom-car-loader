using TMPro;
using UnityEngine;

namespace CCL.Importer
{
    internal static class TMPHelper
    {
        public static TMP_Text GetTMP(GameObject go)
        {
            return go.GetComponent<TMP_Text>();
        }

        public static TMP_Text GetTMP(Component c)
        {
            return c.GetComponent<TMP_Text>();
        }

        public static void SetTextAndUpdate(this TMP_Text component, string text)
        {
            component.text = text;
            component.ForceMeshUpdate();
        }
    }
}
