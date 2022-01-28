using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRTK.GrabAttachMechanics;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    [HarmonyPatch(typeof(ShovelNonPhysicalCoal))]
    public static class ShovelNonPhysicalCoal_Patches
	{
		private const float REQUIRED_DOOR_OPENED_PERCENTAGE_VR = 0.25f;
		private const float REQUIRED_DOOR_OPENED_PERCENTAGE_NON_VR = 0.6f;

		private static readonly MethodInfo ScanHits = AccessTools.Method(typeof(ShovelNonPhysicalCoal), "ScanHits");
		private static readonly MethodInfo OnShovelUsed = AccessTools.Method(typeof(ShovelNonPhysicalCoal), "OnShovelUsed");

		[HarmonyPatch("UnloadCoal")]
		[HarmonyPrefix]
        public static bool UnloadCoal(ShovelNonPhysicalCoal __instance, GameObject target,
			bool ___isVR, ref bool ___shovelLoaded, GameObject ___loadedCoalVisual,
			VRTK_TwoHandedPoleGrab ___vrGrab, ShovelAudio ___shovelAudio)
        {
			var trainCar = TrainCar.Resolve(target);
			if (trainCar)
			{
				var baseSteamLoco = trainCar.GetComponent<LocoControllerSteam>();
				CustomLocoControllerSteam customSteamLoco = null;

				if (!baseSteamLoco)
				{
					customSteamLoco = trainCar.GetComponent<CustomLocoControllerSteam>();
				}

				if (!baseSteamLoco && !customSteamLoco) return false;

				float doorOpen = baseSteamLoco ? 
					baseSteamLoco.GetFireDoorOpen() : 
					customSteamLoco.GetFireDoor();

				if (doorOpen > (___isVR ? REQUIRED_DOOR_OPENED_PERCENTAGE_VR : REQUIRED_DOOR_OPENED_PERCENTAGE_NON_VR))
				{
					for (int i = 0; i < __instance.shovelChunksCapacity; i++)
					{
						if (baseSteamLoco)
						{
							baseSteamLoco.AddCoalChunk();
						}
						else
                        {
							customSteamLoco.AddCoalChunk();
                        }
					}

					___shovelLoaded = false;
					___loadedCoalVisual.SetActive(false);

					if (___isVR && ___vrGrab != null)
					{
						___vrGrab.ToggleHeaviness(false);
					}

					___shovelAudio.OnCoalDropped(___loadedCoalVisual.transform);
					return false;
				}
			}

			return false;
		}

		[HarmonyPatch("Update")]
		[HarmonyPrefix]
		public static bool Update(ShovelNonPhysicalCoal __instance,
			bool ___isVR, bool ___shovelLoaded, ref RaycastHit[] ___hits)
        {
			{
				if (!___isVR)
				{
					int nHits = (int)ScanHits.Invoke(__instance, new object[] { });
					for (int i = 0; i < nHits; i++)
					{
						if (!___shovelLoaded)
						{
							var coalPileTarget = ___hits[i].collider.GetComponent<ShovelCoalPile>();
							if (coalPileTarget)
							{
								if (SingletonBehaviour<InteractionTextControllerNonVr>.Exists)
								{
									InteractionInfoType infoType = coalPileTarget.HasCoal() ? InteractionInfoType.ShovelCoalPileUse : InteractionInfoType.ShovelCoalPileEmpty;
									SingletonBehaviour<InteractionTextControllerNonVr>.Instance.DisplayText(infoType);
									return false;
								}
								return false;
							}
						}
						else
						{
							GameObject potentialTrain = null;
							NonPhysicsCoalTarget coalDumpTarget;
							Fire fire;

							if (coalDumpTarget = ___hits[i].collider.GetComponent<NonPhysicsCoalTarget>())
							{
								potentialTrain = coalDumpTarget.gameObject;
							}
							else if (fire = ___hits[i].collider.GetComponent<Fire>())
							{
								potentialTrain = fire.gameObject;
							}

							TrainCar trainCar = TrainCar.Resolve(potentialTrain);
							if (trainCar)
							{
								var baseSteamLoco = trainCar.GetComponent<LocoControllerSteam>();
								CustomLocoControllerSteam customSteamLoco = null;
								if (!baseSteamLoco)
                                {
									customSteamLoco = trainCar.GetComponent<CustomLocoControllerSteam>();
                                }

								if (!baseSteamLoco && !customSteamLoco) return false;

								float doorOpen = baseSteamLoco ?
									baseSteamLoco.GetFireDoorOpen() :
									customSteamLoco.GetFireDoor();

								if (doorOpen > REQUIRED_DOOR_OPENED_PERCENTAGE_NON_VR && SingletonBehaviour<InteractionTextControllerNonVr>.Exists)
								{
									SingletonBehaviour<InteractionTextControllerNonVr>.Instance.DisplayText(InteractionInfoType.ShovelThrowCoalInFirebox);
									return false;
								}
								return false;
							}
						}
					}
					return false;
				}

				OnShovelUsed.Invoke(__instance, new object[] { });
				return false;
			}
		}
    }
}
