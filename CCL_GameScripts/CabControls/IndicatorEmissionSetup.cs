using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class IndicatorEmissionSetup : IndicatorSetupBase
    {
        public override string TargetTypeName => "IndicatorEmission";
        public override bool DestroyAfterCreation => true;
        public override IndicatorType IndicatorType => IndicatorType.Light;

        [ProxyField]
		public bool binary = true;
		[ProxyField]
		[Tooltip("if the value is higher than this, lamp will be lit. Only used if binary is on")]
		public float valueThreshold = 0.5f;

		[ProxyField]
		[Tooltip("How many seconds does it take for lamp to light. Default: 0.05")]
		public float lag = 0.05f;

		[Header("Texture Emission")]
		[ProxyField]
		[Range(0, 1)]
		public float minEmission;
		[ProxyField]
		[Range(0, 1)]
		public float maxEmission = 1f;
		[ProxyField]
		public Color emissionColor = Color.white;

		[Header("Realtime Lighting (Optional)")]
		[ProxyField("emissionLight")]
		public Light lightComponent;
		[ProxyField]
		public float lightIntensity = 1f;
	}
}