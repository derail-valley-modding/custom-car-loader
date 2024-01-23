using CCL.Types.Json;
using UnityEngine;

namespace CCL.Types.Components.HUD
{
    [DisallowMultipleComponent]
    public class VanillaHudLayout : MonoBehaviour, ICustomSerialized
    {
        [HideInInspector]
        public VanillaHudSettings Settings = new VanillaHudSettings();

        [HideInInspector]
        [SerializeField]
        private string? _json = string.Empty;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(Settings);
        }

        public void AfterImport()
        {
            Settings = JSONObject.FromJsonNotNullOrEmpty(Settings, _json);
        }
    }
}
