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
                    Main.Log("Instantiated static interaction area");
                }
            }

            // handle special case of reverser limiter (if copied directly, causes errors)
            if (spec is CopiedLever lever)
            {
                FinalizeCopiedLeverSetup(lever, newObject);
            }

            // Add wrapper to connect the control to the loco brain
#if DEBUG
            Main.Log($"Add input relay to {newObject.name}");
#endif
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
    }
}
