using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
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
    }

    [Serializable]
    public class TranslationItem
    {
        public DVLanguage Language = DVLanguage.English;
        public string Value = "";
    }
}
