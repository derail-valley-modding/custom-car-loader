using CCL.Types.Json;
using UnityEngine;
using static CCL.Types.Proxies.Wheels.PoweredWheelRotationViaAnimationProxy;

namespace CCL.Types.Proxies.Wheels
{
    public class WheelRotationViaAnimationProxy : WheelRotationBaseProxy
    {
        public AnimatorStartTimeOffsetPair[] animatorSetups;

        [HideInInspector]
        public string? Json;

        public void OnValidate()
        {
            Json = JSONObject.ToJson(animatorSetups);
        }

        public void AfterImport()
        {
            if (Json != null)
            {
                animatorSetups = JSONObject.FromJson<AnimatorStartTimeOffsetPair[]>(Json);
            }
            else
            {
                animatorSetups = new AnimatorStartTimeOffsetPair[0];
            }
        }
    }
}
