using CCL.Types.Json;
using UnityEngine;

namespace CCL.Types.Proxies.Controls
{
    public abstract class ControlSpecProxy : MonoBehaviour, ICustomSerialized
    {
        [Header("Common")]
        public bool disallowShortTriggerLockHold;
        public GameObject[] colliderGameObjects = new GameObject[0];
        public InteractionHandPosesProxy handPosesOverride = null!;

        [Header("Static non-vr interaction area - optional")]
        public StaticInteractionAreaProxy nonVrStaticInteractionArea = null!;

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

            if (nonVrStaticInteractionArea != null)
            {
                if (nonVrStaticInteractionArea.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("nonVrStaticInteractionArea gameObject must be disabled in prefabs! Forcing disable on nonVrStaticInteractionArea gameObject", this);
                    nonVrStaticInteractionArea.gameObject.SetActive(false);
                }

                nonVrStaticInteractionArea.OnValidate();
            }
        }
    }
}
