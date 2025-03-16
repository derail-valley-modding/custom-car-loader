using CCL.Types.Json;
using UnityEngine;

namespace CCL.Types.HUD
{
    [CreateAssetMenu(menuName = "CCL/HUD Layout", order = MenuOrdering.HUDLayout)]
    public class VanillaHUDLayout : ScriptableObject, ICustomSerialized
    {
        public enum BaseHUD
        {
            DE2         = 10,
            DE6         = 40,
            DH4         = 50,
            DM3         = 60,

            S060        = 25,
            S282        = 20,

            BE2         = 70,
            DM1U        = 35,
            Handcar     = 700,

            Custom      = 1000
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
