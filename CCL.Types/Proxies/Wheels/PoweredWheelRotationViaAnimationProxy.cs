using CCL.Types.Json;
using System;
using UnityEngine;

namespace CCL.Types.Proxies.Wheels
{
    public class PoweredWheelRotationViaAnimationProxy : PoweredWheelRotationBaseProxy, ICustomSerialized
    {
        [Serializable]
        public class AnimatorStartTimeOffsetPair
        {
            public Animator animator = null!;

            [Range(0f, 1f)]
            public float startTimeOffset;
        }

        public AnimatorStartTimeOffsetPair[] animatorSetups = new AnimatorStartTimeOffsetPair[0];

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
        }
    }
}
