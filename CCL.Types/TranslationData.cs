using CCL.Types.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types
{
    [Serializable]
    public class TranslationData
    {
        public List<TranslationItem> Items;

        public void Validate()
        {
            if (Items == null || Items.Count == 0)
            {
                Items = new List<TranslationItem>() { new TranslationItem() };
            }
        }

        public static TranslationData Default(string englishName = "") => new TranslationData()
        {
            Items = new List<TranslationItem>() 
            { 
                new TranslationItem() { Value = englishName } 
            }
        };

        public string ToJson()
        {
            return JSONObject.CreateFromObject(Items).ToString();
        }

        public static TranslationData FromJson(string json)
        {
            var parsed = JSONObject.Create(json);
            var items = parsed.ToObject<List<TranslationItem>>();
            return new TranslationData() { Items = items! };
        }
    }

    [Serializable]
    public class TranslationItem
    {
        public DVLanguage Language = DVLanguage.English;
        public string Value = "";
    }
}
