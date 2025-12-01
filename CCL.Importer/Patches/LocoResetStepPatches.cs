using DV.HUD;
using DV.Tutorial.QT;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    //[HarmonyPatch(typeof(LocoResetStep))]
    //internal class LocoResetStepPatches
    //{
    //    [HarmonyPrefix, HarmonyPatch("InternalMakeCurrent")]
    //    private static bool InternalMakeCurrentPrefix(LocoResetStep __instance)
    //    {
    //        if (__instance.overrider)
    //        {
    //            SetToZero(InteriorControlsManager.ControlType.ElectricsFuse);
    //            SetToZero(InteriorControlsManager.ControlType.StarterFuse);
    //            SetToZero(InteriorControlsManager.ControlType.TractionMotorFuse);
    //            SetToZero(InteriorControlsManager.ControlType.CabLight);
    //            SetToZero(InteriorControlsManager.ControlType.StarterControl);
    //            SetToZero(InteriorControlsManager.ControlType.GearboxA);
    //            SetToZero(InteriorControlsManager.ControlType.GearboxB);

    //            __instance.overrider.SetNeutralState();
    //            __instance.overrider = null;
    //        }

    //        return false;

    //        void SetToZero(InteriorControlsManager.ControlType controlType)
    //        {
    //            if (__instance.controls.TryGetControl(controlType, out var control))
    //            {
    //                control.controlImplBase.SetValue(0f);
    //            }
    //        }
    //    }
    //}
}
