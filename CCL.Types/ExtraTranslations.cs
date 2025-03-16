using CCL.Types.Json;
using DVLangHelper.Data;
using System;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Extra Translations", order = MenuOrdering.ExtraTranslations)]
    public class ExtraTranslations : ScriptableObject, ICustomSerialized
    {
        public string? CSV_Url;

        [SerializeField, HideInInspector]
        private string _termsJson;

        public TermData[] Terms;

        public void AfterImport()
        {
            Terms = JSONObject.FromJson(_termsJson, Array.Empty<TermData>);
        }

        public void OnValidate()
        {
            _termsJson = JSONObject.ToJson(Terms);
        }

        [Serializable]
        public class TermData
        {
            public string Term;
            public TranslationData Data = TranslationData.Default();
        }
    }
}
