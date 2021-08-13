using System.Collections;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents
{
    public abstract class CustomLocoAudio : LocoTrainAudio
    {
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

    public static class TrainComponentPool_RequestAudio_Patch
    {
        public static bool Prefix( TrainCar car )
        {
            if( Main.CustomCarManagerInstance.IsRegisteredCustomCar(car) )
            {
                return false;
            }

            return true;
        }
    }
}