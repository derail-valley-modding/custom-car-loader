using CCL.Importer.Components.Controls;
using DV.CabControls.Spec;
using DV.Tutorial.QT;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches.Tutorial
{
    [HarmonyPatch(typeof(ALocoControlStep))]
    internal class ALocoControlStepPatches
    {
        private class State
        {
            public ScrewInternal Original;

            public State(ScrewInternal original)
            {
                Original = original;
            }
        }

        private static Wheel? s_dummyWheel;
        private static Wheel DummyWheel
        {
            get
            {
                if (s_dummyWheel == null)
                {
                    var go = new GameObject("DummyWheel");
                    go.transform.parent = ObjectHelper.Holder;
                    s_dummyWheel = go.AddComponent<Wheel>();
                }

                return s_dummyWheel;
            }
        }

        [HarmonyPrefix, HarmonyPatch("GetVerb")]
        private static void GetVerbPrefix(ALocoControlStep __instance, out State? __state)
        {
            if (__instance.Control is not ScrewInternal screw)
            {
                __state = null;
                return;
            }

            __state = new State(screw);
            __instance.Control = DummyWheel;
        }

        [HarmonyPostfix, HarmonyPatch("GetVerb")]
        private static void GetVerbPostifx(ALocoControlStep __instance, State? __state)
        {
            if (__state != null)
            {
                __instance.Control = __state.Original;
            }
        }
    }
}
