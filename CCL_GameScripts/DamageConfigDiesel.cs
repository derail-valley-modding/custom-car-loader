using System;
using UnityEngine;

namespace CCL_GameScripts
{
    public class DamageConfigDiesel : DamageConfigBasic
    {
        [Header("Engine Damage (HP/s)")]
        public float EngineHitpoints = 4000f;
        public float ColdEngineDPS = 0.1f;
        public float ColdEngineRPMThreshold = 0.5f;
        public float ColdEngineTempThreshold = 45f;

        public float EngineStartDamage = 10f;
        public float EngineRunningDPS = 0.05f;
        public float EngineNoOilDPS = 30f;

        public float EngineCollisionMultiplier = 0.15f;
        public float EngineFireMultiplier = 0.1f;
        public float CollisionShutoffThreshold = 100f;

        [Header("Engine Failures")]
        public float RandomShutoffCheckPeriod = 10f;
        [Header("chance = mult * (damage - threshold)")]
        public float EngineFailureThreshold = 0.8f;
        public float ShutoffChanceMultiplier = 4f;

        public void ApplyDE6Defaults()
        {
            // base
            ApplyDefaults();

            BodyHitpoints = 5600f;
            BodyCollisionResistance = 25f;
            BodyCollisionMultiplier = 1f;
            BodyFireResistance = 7.5f;
            BodyFireMultiplier = 1f;
            DamageTolerance = 0.01f;

            WheelHitpoints = 2000f;

            // diesel
            EngineHitpoints = 4000f;
            ColdEngineDPS = 0.1f;
            ColdEngineRPMThreshold = 0.5f;
            ColdEngineTempThreshold = 45f;

            EngineStartDamage = 10f;
            EngineRunningDPS = 0.05f;
            EngineNoOilDPS = 30f;

            EngineCollisionMultiplier = 0.15f;
            EngineFireMultiplier = 0.1f;
            CollisionShutoffThreshold = 100f;

            RandomShutoffCheckPeriod = 10f;
            EngineFailureThreshold = 0.8f;
            ShutoffChanceMultiplier = 4f;
        }

        public void ApplyShunterDefaults()
        {
            // base
            ApplyDefaults();

            BodyHitpoints = 5600f;
            BodyCollisionResistance = 25f;
            BodyCollisionMultiplier = 1f;
            BodyFireResistance = 7.5f;
            BodyFireMultiplier = 1f;
            DamageTolerance = 0.01f;

            WheelHitpoints = 1000f;

            // diesel
            EngineHitpoints = 1000f;
            ColdEngineDPS = 0.1f;
            ColdEngineRPMThreshold = 0.5f;
            ColdEngineTempThreshold = 45f;

            EngineStartDamage = 10f;
            EngineRunningDPS = 0.05f;
            EngineNoOilDPS = 30f;

            EngineCollisionMultiplier = 0.15f;
            EngineFireMultiplier = 0.1f;
            CollisionShutoffThreshold = 100f;

            RandomShutoffCheckPeriod = 10f;
            EngineFailureThreshold = 0.8f;
            ShutoffChanceMultiplier = 4f;
        }
    }
}
