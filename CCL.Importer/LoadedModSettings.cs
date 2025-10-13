using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace CCL.Importer
{
    internal class LoadedModSettings
    {
        public List<string> CarIds;

        public LoadedModSettings(List<string> carIds)
        {
            CarIds = carIds;
        }

        public void Save(UnityModManager.ModEntry mod)
        {
            CCLPlugin.Settings.Save(CCLPlugin.Instance);
        }

        public void Draw(UnityModManager.ModEntry mod)
        {
            var disabled = CCLPlugin.Settings.DisabledIds;

            GUILayout.Label(new GUIContent("Enabled CCL Rolling Stock",
                "Allows toggling individual car types from spawning in jobs\n" +
                "Trainset types must have all types disabled"), UnityModManager.UI.bold);

            foreach (var item in CarIds)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(item, GUILayout.ExpandWidth(false));
                bool oldState = disabled.Contains(item);
                bool newState = !GUILayout.Toggle(!oldState, "");
                GUILayout.EndHorizontal();

                if (newState == oldState) continue;

                if (newState)
                {
                    disabled.Add(item);
                }
                else
                {
                    disabled.Remove(item);
                }
            }
        }
    }
}
