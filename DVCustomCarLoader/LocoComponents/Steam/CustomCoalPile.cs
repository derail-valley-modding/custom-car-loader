using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomCoalPile : ShovelCoalPile
    {
        protected CustomLocoSimSteam locoSim;

        private readonly EventHandler<CoupleEventArgs> OnCouple;
        private readonly EventHandler<UncoupleEventArgs> OnUncouple;
        private readonly Func<Collider, Shovel> DealWithOverlap;

        private static readonly FieldInfo rearCouplerField = AccessTools.Field(typeof(ShovelCoalPile), "rearCoupler");
        private static readonly FieldInfo shovelTriggerField = AccessTools.Field(typeof(ShovelCoalPile), "shovelTrigger");

        protected Coupler rearCoupler
        {
            get => rearCouplerField.GetValue(this) as Coupler;
            set => rearCouplerField.SetValue(this, value);
        }

        protected Collider shovelTrigger
        {
            get => shovelTriggerField.GetValue(this) as Collider;
            set => shovelTriggerField.SetValue(this, value);
        }

        protected CustomCoalPile()
        {
            OnCouple = AccessTools.MethodDelegate<EventHandler<CoupleEventArgs>>(AccessTools.Method(typeof(CustomCoalPile), "OnCouple"), this);
            OnUncouple = AccessTools.MethodDelegate<EventHandler<UncoupleEventArgs>>(AccessTools.Method(typeof(CustomCoalPile), "OnUncouple"), this);
            DealWithOverlap = AccessTools.MethodDelegate<Func<Collider, Shovel>>(AccessTools.Method(typeof(CustomCoalPile), "DealWithOverlap"), this);
        }

        internal IEnumerator GetSimReference()
        {
            while (!locoSim)
            {
                locoSim = TrainCar.Resolve(gameObject).GetComponent<CustomLocoSimSteam>();
                yield return null;
            }

            LateInit();
            yield break;
        }

        internal void OnEnableOverride()
        {
            StartCoroutine(GetSimReference());
        }

        protected void LateInit()
        {
            var train = TrainCar.Resolve(gameObject);
            if (!train)
            {
                Main.Error("CustomCoalPile: train is null!");
                enabled = false;
                return;
            }

            rearCoupler = train.rearCoupler;
            shovelTrigger = GetComponent<Collider>();

            if (locoSim.simParams.IsTankLoco)
            {
                shovelTrigger.enabled = true;
                rearCoupler.Coupled -= OnCouple;
                rearCoupler.Uncoupled -= OnUncouple;
            }
            else
            {
                if (rearCoupler.coupledTo && rearCoupler.coupledTo.train)
                {
                    shovelTrigger.enabled = CarTypes.IsTender(rearCoupler.coupledTo.train.carType);
                }
                else
                {
                    shovelTrigger.enabled = false;
                }

                rearCoupler.Coupled += OnCouple;
                rearCoupler.Uncoupled += OnUncouple;
            }
        }

        internal void OnTriggerStayOverride(Collider other)
        {
            if (shovelTrigger)
            {
                Shovel shovel = DealWithOverlap(other);
                if (shovel != null && locoSim.tenderFuel.value > 0f)
                {
                    shovel.RequestSpawnCoal(this);
                }
            }
        }

        public bool HasCoalOverride()
        {
            return locoSim && locoSim.tenderFuel.value > 0;
        }
    }

    [HarmonyPatch(typeof(ShovelCoalPile))]
    public static class CustomCoalPile_Patches
    {
        [HarmonyPatch("OnEnable")]
        [HarmonyPrefix]
        public static bool OnEnable(ShovelCoalPile __instance)
        {
            if (__instance is CustomCoalPile ccp)
            {
                ccp.OnEnableOverride();
                return false;
            }
            return true;
        }

        [HarmonyPatch("OnTriggerStay")]
        [HarmonyPrefix]
        public static bool OnTriggerStay(ShovelCoalPile __instance, Collider other)
        {
            if (__instance is CustomCoalPile ccp)
            {
                ccp.OnTriggerStayOverride(other);
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(ShovelCoalPile.HasCoal))]
        [HarmonyPrefix]
        public static bool HasCoal(ShovelCoalPile __instance, ref bool __result)
        {
            if (__instance is CustomCoalPile ccp)
            {
                __result = ccp.HasCoalOverride();
                return false;
            }
            return true;
        }
    }
}
