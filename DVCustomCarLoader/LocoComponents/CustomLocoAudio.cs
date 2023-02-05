using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CCL_GameScripts;
using CCL_GameScripts.Attributes;
using CCL_GameScripts.Effects;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoAudio : LocoTrainAudio
    {
        private static readonly FieldInfo bogieCountField = AccessTools.Field(typeof(TrainAudio), "usualBogieCount");
        private int BogieCount
        {
            get => (int)bogieCountField.GetValue(this);
            set => bogieCountField.SetValue(this, value);
        }

        private static readonly FieldInfo bogieAudioField = AccessTools.Field(typeof(TrainAudio), "bogieAudioControllers");

        protected override void Awake()
        {
            if( bogieAudioField?.GetValue(this) is BogieAudioController[] bogieAudios )
            {
                for( int i = 0; i < bogieAudios.Length; i++ )
                {
                    bogieAudios[i].DestroyAllSources();
                    bogieAudios[i] = null;
                }
            }
            base.Awake();
        }

        public override void SetupForCar( TrainCar car )
        {
            if (car.Bogies.Length != BogieCount)
            {
                BogieCount = car.Bogies.Length;
                Main.LogVerbose($"Resetting audio {GetType()} on car {car.ID}");
                Awake();
            }
            else if ( bogieAudioField.GetValue(this) == null )
            {
                Main.LogVerbose($"Resetting audio {GetType()} on car {car.ID}");
                Awake();
            }

            base.SetupForCar(car);
        }

        public void PullSettingsFromOtherAudio( LocoTrainAudio other )
        {
            Type localType = GetType();
            Type targetType = other.GetType();

            foreach( FieldInfo localField in localType.GetFields() )
            {
                var proxies = localField.GetCustomAttributes().OfType<ProxyFieldAttribute>();
                foreach( var proxy in proxies )
                {
                    string targetName = proxy.TargetName ?? localField.Name;
                    FieldInfo targetField = targetType.GetField(targetName);

                    if( targetField != null )
                    {
                        // direct assignment
                        if( localField.FieldType.IsAssignableFrom(targetField.FieldType) )
                        {
                            localField.SetValue(this, targetField.GetValue(other));
                        }
                        else
                        {
                            Main.Warning($"Proxy {localType.Name}.{localField.Name} is not assignable from {targetType.Name}.{targetName}");
                        }
                    }
                    else
                    {
                        Main.Warning($"From audio type {localType.Name} - target {targetName} not found on {targetType.Name}");
                    }
                }
            }

            // Brakes
            doBrakeAudio = other.doBrakeAudio;
            doBrakeAirflowAudio = other.doBrakeAirflowAudio;
            brakeAudio = other.brakeAudio;
            brakeSquealAudio = other.brakeSquealAudio;
            brakeCylinderExhaustAudio = other.brakeCylinderExhaustAudio;
            airflowAudio = other.airflowAudio;
            brakeVolumeSpeedCurve = other.brakeVolumeSpeedCurve;
            brakeSquealVolumeSpeedCurve = other.brakeSquealVolumeSpeedCurve;

            // Wheels
            wheelslipAudio1 = other.wheelslipAudio1;
            wheelslipAudio2 = other.wheelslipAudio2;
            wheelDamageToMasterVolumeCurve = other.wheelDamageToMasterVolumeCurve;
            wheelDamagedAudio1 = other.wheelDamagedAudio1;
            wheelDamagedAudio2 = other.wheelDamagedAudio2;
        }

        protected AudioSource CreateSource(Vector3 position, float minDistance = 1f, float maxDistance = 500f, AudioMixerGroup mixerGroup = null)
        {
            return AudioUtils.CreateSource(transform, position, minDistance, maxDistance, mixerGroup);
        }

        protected void PlayRandomOneShot(AudioSource source, AudioClip[] clips, float volume = 1f)
        {
            int choice = UnityEngine.Random.Range(0, clips.Length);
            source.PlayOneShot(clips[choice], volume);
        }
    }

    public abstract class CustomLocoAudio<TCtrl, TEvent, TDmg> : CustomLocoAudio
        where TCtrl : CustomLocoController
        where TEvent : CustomLocoSimEvents
        where TDmg : DamageControllerCustomLoco
    {
        protected TCtrl customLocoController;
        protected TEvent simEvents;
        protected TDmg customDmgController;

        protected override void SetupLocoLogic( TrainCar car )
        {
            customLocoController = car.GetComponent<TCtrl>();
            locoController = customLocoController;

            simEvents = car.GetComponent<TEvent>();

            customDmgController = car.GetComponent<TDmg>();
            dmgController = customDmgController;
        }

        protected override void UnsetLocoLogic()
        {
            customLocoController = null;
            locoController = null;
            customDmgController = null;
            dmgController = null;
            simEvents = null;
        }
    }

    [HarmonyPatch(typeof(NAudio))]
    public static class NAudioPatches
    {
        [HarmonyPatch("RequestAudioSource")]
        [HarmonyPostfix]
        public static void RequestAudioSource(ref AudioSource __result)
        {
            if (__result && !__result.isActiveAndEnabled)
            {
                Main.LogVerbose("Found disabled audio source");
                __result.enabled = true;
            }
        }
    }
}