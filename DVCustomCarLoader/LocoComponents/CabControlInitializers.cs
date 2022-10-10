using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CCL_GameScripts.CabControls;
using DV.CabControls.Spec;
using HarmonyLib;
using DVCustomCarLoader.LocoComponents.DieselElectric;

namespace DVCustomCarLoader.LocoComponents
{
    public static class CabControlInitializers
    {
        [CopySpecAfterInit(typeof(CopiedCabInput))]
        public static void FinalizeCabInputSetup(CopiedCabInput spec, GameObject newObject)
        {
            // copy interaction area
            var realControlSpec = newObject.GetComponentInChildren<ControlSpec>(true);

            // try to find interaction area parent
            var iAreaField = AccessTools.Field(realControlSpec.GetType(), "nonVrStaticInteractionArea");
            if (iAreaField != null)
            {
                var iArea = iAreaField.GetValue(realControlSpec) as StaticInteractionArea;
                if (iArea)
                {
                    GameObject newIAObj = UnityEngine.Object.Instantiate(iArea.gameObject, newObject.transform.parent);
                    iArea = newIAObj.GetComponent<StaticInteractionArea>();
                    iAreaField.SetValue(realControlSpec, iArea);
                    Main.LogVerbose("Instantiated static interaction area");
                }
            }

            // handle special case of reverser limiter (if copied directly, causes errors)
            if (spec is CopiedLever lever)
            {
                FinalizeCopiedLeverSetup(lever, newObject);
            }

            // small sh282 valve has no spec attached
            if ((spec is CopiedRotary rotary) && (rotary.RotaryType == CopiedRotaryType.SmallValveSH282))
            {
                ApplySmallSteamValveSpec(rotary);
            }

            // Add wrapper to connect the control to the loco brain
            Main.LogVerbose($"Add input relay to {newObject.name}");

            var inputRelay = newObject.AddComponent<CabInputRelay>();
            inputRelay.Binding = spec.InputBinding;
            inputRelay.MapMin = spec.MappedMinimum;
            inputRelay.MapMax = spec.MappedMaximum;
            inputRelay.AbsPosition = spec.UseAbsoluteMappedValue;
        }

        private static void FinalizeCopiedLeverSetup(CopiedLever spec, GameObject newObject)
        {
            if (spec.LeverType == CopiedLeverType.ReverserShunter)
            {
                var reverserLock = newObject.GetComponentInChildren<ReverserLimiter>(true);
                if (reverserLock) UnityEngine.Object.Destroy(reverserLock);
                newObject.AddComponent<CustomDieselReverserLock>();
            }
            else if (spec.LeverType == CopiedLeverType.ReverserDE6)
            {
                var reverserLock = newObject.GetComponentInChildren<ReverserLimiterDiesel>(true);
                if (reverserLock) UnityEngine.Object.Destroy(reverserLock);
                newObject.AddComponent<CustomDieselReverserLock>();
            }
        }

        [CopySpecAfterInit(typeof(CopiedCabIndicator))]
        public static void FinalizeCopiedIndicator(CopiedCabIndicator spec, GameObject newObject)
        {
            var realIndicator = newObject.GetComponentInChildren<Indicator>(true);
            var indicatorInfo = realIndicator.gameObject.AddComponent<IndicatorRelay>();
            indicatorInfo.Initialize(spec.OutputBinding, realIndicator);
        }

        [InitSpecAfterInit(typeof(PullerSetup))]
        public static void FinalizeCustomPuller(PullerSetup spec, Puller realPuller)
        {
            if (realPuller && !realPuller.pivot)
            {
                realPuller.pivot = realPuller.transform.parent ? realPuller.transform.parent : realPuller.transform;
            }
        }

        private static void ApplySmallSteamValveSpec(CopiedRotary rotary)
        {
            var steamInside = LocoComponentManager.GetTrainCarInterior(TrainCarType.LocoSteamHeavy);
            var largeValve = steamInside.transform.Find("C valve controller/C valve 1");

            if (largeValve)
            {
                var spec = largeValve.GetComponent<Rotary>();
                LocoComponentManager.CopyComponent(spec, rotary.gameObject);
            }
        }
    }
}
