using CCL.Types;
using DV.Localization;
using HarmonyLib;
using I2.Loc;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer
{
    public static class TranslationInjector
    {
        private static GameObject _sourceHolder = null!;
        private static LanguageSource _source = null!;
        internal static LanguageSourceData _langData = null!;

        [AfterStartup(StartupPriority.Critical)]
        public static void Initialize()
        {
            _sourceHolder = new GameObject("CCL_Translations");
            UnityEngine.Object.DontDestroyOnLoad(_sourceHolder);

            _source = _sourceHolder.AddComponent<LanguageSource>();
            _langData = _source.SourceData;

            foreach (DVLanguage language in Enum.GetValues(typeof(DVLanguage)))
            {
                _langData.AddLanguage(language.Name(), language.Code());
                _langData.EnableLanguage(language.Name(), true);
            }
        }

        public static void AddTranslations(string key, TranslationData data)
        {
            string langs = string.Join(", ", data.Items.Select(i => i.Language.Code()));
            CCLPlugin.LogVerbose($"Adding translations for {key}: {langs}");
            var term = _langData.AddTerm(key);
            
            foreach (var item in data.Items)
            {
                int idx = _langData.GetLanguageIndexFromCode(item.Language.Code());
                term.SetTranslation(idx, item.Value);
            }
        }
    }

    [HarmonyPatch(typeof(LocalizationLoader))]
    public static class TranslationPatch
    {
        [HarmonyPatch("Start")]
        public static void Postfix(ref IEnumerator __result)
        {
            __result = WrappedEnumerator.OnceCompleted(__result, InjectNewSource);
        }

        private static readonly MethodInfo _addSourceMethod = AccessTools.Method(typeof(LocalizationManager), "AddSource");

        private static void InjectNewSource()
        {
            CCLPlugin.LogVerbose("Inject CCL language source");
            _addSourceMethod.Invoke(null, new object[] { TranslationInjector._langData });
        }
    }
}
