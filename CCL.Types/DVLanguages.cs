using System;
using System.Collections.Generic;
using System.Reflection;

namespace CCL.Types
{
    public enum DVLanguage
    {
        [DVLangData("en")]      English,
        [DVLangData("cs")]      Czech,
        [DVLangData("da")]      Danish,
        [DVLangData("de")]      German,
        [DVLangData("es")]      Spanish,
        [DVLangData("fi")]      Finnish,
        [DVLangData("fr")]      French,
        [DVLangData("hi")]      Hindi,
        [DVLangData("hu")]      Hungarian,
        [DVLangData("it")]      Italian,
        [DVLangData("ja")]      Japanese,
        [DVLangData("ko")]      Korean,
        [DVLangData("nb")]      Norwegian,
        [DVLangData("nl")]      Dutch,
        [DVLangData("pl")]      Polish,
        [DVLangData("pt")]      Portuguese,
        [DVLangData("pt-br", "Portuguese (Brazil)")] Portuguese_BR,
        [DVLangData("ro")]      Romanian,
        [DVLangData("ru")]      Russian,
        [DVLangData("sk")]      Slovak,
        [DVLangData("sv")]      Swedish,
        [DVLangData("tr")]      Turkish,
        [DVLangData("uk")]      Ukrainian,
        [DVLangData("zh-CN", "Chinese (Simplified)")] Chinese_Simple,
        [DVLangData("zh-TW", "Chinese (Traditional)")] Chinese_Trad,
        //[DVLangData("")]        Bulgarian
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DVLangDataAttribute : Attribute
    {
        public string? Name;
        public string Code;

        public DVLangDataAttribute(string code, string? name = null)
        {
            Code = code;
            Name = name;
        }
    }

    public static class DVLanguageExtensions
    {
        private static readonly Dictionary<DVLanguage, DVLangDataAttribute> _attrMap = new Dictionary<DVLanguage, DVLangDataAttribute>();

        static DVLanguageExtensions()
        {
            foreach (DVLanguage lang in Enum.GetValues(typeof(DVLanguage)))
            {
                var field = typeof(DVLanguage).GetField(lang.ToString());
                if (field.GetCustomAttribute<DVLangDataAttribute>() is DVLangDataAttribute attr)
                {
                    _attrMap.Add(lang, attr);
                }
            }
        }

        public static string Name(this DVLanguage lang)
        {
            if (_attrMap.TryGetValue(lang, out var attr) && (attr.Name != null))
            {
                return attr.Name;
            }
            return lang.ToString();
        }

        public static string Code(this DVLanguage lang)
        {
            if (_attrMap.TryGetValue(lang, out var attr))
            {
                return attr.Code;
            }
            return "";
        }
    }
}
