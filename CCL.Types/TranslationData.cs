using CCL.Types.Json;
using System;
using System.Collections.Generic;
using DVLangHelper.Data;

namespace CCL.Types
{
    public static class TranslationDataExtensions
    {
        public static string ToJson(this TranslationData data)
        {
            return JSONObject.CreateFromObject(data.Items).ToString();
        }

        public static TranslationData FromJson(string json)
        {
            var parsed = JSONObject.Create(json);
            var items = parsed.ToObject<List<TranslationItem>>();
            return new TranslationData() { Items = items! };
        }
    }
}
