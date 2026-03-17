using UnityEngine;

namespace CCL.Importer
{
    internal class ObjectHelper
    {
        private static Transform? s_holder;
        private static Transform? s_failures;

        public static Transform Holder => Extensions.GetCached(ref s_holder, CreateHolder);

        private static Transform CreateHolder()
        {
            CreateFailuresHolder();

            var go = new GameObject("[CCL HOLDER]");
            go.SetActive(false);
            go.AddComponent<DummyScriptToInspectNonObjects>();
            Object.DontDestroyOnLoad(go);

            return go.transform;
        }

        public static void CreateFailuresHolder()
        {
            if (s_failures != null) return;

            var go = new GameObject("[CCL FAILURES]");
            go.AddComponent<ScriptForLoadFailures>();
            Object.DontDestroyOnLoad(go);
            s_failures = go.transform;
        }

        public static GameObject CreateModifiablePrefab(GameObject gameObject)
        {
            GameObject newFab = Object.Instantiate(gameObject, Holder);

            // No (Clone), makes it look bad.
            newFab.name = gameObject.name;
            return newFab;
        }
    }
}
