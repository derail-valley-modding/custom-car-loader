using UnityEngine;

namespace CCL.Importer
{
    internal class DummyScriptToInspectNonObjects : MonoBehaviour
    {
        private StationSpawnChanceData _dummyData = new();
    }

    internal class ScriptForLoadFailures : MonoBehaviour
    {
        private const int WindowId = 9000;

        private Rect _windowRect = new(10, 10, 400, 200);
        private Vector2 _scroll = Vector2.zero;

        private void Start()
        {
            if (CarManager.LoadFailures.Count == 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(WindowId, _windowRect, FailuresWindow, "CCL Load Failures");
        }

        private void FailuresWindow(int id)
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            GUILayout.BeginVertical();
            GUILayout.Space(20);

            foreach (var item in CarManager.LoadFailures)
            {
                GUILayout.Label(item, GUILayout.MaxWidth(380));
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
            {
                Destroy(gameObject);
            }
        }
    }
}
