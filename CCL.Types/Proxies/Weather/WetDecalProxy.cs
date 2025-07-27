using CCL.Types.Json;
using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Wet Decal Proxy")]
    public class WetDecalProxy : MonoBehaviour, ICustomSerialized
    {
        [SerializeField]
        private DecalSettingsProxy _settings = new DecalSettingsProxy();

        public DecalSettingsProxy Settings => _settings;

        [HideInInspector]
        [SerializeField]
        private string? _json = string.Empty;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(_settings);
        }

        public void AfterImport()
        {
            if (!string.IsNullOrEmpty(_json))
            {
                _settings = JSONObject.FromJson<DecalSettingsProxy>(_json);
            }
        }
    }
}
