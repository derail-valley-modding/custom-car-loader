using System.Collections;
using CCL_GameScripts.Attributes;
using UnityEngine;

namespace CCL_GameScripts.Effects
{
    public class SmokeEmissionSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.EngineSmokeEmitter";
        public override bool DestroyAfterCreation => false;

        [ProxyField]
        public Transform EmissionLocation;
        [ProxyField]
        public bool UseBigDieselParticles = false;
        [ProxyField]
        public float MaxParticlesFalloffSpeed = 0.1f;

        [Header("Overheating Particles")]
        [ProxyField]
        public float OverheatMinTemp = 108f;
        [ProxyField]
        public float OverheatMaxTemp = 120f;

        [ProxyField]
        public float HighTempIntervalMin = 1f;
        [ProxyField]
        public float HighTempIntervalMax = 5f;
        [ProxyField]
        public float HighTempSpeedMin = 2f;
        [ProxyField]
        public float HighTempSpeedMax = 5f;

        [Header("Engine Damage Particles")]
        [ProxyField]
        public float DamageThreshold = 0.4f;
        [ProxyField]
        public float DamagedParticlesMinSize = 0.5f;
        [ProxyField]
        public float DamagedParticlesMaxSize = 3f;
    }
}