using System;

namespace CCL.Types
{
    [Serializable]
    public class ResourceReplacement
    {
        public int NameIndex = 0;
        public string FieldName = string.Empty;
        public bool IsArray = false;
        public int ArrayIndex = 0;
    }
}
