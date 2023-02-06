using CCL_GameScripts.Attributes;
using System.Collections;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class SteamEmissionSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.SteamParticlesController";
        public override bool DestroyAfterCreation => false;

        [Header("Chuff Particles")]
        public Transform ChimneyParticlesLocation;
        public Transform ChuffParticlesLeftLocation;
        public Transform ChuffParticlesRightLocation;

        [ProxyField]
        [InspectorName("Exhaust Steam Color")]
        public Color steamColor = Color.white;
        [ProxyField]
        [InspectorName("Light Smoke Color")]
        public Color smokeColorClear = new Color(0.14f, 0.14f, 0.13f, 0.1f);
        [ProxyField]
        [InspectorName("Heavy Smoke Color")]
        public Color smokeColorDark = new Color(0.14f, 0.14f, 0.13f);

        [ProxyField]
        public float chuffSizeMult = 1.6f;
        [ProxyField]
        public float chuffSpeedMult = 2f;
        [ProxyField]
        public float chuffEmissionRate = 1000f;

        [Header("Steam Particles")]
        public Transform WhistleMidLocation;
        public Transform WhistleFrontLocation;

        public Transform ReleaseLeftLocation;
        public Transform ReleaseRightLocation;
        public Transform SafetyReleaseLocation;

        public Transform DynamoLocation;
        public Transform CompressorLocation;
    }
}
