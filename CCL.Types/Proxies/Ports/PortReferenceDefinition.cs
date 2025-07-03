using System;

namespace CCL.Types.Proxies.Ports
{
    [Serializable]
    public class PortReferenceDefinition
    {
        public DVPortValueType valueType;
        public string ID = string.Empty;
        public bool writeAllowed;

        public PortReferenceDefinition() { }

        public PortReferenceDefinition(DVPortValueType valueType, string id, bool writeAllowed = false)
        {
            this.valueType = valueType;
            this.writeAllowed = writeAllowed;
            ID = id;
        }
    }
}
