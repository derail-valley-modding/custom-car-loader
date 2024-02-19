using CCL.Types.Json;
using UnityEngine;

namespace CCL.Types.Components.HUD
{
    public class VanillaHUDLayout : MonoBehaviour, ICustomSerialized
    {
        public enum BaseHUD
        {
            DE2,
            DE6,
            DM3,
            DH4,
            S060,
            S282,
            BE2,
            Handcar,
            Custom
        }

        [Tooltip("The HUD layout this car should use")]
        public BaseHUD HUDType;
        public CustomHUDLayout CustomHUDSettings = new CustomHUDLayout();

        [SerializeField, HideInInspector]
        private string? _json;

        public void OnValidate()
        {
            CustomHUDSettings.Validate();
            _json = JSONObject.ToJson(CustomHUDSettings);
        }

        public void AfterImport()
        {
            CustomHUDSettings = JSONObject.FromJson(_json, () => CustomHUDSettings);
        }
    }
}
