using CCL.Types;
using CCL.Types.Json;
using CCL.Types.Proxies.Indicators;
using DVLangHelper.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CCL.Creator.Utility
{
    public static class TranslationViewer
    {
        private const string MISSING = "[MISSING TRANSLATION]";

        private static Dictionary<string, TranslationData>? _defaultTranslations = null;
        private static bool _attemptedLoad = false;

        private static Dictionary<string, TranslationData> _webTranslations = new Dictionary<string, TranslationData>();
        private static Dictionary<string, DateTime> _lastFetchTime = new Dictionary<string, DateTime>();
        
        [Serializable]
        public class SerializedTerm
        {
            public string Term;
            public List<TranslationItem> Items;
        }

        private static readonly Dictionary<string, DVLanguage> _langNameMap;

        static TranslationViewer()
        {
            _langNameMap = Enum.GetValues(typeof(DVLanguage))
                .Cast<DVLanguage>()
                .ToDictionary(l => l.Name(), StringComparer.OrdinalIgnoreCase);
        }

        #region Menu Items

        private static DVLanguage? _pendingLangToApply = null;

        public static void ViewLanguage(DVLanguage language)
        {
            _attemptedLoad = false;
            _pendingLangToApply = language;

            _refreshCoro = RefreshCSVCoro();
            EditorApplication.update += RefreshCSVEditorUpdate;
        }

        private static void ApplyLanguageCallback()
        {
            if (!_pendingLangToApply.HasValue) return;

            var userData = GetUserTranslations();

            foreach (var localizer in GetLocalizers())
            {
                var textMesh = localizer.GetComponent<TextMeshPro>();
                if (!textMesh) continue;

                if (localizer.selectedDefaultIdx > 0)
                {
                    textMesh.SetText(GetDefaultTranslation(localizer.key, _pendingLangToApply.Value));
                }
                else
                {
                    textMesh.SetText(GetUserTranslation(userData, localizer.key, _pendingLangToApply.Value));
                }
                textMesh.ForceMeshUpdate();
                EditorUtility.SetDirty(textMesh);
            }

            EditorHelpers.SaveAndRefresh();
        }

        [MenuItem("CCL/Language Preview/Refresh Web CSVs")]
        public static void RefreshCSVMenu()
        {
            _refreshCoro = RefreshCSVCoro(true);
            EditorApplication.update += RefreshCSVEditorUpdate;
        }

        [MenuItem("CCL/Language Preview/(preview only applies to normal objects or within prefab editor)")]
        public static void Dummy() { }

        [MenuItem("CCL/Language Preview/English")]
        public static void ViewEnglish() => ViewLanguage(DVLanguage.English);

        [MenuItem("CCL/Language Preview/Czech")]
        public static void ViewCzech() => ViewLanguage(DVLanguage.Czech);

        [MenuItem("CCL/Language Preview/Danish")]
        public static void ViewDanish() => ViewLanguage(DVLanguage.Danish);

        [MenuItem("CCL/Language Preview/German")]
        public static void ViewGerman() => ViewLanguage(DVLanguage.German);

        [MenuItem("CCL/Language Preview/Spanish")]
        public static void ViewSpanish() => ViewLanguage(DVLanguage.Spanish);

        [MenuItem("CCL/Language Preview/Finnish")]
        public static void ViewFinnish() => ViewLanguage(DVLanguage.Finnish);

        [MenuItem("CCL/Language Preview/French")]
        public static void ViewFrench() => ViewLanguage(DVLanguage.French);

        [MenuItem("CCL/Language Preview/Hindi")]
        public static void ViewHindi() => ViewLanguage(DVLanguage.Hindi);

        [MenuItem("CCL/Language Preview/Hungarian")]
        public static void ViewHungarian() => ViewLanguage(DVLanguage.Hungarian);

        [MenuItem("CCL/Language Preview/Italian")]
        public static void ViewItalian() => ViewLanguage(DVLanguage.Italian);

        [MenuItem("CCL/Language Preview/Japanese")]
        public static void ViewJapanese() => ViewLanguage(DVLanguage.Japanese);

        [MenuItem("CCL/Language Preview/Korean")]
        public static void ViewKorean() => ViewLanguage(DVLanguage.Korean);

        [MenuItem("CCL/Language Preview/Norwegian")]
        public static void ViewNorwegian() => ViewLanguage(DVLanguage.Norwegian);

        [MenuItem("CCL/Language Preview/Dutch")]
        public static void ViewDutch() => ViewLanguage(DVLanguage.Dutch);

        [MenuItem("CCL/Language Preview/Polish")]
        public static void ViewPolish() => ViewLanguage(DVLanguage.Polish);

        [MenuItem("CCL/Language Preview/Portuguese")]
        public static void ViewPortuguese() => ViewLanguage(DVLanguage.Portuguese);

        [MenuItem("CCL/Language Preview/Portuguese BR")]
        public static void ViewPortuguese_BR() => ViewLanguage(DVLanguage.Portuguese_BR);

        [MenuItem("CCL/Language Preview/Romanian")]
        public static void ViewRomanian() => ViewLanguage(DVLanguage.Romanian);

        [MenuItem("CCL/Language Preview/Russian")]
        public static void ViewRussian() => ViewLanguage(DVLanguage.Russian);

        [MenuItem("CCL/Language Preview/Slovak")]
        public static void ViewSlovak() => ViewLanguage(DVLanguage.Slovak);

        [MenuItem("CCL/Language Preview/Swedish")]
        public static void ViewSwedish() => ViewLanguage(DVLanguage.Swedish);

        [MenuItem("CCL/Language Preview/Turkish")]
        public static void ViewTurkish() => ViewLanguage(DVLanguage.Turkish);

        [MenuItem("CCL/Language Preview/Ukrainian")]
        public static void ViewUkrainian() => ViewLanguage(DVLanguage.Ukrainian);

        [MenuItem("CCL/Language Preview/Chinese Simple")]
        public static void ViewChinese_Simple() => ViewLanguage(DVLanguage.Chinese_Simple);

        [MenuItem("CCL/Language Preview/Chinese Traditional")]
        public static void ViewChinese_Trad() => ViewLanguage(DVLanguage.Chinese_Trad);

        #endregion

        private static IEnumerable<LabelLocalizer> GetLocalizers()
        {
            IEnumerable<GameObject> sources;

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                sources = new[] { prefabStage.prefabContentsRoot };
            }
            else
            {
                sources = SceneManager.GetActiveScene().GetRootGameObjects().Where(go => !PrefabUtility.IsPartOfPrefabInstance(go));
            }

            foreach (var source in sources)
            {
                foreach (var localizer in source.GetComponentsInChildren<LabelLocalizer>())
                {
                    yield return localizer;
                }
            }
        }

        #region Default Translations

        private static void ReloadDefaultTranslations()
        {
            if (_attemptedLoad) return;

            string jsonPath = Path.Combine(Application.dataPath, "CarCreator", "Bin", "default_translations.json");

            try
            {
                string text = File.ReadAllText(jsonPath);
                var terms = JSONObject.FromJson<SerializedTerm[]>(text);

                _defaultTranslations = terms.ToDictionary(t => t.Term, t => new TranslationData() { Items = t.Items });
            }
            catch
            {
                Debug.LogWarning("Couldn't load default translation data for preview");
                _defaultTranslations = null;
                return;
            }
            finally
            {
                _attemptedLoad = true; 
            }
        }

        private static string GetDefaultTranslation(string key, DVLanguage language)
        {
            if (_defaultTranslations == null) ReloadDefaultTranslations();
            if (_defaultTranslations == null) return MISSING;

            if (_defaultTranslations.TryGetValue(key, out var data))
            {
                if (data.Items.Find(t => t.Language == language) is TranslationItem match)
                {
                    return match.Value;
                }
            }
            return MISSING;
        }

        #endregion

        #region User Translations

        private static string GetSceneWorkspace()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage prefabStage)
            {
                return Path.GetDirectoryName(prefabStage.prefabAssetPath);
            }
            return Path.GetDirectoryName(SceneManager.GetActiveScene().path);
        }

        public static IEnumerable<ExtraTranslations> GetLocalExtraTranslations()
        {
            string[] guids = AssetDatabase.FindAssets("t:ExtraTranslations", new[] { GetSceneWorkspace() });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                yield return AssetDatabase.LoadAssetAtPath<ExtraTranslations>(path);
            }
        }

        private static Dictionary<string, TranslationData> GetUserTranslations()
        {
            var result = new Dictionary<string, TranslationData>();

            foreach (var transSource in GetLocalExtraTranslations())
            {
                foreach (var term in transSource.Terms)
                {
                    result.Add(term.Term, term.Data);
                }
            }

            return result;
        }

        public static IEnumerable<string> GetUserKeys()
        {
            return _webTranslations.Keys.Union(GetLocalExtraTranslations().SelectMany(et => et.Terms.Select(t => t.Term)));
        }

        private static string GetUserTranslation(Dictionary<string, TranslationData> userdata, string key, DVLanguage language)
        {
            if (((_webTranslations != null) && _webTranslations.TryGetValue(key, out var data)) || 
                userdata.TryGetValue(key, out data))
            {
                if (data.Items.Find(t => t.Language == language) is TranslationItem match)
                {
                    return match.Value;
                }
            }
            return MISSING;
        }


        private static IEnumerator? _refreshCoro = null;

        private static void RefreshCSVEditorUpdate()
        {
            if (_refreshCoro == null)
            {
                EditorApplication.update -= RefreshCSVEditorUpdate;
                return;
            }

            if (!_refreshCoro.MoveNext())
            {
                _refreshCoro = null;
                EditorUtility.ClearProgressBar();

                EditorApplication.update -= RefreshCSVEditorUpdate;
                ApplyLanguageCallback();
            }
        }

        private static readonly TimeSpan CSVCacheDuration = new TimeSpan(0, 10, 0);

        private static IEnumerator RefreshCSVCoro(bool force = false)
        {
            var sources = GetLocalExtraTranslations().ToList();
            for (int i = 0; i < sources.Count; i++)
            {
                var transSource = sources[i];

                if (string.IsNullOrWhiteSpace(transSource.CSV_Url)) continue;

                if (!force && _lastFetchTime.TryGetValue(transSource.CSV_Url!, out DateTime lastFetch) && (DateTime.Now - lastFetch < CSVCacheDuration))
                {
                    continue;
                }

                float progress = (i + 1f) / (sources.Count + 1f);
                EditorUtility.DisplayProgressBar("Applying Translations", $"Fetching CSV for {transSource.name}", progress);

                var request = UnityWebRequest.Get(transSource.CSV_Url);
                request.SendWebRequest();

                while (!request.isDone)
                {
                    yield return null;
                }

                if (request.isNetworkError || request.responseCode != 200)
                {
                    _lastFetchTime.Remove(transSource.CSV_Url!);
                    Debug.LogError($"Failed to fetch translation csv \"{transSource.CSV_Url}\"");
                    continue;
                }

                _lastFetchTime[transSource.CSV_Url!] = DateTime.Now;

                int termCount = 0;
                foreach (var term in ParseTranslationCSV(request.downloadHandler.text))
                {
                    termCount++;
                    _webTranslations[term.Key] = term.Value;
                }

                Debug.Log($"Load \"{transSource.CSV_Url}\": {termCount} terms");
            }
        }

        private static bool HeaderEquals(string compare, params string[] target)
        {
            return target.Any(t => t.Equals(compare, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<KeyValuePair<string, TranslationData>> ParseTranslationCSV(string contents)
        {
            var lines = contents
                .Replace("\r", string.Empty)
                .Split('\n')
                .Select(l => l.Split(','))
                .ToList();

            if (lines.Count < 1 || HeaderEquals("Key", lines[0][0]))
            {
                throw new FormatException("Invalid translation CSV header format");
            }

            string[] discardHeaders = new string[] { "Type", "Desc", "Description" };

            int valuesStartIdx = 1;

            // header
            string[] header = lines[0];
            if ((header.Length > 2) && HeaderEquals(header[1], discardHeaders))
            {
                valuesStartIdx = 2;
            }
            if ((header.Length > 3) && HeaderEquals(header[2], discardHeaders))
            {
                valuesStartIdx = 3;
            }

            // get language column order
            var languageOrder = new DVLanguage[header.Length - valuesStartIdx];
            for (int i = 0; i < languageOrder.Length; i++)
            {
                if (_langNameMap.TryGetValue(header[valuesStartIdx + i], out DVLanguage language))
                {
                    languageOrder[i] = language;
                }
                else
                {
                    throw new FormatException("CSV header contains invalid language name");
                }
            }

            // load values
            for (int i = 1; i < lines.Count; i++)
            {
                string[] row = lines[i];

                if ((row.Length <= valuesStartIdx) || string.IsNullOrWhiteSpace(row[0])) continue;

                var items = new List<TranslationItem>();

                for (int col = valuesStartIdx; col < row.Length; col++)
                {
                    if (string.IsNullOrWhiteSpace(row[col])) continue;

                    var language = languageOrder[col - valuesStartIdx];
                    items.Add(new TranslationItem(language, row[col]));
                }

                var data = new TranslationData() { Items = items };
                yield return new KeyValuePair<string, TranslationData>(row[0], data);
            }
        }

        #endregion
    }
}
