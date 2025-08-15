﻿using UnityEngine;

namespace CCL.Types.Proxies.VFX
{
    public class ItemLightProxy : MonoBehaviour
    {
        public Light light = null!;

        private void OnValidate()
        {
            if (GetComponent<LightShadowQualityProxy>())
            {
                Debug.LogError("ItemLight and LightShadowQuality are not compatible with each other! Remove one!");
            }
        }
    }
}
