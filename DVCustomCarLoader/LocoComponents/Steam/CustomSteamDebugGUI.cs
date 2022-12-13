using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
	public class CustomSteamDebugGUI : MonoBehaviour
	{
		private CustomLocoSimSteam sim;
		private CustomChuffController chuffController;

		private void Start()
		{
			sim = GetComponent<CustomLocoSimSteam>();
			chuffController = GetComponent<CustomChuffController>();
		}

		private void OnGUI()
		{
			if (!sim)
			{
				enabled = false;
				return;
			}
			windowRect.height = (sim.components.Length / 2 + 3) * COMPONENT_HEIGHT;
			GUI.skin = DVGUI.skin;
			windowRect = GUILayout.Window(WINDOW_ID, windowRect, new GUI.WindowFunction(Window), "CCL Steam simulation", Array.Empty<GUILayoutOption>());
		}

		private void Window(int id)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(X_MARGIN);
			GUILayout.BeginVertical();
			GUILayout.Space(Y_MARGIN);

			int fontSize = GUI.skin.label.fontSize;
			GUI.skin.label.wordWrap = false;
			GUI.skin.label.normal.textColor = Color.black;
			GUI.skin.label.fontSize = FONT_SIZE;

			bool rowStart = true;

			for (int i = 0; i < sim.components.Length; i++)
			{
				if (rowStart)
				{
					GUILayout.BeginHorizontal(GUILayout.Width(600f));
				}
				else
				{
					GUILayout.Space(2f);
				}

				SimComponent simComponent = sim.components[i];
				string name = simComponent.name;
				GUILayout.BeginHorizontal(GUILayout.Width(120f));
				GUILayout.Label(name, GUILayout.Width(75f));
				GUILayout.Label(simComponent.value.ToString(), GUILayout.Width(45f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.Width(180f));
				GUILayout.HorizontalSlider(simComponent.value, simComponent.min, simComponent.max, GUILayout.Width(120f));

				if (GUILayout.Button("-", GUILayout.Height(Y_MARGIN), GUILayout.Width(COMPONENT_HEIGHT)))
				{
					simComponent.AddValue(-simComponent.valueStep);
				}

				if (GUILayout.Button("+", GUILayout.Height(Y_MARGIN), GUILayout.Width(COMPONENT_HEIGHT)))
				{
					if (name.Equals(sim.fireboxFuel.name))
					{
						sim.tenderFuel.PassValueTo(simComponent, simComponent.valueStep);
					}
					else if (name.Equals(sim.boilerWater.name))
					{
						sim.tenderWater.PassValueTo(simComponent, simComponent.valueStep);
					}
					else
					{
						simComponent.AddValue(simComponent.valueStep);
					}
				}

				GUILayout.EndHorizontal();
				if (!rowStart)
				{
					GUILayout.EndHorizontal();
				}

				rowStart = !rowStart;
			}

			if (!rowStart)
			{
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();

			GUILayout.Label("burnRate", GUILayout.Width(75f));
			GUILayout.Label(sim.fuelConsumptionRate.ToString(), GUILayout.Width(45f));

			GUILayout.Label("draft", GUILayout.Width(75f));
			GUILayout.Label(sim.GetDraftFlowPercent().ToString(), GUILayout.Width(45f));

			GUILayout.Label("blower", GUILayout.Width(75f));
			GUILayout.Label(sim.GetBlowerFlowPercent().ToString(), GUILayout.Width(45f));

			GUILayout.EndHorizontal();

			GUI.skin.label.wordWrap = true;
			GUI.skin.label.fontSize = fontSize;

			GUILayout.BeginHorizontal(GUILayout.Width(COMPONENT_WIDTH * 2));

			if (GUILayout.Button("Refill pressure", Array.Empty<GUILayoutOption>()))
			{
				sim.boilerPressure.SetValue(sim.boilerPressure.max);
			}
			if (GUILayout.Button("Time Multiplier", Array.Empty<GUILayoutOption>()))
			{
				sim.timeMult = ((sim.timeMult == 0.1f) ? 1f : 0.1f);
			}
			if (GUILayout.Button("Refill all", Array.Empty<GUILayoutOption>()))
			{
				sim.DebugForceSteamUp();
			}

			GUILayout.EndHorizontal();

			GUILayout.Label($"Chuff: C: {chuffController.chuffsPerRevolution}, R: {chuffController.revolutionPos:f2}, A: {chuffController.revolutionAccumulator:f2}, CC: {chuffController.currentChuff}, E: {chuffController.currentChuff != chuffController.lastChuff}");

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		private const int WINDOW_ID = 33;

		private const float HEIGHT = 800f;
		private const float WIDTH = 800f;

		private const float COMPONENT_WIDTH = 300f;
		private const float COMPONENT_HEIGHT = 30f;

		private const float COMPONENTS_NUM = 13f;

		private const float X_MARGIN = 10f;
		private const float Y_MARGIN = 15f;

		private const int FONT_SIZE = 9;

		private Rect windowRect = new Rect(Screen.width * 0.01f, Screen.height * 0.4f, WIDTH, COMPONENT_HEIGHT * COMPONENTS_NUM);
	}

	[HarmonyPatch(typeof(SteamSimDebugGUI), "Window")]
	public static class SteamDebugPatch
    {
		private const float COMPONENT_WIDTH = 300f;
		private const float COMPONENT_HEIGHT = 30f;

		private const float X_MARGIN = 10f;
		private const float Y_MARGIN = 15f;

		private const int FONT_SIZE = 9;

		private static readonly PropertyInfo simProperty = AccessTools.Property(typeof(SteamSimDebugGUI), "sim");

		public static bool Prefix(SteamSimDebugGUI __instance)
        {
			SteamLocoSimulation sim = simProperty.GetValue(__instance) as SteamLocoSimulation;

			GUILayout.BeginHorizontal();
			GUILayout.Space(X_MARGIN);
			GUILayout.BeginVertical();
			GUILayout.Space(Y_MARGIN);

			int fontSize = GUI.skin.label.fontSize;
			GUI.skin.label.wordWrap = false;
			GUI.skin.label.normal.textColor = Color.black;
			GUI.skin.label.fontSize = FONT_SIZE;

			for (int i = 0; i < sim.components.Length; i++)
			{
				if (i % 2 == 0)
				{
					GUILayout.BeginHorizontal(GUILayout.Width(600f));
				}
				else
				{
					GUILayout.Space(2f);
				}

				SimComponent simComponent = sim.components[i];
				string name = simComponent.name;
				GUILayout.BeginHorizontal(GUILayout.Width(120f));
				GUILayout.Label(name, GUILayout.Width(75f));
				GUILayout.Label(simComponent.value.ToString(), GUILayout.Width(45f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(GUILayout.Width(180f));
				GUILayout.HorizontalSlider(simComponent.value, simComponent.min, simComponent.max, GUILayout.Width(120f));

				if (GUILayout.Button("-", GUILayout.Height(Y_MARGIN), GUILayout.Width(COMPONENT_HEIGHT)))
				{
					simComponent.AddValue(-simComponent.valueStep);
				}

				if (GUILayout.Button("+", GUILayout.Height(Y_MARGIN), GUILayout.Width(COMPONENT_HEIGHT)))
				{
					if (name.Equals(sim.coalbox.name))
					{
						sim.tenderCoal.PassValueTo(simComponent, simComponent.valueStep);
					}
					else if (name.Equals(sim.boilerWater.name))
					{
						sim.tenderWater.PassValueTo(simComponent, simComponent.valueStep);
					}
					else
					{
						simComponent.AddValue(simComponent.valueStep);
					}
				}

				GUILayout.EndHorizontal();
				if (i % 2 == 1)
				{
					GUILayout.EndHorizontal();
				}
			}

			GUILayout.Label("burnRate", GUILayout.Width(75f));
			GUILayout.Label(sim.coalConsumptionRate.ToString(), GUILayout.Width(45f));

			GUILayout.Label("draft", GUILayout.Width(75f));
			GUILayout.Label(sim.GetDraftBonusNormalized().ToString(), GUILayout.Width(45f));

			GUILayout.Label("blower", GUILayout.Width(75f));
			GUILayout.Label(sim.GetBlowerBonusNormalized().ToString(), GUILayout.Width(45f));

			GUI.skin.label.wordWrap = true;
			GUI.skin.label.fontSize = fontSize;

			GUILayout.BeginHorizontal(GUILayout.Width(COMPONENT_WIDTH * 2));

			if (GUILayout.Button("Refill pressure", Array.Empty<GUILayoutOption>()))
			{
				sim.boilerPressure.SetValue(sim.boilerPressure.max);
			}
			if (GUILayout.Button("Time Multiplier", Array.Empty<GUILayoutOption>()))
			{
				sim.timeMult = ((sim.timeMult == 0.1f) ? 1f : 0.1f);
			}
			if (GUILayout.Button("Refill all", Array.Empty<GUILayoutOption>()))
			{
				sim.DebugForceSteamCreation();
			}

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUI.DragWindow();

			return false;
		}
    }
}
