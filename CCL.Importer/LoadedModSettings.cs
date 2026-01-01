using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace CCL.Importer
{
    internal class LoadedModSettings
    {
        public List<(string Car, List<string> Liveries)> Ids;

        public LoadedModSettings(List<(string, List<string>)> ids)
        {
            Ids = ids;
        }

        public void Save(UnityModManager.ModEntry mod)
        {
            CCLPlugin.Settings.Save(CCLPlugin.Instance);
        }

        public void Draw(UnityModManager.ModEntry mod)
        {
            var disabledLiveries = CCLPlugin.Settings.DisabledLiveryIds;

            GUILayout.Label(new GUIContent("Enabled CCL Rolling Stock",
                "Allows toggling individual car types from spawning in jobs\n" +
                "Trainset types must have all types disabled"), UnityModManager.UI.bold);

            foreach (var (car, liveries) in Ids)
            {
                var allDisabled = AllLiveriesDisabled(liveries);
                var carDisabled = DrawOption(car, CCLPlugin.Settings.DisabledIds, 0.0f, allDisabled);

                GUI.enabled = !carDisabled;

                // Enable all liveries if they were all disabled and the car type was reenabled.
                if (allDisabled && !carDisabled)
                {
                    foreach (var livery in liveries)
                    {
                        disabledLiveries.Remove(livery);
                    }
                }

                foreach (var livery in liveries)
                {
                    DrawOption(livery, disabledLiveries, 20.0f, false);
                }

                GUI.enabled = true;
            }

            static bool AllLiveriesDisabled(List<string> liveries)
            {
                return CCLPlugin.Settings.DisabledLiveryIds.IsSupersetOf(liveries);
            }

            static bool DrawOption(string option, HashSet<string> states, float offset, bool forceTurnOff)
            {
                GUILayout.BeginHorizontal();
                if (offset > 0) GUILayout.Space(offset);
                GUILayout.Label(option, GUILayout.ExpandWidth(false));
                bool oldState = states.Contains(option);
                bool newState = !GUILayout.Toggle(!oldState, "") || (forceTurnOff && !oldState);
                GUILayout.EndHorizontal();

                if (newState == oldState) return newState;

                if (newState)
                {
                    states.Add(option);
                }
                else
                {
                    states.Remove(option);
                }

                return newState;
            }
        }
    }
}
