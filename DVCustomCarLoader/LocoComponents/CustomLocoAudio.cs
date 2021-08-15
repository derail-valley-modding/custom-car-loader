using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CCL_GameScripts;
using HarmonyLib;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoAudio : LocoTrainAudio
    {
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
            if( !(bogieAudioField?.GetValue(this) is BogieAudioController[] bogieAudios) )
            {
                Main.Log($"Pre-awaking audio {GetType()}");
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
}