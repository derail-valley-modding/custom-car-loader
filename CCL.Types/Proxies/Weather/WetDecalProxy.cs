using CCL.Types.Json;
using UnityEngine;
using static CCL.Types.Proxies.Weather.WetDecalEnums;

namespace CCL.Types.Proxies.Weather
{
    [AddComponentMenu("CCL/Proxies/Weather/Wet Decal Proxy")]
    public class WetDecalProxy : MonoBehaviour, ICustomSerialized
    {
        [SerializeField]
        private DecalSettingsProxy _settings = new DecalSettingsProxy();

        [HideInInspector, SerializeField]
        private string? _json = string.Empty;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(_settings);
        }

        public void AfterImport()
        {
            _settings = JSONObject.FromJson(_json, () => new DecalSettingsProxy());
        }

        private void OnDrawGizmos()
        {
            DrawGizmo(false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }

        private void DrawGizmo(bool selected)
        {
            Color color = new Color(0f, 0.7f, 1f, 1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            if (_settings.Shape == DecalShape.Cube || _settings.Shape == DecalShape.Mesh)
            {
                color.a = selected ? 0.3f : 0.1f;
                color.a *= isActiveAndEnabled ? 0.15f : 0.1f;
                Gizmos.color = color;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
            else if (_settings.Shape == DecalShape.Sphere)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0f);
                Gizmos.DrawSphere(Vector3.zero, 0.5f);
            }
            else
            {
                Debug.LogError($"Unknown decal shape: '{_settings.Shape}'");
            }

            color.a = selected ? 0.5f : 0.2f;
            color.a *= isActiveAndEnabled ? 1f : 0.75f;
            Gizmos.color = color;
            if (_settings.Shape == DecalShape.Cube || _settings.Shape == DecalShape.Mesh)
            {
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
            else if (_settings.Shape == DecalShape.Sphere)
            {
                Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
            }
        }
    }
}
