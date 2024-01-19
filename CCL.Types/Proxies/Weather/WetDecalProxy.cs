using UnityEngine;

namespace CCL.Types.Proxies.Weather
{
    public class WetDecalProxy : MonoBehaviour
    {
        [SerializeField]
        private DecalSettingsProxy _settings = new DecalSettingsProxy();

        public DecalSettingsProxy Settings => _settings;
    }
}
