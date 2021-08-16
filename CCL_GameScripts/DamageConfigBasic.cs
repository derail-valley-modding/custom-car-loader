using System.Collections;
using UnityEngine;

namespace CCL_GameScripts
{
    public class DamageConfigBasic : MonoBehaviour
    {
        // defaults from DE6
        [Header("Body Damage (HP/s)")]
        public float BodyHitpoints = 5600f;
        public float BodyCollisionResistance = 25f;
        public float BodyCollisionMultiplier = 1f;
        public float BodyFireResistance = 7.5f;
        public float BodyFireMultiplier = 1f;
        public float DamageTolerance = 0.01f;

        [Header("Wheel Damage (HP/s)")]
        public float WheelHitpoints = 2000f;
        //public float BrakingDamageMultiplier = 0.33f;
        //public float BogieStressDPS = 0.03f;
        //public float WheelslipDPS = 18f;
        //public float WheelCollisionMultiplier = 0.01f;
        //public float WheelFireMultiplier = 0.01f;
        public AnimationCurve BrakeSpeedDamageCurve;

        public DamageConfigBasic()
        {
            // DE6 Defaults
            BrakeSpeedDamageCurve =
                new AnimationCurve(
                    new Keyframe(0, 0, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(2, 0, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(7.5f, 0.125f, 0.0285f, 0.0285f, 0.3333f, 0.3333f),
                    new Keyframe(29, 0.6706f, 0.0151f, 0.0151f, 0.3333f, 0.3333f),
                    new Keyframe(100, 1, 0, 0, 0.3333f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };
        }

        public void ApplyDefaults()
        {
            BodyHitpoints = 5600f;
            BodyCollisionResistance = 25f;
            BodyCollisionMultiplier = 1f;
            BodyFireResistance = 7.5f;
            BodyFireMultiplier = 1f;
            DamageTolerance = 0.01f;

            BrakeSpeedDamageCurve =
                new AnimationCurve(
                    new Keyframe(0, 0, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(2, 0, 0, 0, 0.3333f, 0.3333f),
                    new Keyframe(7.5f, 0.125f, 0.0285f, 0.0285f, 0.3333f, 0.3333f),
                    new Keyframe(29, 0.6706f, 0.0151f, 0.0151f, 0.3333f, 0.3333f),
                    new Keyframe(100, 1, 0, 0, 0.3333f, 0.3333f))
                {
                    preWrapMode = WrapMode.ClampForever,
                    postWrapMode = WrapMode.ClampForever
                };
        }
    }
}