using CCL.Types.Json;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class ControlSpecProxy : MonoBehaviour, ICustomSerialized
    {
        [Header("Common")]
        public bool disallowShortTriggerLockHold;
        public GameObject[] colliderGameObjects;
        public InteractionHandPosesProxy handPosesOverride;

        private string? _json = string.Empty;

        public void AfterImport()
        {
            if (!string.IsNullOrEmpty(_json))
            {
                handPosesOverride = JSONObject.FromJson<InteractionHandPosesProxy>(_json);
            }
        }

        public void OnValidate()
        {
            _json = JSONObject.ToJson(handPosesOverride);
        }
    }
}
