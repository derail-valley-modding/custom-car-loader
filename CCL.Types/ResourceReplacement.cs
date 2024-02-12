using System;

namespace CCL.Types
{
    [Serializable]
    public class ResourceReplacement
    {
        public string ReplacementName = string.Empty;
        public string FieldName = string.Empty;
        public bool IsArray = false;
        public int ArrayIndex = 0;
    }
}
