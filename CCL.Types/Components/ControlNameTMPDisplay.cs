using TMPro;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Control Name TMP Display")]
    public class ControlNameTMPDisplay : MonoBehaviour
    {
        public GameObject ControlObject = null!;
        public TMP_Text Text = null!;
    }
}
