using DV.CabControls.Spec;
using DV.HUD;
using DV.Tutorial.QT;
using HarmonyLib;
using UnityEngine;

using static DV.Tutorial.QT.QuickTutorialFactory;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(TrainTutorialConstructor))]
    internal class TrainTutorialConstructorPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(TrainTutorialConstructor.AddLookAndAcknowledge),
            new[] { typeof(InteriorControlsManager.ControlType), typeof(ControlIconQuickTutorialMessage), typeof(bool) })]
        private static bool AddLookAndAcknowledgePrefix(TrainTutorialConstructor __instance,
            InteriorControlsManager.ControlType controlType, ControlIconQuickTutorialMessage message, bool isSteamLoco)
        {
            if (__instance.Controls.TryGetControl(controlType, out var reference))
            {
                message ??= TrainTutorialConstructor.GetDescriptionFor(controlType, QTSemantic.Look, __instance.Loco, __instance.Controls, isSteamLoco);
                message.spriteIndex = 2;

                var target = reference.controlImplBase.transform;

                if (reference.controlImplBase.spec == null)
                {
                    reference.controlImplBase.spec = reference.controlImplBase.GetComponent<ControlSpec>();
                }

                if (reference.controlImplBase.spec is Lever lever && lever.interactionPoint != null)
                {
                    target = lever.interactionPoint;
                }

                __instance.Phase.Add(new LookStep(message, target, Vector3.zero));
            }

            return false;
        }
    }
}
